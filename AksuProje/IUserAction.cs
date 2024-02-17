using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AksuProje
{
    public interface IUserAction
    {
        void AddUser(Users users);
        public void UpdateUsersFile();
        List<Users> GetUsersList();
        void DeleteUser(string PhoneNumber);
        void GetByUserFilter(string filter);
        Users AuthenticateUser(string Email, string Password);
        bool AuthenticateUser(Users users, string Password);

    }
}
