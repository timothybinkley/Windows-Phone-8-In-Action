using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace NotificationSimulator
{
    public class NotificationService
    {
        /*
        <?xml version="1.0" encoding="utf-8"?>    
        <wp:Notification xmlns:wp="WPNotification">    
            <wp:Toast>    
                <wp:Text1>your title</wp:Text1>    
                <wp:Text2>your content</wp:Text2>    
                <wp:Param>your parameter</wp:Param>    
            </wp:Toast>    
        </wp:Notification>
        */
        const string ToastPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><wp:Notification xmlns:wp=\"WPNotification\">" +
            "<wp:Toast><wp:Text1>{0}</wp:Text1><wp:Text2>{1}</wp:Text2><wp:Param>{2}</wp:Param></wp:Toast></wp:Notification>";

        /*
        <?xml version="1.0" encoding="utf-8"?>
        <wp:Notification xmlns:wp="WPNotification">
            <wp:Tile ID="tile uri">
                <wp:BackgroundImage>your image</wp:BackgroundImage>
                <wp:Count Action="Clear">your badge count</wp:Count>
                <wp:Title Action="Clear">your title</wp:Title>
                <wp:BackBackgroundImage Action="Clear">your back image</wp:BackBackgroundImage>
                <wp:BackTitle Action="Clear">your back title</wp:BackTitle>
                <wp:BackContent Action="Clear">your back content</wp:BackContent>
            </wp:Tile> 
        </wp:Notification>
        */
        const string TilePayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\"><wp:Tile>" +
            "<wp:BackgroundImage>{0}</wp:BackgroundImage>" +
            "<wp:Count {1}>{2}</wp:Count><wp:Title {3}>{4}</wp:Title>" +
            "<wp:BackBackgroundImage {5}>{6}</wp:BackBackgroundImage>" +
            "<wp:BackTitle {7}>{8}</wp:BackTitle>" +
            "<wp:BackContent {9}>{10}</wp:BackContent></wp:Tile></wp:Notification>";

        const string Clear = "Action\"Clear\"";
        
        private void Post(Uri channel, string payload, string target, string interval)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(channel);
            request.Headers["X-NotificationClass"] = interval;
            request.Headers["X-MessageID"] = Guid.NewGuid().ToString();
            if (target.Length > 0)
                request.Headers["X-WindowsPhone-Target"] = target;
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";

            request.BeginGetRequestStream(WriteCallback, new RequestStreamState { Request = request, Payload = payload });
        }

        void WriteCallback(IAsyncResult result)
        {
            RequestStreamState state = (RequestStreamState)result.AsyncState;
            using (var stream = (Stream)state.Request.EndGetRequestStream(result))
            {
                byte[] payloadBytes = Encoding.UTF8.GetBytes(state.Payload);
                stream.Write(payloadBytes, 0, payloadBytes.Length);
            }
            state.Request.BeginGetResponse(ReadCallback, state.Request);
        }

        void ReadCallback(IAsyncResult result)
        {
            string message;
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

                message = string.Format("Push request compeleted with:\n   {0}\n   {1}\n   {2}",
                    response.Headers["X-NotificationStatus"], response.Headers["X-SubscriptionStatus"],
                    response.Headers["X-DeviceConnectionStatus"]);
            }
            catch (Exception ex)
            {
                message = string.Format("{0} pushing notification: {1}", ex.GetType().Name, ex.Message);
            }
            Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(message));
        }

        public void SendToast(Uri channel, string title, string content, string launchUri)
        {
            string payload = string.Format(ToastPayload, title, content, launchUri);
            Post(channel, payload, "toast", "2"); // interval can be 2, 12, or 22 for immediate, 450s or 900s
        }

        public void SendTile(Uri channel, string imagePath, int badgeCount, string title, string backImagePath,
               string backTitle, string content)
        {
            string badgeCountAction = "", titleAction = "", backImagePathAction = "",
                backTitleAction = "", contentAction = "";

            if (badgeCount < 1) badgeCountAction = Clear;
            if (string.IsNullOrEmpty(title)) titleAction = Clear;
            if (string.IsNullOrEmpty(backImagePath)) backImagePathAction = Clear;
            if (string.IsNullOrEmpty(backTitle)) backTitleAction = Clear;
            if (string.IsNullOrEmpty(content)) contentAction = Clear;

            string payload = string.Format(TilePayload, imagePath, badgeCountAction, badgeCount,
                titleAction, title, backImagePathAction, backImagePath, backTitleAction, backTitle,
                contentAction, content);
            Post(channel, payload, "token", "1");   // interval can be 1, 11, or 21 for immediate, 450s, or 900s         
        }

        class RequestStreamState
        {
            public HttpWebRequest Request;
            public string Payload;
        }
    }
}
