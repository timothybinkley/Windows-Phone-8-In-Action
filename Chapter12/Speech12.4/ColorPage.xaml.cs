using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

namespace Speech
{
    public partial class ColorPage : PhoneApplicationPage
    {
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        private IEnumerable<string> _colorNames = new[]{"Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Black", "White"};

        public ColorPage()
        {
            InitializeComponent();
            ExampleTextBlock.TextWrapping = TextWrapping.Wrap;

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
                    case "ChangeColorCommand":
                        if (NavigationContext.QueryString.ContainsKey("color"))
                        {
                            var color = NavigationContext.QueryString["color"];
                            SetColorFromCommand(color);
                        }
                        else
                        {
                            await AskForColor();
                        }
                        break;
                    default:
                        MessageBox.Show("uh oh");

                        break;
                }
            }
        }
        private async Task AskForColor()
        {
            try
            {
                SpeechRecognizer speechRecognizer = new SpeechRecognizer();
                speechRecognizer.AudioCaptureStateChanged += speechRecognizer_AudioCaptureStateChanged;
                speechRecognizer.Grammars.AddGrammarFromList("colorList", _colorNames);

                PromptTextBlock.Text = "Which color?";
                ExampleTextBlock.Text = "'Red', 'Blue', 'Green', 'Yellow', 'Purple', 'Orange', 'Black', 'White'";
                _speechSynthesizer.SpeakTextAsync( "Which color?");
                
                SpeechRecognitionResult result = await speechRecognizer.RecognizeAsync();
                if (result.TextConfidence < SpeechRecognitionConfidence.Medium)
                {
                    FillUi(result);
                    Dispatcher.BeginInvoke(() => PromptTextBlock.Text = "Recognition Confidence too low.");
                    _speechSynthesizer.SpeakTextAsync("Recognition Confidence too low. Please try again.");
                    await AskForColor();
                }
                else
                {
                    SetColorFromCommand(result.Text);
                    PromptTextBlock.Text = "Color set to " + result.Text;
                    FillUi(result);
                    _speechSynthesizer.SpeakTextAsync("Color set to " + result.Text);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private void FillUi(SpeechRecognitionResult result)
        {
            TextConfidenceTextBlock.Text = result.TextConfidence.ToString();
            ConfidenceScoreTextBlock.Text = result.Details.ConfidenceScore.ToString();
            RuleNameTextBlock.Text = result.RuleName;
        }

        void speechRecognizer_AudioCaptureStateChanged(SpeechRecognizer sender, SpeechRecognizerAudioCaptureStateChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => CaptureStatusTextBlock.Text = e.State.ToString());
        }

        private void SetColorFromCommand(string colorName)
        {
            foreach (var color in typeof (Colors).GetProperties())
            {
                if (color.Name == colorName)
                {
                    ColoredRectangle.Fill = new SolidColorBrush((Color)color.GetValue(null, null));
                }
            }
        }
        private async void ChangeColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            await AskForColor();
        }
    }
}