using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using NoterApi.Controllers;
using Repository.CQRS.Commands;
using Repository.CQRS.Queries;
using Repository.Infrastructure;
using Repository.Repositories;
using Service.Services;

namespace NoterApi.RecordFile.UnitTests
{
    public class RecordFileControllerTests
    {
        private readonly Mock<IRecordFileService> _service;
        private readonly IRecordFileService service;
        private readonly IRecordFileRepository _repository;


        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;

        private readonly RecordFileController _controller;


        public RecordFileControllerTests()
        {

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyYTQxNGZkZi1lMGIxLTQ5NmQtYWJhYS03MDUxMDFlYWQyZWUiLCJnaXZlbl9uYW1lIjoiYWRtaW4iLCJqdGkiOiIxNWNjM2RlYy03NjQwLTRlZjMtOWQwOS1mOTVkNjU3NjVmMWQiLCJpYXQiOiIxNzEyNjAyODU3IiwibmJmIjoxNzEyNjAyODU3LCJleHAiOjE3MTYyMDI4NTcsImlzcyI6Imh0dHA6Ly93d3cucGhhcm1hY3kuY29tLyIsImF1ZCI6Imh0dHA6Ly93d3cucGhhcm1hY3kuY29tLyJ9.-2yJaj1aY_ETzFwtnuAeWMGY7WrQ8C78DL652Tu3BUE";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity());
            _service = new Mock<IRecordFileService>();
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            var repo = new Mock<IRecordFileRepository>();
            var repo2 = new Mock<IFileTypeRepository>();

            var command = new Mock<IRecordFileCommand>();
            var query = new Mock<IRecordFileQuery>();


            var uof = new Mock<IUnitOfWork>();

            _repository = new RecordFileRepository(command.Object, query.Object);

            _service.Setup(_ => _.setIHttpContextAccessor(mockHttpContextAccessor.Object));
            _service.Setup(_ => _.getUserId()).Returns("2A414FDF-E0B1-496D-ABAA-705101EAD2EE");

            service = new RecordFileService(repo2.Object, repo.Object, uof.Object, mockHttpContextAccessor.Object);
            _controller = new RecordFileController(_service.Object);
        }



        [Test]
        public async Task getssbtlist_on_model_error_returns_badrequest()
        {

            // ARRANGE
            // _controller.ModelState.AddModelError("Something", "Error");

            // ACT
            var controllerResponse = await _controller.GetAll();
            var response = await _repository.GetAllAsync("2A414FDF-E0B1-496D-ABAA-705101EAD2EE");
            // ASSERT
            //  Assert.IsInstanceOfType(controllerResponse, typeof(BadRequestObjectResult));
        }

        //[TestMethod]
        //public async Task getssbtlist_on_service_exception_returns_servererror()
        //{
        //    // ARRANGE
        //    _ssbtServiceMock
        //        .Setup(u => u.SsbtList(It.IsAny<int>()))
        //        .ThrowsAsync(new Exception());

        //    // ACT
        //    var controllerResponse = await _controller.GetSsbtList(_ssbtListRequest);

        //    // ASSERT
        //    Assert.IsInstanceOfType(controllerResponse, typeof(StatusCodeResult));
        //    Assert.AreEqual(StatusCodes.Status500InternalServerError, ((StatusCodeResult)controllerResponse).StatusCode);
        //}

    }
}
