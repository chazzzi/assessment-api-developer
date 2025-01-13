using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using assessment_platform_developer.Models;

public class CustomerRepository : ICustomerRepository
{
    private readonly AssessmentDbContext _context;

    public CustomerRepository(AssessmentDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context), "AssessmentDbContext cannot be null.");
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers
            .Select(c => new
            {
                c.ID,
                c.Name,
                c.Address,
                c.Email,
                c.Phone,
                c.City,
                c.StateID,
                c.Zip,
                c.CountryID,
                c.Notes,
                c.ContactName,
                c.ContactPhone,
                c.ContactEmail,
                c.ContactTitle,
                c.ContactNotes
            })
            .ToListAsync();

        return customers.Select(c => new Customer
        {
            ID = c.ID,
            Name = c.Name,
            Address = c.Address,
            Email = c.Email,
            Phone = c.Phone,
            City = c.City,
            StateID = c.StateID,
            Zip = c.Zip,
            CountryID = c.CountryID,
            Notes = c.Notes,
            ContactName = c.ContactName,
            ContactPhone = c.ContactPhone,
            ContactEmail = c.ContactEmail,
            ContactTitle = c.ContactTitle,
            ContactNotes = c.ContactNotes
        });
    }
    public async Task<Customer> GetCustomerAsync(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.State)
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        return customer;
    }
    public async Task AddCustomerAsync(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
        }

        try
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in AddCustomer: {ex.Message}");
            throw;
        }
    }
    public async Task UpdateCustomerAsync(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
        }

        var existingCustomer = await _context.Customers.FindAsync(customer.ID);
        if (existingCustomer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {customer.ID} not found.");
        }

        _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }
}