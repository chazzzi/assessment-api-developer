using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using assessment_platform_developer.Models;

public class CustomerRepository : ICustomerRepository
{
    private readonly AssessmentDbContext _context;

    public CustomerRepository(AssessmentDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context), "AssessmentDbContext cannot be null.");
    }

    public IEnumerable<Customer> GetAllCustomers()
    {
        var customers = _context.Customers
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
            .ToList()
            .Select(c => new Customer
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

        return customers;
    }

    public Customer GetCustomer(int id)
    {
        var customer = _context.Customers
            .Include(c => c.State)  
            .Include(c => c.Country) 
            .FirstOrDefault(c => c.ID == id);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        return customer;
    }

    public void AddCustomer(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
        }

        try
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in AddCustomer: {ex.Message}");
            throw;
        }
    }

    public void UpdateCustomer(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
        }

        var existingCustomer = _context.Customers.Find(customer.ID);
        if (existingCustomer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {customer.ID} not found.");
        }

        _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
        _context.SaveChanges();
    }

    public void DeleteCustomer(int id)
    {
        var customer = _context.Customers.Find(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        _context.Customers.Remove(customer);
        _context.SaveChanges();
    }
}
