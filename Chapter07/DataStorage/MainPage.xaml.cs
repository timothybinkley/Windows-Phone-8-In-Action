using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DataStorage.Resources;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DataStorage
{
    public partial class MainPage : PhoneApplicationPage
    {
        //HighScoreSettingsRepository repository;
        HighScoreFileRepository repository;
        //HighScoreDatabaseRepository repository;
        ObservableCollection<HighScore> highscores;
        Random random = new Random();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            //repository = new HighScoreSettingsRepository();
            //repository = new HighScoreDatabaseRepository();
            //highscores = new ObservableCollection<HighScore>(repository.Load());
            //HighScoresList.ItemsSource = highscores;

            repository = new HighScoreFileRepository();
            repository.LoadAsync().ContinueWith((t) =>
            {
                highscores = new ObservableCollection<HighScore>(t.Result);
                Dispatcher.BeginInvoke(() => HighScoresList.ItemsSource = highscores);
            });

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