using UserProject_DAL.Models;

namespace UserProject_DAL.Interfaces
{
    public interface IDataAccess
    {
        Task<(List<User>, int total_pages, int total_users)> GetUsers(FilterData allFilter, int UserId, string User_Password);
        Task<List<string>> Insert_User(User User);
        Task<List<string>> Update_User(User user, int UserId, string password);

        Task<List<string>> DeleteUsers(int delete_id);
        Task<bool> DeleteAllPersons();
        Task<(int, string)> ValidateUserAsync(string email, string login_password = "");
        Task<bool> ValidateAdminAsync(string admin_username, string admin_pas);
        Task<List<string>> GetCountriesAsync();
        Task<List<string>> GetStatesAsync();
        Task<List<string>> GetCitiesAsync();
    }
}
