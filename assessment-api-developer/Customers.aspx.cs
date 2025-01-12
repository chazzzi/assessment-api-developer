using assessment_platform_developer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Container = SimpleInjector.Container;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace assessment_platform_developer
{
	public partial class Customers : Page
	{
		private static List<Customer> customers = new List<Customer>();

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await PopulateCustomerListBox();
                PopulateCustomerDropDownLists();
            }
        }

        private async Task<List<Customer>> FetchCustomersFromApi()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44358/api/");

                var response = await client.GetAsync("customers");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var customers = JsonConvert.DeserializeObject<List<Customer>>(jsonString);
                    return customers;
                }
                else
                {
                    return new List<Customer>();
                }
            }
        }

        private void PopulateCustomerDropDownLists()
		{

			var countryList = Enum.GetValues(typeof(Countries))
				.Cast<Countries>()
				.Select(c => new ListItem
				{
					Text = c.ToString(),
					Value = ((int)c).ToString()
				})
				.ToArray();

			CountryDropDownList.Items.AddRange(countryList);
			CountryDropDownList.SelectedValue = ((int)Countries.Canada).ToString();


			var provinceList = Enum.GetValues(typeof(CanadianProvinces))
				.Cast<CanadianProvinces>()
				.Select(p => new ListItem
				{
					Text = p.ToString(),
					Value = ((int)p).ToString()
				})
				.ToArray();

			StateDropDownList.Items.Add(new ListItem(""));
			StateDropDownList.Items.AddRange(provinceList);
		}

        protected async Task PopulateCustomerListBox()
        {
            CustomersDDL.Items.Clear();

            try
            {
                // Fetch customers from the API
                var allCustomers = await FetchCustomersFromApi();

                if (allCustomers.Any())
                {
                    // Populate the dropdown with customer data
                    foreach (var customer in allCustomers)
                    {
                        CustomersDDL.Items.Add(new ListItem(customer.Name, customer.ID.ToString()));
                    }

                    // Optionally, set the first item as default
                    CustomersDDL.SelectedIndex = 0;
                }
                else
                {
                    // Add a default option if no customers are available
                    CustomersDDL.Items.Add(new ListItem("No customers available", ""));
                }
            }
            catch (Exception ex)
            {
                // Handle API errors
                CustomersDDL.Items.Add(new ListItem($"Error: {ex.Message}", ""));
            }
        }

        protected async void CustomersDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomersDDL.SelectedValue))
            {
                int customerId;
                if (int.TryParse(CustomersDDL.SelectedValue, out customerId))
                {
                    try
                    {
                        // Fetch customer details from the API by ID
                        var selectedCustomer = await FetchCustomerByIdFromApi(customerId);

                        if (selectedCustomer != null)
                        {
                            // Populate fields
                            CustomerName.Text = selectedCustomer.Name;
                            CustomerAddress.Text = selectedCustomer.Address;
                            CustomerEmail.Text = selectedCustomer.Email;
                            CustomerPhone.Text = selectedCustomer.Phone;
                            CustomerCity.Text = selectedCustomer.City;
                            StateDropDownList.SelectedValue = selectedCustomer.State;
                            CustomerZip.Text = selectedCustomer.Zip;
                            CountryDropDownList.SelectedValue = selectedCustomer.Country;
                            CustomerNotes.Text = selectedCustomer.Notes;
                            ContactName.Text = selectedCustomer.ContactName;
                            ContactPhone.Text = selectedCustomer.ContactPhone;
                            ContactEmail.Text = selectedCustomer.ContactEmail;
                        }
                        else
                        {
                            Response.Write("<script>alert('Customer not found');</script>");
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write($"<script>alert('Error fetching customer: {ex.Message}');</script>");
                    }
                }
            }
        }


        private async Task<Customer> FetchCustomerByIdFromApi(int customerId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44358/api/");

                var response = await client.GetAsync($"customers/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Customer>(jsonString);
                }
                else
                {
                    throw new Exception($"Failed to fetch customer: {response.ReasonPhrase}");
                }
            }
        }

        protected async void AddButton_Click(object sender, EventArgs e)
        {
			var customer = new Customer
			{
				Name = CustomerName.Text,
				Address = CustomerAddress.Text,
				City = CustomerCity.Text,
				State = StateDropDownList.SelectedValue,
				Zip = CustomerZip.Text,
				Country = CountryDropDownList.SelectedValue,
				Email = CustomerEmail.Text,
				Phone = CustomerPhone.Text,
				Notes = CustomerNotes.Text,
				ContactName = ContactName.Text,
				ContactPhone = CustomerPhone.Text,
				ContactEmail = CustomerEmail.Text
			};

            string apiUrl = "https://localhost:44358/api/customers";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    string jsonData = JsonConvert.SerializeObject(customer);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var contents =  content.ReadAsStringAsync().Result;
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        customers.Add(customer);
                        CustomersDDL.Items.Add(new ListItem(customer.Name));

                        Response.Write("<script>alert('Customer added successfully!');</script>");
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Response.Write($"<script>alert('Failed to add customer: {errorContent}');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or display the error
                Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
            }
        }
        protected async void UpdateButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomersDDL.SelectedValue))
            {
                string selectedCustomerName = CustomersDDL.SelectedItem.Text;
                var selectedCustomer = customers.FirstOrDefault(c => c.Name == selectedCustomerName);

                if (selectedCustomer != null)
                {
                    // Update customer object with form data
                    selectedCustomer.Name = CustomerName.Text;
                    selectedCustomer.Address = CustomerAddress.Text;
                    selectedCustomer.Email = CustomerEmail.Text;
                    selectedCustomer.Phone = CustomerPhone.Text;
                    selectedCustomer.City = CustomerCity.Text;
                    selectedCustomer.State = StateDropDownList.SelectedValue;
                    selectedCustomer.Zip = CustomerZip.Text;
                    selectedCustomer.Country = CountryDropDownList.SelectedValue;
                    selectedCustomer.Notes = CustomerNotes.Text;
                    selectedCustomer.ContactName = ContactName.Text;
                    selectedCustomer.ContactPhone = ContactPhone.Text;
                    selectedCustomer.ContactEmail = ContactEmail.Text;

                    string apiUrl = $"https://localhost:44358/api/customers/{selectedCustomer.ID}";

                    try
                    {
                        using (var httpClient = new HttpClient())
                        {
                            string jsonData = JsonConvert.SerializeObject(selectedCustomer);
                            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = await httpClient.PutAsync(apiUrl, content);

                            if (response.IsSuccessStatusCode)
                            {
                                Response.Write("<script>alert('Customer updated successfully!');</script>");
                            }
                            else
                            {
                                string errorContent = await response.Content.ReadAsStringAsync();
                                Response.Write($"<script>alert('Failed to update customer: {errorContent}');</script>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
                    }
                }
            }
        }
        protected async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomersDDL.SelectedValue))
            {
                string selectedCustomerName = CustomersDDL.SelectedItem.Text;
                var selectedCustomer = customers.FirstOrDefault(c => c.Name == selectedCustomerName);

                if (selectedCustomer != null)
                {
                    string apiUrl = $"https://localhost:44358/api/customers/{selectedCustomer.ID}";

                    try
                    {
                        using (var httpClient = new HttpClient())
                        {
                            HttpResponseMessage response = await httpClient.DeleteAsync(apiUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                customers.Remove(selectedCustomer);
                                CustomersDDL.Items.Remove(CustomersDDL.SelectedItem);

                                Response.Write("<script>alert('Customer deleted successfully!');</script>");
                            }
                            else
                            {
                                string errorContent = await response.Content.ReadAsStringAsync();
                                Response.Write($"<script>alert('Failed to delete customer: {errorContent}');</script>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
                    }
                }
            }
        }

    }
}