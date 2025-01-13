using System.Collections.Generic;
using assessment_platform_developer.Models;
public interface ICountryService
{
    IEnumerable<Country> GetAllCountries();
    IEnumerable<State> GetStatesByCountry(int countryId);
}

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public IEnumerable<Country> GetAllCountries()
    {
        return _countryRepository.GetAllCountries();
    }

    public IEnumerable<State> GetStatesByCountry(int countryId)
    {
        return _countryRepository.GetStatesByCountry(countryId);
    }
}