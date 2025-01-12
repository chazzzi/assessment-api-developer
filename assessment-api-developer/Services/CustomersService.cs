using assessment_platform_developer.Models;
using System;
using System.Collections.Generic;

namespace assessment_platform_developer.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomer(int id);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        // Constructor injection for the repository
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository), "CustomerRepository is null");
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        public Customer GetCustomer(int id)
        {
            var customer = _customerRepository.GetCustomer(id);
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
            _customerRepository.AddCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
            }

            var existingCustomer = _customerRepository.GetCustomer(customer.ID);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customer.ID} not found for update.");
            }

            _customerRepository.UpdateCustomer(customer);
        }

        public void DeleteCustomer(int id)
        {
            var existingCustomer = _customerRepository.GetCustomer(id);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found for deletion.");
            }

            _customerRepository.DeleteCustomer(id);
        }
    }
}
