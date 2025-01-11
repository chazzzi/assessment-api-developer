using assessment_platform_developer.Models;
using assessment_platform_developer.Services;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace assessment_platform_developer.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly ICustomerService customerService;

        public CustomersController()
        {
            // Dependency injection should be configured
            var container = (SimpleInjector.Container)HttpContext.Current.Application["DIContainer"];
            customerService = container.GetInstance<ICustomerService>();
        }

        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return customerService.GetAllCustomers();
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var customer = customerService.GetCustomer(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            customerService.AddCustomer(customer);
            return CreatedAtRoute("DefaultApi", new { id = customer.ID }, customer);
        }

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existingCustomer = customerService.GetCustomer(id);
            if (existingCustomer == null) return NotFound();
            customer.ID = id;
            customerService.UpdateCustomer(customer);
            return Ok(customer);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var customer = customerService.GetCustomer(id);
            if (customer == null) return NotFound();
            customerService.DeleteCustomer(id);
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}
