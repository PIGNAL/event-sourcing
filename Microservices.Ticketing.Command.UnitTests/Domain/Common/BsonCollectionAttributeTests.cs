using Ticketing.Command.Domain.Common;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Domain.Common
{
    public class BsonCollectionAttributeTests
    {
        [Fact]
        public void BsonCollectionAttribute_Constructor_SetsCollectionName()
        {
            // Arrange
            var collectionName = "TestCollection";
            var attribute = new BsonCollectionAttribute(collectionName);
            // Act
            var result = attribute.CollectionName;
            // Assert
            Assert.Equal(collectionName, result);
        }
    }
}
