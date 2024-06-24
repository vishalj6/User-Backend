using UserProject_BAL.Interfaces;
using UserProject_DAL.Interfaces;
using UserProject_DAL.Models;

namespace UserProject_BAL.Sevices
{
    // BusinessLogicLayer/Services/UserService.cs

    public class UserService : IUserService
    {
        private readonly IDataAccess _dataAccess; // Assuming IDataAccess is used for data access

        public UserService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Task<(List<User>, int total_pages, int total_users)> GetUsers(FilterData allFilter, int UserId, string User_Password)
        {
            return _dataAccess.GetUsers(allFilter, UserId, User_Password); // Delegate to data access layer
        }

        public Task<List<string>> Insert_User(User User)
        {
            return _dataAccess.Insert_User(User);
        }
        public Task<List<string>> Update_User(User user, int UserId, string password)
        {
            return _dataAccess.Update_User(user, UserId, password);
        }

        public Task<List<string>> DeleteUsers(int delete_id)
        {
            return _dataAccess.DeleteUsers(delete_id);
        }
        public Task<bool> DeleteAllPersons()
        {
            return _dataAccess.DeleteAllPersons();
        }

        public Task<(int, string)> ValidateUserAsync(string email, string login_password = "")
        {
            return _dataAccess.ValidateUserAsync(email, login_password);
        }
        public Task<bool> ValidateAdminAsync(string admin_username, string admin_pas)
        {
            return _dataAccess.ValidateAdminAsync(admin_username, admin_pas);
        }
        public Task<List<string>> GetCountriesAsync()
        {
            return _dataAccess.GetCountriesAsync();
        }
        public Task<List<string>> GetStatesAsync()
        {
            return _dataAccess.GetStatesAsync();
        }
        public Task<List<string>> GetCitiesAsync()
        {
            return _dataAccess.GetCitiesAsync();
        }


        // Implement additional business logic methods as needed
    }

}
