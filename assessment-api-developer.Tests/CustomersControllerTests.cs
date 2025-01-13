using Microsoft.VisualStudio.TestTools.UnitTesting;
using assessment_platform_developer.Controllers;
using assessment_platform_developer.Models;
using assessment_platform_developer.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace assessment_platform_developer.Tests
{
    [TestClass]
    public class CustomersControllerTests
    {
        private Mock<ICustomerService> _mockCustomerService;
        private CustomersController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _controller = new CustomersController(_mockCustomerService.Object);
        }

        [TestMethod]
        public async Task GetCustomers_ShouldReturnAllCustomers()
        {
            var mockCustomers = new List<Customer>
            {
                new Customer { ID = 1, Name = "test ", Email = "test@sample.com" },
                new Customer { ID = 2, Name = "test here", Email = "test@mail.com" }
            };

            _mockCustomerService.Setup(service => service.GetAllCustomersAsync())
                .ReturnsAsync(mockCustomers);

            IHttpActionResult actionResult = await _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<Customer>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);

            var contentList = contentResult.Content.ToList();
            Assert.AreEqual(2, contentList.Count);
        }

        [TestMethod]
        public async Task GetCustomers_ShouldReturnNotFound_WhenNoCustomersExist()
        {
            _mockCustomerService.Setup(service => service.GetAllCustomersAsync())
                .ReturnsAsync(new List<Customer>());

            IHttpActionResult actionResult = await _controller.Get();

            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IEnumerable<Customer>>));
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<Customer>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);

            var contentList = contentResult.Content.ToList();
            Assert.AreEqual(0, contentList.Count);
        }

        [TestMethod]
        public async Task GetCustomerById_ShouldReturnCustomer()
        {
            var mockCustomer = new Customer
            {
                ID = 1,
                Name = "test",
                Email = "test@sample.com"
            };

            _mockCustomerService.Setup(service => service.GetCustomerAsync(1))
                .ReturnsAsync(mockCustomer);

            IHttpActionResult actionResult = await _controller.Get(1);
            var contentResult = actionResult as OkNegotiatedContentResult<Customer>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(1, contentResult.Content.ID);
            Assert.AreEqual("test", contentResult.Content.Name);
            Assert.AreEqual("test@sample.com", contentResult.Content.Email);
        }

        [TestMethod]
        public async Task GetCustomerById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            _mockCustomerService.Setup(service => service.GetCustomerAsync(It.IsAny<int>()))
                .ReturnsAsync((Customer)null);

            IHttpActionResult actionResult = await _controller.Get(999);

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }
    }
}
