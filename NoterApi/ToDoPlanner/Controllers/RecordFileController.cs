﻿using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace NoterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetAllStarred()
        {

            var result = await _service.GetAllStarredAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetRecentFiles()
        {

            var result = await _service.GetRecentFilesAsync();
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
        public async Task<IActionResult> GetChildrenOfFolder([FromQuery] string parentId)
        {

            var result = await _service.GetChildrenOfFolder(new Guid(parentId));
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GoBackGetChildren([FromQuery] string parentId)
        {

            var result = await _service.GoBackGetChildren(parentId);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ToggleStarredOfFile([FromQuery] string id)
        {

            await _service.ToggleStarFile(id);
            return Ok();
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UpdateLastOpenedDate([FromQuery] string id)
        {

            await _service.UpdateLastOpenedDate(id);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddAsync([FromBody] RecordFile item)
        {
            var result = await _service.AddAsync(item);
            return Ok(result); ;
        }
        [HttpPost]
        [Route("[action]")]
        public  IActionResult SaveNote([FromBody] Object item)
        {
           string a =item.ToString();
            return Ok(); ;
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> AddContextAsync([FromBody] RecordFile item)
        {
            await _service.AddContext(item);
            return Ok(); ;
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string id)
        {
            await _service.DeleteAsync(new Guid(id));
            return Ok(); ;
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SearchAsync([FromQuery] string key)
        {
           var result= await _service.SearchFiles(key);
            return Ok(result); ;
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
