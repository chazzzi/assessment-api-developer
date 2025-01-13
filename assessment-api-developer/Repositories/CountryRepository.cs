using System.Collections.Generic;
using System.Linq;
using assessment_platform_developer.Models;

public class CountryRepository : ICountryRepository
{
    private readonly AssessmentDbContext _context;

    public CountryRepository(AssessmentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Country> GetAllCountries()
    {
        return _context.Countries.ToList();
    }

    public IEnumerable<State> GetStatesByCountry(int countryId)
    {
        return _context.States.Where(s => s.CountryID == countryId).ToList();
    }
}