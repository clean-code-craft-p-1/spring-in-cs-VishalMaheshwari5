using System;
using System.Collections.Generic;

namespace Statistics
{
    public class Stats
    {
        public float average;
        public float max;
        public float min;
    }

    public class StatsComputer
    {
        public Stats CalculateStatistics(List<float> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                return new Stats
                {
                    average = float.NaN,
                    max = float.NaN,
                    min = float.NaN
                };
            }

            float sum = 0;
            float max = numbers[0];
            float min = numbers[0];

            foreach (float n in numbers)
            {
                sum += n;
                if (n > max) max = n;
                if (n < min) min = n;
            }

            return new Stats
            {
                average = sum / numbers.Count,
                max = max,
                min = min
            };
        }
    }
}
