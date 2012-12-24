using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace NotificationSimulator
{
    class CustomUriMapper :UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            string decodedUri = HttpUtility.UrlDecode(uri.ToString());
            if (decodedUri.StartsWith("/Protocol?encodedLaunchUri=wp8inaction:Launch"))
            {
                decodedUri = decodedUri.Replace("/Protocol?encodedLaunchUri=wp8inaction:Launch", "/MainPage.xaml");
                return new Uri(decodedUri, UriKind.Relative);
            }
            return uri;
        }
    }
}
