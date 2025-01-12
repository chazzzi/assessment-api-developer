using System;
using System.Collections.Generic;
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
        return _context.Customers.ToList();
    }

    public Customer GetCustomer(int id)
    {
        var customer = _context.Customers.Find(id);
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
            // Log the exception to debug the issue
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
            throw new KeyNotFoundException($"Customer with ID {customer.ID} not found for update.");
        }

        _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
        _context.SaveChanges();
    }

    public void DeleteCustomer(int id)
    {
        var customer = _context.Customers.Find(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found for deletion.");
        }

        _context.Customers.Remove(customer);
        _context.SaveChanges();
    }
}
