using Ticketing.Command.Application.Models;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Application.Models
{
    public class MongoSettingsTests
    {
        [Fact]
        public void MongoSettings_Properties_AreSetCorrectly()
        {
            // Arrange
            var mongoSettings = new MongoSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                Database = "TestDatabase"
            };
            // Act & Assert
            Assert.Equal("mongodb://localhost:27017", mongoSettings.ConnectionString);
            Assert.Equal("TestDatabase", mongoSettings.Database);
        }
    }
}
