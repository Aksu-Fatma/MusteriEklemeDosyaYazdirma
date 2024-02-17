using System;
using System.Collections.Generic;
using System.IO;

namespace AksuProje
{
    public class Program
    {
        class SessionManager
        {
            public static string? CurrentUserEmail { get; private set; }

            public static void SetCurrentUser(string email)
            {
                CurrentUserEmail = email;
            }
        }


        static void AddUser(UserAction userAction)
        {
            Console.Write("İsim: ");
            string FirstName = ReadNotNullInput();

            Console.Write("Soyisim: ");
            string LastName = ReadNotNullInput();

            Console.Write("Telefon: ");
            string PhoneNumber = ReadNotNullInput();

            while (!IsValidPhoneNumber(PhoneNumber, userAction))
            {
                Console.WriteLine("Hata: Başında sıfır ile tuşladınız veya telefon numarası zaten kullanılıyor.");
                Console.Write("Telefon Numarasını Tekrar Giriniz: ");
                PhoneNumber = ReadNotNullInput();
            }

            Console.WriteLine("Telefon numarası geçerli.");

            Console.Write("Email: ");
            string Email = ReadNotNullInput();

            Console.Write("Password: ");
            string Password = ReadNotNullInput();

            Console.Write("Admin mi: ");
            string isAdminInput = ReadNotNullInput();
            bool isAdmin;

            while (!bool.TryParse(isAdminInput, out isAdmin))
            {
                Console.WriteLine("Hata: Geçersiz Admin değeri. True veya False olarak giriniz.");
                Console.Write("Admin mi: ");
                isAdminInput = ReadNotNullInput();
            }

            Users newUser = new Users
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email,
                Password = Password,
                IsAdmin = isAdmin
            };

            userAction.AddUser(newUser);

            File.AppendAllText(userAction.UsersFilePath, $"{newUser.CustomerId},{FirstName},{LastName},{PhoneNumber},{Email},{isAdmin},{Password}\n");

            Console.WriteLine("Yeni kullanıcı başarıyla eklendi.");
        }

        static bool IsValidPhoneNumber(string phoneNumber, UserAction userAction)
        {
            return !phoneNumber.StartsWith("0") &&
                   !File.ReadAllText(userAction.UsersFilePath).Contains($"PhoneNumber:{phoneNumber}");
        }

        static string ReadNotNullInput()
        {
            string input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Hata: Boş bir giriş yapamazsınız. Tekrar deneyin.");
                input = Console.ReadLine();
            }
            return input;
        }

        static void DeleteUser(UserAction userAction)
        {
            Console.Write("Silinecek kullanıcının telefon numarasını girin: ");
            string silinecekKullanici = Console.ReadLine();
            userAction.DeleteUser(silinecekKullanici);
        }

        static void AddNote(NoteAction noteAction, Users user)
        {
            Console.Write("Notunuzu giriniz: ");
            string UserNote = Console.ReadLine();

            TimeZoneInfo turkishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateTime turkishTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, turkishTimeZone);
            string Userdate = turkishTime.ToString("dd.MM.yyyy HH:mm:ss");

            noteAction.AddNote(user, UserNote, Userdate);
        }



        static List<Users> DosyadanKullanicilariYukle(string dosyaYolu)
        {
            List<Users> kullaniciListesi = new List<Users>();

            try
            {
                string[] satirlar = File.ReadAllLines(dosyaYolu );

                foreach (string satir in satirlar)
                {
                    string[] dizin = satir.Split(',');

                    Users user = GetUser(dizin);
                    kullaniciListesi.Add(user);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Dosya bulunamadı: {dosyaYolu}");
            }

            return kullaniciListesi;

            static Users GetUser(string[] dizin)
            {
                return new Users
                {
                    CustomerId = int.Parse(dizin[0]),
                    FirstName = dizin[1],
                    LastName = dizin[2],
                    PhoneNumber = dizin[3],
                    Email = dizin[4],
                    IsAdmin = bool.Parse(dizin[5]),
                    Password = dizin[6],

                };
            }
        }

        static void CheckAndCreateFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }



        static void Main(string[] args)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dataFolderPath = Path.Combine(desktopPath, "Data");

            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }

            string usersFilePath = Path.Combine(dataFolderPath, "Users.txt");
            string noticesFilePath = Path.Combine(dataFolderPath, "Notices.txt");

            CheckAndCreateFile(usersFilePath);
            CheckAndCreateFile(noticesFilePath);

            NoteAction noteAction = new NoteAction();
            List<Users> kullaniciListesi = DosyadanKullanicilariYukle(usersFilePath);
            UserAction userAction = new UserAction(kullaniciListesi);

            Console.Write("Kullanıcı eklemesi yapmak istiyorsanız lütfen 1'e basınız.Kullanıcı girişi için ise 2'ye basınız");
            int tmp;
            while (!int.TryParse(Console.ReadLine(), out tmp) || (tmp != 1 && tmp != 2))
            {
                Console.WriteLine("Hata: Geçersiz seçim. 1 veya 2 giriniz.");
            }

            switch (tmp)
            {
                case 1:
                    AddUser(userAction);
                    break;
                case 2:
                    Console.Write("Mail giriniz: ");
                    string UserEmail = Console.ReadLine();

                    Console.Write("Parola giriniz: ");
                    string UserPassword = Console.ReadLine();

                    Users authenticatedUser = userAction.AuthenticateUser(UserEmail, UserPassword);

                    if (authenticatedUser != null)
                    {
                        bool exitProgram = false;

                        if (authenticatedUser.IsAdmin)
                        {
                            while (!exitProgram)
                            {
                                Console.WriteLine("Menü:");
                                Console.WriteLine("1. Kullanıcı Ekle");
                                Console.WriteLine("2. Kullanıcı Ara");
                                Console.WriteLine("3. Kullanıcı Sil");
                                Console.WriteLine("4. Programı Kapat");

                                Console.Write("Seçiminizi yapınız: ");
                                while (!int.TryParse(Console.ReadLine(), out tmp) || tmp < 1 || tmp > 4)
                                {
                                    Console.WriteLine("Hata: Geçersiz seçim. 1-4 arasında bir sayı giriniz.");
                                }

                                switch (tmp)
                                {
                                    case 1:
                                        AddUser(userAction);
                                        break;
                                    case 2:
                                        UserByFilter(userAction);
                                        break;
                                    case 3:
                                        DeleteUser(userAction);
                                        break;
                                    case 4:
                                        exitProgram = true;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            bool exitNoteProgram = false;

                            // Giriş başarılı olduğunda kullanıcıya menü göster
                            while (true) // Sonsuz bir döngü
                            {
                                //Console.Clear(); // Ekranı temizle

                                Console.WriteLine($"Hoşgeldiniz, {authenticatedUser.FirstName}");

                                Console.WriteLine("Menü:");
                                Console.WriteLine("1. Not Ekle");
                                Console.WriteLine("2. Notlarımı Listele");
                                Console.WriteLine("3. Çıkış");

                                Console.Write("Seçiminizi yapınız: ");
                                if (!int.TryParse(Console.ReadLine(), out int userChoice))
                                {
                                    Console.WriteLine("Hata: Geçersiz seçim. 1-3 arasında bir sayı giriniz.");
                                    continue;
                                }

                                switch (userChoice)
                                {
                                    case 1:
                                        AddNote(noteAction, authenticatedUser);
                                        break;
                                    case 2:
                                         noteAction.GetNoteList(authenticatedUser);
                                        break;
                                    case 3:
                                        Console.WriteLine("Çıkış yapılıyor...");
                                        Thread.Sleep(2000); // 2 saniye bekletme
                                        return; 
                                    default:
                                        Console.WriteLine("Geçersiz seçim.");
                                        break;
                                }
                            }
                        
                        }
                    }
                    else
                    {
                        while (authenticatedUser == null)
                        {
                            Console.Clear(); // Ekranı temizle

                            Console.WriteLine("Giriş başarısız. Lütfen geçerli bir e-posta ve şifre kombinasyonu kullanın.");

                      
                            Console.Write("Tekrar denemek ister misiniz? (Evet için 'e' / Hayır için herhangi bir tuş): ");
                            string retryChoice = Console.ReadLine();

                            if (retryChoice.ToLower() == "e")
                            {
                                Console.Write("Mail giriniz: ");
                                UserEmail = Console.ReadLine();

                                Console.Write("Parola giriniz: ");
                                UserPassword = Console.ReadLine();

                                authenticatedUser = userAction.AuthenticateUser(UserEmail, UserPassword);
                            }
                            else
                            {
                                // Kullanıcı programı kapatmayı tercih etti.
                                Console.WriteLine("Program Kapatıldı");
                                return; // Programı sonlandır
                            }
                        }
                    }
                    break;
            }
        }

        public static void UserByFilter(UserAction userAction)
        {
            Console.WriteLine("Kullanıcı Ara:");

            Console.Write("Arama terimini giriniz: ");
            string filter = Console.ReadLine();

            if (string.IsNullOrEmpty(filter) || filter.Length < 3)
            {
                Console.WriteLine("Hata: Arama terimi boş veya geçersiz. En az 3 karakter içermelidir.");
                return;
            }

            List<Users> filteredUsers = userAction.GetUsersByFilter(filter);

            if (filteredUsers.Count > 0)
            {
                Console.WriteLine("Arama sonuçları:");
                foreach (var user in filteredUsers)
                {
                    Console.WriteLine($"Id: {user.CustomerId}, Ad: {user.FirstName}, Soyad: {user.LastName}, Telefon: {user.PhoneNumber}, Email: {user.Email}, Admin mi: {user.IsAdmin}");
                }
            }
            else
            {
                Console.WriteLine("Belirtilen kriterde kullanıcı bulunamadı");
            }
        }
    }
}
