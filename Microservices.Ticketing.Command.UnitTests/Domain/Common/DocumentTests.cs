using Ticketing.Command.Domain.Common;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Domain.Common
{
    public class DocumentTests
    {
        [Fact]
        public void Document_Constructor_SetsId()
        {
            // Arrange
            var objectId = MongoDB.Bson.ObjectId.GenerateNewId();
            var document = new Document
            {
                Id = objectId
            };
            // Act
            var result = document.Id;
            // Assert
            Assert.Equal(objectId, result);
        }
    }
}
