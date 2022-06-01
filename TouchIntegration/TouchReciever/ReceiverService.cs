using OSC.NET;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using TUIO;

namespace TouchReceiver
{
    public class Config
    {
        public int ReceiverPort { get; set; }
        public int PanelCount { get; set; }
        public int PanelOffset { get; set; }
    }

    /// <summary>
    /// The ReceiverService's purpose is to receive touch messages from the touch proxy and the programmatically scale
    /// and execute mouse events based on the the messages and the time between messages.
    /// </summary>
    public class ReceiverService
    {
        private bool running = false;
        private OSCReceiver receiver;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Constructor for the Service object
        /// </summary>
        public ReceiverService()
        {
            var port = ReadConfig();
            receiver = new OSCReceiver(port);
        }

        /// <summary>
        /// Starts listening for messages from the TouchProxy
        /// </summary>
        public void Start()
        {
            ScaleCoordinates(0, 1, 1, out int x, out int y);
            Mouse.Move(x, y);

            Thread thread = new Thread(() => Receive());
            running = true;
            receiver.Connect();
            thread.Start();
        }

        /// <summary>
        /// Stops the Receiver
        /// </summary>
        public void Stop()
        {
            running = false;
            receiver.Close();
        }

        /// <summary>
        /// Handles receiving messages being sent from the TouchProxy
        /// </summary>
        private void Receive()
        {
            while (running)
            {
                try
                {
                    OSCPacket packet = receiver.Receive();
                    if (packet != null)
                    {
                        ProcessMessage((OSCMessage)packet);
                    }
                    else Console.WriteLine("null packet");
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        /// <summary>
        /// Processes messages sent from the TouchProxy
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(OSCMessage message)
        {
            // Return if invalid message
            ArrayList args = message.Values;
            if(args.Count != 3)
            {
                return;
            } else if(args[0] == null || args[0] is not int)
            {
                return;
            } else if (args[1] == null || args[1] is not float)
            {
                return;
            } else if (args[2] == null || args[2] is not float)
            {
                return;
            } 
            
            //else if (args[3] == null || args[3] is not TuioTime)
            //{
            //    return;
            //}

            // Null coalesces to get rid of warning. Handled in above statement
            int panelNumber = (int?)args[0] ?? -1;
            float x = (float?)args[1] ?? 0;
            float y = (float?)args[2] ?? 0;
            //TuioTime time = (TuioTime?)args[0] ?? new TuioTime();

            ScaleCoordinates(panelNumber, x, y, out int sX, out int sY);
            Mouse.Click(MouseButton.Left);


        }


        private static void ScaleCoordinates(int panelNumber, double inX, double inY, out int outX, out int outY)
        {
            var xScreenDimension = Screen.GetXScreenResolution();
            var yScreenDimension = Screen.GetYScreenResolution();
            var scale = Windows.Graphics.Display.ResolutionScale;
            // Multiply by Screen dimensions
            inX *= 
            // Divide by display scale https://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings/21450169#21450169
            //inX = inX / 1.25;
            //inY = inY / 1.25;

            // Normalize back with below equation
            //outX = (int)Math.Floor(inX) * 65535 / GetSystemMetrics(SystemMetric.SM_CXSCREEN);

            outX = (int)Math.Floor(inX) * 65535 / xScreenDimension;
            outY = (int)Math.Floor(inY) * 65535 / yScreenDimension;
        }

        /// <summary>
        /// Reads the config.json file and turns it into a mapping of port to handler ipAddress,port
        /// that are added to the portMap
        /// </summary>
        private int ReadConfig()
        {
            int port = -1;
            try
            {
                using (StreamReader reader = new StreamReader("config.json"))
                {
                    var json = reader.ReadToEnd();
                    var config = JsonSerializer.Deserialize<Config>(json);
                    port = config?.ReceiverPort ?? -1;
                }
            }
            catch (IOException)
            {
                Environment.Exit(1);
            }

            if(port < 1)
            {
                Environment.Exit(1);
            }

            return port;
        }
    }
}


