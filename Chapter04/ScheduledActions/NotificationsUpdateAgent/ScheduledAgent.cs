using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace NotificationsUpdateAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            UpdateDefaultTile();
            NotifyComplete();
        }
        
        /// <summary>
        /// Update the default Live Tile and by extension the lock screen with the count of notifications remaining
        /// in the day, and the title of the next notification.
        /// </summary>
        public static void UpdateDefaultTile()
        {
            // the sample code uses static to enable calls from MainPage.xaml.xs. The code in the book does not use static.
            DateTime now = DateTime.Now;
            //now = new DateTime(now.Year, now.Month, now.Day+2, 0, 0, 0); 
            DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            var notifications = ScheduledActionService.GetActions<ScheduledNotification>()
                .Where((item) => item.BeginTime > now && item.BeginTime < endOfDay)
                .OrderBy((item) => item.BeginTime);
            int count = notifications.Count();
            ScheduledNotification nextNotification = notifications.FirstOrDefault();

            string message = null;
            if (nextNotification != null)
                message = string.Format("{0:t} {1}", nextNotification.BeginTime, nextNotification.Title);

            ShellTile defaultTile = ShellTile.ActiveTiles.First();
            StandardTileData tileData = new StandardTileData
            {
                Count = count,
                BackContent = message, 
            };
            defaultTile.Update(tileData);
        }
    }
}