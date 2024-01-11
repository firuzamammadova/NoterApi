using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace NoterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordFileController : ControllerBase
    {
        private readonly IRecordFileService _service;

        public RecordFileController(IRecordFileService RecordFileService)
        {
            _service = RecordFileService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetById(string id)
        {

            var result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {

            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAncestorFolders()
        {

            var result = await _service.GetAncestorFolders();
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetChildrenOfFolder([FromQuery] Guid parentId)
        {

            var result = await _service.GetChildrenOfFolder(parentId);
            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddAsync([FromBody] RecordFile item)
        {
            var result = await _service.AddAsync(item);
            return Ok(result); ;
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(); ;
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateAsync([FromBody] RecordFile item)
        {
            await _service.UpdateAsync(item);
            return Ok(); ;
        }
    }
}
