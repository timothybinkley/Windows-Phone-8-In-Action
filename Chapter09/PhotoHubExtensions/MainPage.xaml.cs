using ExifLib;
using Microsoft.Devices;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Collections.Generic;

namespace PhotoEditor
{
   public partial class MainPage : PhoneApplicationPage
   {
      private WriteableBitmap currentImage;
      private PhotoCamera camera;

      // Constructor
      public MainPage()
      {
         InitializeComponent();

         // Sample code to localize the ApplicationBar
         //BuildLocalizedApplicationBar();
      }

      // Sample code for building a localized ApplicationBar
      //private void BuildLocalizedApplicationBar()
      //{
      //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
      //    ApplicationBar = new ApplicationBar();

      //    // Create a new button and set the text value to the localized string from AppResources.
      //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
      //    appBarButton.Text = AppResources.AppBarButtonText;
      //    ApplicationBar.Buttons.Add(appBarButton);

      //    // Create a new menu item with the localized string from AppResources.
      //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
      //    ApplicationBar.MenuItems.Add(appBarMenuItem);
      //}

      protected override void OnNavigatedTo(NavigationEventArgs e)
      {
         if (State.ContainsKey("customCamera"))
         {
            State.Remove("customCamera");
            InitializeCamera();
         }

         IDictionary<string, string> queryStrings =  NavigationContext.QueryString;
         
         string action = null;
         if (queryStrings.ContainsKey("Action"))
            action = queryStrings["Action"];
         
         string token = null;
         if (queryStrings.ContainsKey("FileId"))
            token = queryStrings["FileId"];
          
         if (!string.IsNullOrEmpty(token))
         {
            MediaLibrary mediaLib = new MediaLibrary();
            Picture picture = mediaLib.GetPictureFromToken(token);
            currentImage = PictureDecoder.DecodeJpeg(picture.GetImage());
            photoContainer.Fill = new ImageBrush { ImageSource = currentImage };
            imageDetails.Text = string.Format("Image from {0} action.\nPicture name:\n{1}\nMedia library token:\n{2}", action, picture.GetPath(), token);
         }
      }

      protected override void OnNavigatedFrom(NavigationEventArgs e)
      {
         if (camera != null)
         {
            CleanUpCamera();
            State["customCamera"] = true;
         }
      }

      #region chooser code
      private void Choose_Click(object sender, EventArgs e)
      {
         var task = new PhotoChooserTask();
         task.ShowCamera = true;
         task.Completed += chooserTask_Completed;
         task.Show();
      }

      private void Capture_Click(object sender, EventArgs e)
      {
         var task = new CameraCaptureTask();
         task.Completed += chooserTask_Completed;
         task.Show();
      }

      void chooserTask_Completed(object sender, PhotoResult e)
      {
         if (e.TaskResult == TaskResult.OK)
         {
            currentImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

            photoContainer.Fill = new ImageBrush { ImageSource = currentImage };
            imageDetails.Text = string.Format("Image from {0}\n", sender.GetType().Name);
            imageDetails.Text += string.Format("Original filename:\n{0}", e.OriginalFileName);
         }
         else
         {
            photoContainer.Fill = new SolidColorBrush(Colors.Gray);
            imageDetails.Text = e.TaskResult.ToString();
         }
      }

      #endregion

      #region photo camera code

      private void Camera_Click(object sender, EventArgs e)
      {
         if (camera == null)
         {
            currentImage = null;
            imageDetails.Text = string.Format("Choose custom camera again to close camera. Use the hardware shutter button to take a picture.\n");
            InitializeCamera();
         }
         else
         {
            CleanUpCamera();
            photoContainer.Fill = new SolidColorBrush(Colors.Gray);
            imageDetails.Text = "Choose an image source from the menu.";
         }
      }

      void InitializeCamera()
      {
         camera = new PhotoCamera(CameraType.Primary);
         camera.Initialized += camera_Initialized;
         camera.CaptureImageAvailable += camera_CaptureImageAvailable;
         camera.CaptureCompleted += camera_CaptureCompleted;

         CameraButtons.ShutterKeyPressed += cameraButtons_ShutterKeyPressed;

         // create and rotate the brush since our orientation does not match the cameras default orientation.
         var brush = new VideoBrush();
         brush.SetSource(camera);
         brush.RelativeTransform = new RotateTransform { CenterX = 0.5, CenterY = 0.5, Angle = camera.Orientation };
         photoContainer.Fill = brush;
      }

      void camera_Initialized(object sender, CameraOperationCompletedEventArgs e)
      {
         Dispatcher.BeginInvoke(() =>
         {
            imageDetails.Text += string.Format("{0} supported resolutions.\n", camera.AvailableResolutions.Count());
            imageDetails.Text += string.Format("Current resolution: {0}\n", camera.Resolution);
            imageDetails.Text += string.Format("Preview resolution: {0}\n", camera.PreviewResolution);
         });

         camera.Initialized -= camera_Initialized;
      }

      void CleanUpCamera()
      {
         CameraButtons.ShutterKeyPressed -= cameraButtons_ShutterKeyPressed;
         camera.CaptureImageAvailable -= camera_CaptureImageAvailable;
         camera.CaptureCompleted -= camera_CaptureCompleted;
         camera.Dispose();
         camera = null;
      }

      private void cameraButtons_ShutterKeyPressed(object sender, EventArgs e)
      {
         camera.CaptureImage();
      }

      void camera_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
      {
         Dispatcher.BeginInvoke(() =>
         {
            currentImage = DecodeJpeg(e.ImageStream);
            photoContainer.Fill = new ImageBrush { ImageSource = currentImage };
            imageDetails.Text = "Image captured from PhotoCamera.";
         });
      }

      void camera_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
      {
         if (!e.Succeeded)
         {
            photoContainer.Fill = new SolidColorBrush(Colors.Gray);
            imageDetails.Text = "Camera capture failed.\n" + e.Exception.Message;
         }
         CleanUpCamera();
      }

      #endregion

      #region exif orientation code

      ushort GetExifOrientation(Stream imageStream)
      {
         try
         {
            ExifReader reader = new ExifReader(imageStream);
            ushort orientation;
            if (reader.GetTagValue(ExifTags.Orientation, out orientation))
            {
               System.Diagnostics.Debug.WriteLine("{0} {1}", orientation.GetType().Name, orientation.ToString());
               return orientation;
            }
         }
         catch (ExifLibException) { }
         return 1;
      }

      WriteableBitmap RotateBitmap(WriteableBitmap source, int width, int height, int angle)
      {
         var target = new WriteableBitmap(width, height);
         int sourceIndex = 0;
         int targetIndex = 0;
         for (int x = 0; x < source.PixelWidth; x++)
         {
            for (int y = 0; y < source.PixelHeight; y++)
            {
               sourceIndex = x + y * source.PixelWidth;
               switch (angle)
               {
                  case 90:
                     targetIndex = (source.PixelHeight - y - 1) + x * target.PixelWidth;
                     break;
                  case 180:
                     targetIndex = (source.PixelWidth - x - 1) + (source.PixelHeight - y - 1) * source.PixelWidth;
                     break;
                  case 270:
                     targetIndex = y + (source.PixelWidth - x - 1) * target.PixelWidth;
                     break;
               }
               target.Pixels[targetIndex] = source.Pixels[sourceIndex];
            }
         }
         return target;
      }

      WriteableBitmap DecodeJpeg(Stream imageStream)
      {
         WriteableBitmap source = PictureDecoder.DecodeJpeg(imageStream);
         imageStream.Position = 0;
         ushort orientation = GetExifOrientation(imageStream);

         switch (orientation)
         {
            case 3:
               return RotateBitmap(source, source.PixelWidth, source.PixelHeight, 180);
            case 6:
               return RotateBitmap(source, source.PixelHeight, source.PixelWidth, 90);
            case 8:
               return RotateBitmap(source, source.PixelHeight, source.PixelWidth, 270);
            default:
               return source;
         }
      }
      #endregion

      #region open, edit and save code
      void Edit_Click(object sender, EventArgs e)
      {
         if (currentImage != null)
         {
            currentImage.Invalidate();
            var transform = new CompositeTransform
            {
               ScaleX = currentImage.PixelWidth / photoContainer.ActualWidth,
               ScaleY = currentImage.PixelHeight / photoContainer.ActualHeight,
               Rotation = -35,
               TranslateX = 100 * currentImage.PixelWidth / photoContainer.ActualWidth,
               TranslateY = 250 * currentImage.PixelHeight / photoContainer.ActualHeight,
            };
            currentImage.Render(photoStamp, transform);
            currentImage.Invalidate();
            imageDetails.Text = "The picture has been stamped.";
         }
      }

      void Save_Click(object sender, EventArgs e)
      {
         if (currentImage != null)
         {
            using (var stream = new MemoryStream())
            {
               currentImage.SaveJpeg(stream, currentImage.PixelWidth, currentImage.PixelHeight, 0, 100);
               stream.Seek(0, 0);
               var library = new MediaLibrary();
               Picture p = library.SavePicture("customphoto.jpg", stream);
               imageDetails.Text = string.Format("Image saved to media library.\nFilename:\ncustomphoto.jpg");
            }
         }
      }

      async void Open_Click(object sender, EventArgs e)
      {
         try
         {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            using (Stream imageStream = await localFolder.OpenStreamForReadAsync("custom-photo.jpg"))
            {
               currentImage = PictureDecoder.DecodeJpeg(imageStream);
               photoContainer.Fill = new ImageBrush { ImageSource = currentImage };
            }
            imageDetails.Text = string.Format("Image loaded from filename:\ncustom-photo.jpg");
         }
         catch (FileNotFoundException)
         {
            photoContainer.Fill = new SolidColorBrush(Colors.Gray);
            imageDetails.Text = "Image not found!";
         }
      }
      #endregion


      void OpenFromLibrary_Click(object sender, EventArgs e)
      {
         var library = new MediaLibrary();
         var pictures = library.SavedPictures;
         var picture = pictures.FirstOrDefault(item => item.Name == "customphoto.jpg");
         if (picture != null)
         {
            using (var stream = picture.GetImage())
            {
               currentImage = PictureDecoder.DecodeJpeg(stream);
            }
            photoContainer.Fill = new ImageBrush { ImageSource = currentImage };
            imageDetails.Text = string.Format("Image from Album: {0}\nPicture name: {1}", picture.Album, picture.GetPath());
         }
         else
         {
            photoContainer.Fill = new SolidColorBrush(Colors.Gray);
            imageDetails.Text = "Choose an image source from the menu.";
         }
      }

   }
}