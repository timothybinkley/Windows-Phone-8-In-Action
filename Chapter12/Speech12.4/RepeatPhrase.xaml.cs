using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

namespace Speech
{
    public partial class RepeatPhrase : PhoneApplicationPage
    {
        // Start SpeechSynthesizer
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        private SpeechRecognizerUI speechRecognizerUI = new SpeechRecognizerUI();

        private List<VoiceInformation> _availableVoices = InstalledVoices.All.ToList();
        private List<string> _voiceNames;
        private IEnumerable<string> _colorNames = new[]{"Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Black", "White"};


        public RepeatPhrase()
        {
            InitializeComponent();


            _voiceNames = _availableVoices.Select(voice => voice.DisplayName.Split(' ')[1]).ToList();

            LoadListGrammar("voicesList", _voiceNames);
            LoadListGrammar("colorList", _colorNames);
            
        }

        private void LoadListGrammar(string key, IEnumerable<string> listOfItems)
        {
            speechRecognizerUI.Recognizer.Grammars.AddGrammarFromList(key, listOfItems);
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if launched by a voice command
            bool containsKey = NavigationContext.QueryString.ContainsKey("voiceCommandName");
            if (containsKey)
            {
                string s = NavigationContext.QueryString["voiceCommandName"];
                switch (s)
                {
                    case "RepeatPhraseCommand":
                        
                        if (NavigationContext.QueryString.ContainsKey("voice"))
                        {
                            var voice = NavigationContext.QueryString["voice"];
                            SetVoiceFromCommand(voice);
                        }
                        else
                        {
                            await AskForVoice();
                        }
                        await RecognizeSpeech();
                        break;
                    default:
                        MessageBox.Show("uh oh");

                        break;
                }
            }
        }

        private async Task AskForVoice()
        {
            try
            {
                speechRecognizerUI.Settings.ListenText = "Which voice?";
                speechRecognizerUI.Settings.ExampleText = @"examples: '" + _voiceNames[0] + "', '" + _voiceNames[1] + "'";
                speechRecognizerUI.Settings.ReadoutEnabled = true;
                speechRecognizerUI.Settings.ShowConfirmation = true;
                speechRecognizerUI.Recognizer.Grammars["voicesList"].Enabled = true;
                speechRecognizerUI.Recognizer.Grammars["colorList"].Enabled = false;

                SpeechRecognitionUIResult result = await speechRecognizerUI.RecognizeWithUIAsync();
                var gskjs = result.RecognitionResult.Semantics.ToList();
                SetVoiceFromCommand(result.RecognitionResult.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private async void SetVoiceFromCommand(string voice)
        {
            foreach (var voiceInformation in _availableVoices)
            {
                if (voiceInformation.DisplayName.Split(' ')[1].ToLower() == voice.ToLower())
                {
                    _speechSynthesizer.SetVoice(voiceInformation);
                    SelectedVoiceListBox.Text = voice;

                    break;
                }
            }
        }

        private async Task RecognizeSpeech()
        {
            try
            {
                var localSpeechRecognizerUI = new SpeechRecognizerUI();

                localSpeechRecognizerUI.Settings.ListenText = "Say your phrase...";
                localSpeechRecognizerUI.Settings.ExampleText = "What's going on?";
                localSpeechRecognizerUI.Settings.ReadoutEnabled = false;
                localSpeechRecognizerUI.Settings.ShowConfirmation = true;

                SpeechRecognitionUIResult recognitionResult = await localSpeechRecognizerUI.RecognizeWithUIAsync();
                Dispatcher.BeginInvoke(delegate { DetectedTextTextBox.Text = recognitionResult.RecognitionResult.Text; });
                await SayText(recognitionResult.RecognitionResult.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private async Task SayText(string text)
        {
            try // error handling
            {

                // Stop playing any Synthesized speech
                _speechSynthesizer.CancelAll();

                // Check that string is not null or empty
                if (!string.IsNullOrEmpty(text))
                {
                    // Call SpeakTextAsync and await 
                    await _speechSynthesizer.SpeakTextAsync(text);
                }
                else // if string is null or empty, display error message
                {
                    MessageBox.Show("Phrase text is required.",
                                    "Error",
                                    MessageBoxButton.OK);
                }

            }
            catch (Exception ex) // Catch exception and call function to handle it.
            {
                MessageBox.Show(ex.Message);
            }
        }

       

        private async void SayItButton_Click(object sender, RoutedEventArgs e)
        {

            await SayText(DetectedTextTextBox.Text);
        }

        private async void ChangeVoiceButton_OnClick(object sender, RoutedEventArgs e)
        {
            await AskForVoice();
        }

        private void ShowVoicesButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/VoicesList.xaml", UriKind.Relative));
        }
    }
}