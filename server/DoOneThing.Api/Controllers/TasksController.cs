using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoOneThing.Api.Models;
using DoOneThing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoOneThing.Api.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<List<GoogleTaskListModel>> GetLists()
        {
            return await _taskService.GetTaskLists();
        }

        [HttpGet("{listId}")]
        public async Task<List<GoogleTaskModel>> GetTasks(string listId, [FromQuery] string tag = null)
        {
            // TODO: filter by tag
            return await _taskService.GetTasks(listId);
        }

        [HttpGet("{listId}/tasks/{taskId}/tags")]
        public async Task<IEnumerable<string>> AddTag(string listId, string taskId)
        {
            return await _taskTagService.GetTagsForTask(listId, taskId);
        }

        [HttpPost("{listId}/tasks/{taskId}/tags")]
        public async Task<IEnumerable<string>> AddTag(string listId, string taskId, [FromQuery] string tag)
        {
            return await _taskTagService.AddTagToTask(listId, taskId, tag);
        }

        [HttpDelete("{listId}/tasks/{taskId}/tags")]
        public async Task<IEnumerable<string>> RemoveTag(string listId, string taskId, [FromQuery] string tag)
        {
            return await _taskTagService.RemoveTagFromTask(taskId, tag);
        }
    }
}
