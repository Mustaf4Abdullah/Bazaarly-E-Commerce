namespace Bazaarly.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;   
        public string Role { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Additional employee-specific properties can be added here
    }
}
