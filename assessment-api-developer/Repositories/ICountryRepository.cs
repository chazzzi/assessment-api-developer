using System.Collections.Generic;
using assessment_platform_developer.Models;

public interface ICountryRepository
{
    IEnumerable<Country> GetAllCountries();
    IEnumerable<State> GetStatesByCountry(int countryId);
}