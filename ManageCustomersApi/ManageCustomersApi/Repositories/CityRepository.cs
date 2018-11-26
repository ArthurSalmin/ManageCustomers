using ManageCustomersApi.Interfaces;
using ManageCustomersApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ManageCitysApi.Repositories
{
    public class CityRepository : ICityRepository
    {
        private static string _path = Environment.CurrentDirectory + "\\ManageCustomersDb.mdf";
        private string _connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;
                                            AttachDbFilename='{_path}';
                                            Integrated Security=True";

        public async Task<bool> DeleteAsync(int id)
        {
            string queryString = $"DELETE FROM City WHERE Id = {id}";
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

        public async Task<List<CityModel>> GetAllAsync()
        {
            string queryString = "SELECT * FROM City";
            List<CityModel> CitysList = new List<CityModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryString, connection);
                DataTable CitysDt = new DataTable();
                sqlDa.Fill(CitysDt);
                foreach (DataRow row in CitysDt.Rows)
                {
                    var City = new CityModel();
                    City.Id = int.Parse(row["Id"].ToString());
                    City.Name = row["Name"].ToString();
                    CitysList.Add(City);
                }
            }
            if (CitysList != null)
            {
                return CitysList;
            }
            else
            {
                return null;
            }
        }

        public async Task<CityModel> GetAsync(int id)
        {
            string queryString = $"SELECT Name, FirstName, DateOfBirth, Street, CityId FROM City WHERE Id={id}";
            CityModel getedCity = new CityModel();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryString, connection);
                DataTable CitysDt = new DataTable();
                sqlDa.Fill(CitysDt);
                foreach (DataRow row in CitysDt.Rows)
                {
                    var City = new CityModel();
                    City.Id = int.Parse(row["Id"].ToString());
                    City.Name = row["Name"].ToString();
                    getedCity = City;
                }
                if (getedCity != null)
                {
                    return getedCity;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<CityModel> PostAsync(CityModel obj)
        {
            string queryString = "INSERT INTO City (Name, FirstName, DateOfBirth, Street, CityId) VALUES (" +
                $"'{obj.Id}','{obj.Name}' )";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(queryString, connection);
                int newCity = await command.ExecuteNonQueryAsync();
                if (newCity > 0)
                    return obj;
                else
                    return null;
            }
        }

        public async Task<CityModel> PutAsync(CityModel obj)
        {
            string queryString = $"UPDATE City SET Name = '{obj.Name}' WHERE Id = '{obj.Id}'";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(queryString, connection);
                int updatedCity = await command.ExecuteNonQueryAsync();
                if (updatedCity > 0)
                    return obj;
                else
                    return null;
            }
        }
    }
}
