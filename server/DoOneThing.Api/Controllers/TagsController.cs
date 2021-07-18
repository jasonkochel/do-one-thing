using System.Collections.Generic;
using System.Threading.Tasks;
using DoOneThing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoOneThing.Api.Controllers
{
    [Route("api/lists/{listId}/[controller]/")]
    public class TagsController : ControllerBase
    {
        private readonly TagService _service;

        public TagsController(TagService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<string>> Get(string listId)
        {
            return await _service.GetAllForList(listId);
        }

        [HttpPost("{tag}")]
        public async Task<List<string>> Post(string listId, string tag)
        {
           return await _service.Add(listId, tag);
        }

        [HttpDelete("{tag}")]
        public async Task<List<string>> Delete(string listId, string tag)
        {
            return await _service.Delete(listId, tag);
        }
    }
}