using System.IO;
using Xunit;
using APHI.Utilities;

namespace APHI.Tests.Utilities
{
    /// <summary>
    /// Contains unit tests for the <see cref="FileSystemHelper"/> class.
    /// </summary>
    public class FileSystemHelperTests
    {
        /// <summary>
        /// Tests that FileExists returns true for an existing file.
        /// </summary>
        [Fact]
        public void FileExists_ExistingFile_ReturnsTrue()
        {
            // Arrange
            string tempFile = Path.GetTempFileName();
            try
            {
                // Act
                bool exists = FileSystemHelper.FileExists(tempFile);

                // Assert
                Assert.True(exists);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        /// <summary>
        /// Tests that FileExists returns false for a non-existing file.
        /// </summary>
        [Fact]
        public void FileExists_NonExistingFile_ReturnsFalse()
        {
            // Arrange
            string fakePath = @"C:\NonExistentDirectory\FakeFile.xyz";

            // Act
            bool exists = FileSystemHelper.FileExists(fakePath);

            // Assert
            Assert.False(exists);
        }
    }
}
