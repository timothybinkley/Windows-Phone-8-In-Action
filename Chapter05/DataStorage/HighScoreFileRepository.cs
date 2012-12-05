using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace DataStorage
{
    public class HighScoreFileRepository : IHighScoreRepository
    {
        public List<HighScore> Load(int level = 0)
        {
            List<HighScore> storedData;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists("HighScores") &&
                    storage.FileExists(@"HighScores\highscores.xml"))
                {
                    // load the data file into the contacts list.
                    using (IsolatedStorageFileStream stream =
                        storage.OpenFile(@"HighScores\highscores.xml", FileMode.Open))
                    {
                        XmlSerializer serializer =
                            new XmlSerializer(typeof(List<HighScore>));
                        storedData = (List<HighScore>)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    storedData = new List<HighScore>();
                }
            }
            return storedData;
        }

        public void Save(List<HighScore> highscores)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists("HighScores"))
                {
                    storage.CreateDirectory("HighScores");
                }
                using (IsolatedStorageFileStream stream =
                    storage.CreateFile(@"HighScores\highscores.xml"))
                {
                    XmlSerializer serializer =
                        new XmlSerializer(typeof(List<HighScore>));
                    serializer.Serialize(stream, highscores);
                }
            }
        }

        public void Clear()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(@"HighScores\highscores.xml"))
                    storage.DeleteFile(@"HighScores\highscores.xml");
            
                if (storage.DirectoryExists("HighScores"))
                    storage.DeleteDirectory("HighScores");
            }
        }
    }
}
