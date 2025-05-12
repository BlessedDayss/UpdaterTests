namespace Creatio.Updater.Tests
{
    using System;
    using Creatio.Updater.Configuration;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class RedisExecutorTests
    {
        private Mock<ISiteInfo> _mockSiteInfo;

        [SetUp]
        public void Setup()
        {
            _mockSiteInfo = new Mock<ISiteInfo>();

            _mockSiteInfo.Setup(s => s.RedisServer).Returns("localhost");
            _mockSiteInfo.Setup(s => s.RedisPort).Returns("6379");
            _mockSiteInfo.Setup(s => s.RedisDB).Returns("0");
            _mockSiteInfo.Setup(s => s.RedisPassword).Returns(string.Empty);

            Environment.ExitCode = 0;
        }

        [Test]
        public void ClearRedisCache_WhenSkipClearRedisCacheFeatureDisabled_ReturnsTrue()
        {
            // Arrange
            UpdaterConfig.Configuration
                         .GetSection("features")["SkipClearRedisCache"] = "false";

            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ClearRedisCache_WhenSkipClearRedisCacheFeatureEnabled_ReturnsTrue()
        {
            // Arrange
            UpdaterConfig.Configuration
                         .GetSection("features")["SkipClearRedisCache"] = "true";

            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}