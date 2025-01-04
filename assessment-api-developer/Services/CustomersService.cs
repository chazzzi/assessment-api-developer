using assessment_platform_developer.Models;
using assessment_platform_developer.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace assessment_platform_developer.Services
{
	public interface ICustomerService
	{
        Task<IEnumerable<Customer>> GetAllAsync();
        Customer GetCustomer(int id);
		void AddCustomer(Customer customer);
		void UpdateCustomer(Customer customer);
		void DeleteCustomer(int id);
	}

	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository customerRepository;

		public CustomerService(ICustomerRepository customerRepository)
		{
			this.customerRepository = customerRepository;
		}

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await Task.FromResult(customerRepository.GetAllAsync());
        }

        public Customer GetCustomer(int id)
		{
			return customerRepository.Get(id);
		}

		public void AddCustomer(Customer customer)
		{
			customerRepository.Add(customer);
		}

		public void UpdateCustomer(Customer customer)
		{
			customerRepository.Update(customer);
		}

		public void DeleteCustomer(int id)
		{
			customerRepository.Delete(id);
		}
	}

}