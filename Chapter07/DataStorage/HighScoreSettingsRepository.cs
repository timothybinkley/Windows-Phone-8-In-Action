using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace DataStorage
{
    public class HighScoreSettingsRepository : IHighScoreRepository
    {
        public List<HighScore> Load(int level = 0)
        {
            List<HighScore> storedData;
            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("HighScores", out storedData))
            {
                storedData = new List<HighScore>();
            }
            return storedData;
        }

        public void Save(List<HighScore> highScores)
        {
            IsolatedStorageSettings.ApplicationSettings["HighScores"] = highScores;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public void Clear()
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("HighScores");
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
