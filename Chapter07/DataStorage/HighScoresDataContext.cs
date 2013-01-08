using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DataStorage
{
    public class HighScoresDataContext : DataContext
    {
        public Table<HighScore> HighScores;

        public HighScoresDataContext(string path) : base(path) { }
    }
}
