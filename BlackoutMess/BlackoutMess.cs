namespace BlackoutMess
{
    internal static class BlackoutMess
    {
        public static void Main(string[] args)
        {
            var dbHelper = new DBHelper();
            var settings = new Settings();
            var chatDataManager = new ChatDataManager(dbHelper, settings);
            
            
        }
    }
}