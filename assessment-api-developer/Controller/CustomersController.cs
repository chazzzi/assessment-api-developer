using assessment_platform_developer.Models;
using assessment_platform_developer.Services;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace assessment_platform_developer.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;

        public CustomersController()
        {
            // Dependency injection should be configured
            var container = (SimpleInjector.Container)HttpContext.Current.Application["DIContainer"];
            _customerService = container.GetInstance<ICustomerService>();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _customerService.AddCustomerAsync(customer);
            return CreatedAtRoute("DefaultApi", new { id = customer.ID }, customer);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put(int id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCustomer = await _customerService.GetCustomerAsync(id);
            if (existingCustomer == null)
                return NotFound();

            customer.ID = id;
            await _customerService.UpdateCustomerAsync(customer);
            return Ok(customer);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
                return NotFound();

            await _customerService.DeleteCustomerAsync(id);
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}
