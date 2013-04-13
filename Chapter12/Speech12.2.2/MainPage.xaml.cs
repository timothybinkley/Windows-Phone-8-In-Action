using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Windows.Phone.Speech.Synthesis;

namespace Speech
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Start SpeechSynthesizer
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        // Get all available voices
        private IEnumerable<VoiceInformation> _availableVoices = InstalledVoices.All.ToList();
        
        // Get installed languages from  available voices
        private IEnumerable<CultureInfo> InstalledLanguages 
        {  
            get
            {
                return _availableVoices.Select(voice => new CultureInfo(voice.Language)).Distinct().ToList();
            }
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Set the DataContext of the Languages ListBox for data binding
            LanguagesListBox.DataContext = InstalledLanguages;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Set the selectedIndex to trigger the SelectionChanged event
            LanguagesListBox.SelectedIndex = 0;

            base.OnNavigatedTo(e);
        }

        private async void SayItButtonClick(object sender, RoutedEventArgs e)
        {
            try // Basic error handling
            {
                // Disable the ListBoxes to prevent exceptions
                VoicesListBox.IsEnabled = false;
                LanguagesListBox.IsEnabled = false;

                // Check that string is not null or empty
                if (!string.IsNullOrEmpty(PhraseTextBox.Text))
                {
                    // Call SpeakTextAsync and await 
                    await _speechSynthesizer.SpeakTextAsync(PhraseTextBox.Text);
                    MessageBox.Show("Text read: '" + PhraseTextBox.Text + "'");
                }
                else // if string is null or empty, display error message
                {
                    MessageBox.Show("Phrase text is required.", 
                                    "Error", 
                                    MessageBoxButton.OK);
                }

                // Re-enable the ListBoxes
                VoicesListBox.IsEnabled = true;
                LanguagesListBox.IsEnabled = true;
            }
            catch (Exception ex) // Catch exceptions and display in MessageBox
            {
                MessageBox.Show(ex.Message, 
                                "Exception", 
                                MessageBoxButton.OK);
            }
        }

        #region Language and Voice 

        private void LanguagesListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // First we cast the sender from an object to a then cast its 
            // selected item to CultureInfo then update the Voices ListBox. 
            UpdateVoicesListBox((CultureInfo)((ListBox)sender).SelectedItem);
        } 
                      
        private void UpdateVoicesListBox(CultureInfo info)
        {
            // Get only the voices that match then set the datacontext for databinding.
            VoicesListBox.DataContext = _availableVoices.Where(v => v.Language == info.Name).OrderBy(v => v.Gender);
            // Set voice to first in list
            SetVoice(VoicesListBox.Items[0]);
        }

        private void VoicesListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVoice(sender);
        }

        private void SetVoice(object sender)
        {
            // Determine if sender is VoiceInformation
            if (sender is VoiceInformation)
            {
                _speechSynthesizer.SetVoice((VoiceInformation)sender);
            }
            else
            { // If the sender is NOT VoiceInformation, it is a ListBox and cast its SelectedItem to VoiceInformation
                _speechSynthesizer.SetVoice((VoiceInformation)((ListBox)sender).SelectedItem);
            }
        }

        #endregion Language and Voice
    }
}