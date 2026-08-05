using System.Collections.Generic;
using Xunit;
using APHI.Core.Models;
using APHI.Analysis;

namespace APHI.Tests.Analysis
{
    /// <summary>
    /// Contains unit tests for the <see cref="HealthScoreCalculator"/> class.
    /// </summary>
    public class HealthScoreCalculatorTests
    {
        /// <summary>
        /// Tests that calculating the health score with empty issues returns 100.
        /// </summary>
        [Fact]
        public void CalculateScore_NoIssues_Returns100()
        {
            // Arrange
            var calculator = new HealthScoreCalculator();
            var issues = new List<AnalysisIssue>();

            // Act
            int score = calculator.CalculateScore(issues);

            // Assert
            Assert.Equal(100, score);
        }

        /// <summary>
        /// Tests that calculating the health score with a single critical issue deducts the appropriate points.
        /// </summary>
        [Fact]
        public void CalculateScore_OneCriticalIssue_DeductsPoints()
        {
            // Arrange
            var calculator = new HealthScoreCalculator();
            var issues = new List<AnalysisIssue>
            {
                new AnalysisIssue { Severity = IssueSeverity.Critical }
            };

            // Act
            int score = calculator.CalculateScore(issues);

            // Assert
            Assert.Equal(80, score); // 100 - 20
        }

        /// <summary>
        /// Tests that calculating the health score with multiple issues caps the minimum score at 0.
        /// </summary>
        [Fact]
        public void CalculateScore_ManyIssues_CapsAtZero()
        {
            // Arrange
            var calculator = new HealthScoreCalculator();
            var issues = new List<AnalysisIssue>();
            for (int i = 0; i < 10; i++)
            {
                issues.Add(new AnalysisIssue { Severity = IssueSeverity.Critical });
            }

            // Act
            int score = calculator.CalculateScore(issues);

            // Assert
            Assert.Equal(0, score);
        }

        /// <summary>
        /// Tests that different severity levels have the correct point deductions.
        /// </summary>
        [Fact]
        public void CalculateScore_MixedSeverities_CalculatesCorrectly()
        {
            // Arrange
            var calculator = new HealthScoreCalculator();
            var issues = new List<AnalysisIssue>
            {
                new AnalysisIssue { Severity = IssueSeverity.Critical }, // -20
                new AnalysisIssue { Severity = IssueSeverity.Warning },  // -10
                new AnalysisIssue { Severity = IssueSeverity.Info }      // -5
            };

            // Act
            int score = calculator.CalculateScore(issues);

            // Assert
            Assert.Equal(65, score); // 100 - 20 - 10 - 5
        }
    }
}
