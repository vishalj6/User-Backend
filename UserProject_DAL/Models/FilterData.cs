namespace UserProject_DAL.Models
{
    public class FilterData
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? phoneNo { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public int DropDownLimit { get; set; } = 5;
        public int currentPageIndex { get; set; } = 1;
    }
}
