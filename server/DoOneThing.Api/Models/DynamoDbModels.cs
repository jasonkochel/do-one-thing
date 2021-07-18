using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace DoOneThing.Api.Models
{
    [DynamoDBTable("DoOneThing")]
    public class Tags
    {
        public Tags() {}

        public Tags(string listId)
        {
            PK = $"TAGS#{listId}";
            SK = $"TAGS#{listId}";
            ListId = listId;
            TagList = new List<string>();
        }

        [DynamoDBHashKey]
        public string PK { get; set; }

        [DynamoDBRangeKey]
        public string SK { get; set; }

        public string ListId { get; set; }
        public List<string> TagList { get; set; }
    }

    [DynamoDBTable("DoOneThing")]
    public class TaskTags
    {
        public TaskTags() { }

        public TaskTags(string listId, string taskId, string tag)
        {
            PK = $"TASK#{taskId}";
            SK = $"TAG#{listId}#{tag}";
            ListId = listId;
            TaskId = taskId;
            Tag = tag;
        }

        [DynamoDBHashKey]
        public string PK { get; set; }

        [DynamoDBRangeKey]
        public string SK { get; set; }

        public string ListId { get; set; }
        public string TaskId { get; set; }
        public string Tag { get; set; }
    }

}
