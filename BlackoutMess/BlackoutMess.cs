using System;

namespace BlackoutMess
{
    internal static class BlackoutMess
    {
        public static void Main(string[] args)
        {
            var dbHelper = new DBHelper();
            var settings = new Settings();
            var chatDataManager = new ChatDataManager(dbHelper, settings);
            
            int result;
            do
            {
                Console.WriteLine("Authenticating...");
                Console.Write("Enter username: ");
                string userName = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                result = chatDataManager.AuthUser(userName, password);
            } while (result != 0);
            
            chatDataManager.PrintAvailableChats();
            
        }
    }
}