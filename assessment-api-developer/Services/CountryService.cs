using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using assessment_platform_developer.Models;

public interface ICountryService
{
    Task<IEnumerable<Country>> GetAllCountriesAsync();
    Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
}

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository), "CountryRepository is null");
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
        return await _countryRepository.GetAllCountriesAsync();
    }

    public async Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId)
    {
        return await _countryRepository.GetStatesByCountryAsync(countryId);
    }
}