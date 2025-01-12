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
                // Fetch customers asynchronously
                var allCustomers = await FetchCustomersFromApi();
                ViewState["Customers"] = allCustomers;
            }
            else
            {
                customers = (List<Customer>)ViewState["Customers"];
            }

            await PopulateCustomerListBox();
            PopulateCustomerDropDownLists();
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
                var allCustomers = await FetchCustomersFromApi();

                if (allCustomers.Any())
                {
                    foreach (var customer in allCustomers)
                    {
                        CustomersDDL.Items.Add(new ListItem(customer.Name, customer.ID.ToString()));
                    }
                }
                else
                {
                    CustomersDDL.Items.Add(new ListItem("No customers data."));
                }
            }
            catch (Exception ex)
            {
                CustomersDDL.Items.Add(new ListItem($"Error: {ex.Message}"));
            }
        }

        protected void CustomersDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomersDDL.SelectedValue))
            {
                string selectedCustomerName = CustomersDDL.SelectedItem.Text;
                var selectedCustomer = customers.FirstOrDefault(c => c.Name == selectedCustomerName);

                if (selectedCustomer != null)
                {
                    // Populate form fields
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
	}
}