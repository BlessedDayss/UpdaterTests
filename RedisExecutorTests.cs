namespace Updater.Tests.Redis
{
    using System;
    using System.Diagnostics;
    using Creatio.Updater;
    using Creatio.Updater.Configuration;
    using Moq;
    using NUnit.Framework;
    using Updater.Common;
    using Updater.Redis;

    #region Class: RedisExecutorTests

    [TestFixture]
    public class RedisExecutorTests
    {

        #region Fields: Private

        private Mock<ISiteInfo> _mockSiteInfo;
        private readonly Mock<IProcessUtility> _processUtility = new();

        #endregion

        #region Methods: Public

        [SetUp]
        public void Setup()
        {
            _mockSiteInfo = new Mock<ISiteInfo>(MockBehavior.Strict);
            const string defaultHost = "localhost";
            _mockSiteInfo.SetupGet(s => s.RedisServer).Returns(defaultHost);
            _mockSiteInfo.SetupGet(s => s.RedisPort).Returns("6379");
            _mockSiteInfo.SetupGet(s => s.RedisDB).Returns("0");
            _mockSiteInfo.SetupGet(s => s.RedisPassword).Returns(string.Empty);
            _processUtility.Setup(p => p.StartProcess(It.IsAny<ProcessStartInfo>())).Returns(true);
            RedisExecutor.ProcessUtility = _processUtility.Object;
            Environment.ExitCode = 0;
            UpdaterConfig.Configuration.GetSection("features")["SkipClearRedisCache"] = null;
        }

        [TearDown]
        public void TearDown()
        {
            RedisExecutor.ProcessUtility = null;
        }

        [Test]
        public void ClearRedisCache_WhenSkipClearRedisCacheFeatureDisabled_ReturnsTrue()
        {
            // Arrange
            UpdaterConfig.Configuration.GetSection("features")["SkipClearRedisCache"] = "false";
            _processUtility.Setup(p => p.StartProcess(It.IsAny<ProcessStartInfo>())).Returns(true).Callback(() => Environment.ExitCode = 0);
            RedisExecutor.ProcessUtility = _processUtility.Object;
            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ClearRedisCache_WhenSkipClearRedisCacheFeatureEnabled_ReturnsTrue()
        {
            // Arrange
            UpdaterConfig.Configuration.GetSection("features")["SkipClearRedisCache"] = "true";
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
            _processUtility.Setup(p => p.StartProcess(It.Is<ProcessStartInfo>(psi => psi.Arguments.Contains("-a password")))).Returns(true).Callback(() => Environment.ExitCode = 0);
            RedisExecutor.ProcessUtility = _processUtility.Object;
            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);
            // Assert
            Assert.That(result, Is.True);
            _processUtility.Verify();
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
        }

        [Test]
        public void ClearRedisCache_WithNullRedisServer_ReturnsFalse()
        {
            // Arrange
            _mockSiteInfo.SetupGet(s => s.RedisServer).Returns((string)null!);
            _processUtility.Setup(p => p.StartProcess(It.IsAny<ProcessStartInfo>())).Returns(false);
            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ClearRedisCache_WhenProcessUtilityFails_ReturnsFalse()
        {
            // Arrange
            UpdaterConfig.Configuration.GetSection("features")["SkipClearRedisCache"] = "false";
            _processUtility.Reset();
            _processUtility.Setup(p => p.StartProcess(It.IsAny<ProcessStartInfo>())).Returns(false);
            RedisExecutor.ProcessUtility = _processUtility.Object;
            // Act
            bool result = RedisExecutor.ClearRedisCache(_mockSiteInfo.Object);
            // Assert
            Assert.That(result, Is.False);
            _processUtility.Verify(p => p.StartProcess(It.IsAny<ProcessStartInfo>()), Times.Once);
        }

        #endregion

    }

    #endregion

}