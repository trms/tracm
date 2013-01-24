using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.IO;

namespace tracm
{
    public static class LogHelper
    {
        private static ILog m_Logger = null;

        public static ILog Logger
        {
            get
            {
                if (m_Logger == null)
                {
                    SetupLogger();
                }
                return m_Logger;
            }

        }

        private static void SetupLogger()
        {
            m_Logger = LogManager.GetLogger("root");

            FileInfo info = new FileInfo("Log.config");

            log4net.Config.XmlConfigurator.ConfigureAndWatch(info);
        }
    }
}
