using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TouchReceiver
{
    /// <summary>
    /// The Screen class's purpose is to abstract out calling the windows
    /// api to get screen information
    /// </summary>
    public static class Screen
    {
        /// <summary>
        /// Enum used to define which SystemMetric to use
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics
        /// </summary>
        private enum SystemMetric
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        /// <summary>
        /// Retrieves system metric or configuration settings
        /// </summary>
        /// <param name="smIndex">Which setting to retrieve</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);
 
        /// <summary>
        /// Retrieves the screen resolution along the x axis.
        /// Takes into account current system zoom level.
        /// </summary>
        /// <returns></returns>
        public static int GetXScreenResolution()
        {
            return GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        /// <summary>
        /// Retrieves the screen resolution along the y axis.
        /// Takes into account current system zoom level.
        /// </summary>
        /// <returns></returns>
        public static int GetYScreenResolution()
        {
            return GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }
    }
}
