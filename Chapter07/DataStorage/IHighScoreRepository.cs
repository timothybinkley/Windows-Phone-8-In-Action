using System.Collections.Generic;

namespace DataStorage
{
    interface IHighScoreRepository
    {
        List<HighScore> Load(int level = 0);
        void Save(List<HighScore> highScores);
        void Clear();
    }
}
