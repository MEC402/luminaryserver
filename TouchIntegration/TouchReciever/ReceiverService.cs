using OSC.NET;
using System.Collections;
using System.Drawing;
using System.Text.Json;
using System.Diagnostics;

namespace TouchReceiver
{
    public class Config
    {
        public int ReceiverPort { get; set; }
        public int PanelCount { get; set; }
        public int PanelOffset { get; set; }
        public int ExpirationTime { get; set; }
        public int HoldTime { get; set; }
        public int HoldNoise { get; set; }
    }

    /// <summary>
    /// The ReceiverService's purpose is to receive touch messages from the touch proxy and the programmatically scale
    /// and execute mouse events based on the the messages and the time between messages.
    /// </summary>
    public class ReceiverService
    {
        private int _experationTime;
        private int _panelCount;
        private int _holdTime;
        private int _offest;
        private int _holdNoise;

        private OSCReceiver _receiver;
        private bool _running = false;
        private object _lock = new object();

        private readonly Stopwatch _stopWatch = new Stopwatch();
        private Point _lastTouchLocation = new Point(); 
        private bool _clicked = false;
        private Timer _timer;


        /// <summary>
        /// Constructor for the Service object
        /// </summary>
        public ReceiverService()
        {
            var config = ReadConfig();
            _receiver = new OSCReceiver(config.ReceiverPort);
            _timer = new Timer(TimerExpired, null, Timeout.Infinite, Timeout.Infinite);
            _experationTime = config.ExpirationTime;
            _holdTime = config.HoldTime;
            _offest = config.PanelOffset;
            _panelCount = config.PanelCount;
            _holdNoise = config.HoldNoise;
        }

        /// <summary>
        /// Starts listening for messages from the TouchProxy
        /// </summary>
        public void Start()
        {
            ScaleCoordinates(0, 1, 0, out int x, out int y);
            Mouse.Move(x, y);

            Thread thread = new Thread(() => Receive());
            _running = true;
            _receiver.Connect();
            thread.Start();
        }

        /// <summary>
        /// Stops the Receiver
        /// </summary>
        public void Stop()
        {
            _running = false;
            _timer.Dispose();
            _receiver.Close();
        }

        /// <summary>
        /// Handles receiving messages being sent from the TouchProxy
        /// </summary>
        private void Receive()
        {
            while (_running)
            {
                try
                {
                    OSCPacket packet = _receiver.Receive();
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

            int panelNumber = (int?)args[0] ?? -1;
            float x = (float?)args[1] ?? 0;
            float y = (float?)args[2] ?? 0;

            ScaleCoordinates(panelNumber, x, y, out int sX, out int sY);
            HandleClick(sX, sY);
        }

        private void HandleClick(int x, int y)
        {
            var curPos = new Point(x, y);
            lock (_lock)
            {
                Mouse.Move(x, y);                
                // Handle initial touch
                if (!_clicked)
                {
                    Mouse.Click(MouseButton.Left);
                    _clicked = true;
                    _stopWatch.Start();
                } else // Handle holding/dragging
                {
                    // If within distance assume holding
                    if(PointsWithinDistance(curPos, _lastTouchLocation, _holdNoise))
                    {
                        // If held for enough time, right click
                        if(_stopWatch.ElapsedMilliseconds > _holdTime)
                        {
                            Mouse.Click(MouseButton.Right);
                            Mouse.Release(MouseButton.Right);
                            _stopWatch.Reset();
                        }
                    } else // Not within distance assume dragging
                    {
                        _stopWatch.Restart();
                    }
                }
                _timer.Change(_experationTime, Timeout.Infinite);
            }
            _lastTouchLocation = curPos;
        }

        private void TimerExpired(object? source)
        {
            lock (_lock)
            {
                Mouse.Release(MouseButton.Left);
                _clicked = false;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _stopWatch.Reset();
            }
        }

        private static bool PointsWithinDistance(Point p1, Point p2, int distance)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;
            return (dx*dx + dy*dy) < distance * distance;
        }

        private void ScaleCoordinates(int panelNumber, double inX, double inY, out int outX, out int outY)
        {
            var xScreenDimension = Screen.GetXScreenResolution();
            var yScreenDimension = Screen.GetYScreenResolution();

            //inX *= (panelNumber - offest) / panelCount;
            //inY *= 1234;
            //inY += -344;

            // Convert
            outX = (int)Math.Floor(inX * 65535);
            outY = (int)Math.Floor(inY * 65535);
        }

        /// <summary>
        /// Reads the config.json file and turns it into a mapping of port to handler ipAddress,port
        /// that are added to the portMap
        /// </summary>
        private static Config ReadConfig()
        {
            Config? config = null;
            try
            {
                using (StreamReader reader = new StreamReader("config.json"))
                {
                    var json = reader.ReadToEnd();
                    config = JsonSerializer.Deserialize<Config>(json);
                }
            }
            catch (IOException)
            {
                Environment.Exit(1);
            }

            if(config == null)
            {
                Environment.Exit(1);
            }

            return config;
        }
    }
}


