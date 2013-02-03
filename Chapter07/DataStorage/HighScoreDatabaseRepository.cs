using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.Data.Linq;
using Windows.Storage;
using System.Threading.Tasks;

namespace DataStorage
{
    public class HighScoreDatabaseRepository
    {
        HighScoresDataContext db;

        Func<HighScoresDataContext, IOrderedQueryable<HighScore>> allQuery;
        Func<HighScoresDataContext, int, IQueryable<HighScore>> levelQuery;

        public HighScoreDatabaseRepository()
        {
            allQuery = CompiledQuery.Compile((HighScoresDataContext context) => from score in db.HighScores
                                                                                orderby score.Score descending
                                                                                select score);

            levelQuery = CompiledQuery.Compile((HighScoresDataContext context, int level) => from score in db.HighScores
                                                                                             orderby score.Score descending
                                                                                             where score.LevelsCompleted == level
                                                                                             select score);
        }

        public async Task Initialize()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder scoresFolder = await localFolder.CreateFolderAsync("HighScoreDatabase", CreationCollisionOption.OpenIfExists);

            db = new HighScoresDataContext(@"isostore:/HighScoreDatabase/highscores.sdf");
            if (!db.DatabaseExists())
            {
                db.CreateDatabase();
                DatabaseSchemaUpdater updater = db.CreateDatabaseSchemaUpdater();
                updater.DatabaseSchemaVersion = 1;
                updater.Execute();
            }
            else
            {
                DatabaseSchemaUpdater updater = db.CreateDatabaseSchemaUpdater();
                int databaseSchemaVersion = updater.DatabaseSchemaVersion;
                if (databaseSchemaVersion == 0)
                {
                    // add the difficulty column introduced in version one
                    updater.AddColumn<HighScore>("Difficulty");
                    updater.DatabaseSchemaVersion = 1;
                    updater.Execute();
                }
            }
        }

        public List<HighScore> Load(int level = 0)
        {
            IEnumerable<HighScore> highscores;
            if (level == 0)
            {
                highscores = allQuery(db);
            }
            else
            {
                highscores = levelQuery(db, level);
            }
            return highscores.ToList();
        }

        public void Save(List<HighScore> highScores)
        {
            var newscores = highScores.Where(item => item.Id == 0);
            db.HighScores.InsertAllOnSubmit(newscores);
            db.SubmitChanges();
        }

        public void Clear()
        {
            var scores = from score in db.HighScores
                         select score;

            db.HighScores.DeleteAllOnSubmit(scores);
            db.SubmitChanges();

            //db.DeleteDatabase();
        }
    }
}
