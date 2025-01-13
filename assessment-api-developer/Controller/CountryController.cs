using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace assessment_platform_developer.Controllers
{
    public class CountryController : ApiController
    {
        private readonly ICountryService _countryService;

        public CountryController()
        {
            var container = (SimpleInjector.Container)HttpContext.Current.Application["DIContainer"];
            _countryService = container.GetInstance<ICountryService>();
        }

        [HttpGet]
        [Route("api/countries")]
        public async Task<IHttpActionResult> GetCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries.Select(c => new { c.ID, c.Name }));
        }

        [HttpGet]
        [Route("api/countries/{countryId}/states")]
        public async Task<IHttpActionResult> GetStatesByCountry(int countryId)
        {
            var states = await _countryService.GetStatesByCountryAsync(countryId);
            return Ok(states.Select(s => new { s.ID, s.Name }));
        }
    }
}