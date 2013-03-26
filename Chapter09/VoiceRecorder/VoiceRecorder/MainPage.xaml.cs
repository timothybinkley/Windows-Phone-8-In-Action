using Microsoft.Devices;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Resources;
using Windows.Storage;
using Windows.Storage.Search;

namespace VoiceRecorder
{
   public partial class MainPage : PhoneApplicationPage
   {
      private MemoryStream audioStream = null;
      byte[] audioBuffer = null;
      private SoundEffectInstance audioPlayerInstance = null;

      // Constructor
      public MainPage()
      {
         InitializeComponent();

         // Sample code to localize the ApplicationBar
         //BuildLocalizedApplicationBar();

         Microphone.Default.BufferDuration = TimeSpan.FromSeconds(1);
         Microphone.Default.BufferReady += microphone_BufferReady;
         recordingList.ItemsSource = new ObservableCollection<VoiceRecording>();
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

      /// <summary>If a voice recording file name is passed in a query string (from the Music + Video hub)
      /// then play the wav file. 
      /// </summary>
      protected override async void OnNavigatedTo(NavigationEventArgs e)
      {
         await DisplayRecordingNames();
         IDictionary<string, string> queryStrings = NavigationContext.QueryString;
         navUri.Text = e.Uri.ToString();
         if (queryStrings.ContainsKey("vrec-filename"))
         {
            PlayFile(queryStrings["vrec-filename"]);
         }
      }

      /// <summary>Copies the available audio data to the MemoryStream.
      /// </summary>
      void microphone_BufferReady(object sender, EventArgs e)
      {
         int count = Microphone.Default.GetData(audioBuffer);
         audioStream.Write(audioBuffer, 0, count);
      }

      private void play_Click(object sender, EventArgs e)
      {
         //if (audioPlayerInstance != null && audioPlayerInstance.State == SoundState.Playing)
         //{
         //   audioPlayerInstance.Pause();
         //}
         if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
         {
            BackgroundAudioPlayer.Instance.Pause();
         }
         else
         {
            var button = (Button)sender;
            string filename = (string)button.Tag;
            PlayFile(filename);
         }
      }

      private void record_Click(object sender, EventArgs e)
      {
         // if we are recording do nothing.
         if (Microphone.Default.State == MicrophoneState.Stopped)
         {
            recordingList.IsEnabled = false;
            recordingMessage.Visibility = Visibility.Visible;

            audioStream = new MemoryStream();
            audioBuffer = new byte[Microphone.Default.GetSampleSizeInBytes(TimeSpan.FromSeconds(1))];

            Microphone.Default.Start();
         }
      }

      private async void stopRecord_Click(object sender, EventArgs e)
      {
         if (Microphone.Default.State == MicrophoneState.Started)
         {
            Microphone.Default.Stop();

            string filename = await WriteFile();
            audioBuffer = null;
            audioStream = null;

            recordingMessage.Visibility = Visibility.Collapsed;
            recordingList.IsEnabled = true;
            recordingList.ItemsSource.Add(new VoiceRecording { Title = filename, Date = DateTime.Now });

            // integrate with music hub
            MediaHistory.Instance.WriteAcquiredItem(createMediaHistoryItem(filename, true));
         }
      }

      /// <summary>Add the file and recorded date to the ListBox
      /// </summary>
      private async Task DisplayRecordingNames()
      {
         StorageFolder folder = ApplicationData.Current.LocalFolder;
         IReadOnlyList<StorageFile> files = await folder.GetFilesAsync(); //CommonFileQuery.OrderByDate);
         foreach (var file in files)
         {
            recordingList.ItemsSource.Add(new VoiceRecording { Title = file.Name, Date = file.DateCreated });
         }
      }

      private async Task<string> WriteFile()
      {
         StorageFolder localFolder = ApplicationData.Current.LocalFolder;
         StorageFile file = await localFolder.CreateFileAsync("voice-recording.wav", CreationCollisionOption.GenerateUniqueName);

         using (Stream fileStream = await file.OpenStreamForWriteAsync())
         {
            using (var writer = new BinaryWriter(fileStream))
            {
               writer.Write(new char[4] { 'R', 'I', 'F', 'F' }); // start of the RIFF header
               writer.Write((Int32)(36 + audioStream.Length)); // FileSize - 8
               writer.Write(new char[4] { 'W', 'A', 'V', 'E' }); // the characters WAVE intdicate the format of the data
               writer.Write(new char[4] { 'f', 'm', 't', ' ' });  // the fmt characters specify that this is the section of the file describing the format
               writer.Write((Int32)16); // size of the WAVEFORMATEX data to follow
               // WAVEFORMATEX
               writer.Write((UInt16)1); // wFormatTag = 1 indicates that the audio data is PCM
               writer.Write((UInt16)1); // nChannels = 1 for mono
               writer.Write((UInt32)16000); // nSamplesPerSec, Sample rate of the waveform in samples per second
               writer.Write((UInt32)32000); // nAvgBytesPerSec, Average bytes per second which can be used to determine the time-wise length of the audio
               writer.Write((UInt16)2); // nBlockAlign, Specifies how each audio block must be aligned in bytes
               writer.Write((UInt16)16); // wBitsPerSample, How many bits represent a single sample (typically 8 or 16)

               writer.Write(new char[4] { 'd', 'a', 't', 'a' }); //, The "data" characters specify that the audio data is next in the file
               writer.Write((Int32)audioStream.Length); // The length of the data in bytes
               writer.Write(audioStream.GetBuffer(), 0, (int)audioStream.Length); //Data, The rest of the file is the actual samples
               writer.Flush();
            }
         }
         return file.Name;
      }

      //private async void PlayFile(string filename)
      //{
      //   StorageFolder localFolder = ApplicationData.Current.LocalFolder;
      //   using (Stream fileStream = await localFolder.OpenStreamForReadAsync(filename))
      //   {
      //      var soundEffect = SoundEffect.FromStream(fileStream);
      //      audioPlayerInstance = soundEffect.CreateInstance();
      //      audioPlayerInstance.Play();

      //      // integrate with music hub
      //      MediaHistory.Instance.NowPlaying = createMediaHistoryItem(filename, false);
      //      MediaHistory.Instance.WriteRecentPlay(createMediaHistoryItem(filename, true));
      //   }
      //}

      private void PlayFile(string filename)
      {
         Uri fileUri = new Uri(filename, UriKind.Relative);
         BackgroundAudioPlayer.Instance.Track = new AudioTrack(
             fileUri,
             filename,
             "Windows Phone 8 in Action",
             null, null, null, EnabledPlayerControls.Pause);

         BackgroundAudioPlayer.Instance.Play();

         // integrate with music hub
         MediaHistory.Instance.NowPlaying = createMediaHistoryItem(filename, false);
         MediaHistory.Instance.WriteRecentPlay(createMediaHistoryItem(filename, true));

      }

      private MediaHistoryItem createMediaHistoryItem(string fullFileName, Boolean smallSize)
      {
         string imageName = smallSize ? "Assets/artwork173.jpg" : "Assets/artwork358.jpg";
         StreamResourceInfo imageInfo = Application.GetResourceStream(new Uri(imageName, UriKind.Relative));

         var mediaHistoryItem = new MediaHistoryItem
         {
            ImageStream = imageInfo.Stream,
            Source = "", // must be an empty string
            Title = fullFileName
         };
         mediaHistoryItem.PlayerContext.Add("vrec-filename", fullFileName);

         return mediaHistoryItem;
      }

   }
}