using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DoOneThing.Api.Models;

namespace DoOneThing.Api.Services
{
    public class TaskTagService
    {
        private readonly DynamoDBContext _db;

        public TaskTagService(IAmazonDynamoDB client)
        {
            _db = new DynamoDBContext(client, new DynamoDBContextConfig
            {
                Conversion = DynamoDBEntryConversion.V2
            });
        }

        public async Task<IEnumerable<string>> GetTagsForTask(string listId, string taskId)
        {
            var tags = await LoadFromDb(taskId) ?? new List<TaskTags>();
            return tags.Select(t => t.Tag);
        }

        public async Task<IEnumerable<string>> GetTasksForTag(string listId, string tag)
        {
            var tags = await QueryByTag(listId, tag) ?? new List<TaskTags>();
            return tags.Select(t => t.TaskId);
        }

        public async Task<IEnumerable<string>> AddTagToTask(string listId, string taskId, string tag)
        {
            var tags = await LoadFromDb(taskId) ?? new List<TaskTags>();

            if (!tags.Exists(tt => tt.Tag == tag))
            {
                var data = new TaskTags(listId, taskId, tag);

                await _db.SaveAsync(data);

                tags.Add(data);
            }

            return tags.Select(t => t.Tag);
        }

        public async Task<IEnumerable<string>> RemoveTagFromTask(string taskId, string tag)
        {
            var tags = await LoadFromDb(taskId) ?? new List<TaskTags>();
            var tagToDelete = tags.Find(tt => tt.Tag == tag);

            if (tagToDelete != null)
            {
                await _db.DeleteAsync(tagToDelete);
                tags.Remove(tagToDelete);
            }

            return tags.Select(t => t.Tag);
        }

        private async Task<List<TaskTags>> LoadFromDb(string taskId) =>
            await _db.QueryAsync<TaskTags>($"TASK#{taskId}", QueryOperator.BeginsWith, new[] {"TAG#"})
                .GetRemainingAsync();

        private async Task<List<TaskTags>> QueryByTag(string listId, string tag) =>
            await _db.QueryAsync<TaskTags>($"TAG#{listId}#{tag}", QueryOperator.BeginsWith, new[] {"TASK#"},
                new DynamoDBOperationConfig
                {
                    IndexName = "GSI1"
                }).GetRemainingAsync();
    }
}