using System.Net;
using KinoDev.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace KinoDev.Shared.Tests.Extensions
{
    public class GenericResponseExtensionTests
    {
        private readonly Mock<ILogger<GenericResponseExtensionTests>> _loggerMock;

        public GenericResponseExtensionTests()
        {
            _loggerMock = new Mock<ILogger<GenericResponseExtensionTests>>();
        }

        [Fact]
        public async Task GetResponseAsync_NullResponse_ReturnsNull()
        {
            // Arrange
            HttpResponseMessage? response = null;

            // Act
            var result = await response.GetResponseAsync<TestModel>(_loggerMock.Object);

            // Assert
            Assert.Null(result);

            // Verify that a warning was logged
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Response is null")),
                    null,
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());
        }

        [Fact]
        public async Task GetResponseAsync_SuccessStatusCodeWithValidJson_ReturnsDeserializedObject()
        {
            // Arrange
            var testModel = new TestModel { Id = 1, Name = "Test" };
            var content = JsonConvert.SerializeObject(testModel);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            };

            // Act
            var result = await httpResponse.GetResponseAsync<TestModel>(_loggerMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testModel.Id, result.Id);
            Assert.Equal(testModel.Name, result.Name);
        }

        [Fact]
        public async Task GetResponseAsync_SuccessStatusCodeWithStringType_ReturnsContentAsString()
        {
            // Arrange
            var content = "This is a test string";
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            };

            // Act
            var result = await httpResponse.GetResponseAsync<string>(_loggerMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task GetResponseAsync_SuccessStatusCodeWithInvalidJson_ReturnsNull()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(invalidJson)
            };

            // Act
            var result = await httpResponse.GetResponseAsync<TestModel>(_loggerMock.Object);

            // Assert
            Assert.Null(result);

            // Verify that an error was logged
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());
        }

        [Fact]
        public async Task GetResponseAsync_FailureStatusCode_ReturnsNull()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not found")
            };

            // Act
            var result = await httpResponse.GetResponseAsync<TestModel>(_loggerMock.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetResponseAsync_NoLogger_HandlesExceptionsGracefully()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(invalidJson)
            };

            // Act
            var result = await httpResponse.GetResponseAsync<TestModel>(); // No logger provided

            // Assert
            Assert.Null(result);
        }

        // Test model class for deserialization tests
        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
