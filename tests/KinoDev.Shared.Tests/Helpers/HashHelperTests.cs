namespace KinoDev.Shared.Tests.Helpers
{
    using System;
    using KinoDev.Shared.Helpers;
    using Xunit;

    public class HashHelperTests
    {
        [Fact]
        public void CalculateSha256Hash_WithValidInput_ReturnsConsistentHash()
        {
            // Arrange
            string input = "password123";
            string salt = "user@example.com";

            // Act
            string result = HashHelper.CalculateSha256Hash(input, salt);
            string secondResult = HashHelper.CalculateSha256Hash(input, salt);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(64, result.Length); // SHA256 hash should be 64 characters in hex representation
            Assert.Equal(result, secondResult); // Should be deterministic
        }

        [Fact]
        public void CalculateSha256Hash_WithDifferentInputs_ReturnsDifferentHashes()
        {
            // Arrange
            string input1 = "password123";
            string input2 = "password124";
            string salt = "user@example.com";

            // Act
            string result1 = HashHelper.CalculateSha256Hash(input1, salt);
            string result2 = HashHelper.CalculateSha256Hash(input2, salt);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void CalculateSha256Hash_WithDifferentSalts_ReturnsDifferentHashes()
        {
            // Arrange
            string input = "password123";
            string salt1 = "user1@example.com";
            string salt2 = "user2@example.com";

            // Act
            string result1 = HashHelper.CalculateSha256Hash(input, salt1);
            string result2 = HashHelper.CalculateSha256Hash(input, salt2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void CalculateSha256Hash_WithNullSalt_UseEmptySalt()
        {
            // Arrange
            string input = "password123";
            string? salt = null;
            string emptySalt = string.Empty;            // Act
            string resultWithNullSalt = HashHelper.CalculateSha256Hash(input, salt!);
            string resultWithEmptySalt = HashHelper.CalculateSha256Hash(input, emptySalt);

            // Assert
            Assert.Equal(resultWithNullSalt, resultWithEmptySalt);
        }

        [Fact]
        public void CalculateSha256Hash_WithNullInput_ThrowsArgumentNullException()
        {
            // Arrange
            string? input = null;
            string salt = "salt";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => HashHelper.CalculateSha256Hash(input!, salt));
        }

        [Fact]
        public void CalculateSha256Hash_WithEmptyInput_ThrowsArgumentNullException()
        {
            // Arrange
            string input = string.Empty;
            string salt = "salt";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => HashHelper.CalculateSha256Hash(input, salt));
        }        [Fact]
        public void CalculateSha256Hash_KnownValue_ReturnsExpectedHash()
        {
            // Arrange
            string input = "test";
            string salt = "salt";
            // Expected hash for "testsalt" using SHA256
            string expectedHash = "4edf07edc95b2fdcbcaf2378fd12d8ac212c2aa6e326c59c3e629be3039d6432";

            // Act
            string actualHash = HashHelper.CalculateSha256Hash(input, salt);

            // Assert
            Assert.Equal(expectedHash, actualHash);
        }
    }
}