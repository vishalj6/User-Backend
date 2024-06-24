using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using UserProject_DAL.Interfaces;
using UserProject_DAL.Models;

namespace UserProject_DAL.Repositories
{
    public class FileDataAccess : IDataAccess
    {
        private readonly SqlConnection _mainSqlConn;
        public FileDataAccess(SqlDataAccess mainSqlConn)
        {
            _mainSqlConn = mainSqlConn.GetConnection();
        }

        public async Task<(List<User>, int total_pages, int total_users)> GetUsers(FilterData allFilter, int UserId, string User_Password)
        {

            List<User> lstUsers = new();
            string? firstName = allFilter.firstName;
            string? lastName = allFilter.lastName;
            string? phoneNo = allFilter.phoneNo;
            string? Email = allFilter.Email;
            string? Country = allFilter.Country;
            string? State = allFilter.State;
            string? City = allFilter.City;
            SqlCommand command = new SqlCommand("FILTER_USER", _mainSqlConn);
            command.CommandType = CommandType.StoredProcedure;
            try
            {
                //SqlCommand command = new("SELECT * FROM USER_TABLE_VIEW", conn);
                command.Parameters.AddWithValue("@USER_ID", UserId == 0 ? null : UserId);
                command.Parameters.AddWithValue("@User_password", string.IsNullOrEmpty(User_Password) ? null : User_Password);
                command.Parameters.AddWithValue("@First_name", firstName);
                command.Parameters.AddWithValue("@Last_name", lastName);
                command.Parameters.AddWithValue("@phone_no", phoneNo);
                command.Parameters.AddWithValue("@email", Email);
                command.Parameters.AddWithValue("@Country", Country);
                command.Parameters.AddWithValue("@State", State);
                command.Parameters.AddWithValue("@City", City);
                command.Parameters.AddWithValue("@PageNumber", allFilter.currentPageIndex);
                command.Parameters.AddWithValue("@Limit", allFilter.DropDownLimit);
                await _mainSqlConn.OpenAsync();
                SqlDataReader sdr = await command.ExecuteReaderAsync();

                if (sdr.HasRows)
                {
                    while (await sdr.ReadAsync())
                    {
                        User Users = new()
                        {
                            UserID = Convert.ToInt32(sdr["person_id"]),
                            FirstName = sdr["First_name"].ToString(),
                            LastName = sdr["Last_name"].ToString(),
                            PhoneNumber = sdr["Phone_no"].ToString(),
                            Country = sdr["Country_name"].ToString(),
                            State = sdr["State_name"].ToString(),
                            City = sdr["city_name"].ToString(),
                            Email = sdr["email"].ToString(),
                            Address = sdr["address_col"].ToString()
                        };
                        lstUsers.Add(Users);
                    }
                }
                int total_users = 0;
                int total_pages = 0;
                if (await sdr.NextResultAsync())
                {
                    if (sdr.HasRows)
                    {
                        await sdr.ReadAsync();
                        total_pages = Convert.ToInt32(sdr["TotalCount"]);
                        total_users = Convert.ToInt32(sdr["TotalUsers"]);
                    }
                }
                sdr.Close();
                return (lstUsers, total_pages, total_users);
            }
            catch (SqlException e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return (lstUsers, 0, 0);
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<List<string>> Insert_User(User User)
        {
            int user_id = 0;
            try
            {
                if (!Regex.IsMatch(User.Email, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
                {
                    return ["false", "Email is not valid!!"];
                }

                string pattern = @"^\d{10}$";
                if (!Regex.IsMatch(User.PhoneNumber, pattern))
                {
                    return ["false", "Phone no is not valid!!"];
                }

                SqlCommand cmd = new SqlCommand("InsertPersonData", _mainSqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@First_name", User.FirstName);
                cmd.Parameters.AddWithValue("@Last_name", User.LastName);
                cmd.Parameters.AddWithValue("@Phone_no", User.PhoneNumber);
                cmd.Parameters.AddWithValue("@city_name", User.City);
                cmd.Parameters.AddWithValue("@email", User.Email);
                cmd.Parameters.AddWithValue("@address_col", User.Address);
                cmd.Parameters.AddWithValue("@User_password", User.Password);

                await _mainSqlConn.OpenAsync();
                SqlDataReader sdr = await cmd.ExecuteReaderAsync();
                if (await sdr.NextResultAsync())
                {
                    if (sdr.HasRows)
                    {
                        await sdr.ReadAsync();
                        user_id = Convert.ToInt32(sdr["UserId"]);
                    }
                }

                if (user_id != 0)
                {
                    return new List<string> { "true", "User Inserted Successfully", user_id.ToString(), User.FirstName };
                }
                return ["false", "An error occurred."];
            }
            catch (SqlException e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                if (e.Number == 2627 || e.Number == 50000) // error code for unique key violation
                {
                    return ["false", "Duplicate entry detected.(Phone no or Email)"];
                }
                else
                {
                    return ["false", "An error occurred."];
                }
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<List<string>> Update_User(User User, int UserId, string password)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE_USER", _mainSqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (User == null)
                {
                    if (UserId != 0 && !string.IsNullOrEmpty(password))
                    {
                        cmd.Parameters.AddWithValue("@Update_id", UserId);
                        cmd.Parameters.AddWithValue("@Update_pas", password);

                        await _mainSqlConn.OpenAsync();
                        SqlDataReader sdr = await cmd.ExecuteReaderAsync();
                        if (sdr.HasRows)
                        {
                            await sdr.ReadAsync();
                            string username = sdr["First_name"].ToString();
                            return ["true", "Updated Successfully", UserId.ToString(), username];
                        }
                        else
                        {
                            return ["false", "An error occurred."];
                        }
                    }
                    return ["false", "An error occurred."];

                }
                else
                {
                    if (!Regex.IsMatch(User.Email, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
                    {
                        return ["false", "Email is not valid!!"];

                    }
                    string pattern = @"^\d{10}$";
                    if (!Regex.IsMatch(User.PhoneNumber, pattern))
                    {
                        return ["false", "Phone no is not valid!!"];
                    }

                    cmd.Parameters.AddWithValue("@Update_id", User.UserID);
                    cmd.Parameters.AddWithValue("@First_name", User.FirstName);
                    cmd.Parameters.AddWithValue("@Last_name", User.LastName);
                    cmd.Parameters.AddWithValue("@Phone_no", User.PhoneNumber);
                    cmd.Parameters.AddWithValue("@city_name", User.City);
                    cmd.Parameters.AddWithValue("@email", User.Email);
                    cmd.Parameters.AddWithValue("@Address", User.Address);

                    await _mainSqlConn.OpenAsync();
                    int rowAff = await cmd.ExecuteNonQueryAsync();
                    if (rowAff > 0)
                    {
                        return ["true", "Updated Successfully", User.UserID.ToString(), User.FirstName.ToString()];
                    }
                    else
                    {
                        return ["false", "An error occurred."];
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                if (e.Number == 2627) // error code for unique key violation
                {
                    return ["false", "Duplicate entry detected. (Phone no or Email)"];
                }
                else
                {
                    return ["false", "An error occurred."];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return ["false", "An error occurred."];
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }


        public async Task<List<string>> DeleteUsers(int delete_id)
        {
            try
            {
                SqlCommand command = new("DeletePersonData", _mainSqlConn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@deleteid", delete_id);
                await _mainSqlConn.OpenAsync();
                int rowAff = await command.ExecuteNonQueryAsync();
                if (rowAff > 0)
                {
                    return ["true", "Successful"];
                }
                else
                {
                    return ["false", "No User Found!!"];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return ["false", "Error occured"];
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<bool> DeleteAllPersons()
        {
            try
            {
                SqlCommand command = new("DELETE_ALL_USERS", _mainSqlConn);
                command.CommandType = CommandType.StoredProcedure;
                await _mainSqlConn.OpenAsync();
                int rowAff = await command.ExecuteNonQueryAsync();
                if (rowAff > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return false;
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<(int, string)> ValidateUserAsync(string email, string login_password = "")
        {
            try
            {
                using (SqlCommand command = new SqlCommand("FILTER_USER", _mainSqlConn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });
                    command.Parameters.Add(new SqlParameter("@User_password", SqlDbType.VarChar) { Value = login_password });

                    await _mainSqlConn.OpenAsync();
                    using (SqlDataReader sdr = await command.ExecuteReaderAsync())
                    {
                        if (sdr.HasRows)
                        {
                            await sdr.ReadAsync();
                            return (Convert.ToInt32(sdr["person_id"]), sdr["First_name"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log the exception properly
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
            }
            finally
            {
                if (_mainSqlConn.State == ConnectionState.Open)
                {
                    await _mainSqlConn.CloseAsync();
                }
            }

            // Return a default value or indicate failure
            return (0, "");
        }

        public async Task<bool> ValidateAdminAsync(string admin_username, string admin_pas)
        {
            try
            {
                SqlCommand command = new SqlCommand("ValidateAdmin", _mainSqlConn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@admin_username", admin_username);
                command.Parameters.AddWithValue("@admin_password", admin_pas);
                await _mainSqlConn.OpenAsync();
                SqlDataReader sdr = await command.ExecuteReaderAsync();
                if (sdr.HasRows)
                {
                    await sdr.ReadAsync();
                    if (sdr["Result"].ToString() == "Admin exists")
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return false;
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<List<string>> GetCountriesAsync()
        {
            List<string> countryList = [];
            try
            {
                SqlCommand command = new("SELECT (country_name) FROM COUNTRIES", _mainSqlConn);
                await _mainSqlConn.OpenAsync();
                SqlDataReader sdr = await command.ExecuteReaderAsync();
                while (await sdr.ReadAsync())
                {
                    string onecountry = sdr["country_name"].ToString();
                    countryList.Add(onecountry);
                }
                return countryList;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return countryList;
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<List<string>> GetStatesAsync()
        {
            List<string> stateList = [];
            try
            {
                SqlCommand command = new("SELECT (state_name) FROM STATE_TABLE_VIEW", _mainSqlConn);
                await _mainSqlConn.OpenAsync();
                SqlDataReader sdr = await command.ExecuteReaderAsync();
                while (await sdr.ReadAsync())
                {
                    string onestate = sdr["state_name"].ToString();
                    stateList.Add(onestate);
                }
                return stateList;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return stateList;
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

        public async Task<List<string>> GetCitiesAsync()
        {
            List<string> cityList = [];
            try
            {
                SqlCommand command = new("SELECT (city_name) FROM CITY_TABLE_VIEW", _mainSqlConn);
                await _mainSqlConn.OpenAsync();
                SqlDataReader sdr = await command.ExecuteReaderAsync();
                while (await sdr.ReadAsync())
                {
                    string onecity = sdr["city_name"].ToString();
                    cityList.Add(onecity);
                }
                return cityList;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nOOPs, something went wrong.\n" + e.Message);
                return cityList;
            }
            finally
            {
                await _mainSqlConn.CloseAsync();
            }
        }

    }
}
