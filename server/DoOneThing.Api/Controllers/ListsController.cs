using System.Collections.Generic;
using System.Threading.Tasks;
using DoOneThing.Api.Models;
using DoOneThing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoOneThing.Api.Controllers
{
    [Route("api/[controller]")]
    public class ListsController : ControllerBase
    {
        private readonly GoogleTaskService _taskService;

        public ListsController(GoogleTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<List<GoogleTaskListModel>> GetLists()
        {
            return await _taskService.GetTaskLists();
        }
    }
}