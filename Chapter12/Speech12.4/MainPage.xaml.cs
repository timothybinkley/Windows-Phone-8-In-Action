using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Windows.ApplicationModel;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;
using Windows.Phone.Speech.VoiceCommands;

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
            _speechSynthesizer.BookmarkReached += _speechSynthesizer_BookmarkReached;
            // Set the DataContext of the Languages ListBox for data binding
            LanguagesListBox.DataContext = InstalledLanguages;
            RegisterVoiceCommands();

        }

        void _speechSynthesizer_BookmarkReached(SpeechSynthesizer sender, SpeechBookmarkReachedEventArgs args)
        {   
            Dispatcher.BeginInvoke(() => MessageBox.Show(args.Bookmark + " Reached"));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Set the selectedIndex to trigger the SelectionChanged event
            LanguagesListBox.SelectedIndex = 0;

            // Check if launched by a voice command
            bool containsKey = NavigationContext.QueryString.ContainsKey("voiceCommandName");
            if (containsKey)
            {
                string s = NavigationContext.QueryString["voiceCommandName"];
                switch (s)
                {
                    case "SetVoiceCommand":
                        string action = string.Empty;
                        string voice = NavigationContext.QueryString["voice"];
                        if (NavigationContext.QueryString.ContainsKey("action"))
                        {
                            action = NavigationContext.QueryString["action"];
                        }
                        SetVoiceFromCommand(voice, action);
                        break;
                    default:
                        MessageBox.Show("uh oh");

                    break;
                }
            }
        }

        private async void RegisterVoiceCommands()
        {
           await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommands.xml", UriKind.Absolute));
           UpdatePhraseList("EnglishCommands", "voice");
           MessageBox.Show("Voices PhraseList Updated.");
        }

        private async void UpdatePhraseList(string commandSetName, string phraseListName)
        {
            // Create List to hold the voice names
            List<string> voiceNames = new List<string>();
            
            // Loop through the installed voices
            
            foreach (VoiceInformation voice in _availableVoices)
            {
                // Take the DisplayName of the voice and split it at
                // the spaces. Take the second item in the results
                // and assign it to a new string. 
                string voiceName = voice.DisplayName.Split(' ')[1];
                
                // add voiceName to the list of VoiceNames
                voiceNames.Add(voiceName);
            }
            // lookup the CommandSet we want to update
            VoiceCommandSet commandSet = VoiceCommandService.InstalledCommandSets[commandSetName];

            // Update commandSet with the new phrase list.
            await commandSet.UpdatePhraseListAsync(phraseListName, voiceNames);
        }
        
        private async void SetVoiceFromCommand(string voice, string action)
        {
            foreach (var voiceInformation in _availableVoices)
            {
                if (voiceInformation.DisplayName.Split(' ')[1].ToLower() == voice.ToLower())
                {
                    var voiceInfo = voiceInformation;
                    var language = InstalledLanguages.Single(l => l.Name == voiceInfo.Language);

                    LanguagesListBox.SelectedItem = language;

                    VoicesListBox.SelectedItem = voiceInfo;

                    await _speechSynthesizer.SpeakTextAsync("Voice set to " + voice);
                    if (action == "read")
                    {
                        await SayText();
                    }
                    break;
                }
            }
        }

        private async void SayItButtonClick(object sender, RoutedEventArgs e)
        {
            await SayText();
        }

        private async Task SayText()
        {
            try // error handling
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
            catch (Exception ex) // Catch exception and call function to handle it.
            {
                HandleSpeechSynthesisError(ex);
            }
        }

        private async void SaySsml_Click(object sender, RoutedEventArgs e)
        {
            try // error handling
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
                    var SsmlString = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xml:lang=\""+ _speechSynthesizer.GetVoice().Language +"\">";
                    SsmlString += "<s>" + PhraseTextBox.Text + "</s></speak>";

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
            catch (Exception ex) // Catch exception and call function to handle it.
            {
                HandleSpeechSynthesisError(ex);
            }
        }

        // Speaks the content of a standalone SSML file.
        private async void SpeakSsmlFromFile_Click(object sender, RoutedEventArgs e)
        {
            try // error handling
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
            catch (Exception ex) // Catch exception and call function to handle it.
            {
                HandleSpeechSynthesisError(ex);
            }
        }

        private static void HandleSpeechSynthesisError(Exception ex)
        {
            // convert the HResult code to an int for comparison to returned
            const uint AbortedCallHResult = 0x80045508; // SPERR_SYSTEM_CALL_INTERRUPTED
            const uint GenericInternalHResult = 0x800455A0; // SPERR_WINRT_INTERNAL_ERROR
            const uint AlreadyInLexiconHResult = 0x800455A1; // SPERR_WINRT_ALREADY_IN_LEX
            const uint NotInLexiconHResult = 0x800455A2; // SPERR_WINRT_NOT_IN_LEX
            const uint UnsupportedPhonemeHResult = 0x800455B5; // SPERR_WINRT_UNSUPPORTED_PHONEME
            const uint PhonemeConversionHResult = 0x800455B6; // SPERR_WINRT_PHONEME_CONVERSION
            const uint InvalidLexiconHResult = 0x800455B8; // SPERR_WINRT_LEX_INVALID_DATA
            const uint UnsupportedLanguageHResult = 0x800455BC; // SPERR_WINRT_UNSUPPORTED_LANG
            const uint StringTooLongHResult = 0x800455BD; // SPERR_WINRT_STRING_TOO_LONG
            const uint StringEmptyHResult = 0x800455BE; // SPERR_WINRT_STRING_EMPTY
            const uint NoMoreItemsHResult = 0x800455BF; // SPERR_WINRT_NO_MORE_ITEMS


            if ((uint) ex.HResult == AbortedCallHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_SYSTEM_CALL_INTERRUPTED", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == GenericInternalHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_INTERNAL_ERROR", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == AlreadyInLexiconHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_ALREADY_IN_LEX", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == NotInLexiconHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_NOT_IN_LEX", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == UnsupportedPhonemeHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_UNSUPPORTED_PHONEME", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == PhonemeConversionHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_PHONEME_CONVERSION", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == InvalidLexiconHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_LEX_INVALID_DATA", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == UnsupportedLanguageHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_UNSUPPORTED_LANG", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == StringTooLongHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_STRING_TOO_LONG", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == StringEmptyHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_STRING_EMPTY", MessageBoxButton.OK);
            }
            else if ((uint) ex.HResult == NoMoreItemsHResult)
            {
                MessageBox.Show(ex.Message, "SPERR_WINRT_NO_MORE_ITEMS", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButton.OK);
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