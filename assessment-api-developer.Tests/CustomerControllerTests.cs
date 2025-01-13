using assessment_platform_developer.Controllers;
using assessment_platform_developer.Models;
using assessment_platform_developer.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;

public class CustomerControllerTests
{
    [Fact]
    public async Task GetCustomers_ReturnsAllCustomers()
    {
        var mockCustomerService = new Mock<ICustomerService>();
        mockCustomerService.Setup(service => service.GetAllCustomersAsync())
            .ReturnsAsync(new List<Customer>
            {
                new Customer { ID = 1, Name = "test", Email = "test@mail.com" },
                new Customer { ID = 2, Name = "test here", Email = "test@mail.com" }
            });

        var controller = new CustomersController(mockCustomerService.Object);

        var result = await controller.Get();

        var okResult = Assert.IsType<OkNegotiatedContentResult<IEnumerable<Customer>>>(result);
        var customers = okResult.Content.ToList();

        Assert.Equal(2, customers.Count);
        Assert.Equal("test", customers[0].Name);
        Assert.Equal("test here", customers[1].Name);
    }
}
