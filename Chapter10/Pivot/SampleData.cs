using System;
using System.Collections.Generic;

namespace Pivot
{
    public enum SampleCategory{ Even, Odd }
    
    public class SampleData
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public SampleCategory Category { get; set; }

        public static IEnumerable<SampleData> GenerateSampleData()
        {
            var results = new List<SampleData>();
            var generator = new Random();

            for (int i = 1; i < 100; i++)
            {
                var value = generator.Next(1000);
                var data = new SampleData
                {
                    Name = "data point " + i,
                    Value = value,
                    Category = value % 2 == 0 ? SampleCategory.Even : SampleCategory.Odd,
                };
                results.Add(data);
            }
            return results;
        }
    }
}
