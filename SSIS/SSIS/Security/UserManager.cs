using SSIS.Models;
using SSIS.Services;

namespace SSIS.Security
{
    public class UserManager
    {
        private LoginServices loginService;

        public UserManager()
        {
            loginService = new LoginServices();
        }
        public User IsValid(string username, string password)
        {
            User user = loginService.CheckEmployee(username);
            if (user == null)
            {
                if (user.Role != Enums.Role.STORE_CLERK && user.Role != Enums.Role.STORE_MANAGER && user.Role != Enums.Role.STORE_SUPERVISOR)
                {
                    loginService.CheckDelegationTable(user);
                }
                user = loginService.CheckStorePerson(username);
            }
            if (user != null)
            {
                if (user.Password == EncodePasswordToBase64(password))
                {
                    loginService.CheckDelegationTable(user);
                    return user;
                }
            }
            return null;
        }

        public static string EncodePasswordToBase64(string password)
        {
            return password;
        }
    }
}
