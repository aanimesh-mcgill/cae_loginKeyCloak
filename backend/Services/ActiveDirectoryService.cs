using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using LoginSystem.API.Interfaces;

namespace LoginSystem.API.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly IConfiguration _configuration;
        private readonly string _domain;
        private readonly string _ldapServer;

        public ActiveDirectoryService(IConfiguration configuration)
        {
            _configuration = configuration;
            _domain = _configuration["ActiveDirectory:Domain"] ?? "yourdomain.com";
            _ldapServer = _configuration["ActiveDirectory:LdapServer"] ?? "ldap://yourdomain.com:389";
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var context = new PrincipalContext(ContextType.Domain, _domain);
                    return context.ValidateCredentials(username, password);
                }
                catch (Exception)
                {
                    // For development/demo purposes, allow some test credentials
                    if (IsDevelopmentEnvironment())
                    {
                        return ValidateTestCredentials(username, password);
                    }
                    return false;
                }
            });
        }

        public async Task<AdUserInfo?> GetUserInfoAsync(string username)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var context = new PrincipalContext(ContextType.Domain, _domain);
                    using var userPrincipal = UserPrincipal.FindByIdentity(context, username);

                    if (userPrincipal != null)
                    {
                        return new AdUserInfo
                        {
                            Username = userPrincipal.SamAccountName ?? username,
                            DisplayName = userPrincipal.DisplayName ?? username,
                            Email = userPrincipal.EmailAddress ?? "",
                            Groups = GetUserGroups(userPrincipal)
                        };
                    }
                }
                catch (Exception)
                {
                    // For development/demo purposes, return test user info
                    if (IsDevelopmentEnvironment())
                    {
                        return GetTestUserInfo(username);
                    }
                }

                return null;
            });
        }

        private List<string> GetUserGroups(UserPrincipal userPrincipal)
        {
            var groups = new List<string>();
            try
            {
                var groupPrincipals = userPrincipal.GetGroups();
                foreach (var group in groupPrincipals)
                {
                    groups.Add(group.Name);
                }
            }
            catch (Exception)
            {
                // Handle group retrieval errors
            }
            return groups;
        }

        private bool IsDevelopmentEnvironment()
        {
            var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
            return environment == "Development";
        }

        // Test credentials for development/demo purposes
        private bool ValidateTestCredentials(string username, string password)
        {
            var testUsers = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "editor", "editor123" },
                { "viewer", "viewer123" }
            };

            return testUsers.ContainsKey(username) && testUsers[username] == password;
        }

        private AdUserInfo? GetTestUserInfo(string username)
        {
            var testUserInfo = new Dictionary<string, AdUserInfo>
            {
                { "admin", new AdUserInfo { Username = "admin", DisplayName = "Administrator", Email = "admin@company.com", Groups = new List<string> { "Administrators" } } },
                { "editor", new AdUserInfo { Username = "editor", DisplayName = "Content Editor", Email = "editor@company.com", Groups = new List<string> { "Editors" } } },
                { "viewer", new AdUserInfo { Username = "viewer", DisplayName = "Content Viewer", Email = "viewer@company.com", Groups = new List<string> { "Viewers" } } }
            };

            return testUserInfo.ContainsKey(username) ? testUserInfo[username] : null;
        }
    }
} 