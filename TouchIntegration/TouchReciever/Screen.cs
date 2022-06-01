using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TouchReceiver
{
    public static class Screen
    {
        private enum SystemMetric
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        private enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);
 
        public static int GetXScreenResolution()
        {
            return GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        public static int GetYScreenResolution()
        {
            return GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }
    }
}
