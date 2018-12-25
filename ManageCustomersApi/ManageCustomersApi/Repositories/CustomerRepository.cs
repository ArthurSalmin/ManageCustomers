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
            string queryString = $"SELECT * FROM Customer WHERE Id={id}";
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
                    customer.LockState = row["LockState"].ToString();
                    if (row["IdUserLocked"].ToString() == string.Empty)
                        customer.IdUserLocked = null;
                    else
                        customer.IdUserLocked = int.Parse(row["IdUserLocked"].ToString());

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

        public async Task<SetStatusModel> GetStatus(int idCustomer)
        {
            string queryString = $"SELECT LockState, IdUserLocked FROM Customer WHERE Id={idCustomer}";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(queryString, connection);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                SetStatusModel setStatusModel = new SetStatusModel();
                setStatusModel.IdCustomer = idCustomer;
                setStatusModel.Status = dataTable.Rows[0]["LockState"].ToString();
                if (dataTable.Rows[0]["IdUserLocked"] == DBNull.Value)
                    setStatusModel.IdUserLocked = null;
                else
                    setStatusModel.IdUserLocked = int.Parse(dataTable.Rows[0]["IdUserLocked"].ToString());
                setStatusModel.Status = dataTable.Rows[0]["LockState"].ToString();
                return setStatusModel;
            }
        }

        public async Task<CustomerModel> PostAsync(CustomerModel obj)
        {
            string DateOfBirth = obj.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:ss");
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
            string DateOfBirth = obj.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:ss");
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
                if (obj.Street == null) command.Parameters.AddWithValue("Street", DBNull.Value); else command.Parameters.AddWithValue("Street", obj.Street);
                if (obj.CityId == null) command.Parameters.AddWithValue("CityId", DBNull.Value); else command.Parameters.AddWithValue("CityId", obj.Street);
                int updatedCustomer = await command.ExecuteNonQueryAsync();
                if (updatedCustomer > 0)
                    return obj;
                else
                    return null;
            }
        }

        public async Task<SetStatusModel> SetStatus(int idCustomer, string status, int? IdUserLocked)
        {
            string queryString = $"UPDATE Customer SET " +
                    $"[Name] = @Name, " +
                    $"[FirstName] = @FirstName, " +
                    $"[DateOfBirth] = @DateOfBirth, " +
                    $"[Street] = @Street, " +
                    $"[CityId] = @CityId, " +
                    $"[LockState]= @LockState, " +
                    $"[IdUserLocked]= @IdUserLocked " +
                    $"WHERE Id= @Id";
            string getCustomerQuery = $"Select * FROM Customer WHERE Id={idCustomer}";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var customer = await GetAsync(idCustomer);
                if (customer != null)
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("Id", customer.Id);
                    command.Parameters.AddWithValue("Name", customer.Name);
                    command.Parameters.AddWithValue("FirstName", customer.FirstName);
                    command.Parameters.AddWithValue("DateOfBirth", customer.DateOfBirth);
                    if(customer.Street == null) command.Parameters.AddWithValue("Street", DBNull.Value); else command.Parameters.AddWithValue("Street", customer.Street);
                    if(customer.CityId == null) command.Parameters.AddWithValue("CityId", DBNull.Value); else command.Parameters.AddWithValue("CityId", customer.Street);
                    command.Parameters.AddWithValue("LockState", status);
                    if(IdUserLocked == null) command.Parameters.AddWithValue("IdUserLocked", DBNull.Value); else command.Parameters.AddWithValue("IdUserLocked", IdUserLocked);
                    int updatedCustomer = await command.ExecuteNonQueryAsync();
                    if (updatedCustomer > 0)
                        return new SetStatusModel
                        {
                            IdCustomer = idCustomer,
                            Status = status,
                            IdUserLocked = IdUserLocked
                        };
                    else
                        return null;
                }
                else
                    return null;
            }
        }

    }
}
