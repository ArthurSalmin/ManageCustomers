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
            string queryString = "SELECT DISTINCT cust.Id, cust.Name, cust.FirstName, cust.DateOfBirth, cust.LockState, " +
                                  "cust.IdUserLocked, cm1.TimeOfMigration, cm1.Street, cm1.IdCity "+
                                  "FROM customermigrations cm1 "+
                                  "LEFT JOIN customermigrations cm2 ON cm1.TimeOfMigration < cm2.TimeOfMigration " +
                                  "AND cm1.IdCustomer = cm2.IdCustomer "+
                                  "INNER JOIN customer cust ON cm1.IdCustomer = cust.Id "+
                                  "WHERE cm2.Id IS NULL";
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
                    customer.LockState = row["LockState"].ToString();
                    if (row["Street"] == DBNull.Value) customer.Street = null; else customer.Street = row["Street"].ToString();
                    if (row["IdUserLocked"] == DBNull.Value) customer.IdUserLocked = null; else customer.IdUserLocked = int.Parse(row["IdUserLocked"].ToString());
                    if (row["IdCity"] == DBNull.Value) customer.CityId = null; else customer.CityId = int.Parse(row["IdCity"].ToString());
                    customersList.Add(customer);
                }
            }
            return customersList;
        }

        public async Task<CustomerModel> GetAsync(int id)
        {
            string queryString = "SELECT cust.Id, cust.Name, cust.FirstName, cust.DateOfBirth, cust.LockState, " +
                                  "cust.IdUserLocked, cm1.TimeOfMigration, cm1.Street, cm1.IdCity " +
                                  "FROM customermigrations cm1 " +
                                  "LEFT JOIN customermigrations cm2 ON cm1.TimeOfMigration < cm2.TimeOfMigration " +
                                  "AND cm1.IdCustomer = cm2.IdCustomer " +
                                  "INNER JOIN customer cust ON cm1.IdCustomer = cust.Id " +
                                  $"AND cust.Id = {id} " +
                                  "WHERE cm2.Id IS NULL";
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
                    customer.CityId = int.Parse(row["IdCity"].ToString());
                    customer.LockState = row["LockState"].ToString();
                    if (row["IdUserLocked"] == DBNull.Value) customer.IdUserLocked = null; else customer.IdUserLocked = int.Parse(row["IdUserLocked"].ToString());
                    getedCustomer = customer;
                }
                if (getedCustomer.Name != null)
                {
                    return getedCustomer;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<int> GetCountMigrations()
        {
            int idMigration = 0;
            string queryCountMigrations = "SELECT COUNT(*) FROM CustomerMigrations";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand countMigrCommand = new SqlCommand(queryCountMigrations, connection);

                idMigration = (int)await countMigrCommand.ExecuteScalarAsync() + 1;
                if (idMigration != 0)
                {
                    return idMigration;
                }
                else
                    return 0;
            }
        }

        public async Task<SetStatusModel> GetStatus(int idCustomer)
        {
            string queryString = "SELECT " +
                "LockState, " +
                "IdUserLocked " +
                $"FROM Customer WHERE Id={idCustomer}";

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
            string DateOfMigration = DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ss");
            string queryStringCustomer = "INSERT INTO Customer (Id, Name, FirstName, TimeOfMigration, DateOfBirth, LockState)" +
                $" VALUES ('{obj.Id}','{obj.Name}','{obj.FirstName}','{DateOfMigration}', '{DateOfBirth}','online' )";
            int idMigration = 0;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand newCustomerCommand = new SqlCommand(queryStringCustomer, connection);

                idMigration = await GetCountMigrations();
                string queryStringCustomerMigrations = "INSERT INTO CustomerMigrations(Id, IdCity, Street, IdCustomer) " +
                $"VALUES('{idMigration}', '{obj.CityId}', '{obj.Street}', '{obj.Id}')";

                SqlCommand newCustomerMigrCommand = new SqlCommand(queryStringCustomerMigrations, connection);
                int newCustomer = await newCustomerCommand.ExecuteNonQueryAsync();
                int newMigration = await newCustomerMigrCommand.ExecuteNonQueryAsync();
                if (newCustomer > 0 && newMigration > 0)
                {
                    obj.LockState = "online";
                    return obj;
                }
                else
                    return null;
            }
        }

        public async Task<CustomerModel> PutAsync(CustomerModel obj)
        {
            string DateOfBirth = obj.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:ss");
            string queryCustomerString = $"UPDATE Customer SET " +
                $"[Name] = @Name, " +
                $"[FirstName] = @FirstName, " +
                $"[DateOfBirth] = @DateOfBirth " +
                $"WHERE (Id = @Id)";
            string DateOfMigration = DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ss");
            int countMigrations = 0;
            

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand updateCustomerCommand = new SqlCommand(queryCustomerString, connection);
                

                updateCustomerCommand.Parameters.AddWithValue("Id", obj.Id);
                updateCustomerCommand.Parameters.AddWithValue("Name", obj.Name);
                updateCustomerCommand.Parameters.AddWithValue("FirstName", obj.FirstName);
                updateCustomerCommand.Parameters.AddWithValue("DateOfBirth", DateOfBirth);
                int updatedCustomer = await updateCustomerCommand.ExecuteNonQueryAsync();

                if (await IsNewMigration(obj.CityId, obj.Street, obj.Id))
                {
                    countMigrations = await GetCountMigrations();
                    string queryNewMigration = "INSERT INTO CustomerMigrations (Id, IdCity, Street, TimeOfMigration, IdUser, IdCustomer) " +
                                       $"VALUES ('{countMigrations}', '{obj.CityId}', '{obj.Street}', '{DateOfMigration}', " +
                                       $"'{obj.IdUser}', '{obj.Id}')";
                    SqlCommand newCustomerMigration = new SqlCommand(queryNewMigration, connection);
                    int newMigration = await newCustomerMigration.ExecuteNonQueryAsync();
                    if (newMigration == 0)
                        return null;
                }
                if (updatedCustomer > 0)
                    return obj;
                else
                    return null;
            }
        }

        private async Task<bool> IsNewMigration(int? CityId, string Street, int IdCustomer)
        {
            string queryPlaceString = "SELECT CustomerMigrations.IdCity, CustomerMigrations.Street " +
                                      $"FROM CustomerMigrations WHERE CustomerMigrations.IdCustomer={IdCustomer}";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlDataAdapter getCustomerMigrationCommand = new SqlDataAdapter(queryPlaceString, connection);
                DataTable dataTable = new DataTable();
                getCustomerMigrationCommand.Fill(dataTable);

                int idCity = int.Parse(dataTable.Rows[0]["IdCity"].ToString());
                string street = dataTable.Rows[0]["Street"].ToString();

                if (CityId != idCity || Street != street)
                    return true;
                else
                    return false;

            }
        }

        public async Task<SetStatusModel> SetStatus(int idCustomer, string status, int? IdUserLocked)
        {
            string queryString = $"UPDATE Customer SET " +
                    $"[Name] = @Name, " +
                    $"[FirstName] = @FirstName, " +
                    $"[DateOfBirth] = @DateOfBirth, " +
                    $"[LockState]= @LockState, " +
                    $"[IdUserLocked]= @IdUserLocked " +
                    $"WHERE Id= @Id";

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
                    command.Parameters.AddWithValue("LockState", status);
                    if (IdUserLocked == null) command.Parameters.AddWithValue("IdUserLocked", DBNull.Value); else command.Parameters.AddWithValue("IdUserLocked", IdUserLocked);
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
