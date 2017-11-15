using IP21Streamer.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IP21Streamer.Application
{
    class App
    {
        #region Constants
        public static readonly int SECONDS = 1000;
        public static readonly int MINUTES = 60 * SECONDS;

        public static readonly int UPDATE_SETTINGS_INTERVAL = 1 * MINUTES;
        #endregion

        public static Settings Settings = new Settings();


        public App()
        {

            new Thread(KeepSettingsUpToDate).Start();
        }

        private void KeepSettingsUpToDate()
        {
            Thread.CurrentThread.IsBackground = true;

            while (true)
            {
                Thread.Sleep(UPDATE_SETTINGS_INTERVAL);
                Settings.Update();
            }
        }

    }
}
