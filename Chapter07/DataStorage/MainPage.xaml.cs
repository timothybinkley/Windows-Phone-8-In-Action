using Microsoft.Phone.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataStorage
{
    public partial class MainPage : PhoneApplicationPage
    {
        //HighScoreSettingsRepository repository = new HighScoreSettingsRepository();
        //HighScoreFileRepository repository = new HighScoreFileRepository();
        HighScoreDatabaseRepository repository = new HighScoreDatabaseRepository();

        ObservableCollection<HighScore> highscores;
        Random random = new Random();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await repository.Initialize();
            var results = repository.Load();
            //var results = await repository.LoadAsync();
            
            highscores = new ObservableCollection<HighScore>(results);
            HighScoresList.ItemsSource = highscores;
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

        private void clear_Click(object sender, EventArgs e)
        {
            highscores.Clear();
            repository.Clear();
        }

        private void add_Click(object sender, EventArgs e)
        {
            int score = random.Next(100, 1000);
            int level = random.Next(1, 5);
            string name = string.Format("{0}{1}{2}", (char)random.Next(65, 90), (char)random.Next(65, 90), (char)random.Next(65, 90));

            var highscore = new HighScore { Name = name, Score = score, LevelsCompleted = level };

            bool added = false;
            for (int i = 0; i < highscores.Count; i++)
            {
                if (highscores[i].Score < highscore.Score)
                {
                    highscores.Insert(i, highscore);
                    added = true;
                    break;
                }
            }
            if (!added)
                highscores.Add(highscore);

            repository.Save(highscores.ToList());
        }

        private void save_Click(object sender, EventArgs e)
        {
            var nameInput = FocusManager.GetFocusedElement() as TextBox;
            if (nameInput != null)
                nameInput.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            repository.Save(highscores.ToList());
        }

    }
}