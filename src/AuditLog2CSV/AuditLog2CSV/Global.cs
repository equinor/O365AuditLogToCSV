using System;
using System.Collections.Generic;
using System.Text;

namespace AuditLog2CSV {
    internal static class Global {
        private static AppConfig _config;
        /// <summary>
        /// Current Application Config
        /// </summary>
        internal static AppConfig Config {
            get => _config ??= AppConfig.GetConfig();
            set => _config = value;
        }
    }
}
