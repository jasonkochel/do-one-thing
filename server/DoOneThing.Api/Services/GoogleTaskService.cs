using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DoOneThing.Api.Models;

namespace DoOneThing.Api.Services
{
    public class GoogleTaskService
    {
        private readonly GoogleApiService _apiService;

        public GoogleTaskService(GoogleApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<GoogleTaskListModel>> GetTaskLists()
        {
            var resp = await _apiService.MakeRequest<GoogleResponseModel<GoogleTaskListModel>>(
                "https://tasks.googleapis.com/tasks/v1/users/@me/lists", HttpMethod.Get);

            return resp.items;
        }

        public async Task<List<GoogleTaskModel>> GetTasks(string listId)
        {
            var resp = await _apiService.MakeRequest<GoogleResponseModel<GoogleTaskModel>>(
                $"https://tasks.googleapis.com/tasks/v1/lists/{listId}/tasks", HttpMethod.Get);

            return resp.items;
        }
    }
}
