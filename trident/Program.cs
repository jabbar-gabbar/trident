using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using log4net;
using System.Configuration;
using System.Reflection;
//using log4net.Config;

namespace trident
{
    class Program
    {
        static List<Settings> syncSettings;
        static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static string settingsFileName = string.Empty;
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
                log.Error("some error");
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
                // read and check all the config details are present in app.config file. 
                settingsFileName = ConfigurationManager.AppSettings["SettingsFileName"];
                if (string.IsNullOrEmpty(settingsFileName))
                {
                    throw new InvalidOperationException("Cannot find SettingsFileName property or value in app.config file.");
                }
                string inventoryFolderName = ConfigurationManager.AppSettings["InventoryFolderName"];
                if (string.IsNullOrEmpty(inventoryFolderName))
                {
                    throw new InvalidOperationException("Cannot find InventoryFolderName property or value in app.config file.");
                }
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AWSProfileName"]))
                {
                    throw new InvalidOperationException("Cannot find AWSProfileName property or value in app.config file.");
                }
                // find out current executing directory through Assembly class to make sure if console app is started through other process such as task schedular.
                string currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                // verify values of the properties are valid. 
                if (!Directory.Exists(currentDirPath + "\\" + inventoryFolderName))
                {
                    // create inventory folder.
                    Directory.CreateDirectory(currentDirPath + "\\" + inventoryFolderName);
                }
                if (!File.Exists(currentDirPath + "\\" + settingsFileName))
                {
                    throw new InvalidOperationException("Cannot find " + settingsFileName + " file in the application folder. ");
                }

            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error(ex.Message, ex);
                throw new ConfigurationErrorsException("Error occured during readAllConfig(). ", ex);
            }
            //catch (IOException ioex)
            //{
            //    log.Error(ioex.Message, ioex);
            //    throw new IOException("Error occured during readAllCOnfig() IO Operation. ", ioex);
            //}
            //catch(UnauthorizedAccessException ex){
            //    log.Error(ex.Message, ex);
            //    throw new UnauthorizedAccessException("Error occured during readAllConfig(). The current user does not have sufficient permission to create folder in current directory.",ex);
            //}
        }

        static void loadSyncSettings()
        {
            // TODO: add file exists checks. and Exception block. 
            string strSettings = string.Empty;
            using (StreamReader r = new StreamReader(AppContext.BaseDirectory + "\\" + settingsFileName))
            {
                strSettings = r.ReadToEnd();
            }
            syncSettings = JsonConvert.DeserializeObject<List<Settings>>(strSettings);
        }
    }
}
