using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Threading.Tasks;

namespace DataStorage
{
    public class HighScoreFileRepository
    {
        public async Task<List<HighScore>> LoadAsync()
        {
            List<HighScore> storedData;
            try
            {

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder scoresFolder = await localFolder.GetFolderAsync("HighScores");
                StorageFile scoresFile = await scoresFolder.GetFileAsync("highscores.xml");
                using (var randomAccess = await scoresFile.OpenReadAsync())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<HighScore>));
                    storedData = (List<HighScore>)serializer.Deserialize(randomAccess.AsStreamForRead());
                }
            }
            catch (FileNotFoundException ex)
            {
                storedData = new List<HighScore>();
            }
            return storedData;
        }

        public async void Save(List<HighScore> highscores)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder scoresFolder = await localFolder.CreateFolderAsync("HighScores", CreationCollisionOption.OpenIfExists);
            StorageFile scoresFile = await scoresFolder.CreateFileAsync("highscores.xml", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream randomAccess = await scoresFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IOutputStream output = randomAccess.GetOutputStreamAt(0))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<HighScore>));
                    serializer.Serialize(output.AsStreamForWrite(), highscores);
                }
            }
        }

        public async void Clear()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder scoresFolder = await localFolder.GetFolderAsync("HighScores");
                await scoresFolder.DeleteAsync();
            }
            catch (FileNotFoundException)
            {
            }
        }
    }
}
