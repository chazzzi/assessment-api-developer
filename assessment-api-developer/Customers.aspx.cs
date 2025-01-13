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
                await PopulateCountryDropdown();
                int defaultCountryId = 1; // Set a default country Canada
                await PopulateStateDropdown(defaultCountryId);
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
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return new List<Customer>();
                }
            }
        }

        protected async void CountryDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countryId = int.Parse(CountryDropDownList.SelectedValue);
            await PopulateStateDropdown(countryId);
        }
        private async Task PopulateCountryDropdown()
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = "https://localhost:44358/api/countries";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var countries = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);

                    CountryDropDownList.DataSource = countries;
                    CountryDropDownList.DataTextField = "Name";
                    CountryDropDownList.DataValueField = "ID";
                    CountryDropDownList.DataBind();
                }
            }
        }
        private async Task PopulateStateDropdown(int countryId)
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = $"https://localhost:44358/api/countries/{countryId}/states";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var states = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);

                    StateDropDownList.DataSource = states;
                    StateDropDownList.DataTextField = "Name";
                    StateDropDownList.DataValueField = "ID";
                    StateDropDownList.DataBind();
                }
            }
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
                CustomersDDL.Items.Add(new ListItem($"Error: {ex.Message}", ""));
            }
        }
        protected async void CustomersDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomersDDL.SelectedValue))
            {
                if (int.TryParse(CustomersDDL.SelectedValue, out int customerId))
                {
                    try
                    {
                        // Fetch customer details from the API by ID
                        var selectedCustomer = await FetchCustomerByIdFromApi(customerId);

                        if (selectedCustomer != null)
                        {
                            CustomerName.Text = selectedCustomer.Name;
                            CustomerAddress.Text = selectedCustomer.Address;
                            CustomerEmail.Text = selectedCustomer.Email;
                            CustomerPhone.Text = selectedCustomer.Phone;
                            CustomerCity.Text = selectedCustomer.City;
                            CustomerZip.Text = selectedCustomer.Zip;
                            CustomerNotes.Text = selectedCustomer.Notes;
                            ContactName.Text = selectedCustomer.ContactName;
                            ContactPhone.Text = selectedCustomer.ContactPhone;
                            ContactEmail.Text = selectedCustomer.ContactEmail;

                            if (CountryDropDownList.Items.FindByValue(selectedCustomer.CountryID.ToString()) != null)
                            {
                                CountryDropDownList.SelectedValue = selectedCustomer.CountryID.ToString();

                                await PopulateStateDropdown(selectedCustomer.CountryID);

                                if (StateDropDownList.Items.FindByValue(selectedCustomer.StateID.ToString()) != null)
                                {
                                    StateDropDownList.SelectedValue = selectedCustomer.StateID.ToString();
                                }
                            }

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
            try
            {
                // Validate dropdown selections
                if (!int.TryParse(StateDropDownList.SelectedValue, out int stateId) ||
                    !int.TryParse(CountryDropDownList.SelectedValue, out int countryId))
                {
                    Response.Write("<script>alert('Please select valid State and Country.');</script>");
                    return;
                }

                // Create the customer object
                var customer = new Customer
                {
                    Name = CustomerName.Text.Trim(),
                    Address = CustomerAddress.Text.Trim(),
                    City = CustomerCity.Text.Trim(),
                    StateID = stateId, // StateID instead of State
                    Zip = CustomerZip.Text.Trim(),
                    CountryID = countryId, // CountryID instead of Country
                    Email = CustomerEmail.Text.Trim(),
                    Phone = CustomerPhone.Text.Trim(),
                    Notes = CustomerNotes.Text.Trim(),
                    ContactName = ContactName.Text.Trim(),
                    ContactPhone = CustomerPhone.Text.Trim(),
                    ContactEmail = CustomerEmail.Text.Trim()
                };

                string apiUrl = "https://localhost:44358/api/customers";

                using (var httpClient = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(customer);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
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
                Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
            }
        }

        protected async void UpdateButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(CustomersDDL.SelectedValue, out int customerId) || customerId <= 0)
            {
                Response.Write("<script>alert('Please select a valid customer to update.');</script>");
                return;
            }

            try
            {
                var updatedCustomer = new Customer
                {
                    ID = customerId,
                    Name = CustomerName.Text,
                    Address = CustomerAddress.Text,
                    Email = CustomerEmail.Text,
                    Phone = CustomerPhone.Text,
                    City = CustomerCity.Text,
                    StateID = int.Parse(StateDropDownList.SelectedValue),
                    Zip = CustomerZip.Text,
                    CountryID = int.Parse(CountryDropDownList.SelectedValue),
                    Notes = CustomerNotes.Text,
                    ContactName = ContactName.Text,
                    ContactPhone = CustomerPhone.Text,
                    ContactEmail = CustomerEmail.Text
                };

                string apiUrl = $"https://localhost:44358/api/customers/{customerId}";

                var httpClient = new HttpClient();
                var response = await httpClient.PutAsJsonAsync(apiUrl, updatedCustomer);

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
            catch (Exception ex)
            {
                Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
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