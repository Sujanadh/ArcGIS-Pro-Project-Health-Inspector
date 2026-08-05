using Xunit;
using APHI.Utilities;

namespace APHI.Tests.Utilities
{
    /// <summary>
    /// Contains unit tests for the <see cref="PathHelper"/> class.
    /// </summary>
    public class PathHelperTests
    {
        /// <summary>
        /// Tests that IsNetworkPath returns true for UNC paths.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(@"\\server\share\file.txt", true)]
        [InlineData(@"C:\local\file.txt", false)]
        [InlineData(@"D:\data.gdb", false)]
        [InlineData(@"\\192.168.1.1\share\map.mxd", true)]
        public void IsNetworkPath_ValidatesCorrectly(string path, bool expected)
        {
            // Act
            bool result = PathHelper.IsNetworkPath(path);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests that GetRelativePath returns the correct relative path.
        /// </summary>
        [Fact]
        public void GetRelativePath_ValidPaths_ReturnsRelativePath()
        {
            // Arrange
            string basePath = @"C:\Project\";
            string fullPath = @"C:\Project\Data\file.txt";

            // Act
            string relativePath = PathHelper.GetRelativePath(basePath, fullPath);

            // Assert
            Assert.Equal(@"Data\file.txt", relativePath);
        }
    }
}
