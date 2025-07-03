namespace LoginSystem.API.Interfaces
{
    public interface IActiveDirectoryService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<AdUserInfo?> GetUserInfoAsync(string username);
    }

    public class AdUserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Groups { get; set; } = new List<string>();
    }
} 