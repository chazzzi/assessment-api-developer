using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using assessment_platform_developer.Models;

public class CountryRepository : ICountryRepository
{
    private readonly AssessmentDbContext _context;

    public CountryRepository(AssessmentDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context), "AssessmentDbContext cannot be null.");
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
        return await _context.Countries.ToListAsync();
    }

    public async Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId)
    {
        return await _context.States
            .Where(s => s.CountryID == countryId)
            .ToListAsync();
    }
}