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
using Service.Services;

namespace NoterApi.RecordFile.UnitTests
{
    [TestClass]
    public class RecordFileControllerTests
    {
        private readonly Mock<IRecordFileService> _service;
        private readonly RecordFileController _controller;


        public RecordFileControllerTests()
        {
            _service = new Mock<IRecordFileService>();

            _controller = new RecordFileController(_service.Object);
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
