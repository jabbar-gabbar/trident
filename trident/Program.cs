using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace trident
{
    class Program
    {
        
        static List<Settings> syncSettings; 

        static void Main(string[] args)
        {
            // load settings.json file and retrieve sync settings
            // iterate through each sync setting item and start sync.
            loadSyncSettings();
            if (syncSettings == null || syncSettings.Count == 0)
                return;
            Sync sync = new Sync(syncSettings);
            sync.start();
         
        }

        static void loadSyncSettings()
        {
            string strSettings = string.Empty;
            using (StreamReader r = new StreamReader(AppContext.BaseDirectory + "\\settings.json"))
            {
                strSettings = r.ReadToEnd();
            }
            syncSettings = JsonConvert.DeserializeObject<List<Settings>>(strSettings);
        }
    }
}
