using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace NoterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileTypeController : ControllerBase
    {
        private readonly IFileTypeService _service;

        public FileTypeController(IFileTypeService fileTypeService)
        {
            _service = fileTypeService;
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddAsync([FromBody] FileType item)
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
        public async Task<IActionResult> UpdateAsync([FromBody] FileType item)
        {
             await _service.UpdateAsync(item);
            return Ok(); ;
        }
    }
}
