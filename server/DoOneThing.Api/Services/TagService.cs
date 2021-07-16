using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace DoOneThing.Api.Services
{
    [DynamoDBTable("DoOneThing")]
    public class Tags
    {
        public string PK { get; set; }
        public string SK { get; set; }
        public List<string> TagList { get; set; }
    }

    public class TagService
    {
        private readonly DynamoDBContext _db;

        public TagService(IAmazonDynamoDB client)
        {
            _db = new DynamoDBContext(client, new DynamoDBContextConfig
            {
                Conversion = DynamoDBEntryConversion.V2
            });
        }

        public async Task<List<string>> GetAllForList(string listId)
        {
            var data = await LoadFromDb(listId);
            return data?.TagList;
        }

        public async Task<List<string>> Add(string listId, string tag)
        {
            var data = await LoadFromDb(listId) ?? new Tags
            {
                PK = $"TAGS#{listId}",
                SK = $"TAGS#{listId}",
                TagList = new List<string>()
            };

            if (!data.TagList.Contains(tag))
            {
                data.TagList.Add(tag);
            }

            await _db.SaveAsync(data);

            return data.TagList;
        }


        public async Task<List<string>> Delete(string listId, string tag)
        {
            var data = await LoadFromDb(listId);

            if (data == null)
            {
                return null;
            }

            if (data.TagList.Remove(tag))
            {
                await _db.SaveAsync(data);
            }


            return data.TagList;
        }

        private async Task<Tags> LoadFromDb(string listId) => await _db.LoadAsync<Tags>($"TAGS#{listId}", $"TAGS#{listId}");
    }
}