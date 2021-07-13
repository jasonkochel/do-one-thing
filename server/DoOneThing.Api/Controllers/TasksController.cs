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
        private readonly GoogleTaskService _service;

        public TasksController(GoogleTaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<GoogleTaskListModel>> Get()
        {
            return await _service.GetTaskLists();
        }

        [HttpGet("{listId}")]
        public async Task<List<GoogleTaskModel>> Get(string listId)
        {
            return await _service.GetTasks(listId);
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id:int}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}
