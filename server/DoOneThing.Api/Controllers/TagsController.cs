using System.Collections.Generic;
using System.Threading.Tasks;
using DoOneThing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoOneThing.Api.Controllers
{
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly TagService _service;

        public TagsController(TagService service)
        {
            _service = service;
        }

        [HttpGet("listId")]
        public async Task<List<string>> Get(string listId)
        {
            return await _service.GetAllForList(listId);
        }

        [HttpPost("{listId}/{tag}")]
        public async Task Post(string listId, string tag)
        {
            await _service.Add(listId, tag);
        }

        [HttpDelete("{listId}/{tag}")]
        public async Task Delete(string listId, string tag)
        {
            await _service.Delete(listId, tag);
        }
    }
}