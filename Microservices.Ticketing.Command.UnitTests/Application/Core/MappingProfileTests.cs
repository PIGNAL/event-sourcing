using AutoMapper;
using Ticketing.Command.Application.Core;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Application.Core
{
    public class MappingProfileTests
    {

        [Fact]
        public void MappingProfile_Configuration_IsValid()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            // Act
            var mappingExpression = config.CreateMapper();
            // Assert
            Assert.NotNull(mappingExpression);
        }
    }
}
