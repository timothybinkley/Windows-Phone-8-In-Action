using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.Data.Linq;

namespace DataStorage
{
    public class HighScoreDatabaseRepository : IHighScoreRepository
    {
        HighScoresDataContext db;

        Func<HighScoresDataContext, IOrderedQueryable<HighScore>> allQuery;
        Func<HighScoresDataContext, int, IQueryable<HighScore>> levelQuery;

        public HighScoreDatabaseRepository()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists("HighScoreDatabase"))
                {
                    storage.CreateDirectory("HighScoreDatabase");
                }
            }
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

            allQuery = CompiledQuery.Compile((HighScoresDataContext context) => from score in db.HighScores
                                                                                orderby score.Score descending
                                                                                select score);

            levelQuery = CompiledQuery.Compile((HighScoresDataContext context, int level) => from score in db.HighScores
                                                                                             orderby score.Score descending
                                                                                             where score.LevelsCompleted == level
                                                                                             select score);
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
