using System;
using System.Collections.Generic;
using Xunit;
using Statistics;

namespace Statistics.Test
{
    public class StatsUnitTest
    {
        // Epsilon of 0.01°F: tighter than IoT sensor accuracy (~±0.1°F),
        // wide enough to absorb floating-point rounding in averages.
        private const float Epsilon = 0.01f;

        [Fact]
        public void ReportsAverageMinMax()
        {
            var statsComputer = new StatsComputer();
            var computedStats = statsComputer.CalculateStatistics(
                new List<float>{98.6f, 98.2f, 97.8f, 102.2f});
            Assert.True(Math.Abs(computedStats.average - 99.2f) <= Epsilon);
            Assert.True(Math.Abs(computedStats.max - 102.2f) <= Epsilon);
            Assert.True(Math.Abs(computedStats.min - 97.8f) <= Epsilon);
        }

        [Fact]
        public void ReportsNaNForEmptyInput()
        {
            var statsComputer = new StatsComputer();
            var computedStats = statsComputer.CalculateStatistics(
                new List<float>{});
            // No readings from device: all stats must be NaN
            Assert.True(float.IsNaN(computedStats.average));
            Assert.True(float.IsNaN(computedStats.max));
            Assert.True(float.IsNaN(computedStats.min));
        }

        [Fact]
        public void SingleReadingMakesAvgMinMaxEqual()
        {
            // IoT device sends exactly one packet; avg, min and max must all equal that reading
            var statsComputer = new StatsComputer();
            var computedStats = statsComputer.CalculateStatistics(
                new List<float>{98.6f});
            Assert.True(Math.Abs(computedStats.average - 98.6f) <= Epsilon);
            Assert.True(Math.Abs(computedStats.max - 98.6f) <= Epsilon);
            Assert.True(Math.Abs(computedStats.min - 98.6f) <= Epsilon);
        }

        [Fact]
        public void IdenticalReadingsHaveZeroRange()
        {
            // Sensor reports the same stable value repeatedly; min must equal max
            var statsComputer = new StatsComputer();
            var computedStats = statsComputer.CalculateStatistics(
                new List<float>{98.6f, 98.6f, 98.6f, 98.6f});
            Assert.True(Math.Abs(computedStats.average - 98.6f) <= Epsilon);
            Assert.True(Math.Abs(computedStats.max - computedStats.min) <= Epsilon);
        }

        [Fact]
        public void SensorSpikeIsReflectedInMax()
        {
            // A transient spike from the IoT device must be captured as the max
            var statsComputer = new StatsComputer();
            var computedStats = statsComputer.CalculateStatistics(
                new List<float>{98.4f, 98.6f, 98.7f, 104.0f}); // 104.0°F spike
            Assert.True(Math.Abs(computedStats.max - 104.0f) <= Epsilon);
            Assert.True(Math.Abs(computedStats.min - 98.4f) <= Epsilon);
        }

        [Fact]
        public void HypothermiaReadingsDetected()
        {
            // Readings below 97°F indicate hypothermia; min must reflect the lowest value
            var statsComputer = new StatsComputer();
            var computedStats = statsComputer.CalculateStatistics(
                new List<float>{96.2f, 96.5f, 97.0f, 97.1f});
            Assert.True(Math.Abs(computedStats.min - 96.2f) <= Epsilon);
            Assert.True(computedStats.average < 97.0f);
        }
    }
}
