using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NoterApi.Controllers;
using Repository.Infrastructure;
using Repository.Repositories;
using Service.Services;

namespace Test
{
    [TestClass]
    public class RecordFileServiceTests
    {
        private readonly Mock<IRecordFileRepository> _repo;
        private readonly Mock<IFileTypeRepository> _typerepo;
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<IHttpContextAccessor> _context;


        private readonly RecordFileService _service;


        public RecordFileServiceTests()
        {
            _repo = new Mock<IRecordFileRepository>();
            _typerepo = new Mock<IFileTypeRepository>();
            _uow = new Mock<IUnitOfWork>();
            _context = new Mock<IHttpContextAccessor>();


            _service = new RecordFileService(_typerepo.Object,_repo.Object,_uow.Object,_context.Object);
        }



        [TestMethod]
        public async Task getssbtlist_on_model_error_returns_badrequest()
        {
            // ARRANGE
            _controller.ModelState.AddModelError("Something", "Error");

            // ACT
            var controllerResponse = await _controller.GetAll();

            // ASSERT
            Assert.IsInstanceOfType(controllerResponse, typeof(BadRequestObjectResult));
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
