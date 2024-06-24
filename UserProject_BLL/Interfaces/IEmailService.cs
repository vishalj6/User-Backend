namespace UserProject_DAL.Interfaces
{
    public interface IEmailService
    {
        Task<string> SendOTPEmail(string userEmail, string firstName);
    }
}
