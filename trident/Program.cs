using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using log4net;
//using log4net.Config;

namespace trident
{
    class Program
    {
        static List<Settings> syncSettings;
        static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            //XmlConfigurator.Configure(); // optional way to load log4net config from app.config
            if (log.IsInfoEnabled)
            {
                log.Info("Starting trident backup...");
            }
            // load settings.json file and retrieve sync settings
            // iterate through each sync setting item and start sync.

            loadSyncSettings();
            if (syncSettings == null || syncSettings.Count == 0)
            {
                log.Warn("syncSettings object is null or has zero sync settings.  Please check settings.json file content");
                return;
            }

            Sync sync = new Sync(syncSettings);
            sync.start();

            log.Info("Completing backup!!!");
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
