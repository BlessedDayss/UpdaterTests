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

        [Test]
        public void ClearRedisCache_WhenExitCodeNotZero_ReturnsFalse()
        {
            // Arrange
            Environment.ExitCode = 1;

            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ClearRedisCache_WhenRedisPasswordProvided_ExecutesCommandWithPassword()
        {
            // Arrange
            _mockSiteInfo.Setup(s => s.RedisPassword).Returns("password");

            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);

            // Assert
            Assert.That(result, Is.True);
            // Additional assertions can be made to verify the command execution with password
        }

        [Test]
        public void ClearRedisCache_WhenRedisPasswordNotProvided_ExecutesCommandWithoutPassword()
        {
            // Arrange
            _mockSiteInfo.Setup(s => s.RedisPassword).Returns(string.Empty);

            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);

            // Assert
            Assert.That(result, Is.True);
            // Additional assertions can be made to verify the command execution without password
        }

        [Test]
        public void ClearRedisCache_WhenExceptionThrown_ReturnsFalse()
        {
            // Arrange
            _mockSiteInfo.Setup(s => s.RedisServer).Throws(new Exception("Test exception"));

            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ClearRedisCache_WhenRedisServerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _mockSiteInfo.Setup(s => s.RedisServer).Returns((string)null);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => RedisExecutor.ClearRedisCache(_mockSiteInfo.Object));
            Assert.That(ex.ParamName, Is.EqualTo("RedisServer"));
            Assert.That(ex.Message, Does.Contain("Value cannot be null. (Parameter 'RedisServer')"));
        }
    }
}