using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AksuProje
{
    public class UserAction : IUserAction
    {

        private List<Users> Users;
        public string UsersFilePath { get; private set; }
        public string NoticesFilePath { get; private set; }

        public UserAction(List<Users> UserList)
        {
            // Otomatik dosya yollarını oluştur
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dataFolderPath = Path.Combine(desktopPath, "Data");

            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }

            UsersFilePath = Path.Combine(dataFolderPath, "Users.txt");
            NoticesFilePath = Path.Combine(dataFolderPath, "Notices.txt");

            // dosyaları kontrol edip oluşturdm (CheckAndCreateFile fonksiyonu gibi)

            Users = GetUsersList(); // Kullanıcı listesini buradan aldım
        }
       

        


        private int GenerateNextId()
        {

            int maxId = Users.Count > 0 ? Users.Max(u => u.CustomerId) : 0;
            return maxId + 1;
        }
        public void AddUser(Users user)
        {
            user.CustomerId = GenerateNextId();
            user.CustomerId = Users.Count + 1;
            Users.Add(user);

        }

        public Users AuthenticateUser(string Email, string Password)
        {
            Users AuthenticatedUser = Users.FirstOrDefault(u => u.Email == Email);

            if (AuthenticatedUser != null && AuthenticatePassword(AuthenticatedUser, Password))
            {
                Console.WriteLine("Kimlik doğrulama başarılı!");
                return AuthenticatedUser;
            }
            else
            {
                Console.WriteLine("Kimlik doğrulama başarısız. Geçersiz e-posta veya şifre.");
                return null;
            }
        }

        private bool AuthenticatePassword(Users authenticatedUser, string password)
        {
            if (authenticatedUser.Password == password)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AuthenticateUser(Users user, string Password)
        {
            return user.Password == Password;
        }

        public void DeleteUser(string PhoneNumber)
        {
            Users UserToDelete = Users.SingleOrDefault(u => u.PhoneNumber == PhoneNumber);

            if (UserToDelete != null)
            {
                Users.Remove(UserToDelete);
                Console.WriteLine($"Kullanıcı silindi Ad: {UserToDelete.FirstName}, Soyad: {UserToDelete.LastName}, Telefon: {UserToDelete.PhoneNumber}");
                UpdateUsersFile();
            }
            else
            {
                Console.WriteLine($"Telefon numarası {PhoneNumber} ile kayıtlı kullanıcı bulunamadı....");
            }
        }


        public void GetByUserFilter(string filter)
        {
            if (filter.Length >= 3)
            {
                if (File.Exists(UsersFilePath))
                {
                    string[] satirlar = File.ReadAllLines(UsersFilePath);
                    bool kullaniciBulundu = false;

                    foreach (var satir in satirlar)
                    {
                        if (satir.Contains($"FirstName:{filter}", StringComparison.OrdinalIgnoreCase) ||
                            satir.Contains($"LastName:{filter}", StringComparison.OrdinalIgnoreCase) ||
                            satir.Contains($"Email:{filter}", StringComparison.OrdinalIgnoreCase) ||
                            satir.Contains($"PhoneNumber:{filter}", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine(satir);
                            kullaniciBulundu = true;
                        }
                    }

                    if (!kullaniciBulundu)
                    {
                        Console.WriteLine("Belirtilen kriterde kullanıcı bulunamadı");
                    }
                }
                else
                {
                    Console.WriteLine("Kullanıcı kayıtları bulunamadı");
                }
            }
            else
            {
                Console.WriteLine("Girdi uzunluğu min 3 karakter olmalıdır");
            }
        }



        public List<Users> GetUsersList()
        {
            List<Users> UserList = new List<Users>();

            // Dosya var mı yok mu kontrol et 
            if (File.Exists(UsersFilePath))
            {
                string[] lines = File.ReadAllLines(UsersFilePath);

                foreach (var line in lines)
                {
                    string[] UserFields = line.Split(',');

                    if (UserFields.Length >= 5 &&
                        bool.TryParse(UserFields[5], out bool isAdmin))
                    {
                        
                        Users user = new Users
                        {
                            CustomerId = int.Parse(UserFields[0]),
                            FirstName = UserFields[1],
                            LastName = UserFields[2],
                            PhoneNumber = UserFields[3],
                            Email = UserFields[4],
                            Password = UserFields[6],
                            IsAdmin = isAdmin
                        };

                        UserList.Add(user);
        


                    }
                }
            }
            return UserList;
        }



        public void UpdateUsersFile()
        {
            try
            {
                using (StreamWriter x = new StreamWriter(UsersFilePath))
                {
                    foreach (Users User in Users)
                    {
                        x.WriteLine($"{User.CustomerId},{User.FirstName},{User.LastName},{User.PhoneNumber},{User.Email},{User.IsAdmin}, {User.Password}");
                    }
                }
                Console.WriteLine("Kullanıcılar dosyaya güncellendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosyaya yazma hatası: {ex.Message}");
            }
        }
        public List<Users> GetUsersByFilter(string filter)
        {
            List<Users> filteredUsers = new List<Users>();

            foreach (var user in Users)
            {
                if (user.FirstName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    user.LastName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    user.PhoneNumber.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    filteredUsers.Add(user);
                }
            }

            UpdateUsersFile(); // Eğer kullanıcı listesinde bir değişiklik yapıldıysa dosyayı güncelle

            return filteredUsers;
        }



    }
}
