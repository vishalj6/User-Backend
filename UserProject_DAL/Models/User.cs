﻿namespace UserProject_DAL.Models
{
    public class User
    {
        public int UserID { get; set; } // Consider using PascalCase for property names
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Password { get; set; }

    }
}