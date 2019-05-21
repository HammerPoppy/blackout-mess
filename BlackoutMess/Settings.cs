using System.Collections.Generic;

namespace BlackoutMess
{
    public class Settings
    {
        private List<Setting> Items;

        public Settings()
        {
            Items = new List<Setting>();

            Items.Add(new Setting("test setting", new[] {"true", "false"}, "true"));
            Items.Add(new Setting("show grid", new[] {"true", "false"}, "false"));
            Items.Add(new Setting("messages to display", new[] {"10", "20", "30", "50", "100"}, "30"));
        }

        public List<Setting> GetSettings()
        {
            return Items;
        }

        public Setting GetSetting(string name)
        {
            return Items.Find(x => x.Name == name);
        }
    }
}