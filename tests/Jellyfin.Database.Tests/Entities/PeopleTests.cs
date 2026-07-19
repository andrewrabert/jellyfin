using System;
using System.Collections.Generic;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class PeopleTests
    {
        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var id = Guid.NewGuid();
            var people = new People { Id = id, Name = "Jane Doe" };

            Assert.Equal(id, people.Id);
            Assert.Equal("Jane Doe", people.Name);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var people = new People { Id = Guid.NewGuid(), Name = "Jane Doe" };

            Assert.Null(people.PersonType);
            Assert.Null(people.BaseItems);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var baseItems = new List<PeopleBaseItemMap>();
            var people = new People
            {
                Id = Guid.NewGuid(),
                Name = "Jane Doe",
                PersonType = "Actor",
                BaseItems = baseItems
            };

            Assert.Equal("Actor", people.PersonType);
            Assert.Same(baseItems, people.BaseItems);
        }
    }
}
