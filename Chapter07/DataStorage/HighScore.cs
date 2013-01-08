using System;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;

namespace DataStorage
{
    [Table]
    [Index(Columns="Score")]
    public class HighScore
    {
        public HighScore() { Date = DateTime.Now; }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column] public string Name { get; set; }
        [Column] public int Score { get; set; }
        [Column] public int LevelsCompleted { get; set; }
        [Column] public DateTime Date { get; set; }
        [Column] public string Difficulty { get; set; }

    }
}
