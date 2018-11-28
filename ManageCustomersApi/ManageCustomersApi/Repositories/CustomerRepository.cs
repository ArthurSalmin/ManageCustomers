using ManageCustomersApi.Interfaces;
using ManageCustomersApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ManageCustomersApi.Repositories
{
    /// <summary>
    /// Class for operations with database
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private static string _path = Environment.CurrentDirectory + "\\ManageCustomersDb.mdf";
        private string _connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;
                                            AttachDbFilename='{_path}';
                                            Integrated Security=True";

        public async Task<bool> DeleteAsync(int id)
        {
            string queryString = $"DELETE FROM Customer WHERE Id = {id}";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(queryString, connection);
                int count = command.ExecuteNonQuery();
                if (count > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<List<CustomerModel>> GetAllAsync()
        {
            string queryString = "SELECT * FROM Customer";
            List<CustomerModel> customersList = new List<CustomerModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryString, connection);
                DataTable customersDt = new DataTable();
                sqlDa.Fill(customersDt);
                foreach (DataRow row in customersDt.Rows)
                {
                    var customer = new CustomerModel();
                    customer.Id = int.Parse(row["Id"].ToString());
                    customer.Name = row["Name"].ToString();
                    customer.FirstName = row["FirstName"].ToString();
                    customer.DateOfBirth = (DateTime)row["DateOfBirth"];
                    customer.Street = row["Street"].ToString();
                    customer.CityId = int.Parse(row["CityId"].ToString());
                    customersList.Add(customer);
                }
            }
            if (customersList != null)
            {
                return customersList;
            }
            else
            {
                return null;
            }
        }

        public async Task<CustomerModel> GetAsync(int id)
        {
            string queryString = $"SELECT Name, FirstName, DateOfBirth, Street, CityId FROM Customer WHERE Id={id}";
            CustomerModel getedCustomer = new CustomerModel();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryString, connection);
                DataTable customersDt = new DataTable();
                sqlDa.Fill(customersDt);
                foreach (DataRow row in customersDt.Rows)
                {
                    var customer = new CustomerModel();
                    customer.Id = int.Parse(row["Id"].ToString());
                    customer.Name = row["Name"].ToString();
                    customer.FirstName = row["FirstName"].ToString();
                    customer.DateOfBirth = (DateTime)row["DateOfBirth"];
                    customer.Street = row["Street"].ToString();
                    customer.CityId = int.Parse(row["CityId"].ToString());
                    getedCustomer = customer;
                }
                if (getedCustomer != null)
                {
                    return getedCustomer;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<CustomerModel> PostAsync(CustomerModel obj)
        {
            string DateOfBirth;
            string day;
            string month;
            if (obj.DateOfBirth.Day < 10)
                day = "0" + obj.DateOfBirth.Day;
            else
                day = obj.DateOfBirth.Day.ToString();
            if (obj.DateOfBirth.Month < 10)
                month = "0" + obj.DateOfBirth.Month;
            else
                month = obj.DateOfBirth.Month.ToString();


            DateOfBirth = obj.DateOfBirth.Year + "-" +
                             month + "-" +
                             day + "T" +
                             obj.DateOfBirth.TimeOfDay;
            string queryString = "INSERT INTO Customer (Id, Name, FirstName, DateOfBirth, Street, CityId) VALUES (" +
                $"'{obj.Id}','{obj.Name}','{obj.FirstName}','{DateOfBirth}','{obj.Street}','{obj.CityId}' )";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(queryString, connection);
                int newCustomer = await command.ExecuteNonQueryAsync();
                if (newCustomer > 0)
                    return obj;
                else
                    return null;
            }
        }

        public async Task<CustomerModel> PutAsync(CustomerModel obj)
        {
            string DateOfBirth;
            string day;
            string month;
            if (obj.DateOfBirth.Day < 10)
                day = "0" + obj.DateOfBirth.Day;
            else
                day = obj.DateOfBirth.Day.ToString();
            if (obj.DateOfBirth.Month < 10)
                month = "0" + obj.DateOfBirth.Month;
            else
                month = obj.DateOfBirth.Month.ToString();


            DateOfBirth = obj.DateOfBirth.Year + "-" +
                             month + "-" +
                             day + "T" +
                             obj.DateOfBirth.TimeOfDay;
            string queryString = $"UPDATE Customer SET " +
                $"[Name] = @Name, " +
                $"[FirstName] = @FirstName, " +
                $"[DateOfBirth] = @DateOfBirth, " +
                $"[Street] = @Street, " +
                $"[CityId] = @CityId " +
                $"WHERE (Id = @Id)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("Id", obj.Id);
                command.Parameters.AddWithValue("Name", obj.Name);
                command.Parameters.AddWithValue("FirstName", obj.FirstName);
                command.Parameters.AddWithValue("DateOfBirth", DateOfBirth);
                command.Parameters.AddWithValue("Street", obj.Street);
                command.Parameters.AddWithValue("CityId", obj.CityId);
                int updatedCustomer = await command.ExecuteNonQueryAsync();
                if (updatedCustomer > 0)
                    return obj;
                else
                    return null;
            }
        }
    }
}
