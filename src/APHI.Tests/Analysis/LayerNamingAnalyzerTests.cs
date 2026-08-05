using System.Collections.Generic;
using System.Linq;
using Xunit;
using APHI.Models;
using APHI.Analysis;

namespace APHI.Tests.Analysis
{
    /// <summary>
    /// Contains unit tests for the <see cref="LayerNamingAnalyzer"/> class.
    /// </summary>
    public class LayerNamingAnalyzerTests
    {
        /// <summary>
        /// Tests that valid layer names do not produce issues.
        /// </summary>
        [Fact]
        public void Analyze_ValidNames_NoIssues()
        {
            // Arrange
            var analyzer = new LayerNamingAnalyzer();
            var project = new ArcGISProject
            {
                Layers = new List<LayerInfo>
                {
                    new LayerInfo { Name = "Roads" },
                    new LayerInfo { Name = "City_Boundaries" }
                }
            };

            // Act
            var issues = analyzer.Analyze(project).ToList();

            // Assert
            Assert.Empty(issues);
        }

        /// <summary>
        /// Tests that invalid layer names produce warnings.
        /// </summary>
        [Fact]
        public void Analyze_InvalidNames_ProducesIssues()
        {
            // Arrange
            var analyzer = new LayerNamingAnalyzer();
            var project = new ArcGISProject
            {
                Layers = new List<LayerInfo>
                {
                    new LayerInfo { Name = "Layer_1" },
                    new LayerInfo { Name = "Export_Output" }
                }
            };

            // Act
            var issues = analyzer.Analyze(project).ToList();

            // Assert
            Assert.NotEmpty(issues);
            Assert.All(issues, i => Assert.Equal(IssueSeverity.Warning, i.Severity));
        }
    }
}
