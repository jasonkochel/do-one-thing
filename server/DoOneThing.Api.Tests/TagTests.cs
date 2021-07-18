using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using DoOneThing.Api.Services;
using FluentAssertions;
using Xunit;

namespace DoOneThing.Api.Tests
{
    public class TagTests
    {
        private readonly TagService _service;

        public TagTests()
        {
            var client = new AmazonDynamoDBClient();
            _service = new TagService(client);
        }

        [Fact]
        public async Task ShouldTestTags()
        {
            // arrange
            var listId = "1";
            var tag1 = "household";
            var tag2 = "contractor";
            var expected1 = new List<string> { tag1 };
            var expected2 = new List<string> { tag1, tag2 };
            var expected4 = new List<string> { tag2 };
            var expected5 = new List<string>();

            // act
            var actual1 = await _service.Add(listId, tag1);
            var actual2 = await _service.Add(listId, tag2);
            var actual3 = await _service.GetAllForList(listId);
            var actual4 = await _service.Delete(listId, tag1);
            var actual5 = await _service.Delete(listId, tag2);

            // assert
            actual1.Should().BeEquivalentTo(expected1);
            actual2.Should().BeEquivalentTo(expected2);
            actual3.Should().BeEquivalentTo(expected2);
            actual4.Should().BeEquivalentTo(expected4);
            actual5.Should().BeEquivalentTo(expected5);
        }
    }
}
