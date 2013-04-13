using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Windows.Phone.Speech.Synthesis;

namespace Speech
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Start SpeechSynthesizer
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private async void SayItButtonClick(object sender, RoutedEventArgs e)
        {
            try // Basic error handling
            {
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
            }
            catch (Exception ex) // Catch exceptions and display in MessageBox
            {
                MessageBox.Show(ex.Message, 
                                "Exception", 
                                MessageBoxButton.OK);
            }
        }
    }
}