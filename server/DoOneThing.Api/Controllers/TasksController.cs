using System.Collections.Generic;
using System.Threading.Tasks;
using DoOneThing.Api.Models;
using DoOneThing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoOneThing.Api.Controllers
{
    [Route("api/lists/{listId}/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly GoogleTaskService _taskService;
        private readonly TaskTagService _taskTagService;

        public TasksController(GoogleTaskService taskService, TaskTagService taskTagService)
        {
            _taskService = taskService;
            _taskTagService = taskTagService;
        }

        [HttpGet]
        public async Task<List<GoogleTaskModel>> GetTasks(string listId, [FromQuery] string tag = null)
        {
            return await _taskService.GetTasks(listId, tag);
        }

        [HttpGet("{taskId}/tags")]
        public async Task<IEnumerable<string>> AddTag(string listId, string taskId)
        {
            return await _taskTagService.GetTagsForTask(listId, taskId);
        }

        [HttpPost("{taskId}/tags")]
        public async Task<IEnumerable<string>> AddTag(string listId, string taskId, [FromQuery] string tag)
        {
            return await _taskTagService.AddTagToTask(listId, taskId, tag);
        }

        [HttpDelete("{taskId}/tags")]
        public async Task<IEnumerable<string>> RemoveTag(string listId, string taskId, [FromQuery] string tag)
        {
            return await _taskTagService.RemoveTagFromTask(taskId, tag);
        }
    }
}
