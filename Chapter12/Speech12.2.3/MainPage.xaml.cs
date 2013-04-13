using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Windows.ApplicationModel;
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
            // Bookmark reached event triggers on reaching a mark element int a SSML file
            _speechSynthesizer.BookmarkReached += _speechSynthesizer_BookmarkReached;
            // Set the DataContext of the Languages ListBox for data binding
            LanguagesListBox.DataContext = InstalledLanguages;
        }

        void _speechSynthesizer_BookmarkReached(SpeechSynthesizer sender, SpeechBookmarkReachedEventArgs args)
        {
            Dispatcher.BeginInvoke(() => MessageBox.Show(args.Bookmark + " Reached"));
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

                // Stop playing any Synthesized speech
                _speechSynthesizer.CancelAll();

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
        // good for a single sentence
        public string CreateSSMLWithXDocument()
        {
            XNamespace xmlns =  XNamespace.Get("http://www.w3.org/2001/10/synthesis");
            XDocument ssmlDocument =
                new XDocument(
                    new XElement(xmlns + "speak",
                                 new XAttribute("version", "1.0"),
                                 new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                                 new XAttribute(XNamespace.Xml + "lang", "en-US"),
                                 new XElement("voice",
                                              new XAttribute("name", _speechSynthesizer.GetVoice().DisplayName),
                                              new XElement("p",
                                                           new XElement("s", PhraseTextBox.Text)
                                                  )
                                     )
                        )
                    );
            return ssmlDocument.ToString(SaveOptions.DisableFormatting);
        }

        public List<List<string>> GeneratePoem(int verseCount, int linesPerVerse)
        {
            List<List<string>> poem = new List<List<string>>();
            for (int i = 1; i <= verseCount; i++)
            {
                List<string> verse = new List<string>();
                for (int j = 1; j <= linesPerVerse; j++)
                {
                    string line = "Verse " + i + ", Line " + j;
                    verse.Add(line);
                }
                poem.Add(verse);
            }
            return poem;
        }

        public string CreateSsmlWithXDocument2(List<List<string>> poem )
        {
            XNamespace xmlns = XNamespace.Get("http://www.w3.org/2001/10/synthesis");

            XDocument ssmlDocument = new XDocument();

            XElement rootElement = new XElement(xmlns + "speak");
            rootElement.SetAttributeValue("version", "1.0");
            rootElement.SetAttributeValue(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance");
            rootElement.SetAttributeValue(XNamespace.Xml + "lang", "en-US");

            XElement voiceElement = new XElement("voice");
            voiceElement.SetAttributeValue("name", _speechSynthesizer.GetVoice().DisplayName);
            
            foreach (var verse in poem)
            {
                XElement paragraphElement = new XElement("p");
                foreach (string line in verse)
                {
                    paragraphElement.Add(new XElement("s", line));
                }
                voiceElement.Add(paragraphElement);
            }
            rootElement.Add(voiceElement);
            ssmlDocument.Add(rootElement);

            return ssmlDocument.ToString(SaveOptions.DisableFormatting);
        }
        private async void Poem_Click(object sender, RoutedEventArgs e)
        {
            try // Basic error handling
            {

                // Disable the ListBoxes to prevent exceptions
                VoicesListBox.IsEnabled = false;
                LanguagesListBox.IsEnabled = false;

                // Stop playing any Synthesized speech
                _speechSynthesizer.CancelAll();

                  // Create SSML string
                var SsmlString = CreateSsmlWithXDocument2(GeneratePoem(2, 2));

                // Call SpeakTextAsync and await 
                await _speechSynthesizer.SpeakSsmlAsync(SsmlString);
                MessageBox.Show("SSML Text read: '" + SsmlString + "'");

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
        private async void SaySsml_Click(object sender, RoutedEventArgs e)
        {
            try // Basic error handling
            {

                // Disable the ListBoxes to prevent exceptions
                VoicesListBox.IsEnabled = false;
                LanguagesListBox.IsEnabled = false;

                // Stop playing any Synthesized speech
                _speechSynthesizer.CancelAll();

                // Check that string is not null or empty
                if (!string.IsNullOrEmpty(PhraseTextBox.Text))
                {
                    // Create SSML string

                    var SsmlString = CreateSSMLWithXDocument();

                    // Call SpeakTextAsync and await 
                    await _speechSynthesizer.SpeakSsmlAsync(SsmlString);
                    MessageBox.Show("SSML Text read: '" + SsmlString + "'");
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

        // Speaks the content of a standalone SSML file.
        private async void SpeakSsmlFromFile_Click(object sender, RoutedEventArgs e)
        {
            try // Basic error handling
            {
                // Disable the ListBoxes to prevent exceptions
                VoicesListBox.IsEnabled = false;
                LanguagesListBox.IsEnabled = false;

                // Stop playing any Synthesized speech
                _speechSynthesizer.CancelAll();

                // Set the path to the SSML-compliant XML file.
                string path = Package.Current.InstalledLocation.Path + "\\SSMLExample.xml";
                Uri fileToRead = new Uri(path, UriKind.Absolute);

                // Speak the SSML prompt.
                await _speechSynthesizer.SpeakSsmlFromUriAsync(fileToRead);

                    MessageBox.Show("SSML file read.");

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