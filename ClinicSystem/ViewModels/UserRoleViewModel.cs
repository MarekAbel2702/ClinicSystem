namespace ClinicSystem.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string CurrentRole { get; set; } = string.Empty;

        public string SelectedRole { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}