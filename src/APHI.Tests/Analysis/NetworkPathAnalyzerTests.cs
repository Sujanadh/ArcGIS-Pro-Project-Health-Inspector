using System.Collections.Generic;
using System.Linq;
using Xunit;
using APHI.Core.Models;
using APHI.Analysis;

namespace APHI.Tests.Analysis
{
    /// <summary>
    /// Contains unit tests for the <see cref="NetworkPathAnalyzer"/> class.
    /// </summary>
    public class NetworkPathAnalyzerTests
    {
        /// <summary>
        /// Tests that layers with local paths produce no issues.
        /// </summary>
        [Fact]
        public void Analyze_LocalPaths_NoIssues()
        {
            // Arrange
            var analyzer = new NetworkPathAnalyzer();
            var project = new ArcGISProject
            {
                Layers = new List<LayerInfo>
                {
                    new LayerInfo { Name = "LocalLayer", DataSourcePath = @"C:\Data\local.gdb" }
                }
            };

            // Act
            var issues = analyzer.Analyze(project).ToList();

            // Assert
            Assert.Empty(issues);
        }

        /// <summary>
        /// Tests that layers with network paths produce a warning issue.
        /// </summary>
        [Fact]
        public void Analyze_NetworkPaths_ProducesIssues()
        {
            // Arrange
            var analyzer = new NetworkPathAnalyzer();
            var project = new ArcGISProject
            {
                Layers = new List<LayerInfo>
                {
                    new LayerInfo { Name = "NetworkLayer", DataSourcePath = @"\\Server\Data\shared.gdb" }
                }
            };

            // Act
            var issues = analyzer.Analyze(project).ToList();

            // Assert
            Assert.Single(issues);
            Assert.Equal(IssueSeverity.Warning, issues[0].Severity);
            Assert.Contains("NetworkLayer", issues[0].Description);
        }
    }
}
