using System.Web.Http;
using System.Linq;
using System.Web;

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
        public IHttpActionResult GetCountries()
        {
            var countries = _countryService.GetAllCountries();
            return Ok(countries.Select(c => new { c.ID, c.Name }));
        }

        [HttpGet]
        [Route("api/countries/{countryId}/states")]
        public IHttpActionResult GetStatesByCountry(int countryId)
        {
            var states = _countryService.GetStatesByCountry(countryId);
            return Ok(states.Select(s => new { s.ID, s.Name }));
        }
    }
}