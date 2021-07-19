using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DoOneThing.Api.Models;

namespace DoOneThing.Api.Services
{
    public class GoogleTaskService
    {
        private readonly GoogleApiService _apiService;
        private readonly TaskTagService _taskTagService;

        public GoogleTaskService(GoogleApiService apiService, TaskTagService taskTagService)
        {
            _apiService = apiService;
            _taskTagService = taskTagService;
        }

        public async Task<List<GoogleTaskListModel>> GetTaskLists()
        {
            var resp = await _apiService.MakeRequest<GoogleResponseModel<GoogleTaskListModel>>(
                "https://tasks.googleapis.com/tasks/v1/users/@me/lists", HttpMethod.Get);

            return resp.items;
        }

        public async Task<List<GoogleTaskModel>> GetTasks(string listId, string tag)
        {
            var resp = await _apiService.MakeRequest<GoogleResponseModel<GoogleTaskModel>>(
                $"https://tasks.googleapis.com/tasks/v1/lists/{listId}/tasks", HttpMethod.Get);

            if (resp.items != null && resp.items.Any() && !string.IsNullOrEmpty(tag))
            {
                var tasksWithTag = (await _taskTagService.GetTasksForTag(listId, tag)) ?? new List<string>();
                resp.items.RemoveAll(i => !tasksWithTag.Contains(i.id));
            }

            return resp.items;
        }
    }
}
