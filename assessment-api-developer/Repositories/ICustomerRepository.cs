using System.Collections.Generic;
using assessment_platform_developer.Models;
public interface ICustomerRepository
{
    IEnumerable<Customer> GetAllCustomers();
    Customer GetCustomer(int id);
    void AddCustomer(Customer customer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(int id);
}
