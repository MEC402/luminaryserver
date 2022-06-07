using OSC.NET;
using System.Collections;
using System.Drawing;
using System.Text.Json;
using System.Diagnostics;

namespace TouchReceiver
{
    /// <summary>
    /// Object representation of the config file
    /// used by the ReceiverService.
    /// </summary>
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
        private int _xScreenDimension = Screen.GetXScreenResolution();
        private int _yScreenDimension = Screen.GetYScreenResolution();
        private decimal _xScale;
        //private decimal _yScale = (decimal) 66.0/ (decimal) 96.0;
        private decimal _yScale = (decimal)66.0 / (decimal)96.0;




        private readonly object _runLock = new object();
        private readonly object _clickLock = new object();
        private volatile bool _running = false;
        private bool _isRunning
        {
            get
            {
                lock (_runLock)
                {
                    return _running;
                }
            }
            set
            {
                lock (_runLock)
                {
                    _running = value;
                }
            }
        }

        private readonly Stopwatch _stopWatch = new Stopwatch();
        private Point _lastTouchLocation = new Point();
        private OSCReceiver _receiver;
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
            _xScale = (decimal) _xScreenDimension /_panelCount;

            Console.WriteLine($"Dimensions {_xScreenDimension}x{_yScreenDimension}");
            Console.WriteLine($"Scales {_xScale}, {_yScale}");
        }

        /// <summary>
        /// Starts listening for messages from the TouchProxy
        /// </summary>
        public void Start()
        {
            ScaleCoordinates(0, 1, 0, out int x, out int y);
            Mouse.Move(x, y);

            Thread thread = new Thread(() => Receive());
            _isRunning = true;
            _receiver.Connect();
            thread.Start();
        }

        /// <summary>
        /// Stops the Receiver
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _timer.Dispose();
            _receiver.Close();
        }

        /// <summary>
        /// Handles receiving messages being sent from the TouchProxy
        /// </summary>
        private void Receive()
        {
            while (_isRunning)
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
        /// <param name="message">Message sent from the TouchProxy</param>
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

            ScaleCoordinates(panelNumber, (decimal) x, (decimal) y, out int sX, out int sY);
            HandleClick(sX, sY);
        }

        /// <summary>
        /// Handles preforming a left/right click
        /// and moving/dragging the cursor
        /// </summary>
        /// <param name="x">Scaled X coordinate of the touch location</param>
        /// <param name="y">Scaled Y coordinate of the touch location</param>
        private void HandleClick(int x, int y)
        {
            lock (_clickLock)
            {
                var curPos = new Point(x, y);
                Mouse.Move(x, y);
                // Handle initial touch
                //if (!_clicked)
                //{
                //    Mouse.Click(MouseButton.Left);
                //    _clicked = true;
                //    _stopWatch.Start();
                //}
                //else // Handle holding/dragging
                //{
                //    // If within distance assume holding
                //    if (PointsWithinDistance(curPos, _lastTouchLocation, _holdNoise))
                //    {
                //        // If held for enough time, right click
                //        if (_stopWatch.ElapsedMilliseconds > _holdTime)
                //        {
                //            Mouse.Click(MouseButton.Right);
                //            Mouse.Release(MouseButton.Right);
                //            _stopWatch.Reset();
                //        }
                //    }
                //    else // Not within distance assume dragging
                //    {
                //        _stopWatch.Restart();
                //    }
                //}
                //_timer.Change(_experationTime, Timeout.Infinite);
                //_lastTouchLocation = curPos;   
            }
        }

        /// <summary>
        /// Releases the left mouse click when
        /// the timer expires.
        /// </summary>
        /// <param name="source"></param>
        private void TimerExpired(object? source)
        {
            lock (_clickLock)
            {
                Mouse.Release(MouseButton.Left);
                _clicked = false;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _stopWatch.Reset();
            }
        }

        /// <summary>
        /// Calculates if two points are within a specified distance
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <param name="distance">Distance between the two points</param>
        /// <returns></returns>
        private static bool PointsWithinDistance(Point p1, Point p2, int distance)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;
            return (dx*dx + dy*dy) < distance * distance;
        }

        /// <summary>
        /// Scales the x and y coordinates from the proxy
        /// to account for panel foil size and what panel
        /// they came from.
        /// </summary>
        /// <param name="panelNumber">The panel number the coordinates came from</param>
        /// <param name="inX">Normalized x coordinate from the proxy</param>
        /// <param name="inY">Normalized y coordinate from the proxy</param>
        /// <param name="outX">Scaled x coordinate</param>
        /// <param name="outY">Scaled y coordinate</param>
        private void ScaleCoordinates(int panelNumber, decimal inX, decimal inY, out int outX, out int outY)
        {
            // double xPanelDimension = _xScreenDimension / (double) _panelCount;
            Console.WriteLine($"inX -> {inX}, inY -> {inY}");
            decimal x = inX * _xScale + _xScale * panelNumber - (decimal) Math.Pow((double)inX, 2) * 30;



            decimal y = inY * _yScale * _yScreenDimension ;
            Console.WriteLine($"x -> {x}, y -> {y}");
            Mouse.GetCursorPos(out Mouse.PointInter point);
            Console.WriteLine(point.Y);

            //double yPanelScale = 66.0/96.0;
            //double botDeadZone = 28.0 / 96.0;

            //Console.WriteLine(_xScreenDimension);
            //inX = (inX * xPanelDimension + panelNumber * xPanelDimension) / _xScreenDimension;
            ////inY = (inY * (yScreenDimension - yScreenDimension * deadZone)) / yScreenDimension * yPanelScale;
            //double testY = inY - botDeadZone + (0.01 * inY);
            //double testX = inX + (-0.005 * inX/2);

            //Test1, red, -0.01
            //Test2, yellow, 0.02
            //double testY = inY * yPanelScale;
            //Console.WriteLine($"{inY} scales to {testY}");
            //Console.WriteLine($"Input to Screen Ratio {inY*_yScreenDimension / testY*(_yScreenDimension - (_yScreenDimension * botDeadZone))}");
            ////inY = inY / (yPanelScale);
            //inY = inY * ;
            // Convert
            outX = (int)Math.Round(x);
            outY = (int)Math.Round(y);
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


