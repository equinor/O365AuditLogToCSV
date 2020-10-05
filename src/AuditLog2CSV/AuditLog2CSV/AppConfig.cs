using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NLog;

namespace AuditLog2CSV {
    public class AppConfig {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string _defaultFile = "Settings.config";

        /// <summary>
        /// Save delimiter to use when saving CSV files
        /// </summary>
        public string CsvSaveDelimiter { get; set; } = ",";

        /// <summary>
        /// Audit Log delimiter to use when saving CSV files
        /// </summary>
        public string AuditLogCsvDelimiter { get; set; } = ",";

        /// <summary>
        /// Ignore Quotes while reading Audit log CSV file
        /// </summary>
        public bool IgnoreQuotesWhileReading { get; set; } = false;

        /// <summary>
        /// Export folder
        /// </summary>
        public string ExportFolder { get; set; } = "export\\";


        /// <summary>
        /// Use Sanitize for injection when saving CSV file (adding \ in front of  =, @, +, or -
        /// </summary>
        public bool UseCsvSanitizeForInjection { get; set; } = true;

        #region Methods

        /// <summary>
        /// Get config
        /// </summary>
        /// <returns></returns>
        public static AppConfig GetConfig() {
            try {
                logger.Debug("Loading settings from {0}", _defaultFile);
                AppConfig config;
                using (StreamReader file = File.OpenText(_defaultFile)) {
                    JsonSerializer serializer = new JsonSerializer();
                    config = (AppConfig)serializer.Deserialize(file, typeof(AppConfig));
                }
                return config;
            } catch (FileNotFoundException ex) {
                logger.Warn(ex, "Configuration file not found, creating new config");
                AppConfig newConfig = new AppConfig();
                try {
                    newConfig.SaveConfig();
                } catch (Exception e) {
                    logger.Error(e, "Unable to save new config created: {0}", e.Message);
                }
                return newConfig;
            }
        }
        /// <summary>
        /// Save configuration
        /// </summary>
        internal void SaveConfig() {
            logger.Debug("Saving settings to {0}", _defaultFile);
            using (StreamWriter file = File.CreateText(_defaultFile)) {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }
        }

        #endregion
    }
}
