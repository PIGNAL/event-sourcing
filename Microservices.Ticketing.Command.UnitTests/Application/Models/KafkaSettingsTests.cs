using Ticketing.Command.Application.Models;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Application.Models
{
    public class KafkaSettingsTests 
    {

        [Fact]
        public void KafkaSettings_Properties_AreSetCorrectly()
        {
            // Arrange
            var kafkaSettings = new KafkaSettings
            {
                Hostname = "localhost",
                Port = "9092",
                Topic = "test-topic"
            };
            // Act & Assert
            Assert.Equal("localhost", kafkaSettings.Hostname);
            Assert.Equal("9092", kafkaSettings.Port);
            Assert.Equal("test-topic", kafkaSettings.Topic);
        }
    }
}
