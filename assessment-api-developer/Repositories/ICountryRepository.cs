using System.Collections.Generic;
using System.Threading.Tasks;
using assessment_platform_developer.Models;
public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAllCountriesAsync();
    Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
}