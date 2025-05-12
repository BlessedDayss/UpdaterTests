using System;
using System.Diagnostics;
using Creatio.Updater.Configuration;
using Moq;
using NUnit.Framework;

namespace Creatio.Updater.Tests
{
    [TestFixture]
    public class RedisExecutorTests
    {
        private Mock<ISiteInfo> _mockSiteInfo;

        [SetUp]
        public void Setup()
        {
            _mockSiteInfo = new Mock<ISiteInfo>();

            // Setup default site info properties
            _mockSiteInfo.Setup(s => s.RedisServer).Returns("localhost");
            _mockSiteInfo.Setup(s => s.RedisPort).Returns("6379");
            _mockSiteInfo.Setup(s => s.RedisDB).Returns("0");
            _mockSiteInfo.Setup(s => s.RedisPassword).Returns("");

            // Reset Environment.ExitCode
            Environment.ExitCode = 0;
        }

        [Test]
        public void ClearRedisCache_WhenSkipClearRedisCacheFeatureDisabled_ReturnsTrueAndProcessStarted()
        {
            // Arrange
            try
            {
                // Set the feature flag
                UpdaterConfig.Configuration.GetSection("features")["SkipClearRedisCache"] = "false";
                var mockProcUtility = new Mock<IProcUtility>();
                mockProcUtility.Setup(p => p.StartProcess(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProcessStartInfo>())).Returns(true);
                // Act
                bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object, mockProcUtility.Object);

                // Assert
                Assert.That(result, Is.True);
                mockProcUtility.Verify(p => p.StartProcess(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProcessStartInfo>()), Times.Once);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }
        [Test]
        public void ClearRedisCache_WhenSkipClearRedisCacheFeatureEnabled_ReturnsTrueAndProcessNotStarted()
        {
            // Arrange
            try
            {
                // Set the feature flag
                UpdaterConfig.Configuration.GetSection("features")["SkipClearRedisCache"] = "true";
                var mockProcUtility = new Mock<IProcUtility>();

                // Act
                bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object, mockProcUtility.Object);

                // Assert
                Assert.That(result, Is.True);
                mockProcUtility.Verify(p => p.StartProcess(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProcessStartInfo>()), Times.Never);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }
    }
}