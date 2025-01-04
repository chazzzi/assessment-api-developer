using assessment_platform_developer.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace assessment_platform_developer.Controllers
{
    [RoutePrefix("api/customers")] 
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers); 
        }
    }
}
