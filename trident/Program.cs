using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using log4net;
using System.Configuration;
//using log4net.Config;

namespace trident
{
    class Program
    {
        static List<Settings> syncSettings;
        static ILog log = LogManager.GetLogger(typeof(Program));
        static string settingFileName = string.Empty;
        static void Main(string[] args)
        {
            try
            {
                //XmlConfigurator.Configure(); // optional way to load log4net config from app.config
                if (log.IsInfoEnabled)
                {
                    log.Info("Starting trident backup...");
                }
                // Check for configuration are in proper state. 
                readAllConfig();
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
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}", ex.Message), ex);
            }
        }

        private static void readAllConfig()
        {
            try
            {
                settingFileName = ConfigurationManager.AppSettings["SettingsFileNameasdfasdf"];
                if (string.IsNullOrEmpty(settingFileName))
                {
                    throw new SettingsPropertyNotFoundException("Cannot find SettingsFileName property in config file.");
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error(ex.Message, ex);
                throw new ConfigurationErrorsException("Error occured during readAllConfig(). ", ex);
            }
        }

        static void loadSyncSettings()
        {
            // TODO: add file exists checks. and Exception block. 
            string strSettings = string.Empty;
            using (StreamReader r = new StreamReader(AppContext.BaseDirectory + "\\" + settingFileName))
            {
                strSettings = r.ReadToEnd();
            }
            syncSettings = JsonConvert.DeserializeObject<List<Settings>>(strSettings);
        }
    }
}
