using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Synthesis;

namespace Speech
{
    public partial class VoicesList : PhoneApplicationPage
    {
        private IEnumerable<VoiceInformation> _availableVoices = InstalledVoices.All.ToList();

        List<VoiceInfo> _voices = new List<VoiceInfo>(); 
        public VoicesList()
        {
            InitializeComponent();
            // Get only the voices that match then set the datacontext for databinding.
            
            foreach (var voiceInformation in _availableVoices)
            {
                _voices.Add(new VoiceInfo()
                    {
                        DisplayName = voiceInformation.DisplayName.Split(' ')[1],
                        Gender = voiceInformation.Gender,
                        Language = voiceInformation.Language
                    });
            } 
            

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            

            List<VoiceInfo> voices = _voices.OrderBy(v => v.Language).ThenBy(v => v.Gender).ToList();
                
            // Check if launched by a voice command

            bool containsKey = NavigationContext.QueryString.ContainsKey("voiceCommandName");
            if (containsKey)
            {
                string s = NavigationContext.QueryString["voiceCommandName"];
                switch (s)
                {
                    case "ListVoicesCommand":
                        string gender = string.Empty;
                        if (NavigationContext.QueryString.ContainsKey("gender"))
                        {
                            gender = NavigationContext.QueryString["gender"];

                            voices = _voices.Where(v => v.Gender.ToString().ToLower() == gender.ToLower()).OrderBy(v => v.Language).ToList();
                        }

                        break;

                    default:
                        MessageBox.Show("uh oh");

                        break;
                }
            }
            VoicesListBox.DataContext = voices;
        }
    }

    public class VoiceInfo
    {
        public string DisplayName { get; set; }
        public string Language { get; set; }
        public VoiceGender Gender { get; set; }
    }
}