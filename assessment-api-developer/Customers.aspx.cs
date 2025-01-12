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
                CustomersDDL.Items.Add(new ListItem("Add New Customer", "0"));

                // Fetch customers from the API
                var allCustomers = await FetchCustomersFromApi();

                if (allCustomers.Any())
                {
                    // Populate the dropdown with customer data
                    foreach (var customer in allCustomers)
                    {
                        CustomersDDL.Items.Add(new ListItem(customer.Name, customer.ID.ToString()));
                    }

                    CustomersDDL.SelectedIndex = 0;
                }
                else
                {
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

                            // Update UI for editing
                            AddButton.Visible = false;
                            UpdateButton.Visible = true;
                            DeleteButton.Visible = true;
                            CustomerActionLabel.InnerText = "Edit customer";
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

                        await PopulateCustomerListBox();
                        ResetForm();
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
            if (int.TryParse(CustomersDDL.SelectedValue, out int customerId) && customerId > 0)
            {
                var updatedCustomer = new Customer
                {
                    ID = customerId, // Use the selected customer's ID
                    Name = CustomerName.Text,
                    Address = CustomerAddress.Text,
                    Email = CustomerEmail.Text,
                    Phone = CustomerPhone.Text,
                    City = CustomerCity.Text,
                    State = StateDropDownList.SelectedValue,
                    Zip = CustomerZip.Text,
                    Country = CountryDropDownList.SelectedValue,
                    Notes = CustomerNotes.Text,
                    ContactName = ContactName.Text,
                    ContactPhone = ContactPhone.Text,
                    ContactEmail = ContactEmail.Text
                };

                string apiUrl = $"https://localhost:44358/api/customers/{customerId}";

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string jsonData = JsonConvert.SerializeObject(updatedCustomer);
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await httpClient.PutAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            // Refresh the dropdown and reset the form
                            await PopulateCustomerListBox();
                            ResetForm();

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
            else
            {
                Response.Write("<script>alert('Please select a valid customer to update.');</script>");
            }
        }

        protected async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(CustomersDDL.SelectedValue, out int customerId) && customerId > 0)
            {
                string apiUrl = $"https://localhost:44358/api/customers/{customerId}";

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        HttpResponseMessage response = await httpClient.DeleteAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            // Refresh the dropdown and reset the form
                            await PopulateCustomerListBox();
                            ResetForm();

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
            else
            {
                Response.Write("<script>alert('Please select a valid customer to delete.');</script>");
            }
        }

        private void ResetForm()
        {
            // Clear all form fields
            CustomerName.Text = string.Empty;
            CustomerAddress.Text = string.Empty;
            CustomerEmail.Text = string.Empty;
            CustomerPhone.Text = string.Empty;
            CustomerCity.Text = string.Empty;
            StateDropDownList.SelectedIndex = 0;
            CustomerZip.Text = string.Empty;
            CountryDropDownList.SelectedIndex = 0;
            CustomerNotes.Text = string.Empty;
            ContactName.Text = string.Empty;
            ContactPhone.Text = string.Empty;
            ContactEmail.Text = string.Empty;

            // Reset UI for adding a new customer
            AddButton.Visible = true;
            UpdateButton.Visible = false;
            DeleteButton.Visible = false;
            CustomerActionLabel.InnerText = "Add customer";
            CustomersDDL.SelectedIndex = 0;
        }
    }
}