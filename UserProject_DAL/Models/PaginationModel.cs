namespace UserProject_DAL.Models
{
    public class PaginationModel
    {
        public List<User> AllUsers { get; set; }
        public int total_pages { get; set; } = 1;
        public int? total_users { get; set; }
    }
}
