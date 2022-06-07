using OSC.NET;
using System.Collections;
using System.Net;
using System.Text.Json;
using TUIO;

namespace TouchProxy
{
    /// <summary>
    /// The ProxyService class's purpose is to act as a proxy that listens for TUIO events 
    /// and then forwards them to an ip address and port based on the config.json file. 
    /// The service expects to have TUIO events sent to it from multiple panels, 
    /// each on their own port. It then forwards those TUIO events to the ip and port that 
    /// the panel's port map to.
    /// </summary>
    public class ProxyService
    {
        private readonly object _runLock = new object();
        private volatile bool _running = false;
        private bool _isRunning
        {
            get
            {
                lock( _runLock)
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
        private TuioClient[] _clients;
        //private Thread[] _threads;

        /// <summary>
        /// Constructor for the Service object
        /// </summary>
        public ProxyService()
        {
            var portMap = ReadConfig();
            var keys = portMap.Keys.ToArray();
            _clients = new TuioClient[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                var client = new TuioClient(keys[i]);
                var listener = new Listener(i, portMap[keys[i]].Address, portMap[keys[i]].Port);
                client.addTuioListener(listener);
                _clients[i] = client;
            }
        }

        //public ProxyService(int a)
        //{
        //    var portMap = ReadConfig();
        //    var keys = portMap.Keys.ToArray();
        //    _threads = new Thread[keys.Length];
        //    for (int i = 0; i < keys.Length; i++)
        //    {
        //        _threads[i] = new Thread(() => Listen(i + 1, portMap[keys[i]].Address, portMap[keys[i]].Port);
        //    }
        //}

        //private void Listen(int panelNumber, string receiverAddress, int receiverPort)
        //{
        //    var receiver = new OSCReceiver(receiverPort);
        //    receiver.Connect();
        //    while (_isRunning)
        //    {
        //        try
        //        {
        //            OSCPacket packet = receiver.Receive();
        //            if (packet != null)
        //            {
        //                if (packet.IsBundle())
        //                {
        //                    ArrayList messages = packet.Values;
        //                    for (int i = 0; i < messages.Count; i++)
        //                    {
        //                       // processMessage((OSCMessage)messages[i]);
        //                    }
        //                }
        //                //else processMessage((OSCMessage)packet);
        //                Console.WriteLine(packet.Values);
        //            }
        //            else Console.WriteLine("null packet");
        //        }
        //        catch (Exception e) { Console.WriteLine(e.Message); }
        //    }
        //    receiver.Close();
        //}


        /// <summary>
        /// Connects and starts the TuioClients when
        /// the service is started
        /// </summary>
        public void Start()
        {
            foreach (var client in _clients)
            {
                client.connect();
            }
        }
        //public void Start()
        //{
        //    _isRunning = true;
        //    foreach (var thread in _threads)
        //    {
        //        thread.Start();
        //    }
        //}

        /// <summary>
        /// Disconnects and stops the TuioClients when
        /// the service is shutdown.
        /// </summary>
        public void Stop()
        {
            foreach (var client in _clients)
            {
                client.disconnect();
            }
        }
        //public void Stop()
        //{

        //    _isRunning = false;

        //}


        /// <summary>
        /// Reads the config.json file and turns it into a mapping of port to handler ipAddress,port
        /// that are added to the portMap
        /// </summary>
        private Dictionary<int, Receiver> ReadConfig()
        {
            var portMap = new Dictionary<int, Receiver>();
            try
            {
                using (StreamReader reader = new StreamReader("config.json"))
                {
                    var json = reader.ReadToEnd();
                    var config = JsonSerializer.Deserialize<List<WallConfig>>(json);
                    config = config ?? new List<WallConfig>();

                    foreach (var wall in config)
                    {
                        foreach (var port in wall.PanelPorts)
                        {
                            portMap.Add(port, new Receiver()
                            {
                                Address = wall.ReceiverAddress,
                                Port = wall.ReceiverPort
                            });
                        }
                    }
                }
            }
            catch (IOException)
            {
                Environment.Exit(1);
            }
            return portMap;
        }
    }

    /// <summary>
    /// The listener class is an implementation of the TuioListerner interface.
    /// It's defined functions get called by the TuioClient. Only the 
    /// updateTuioCursor needs to be implemented as panels will only send
    /// cursor events.
    /// </summary>
    public class Listener : TuioListener
    {
        private OSCTransmitter _transmitter;
        private int _panelNumber;

        public Listener(int panelNumber, string receiverAddr, int receiverPort)
        {
            _panelNumber = panelNumber;
            _transmitter = new OSCTransmitter(receiverAddr, receiverPort);
        }

        public void addTuioBlob(TuioBlob tblb) { }
        public void addTuioCursor(TuioCursor tcur) { }
        public void addTuioObject(TuioObject tobj) { }
        public void refresh(TuioTime ftime) { }
        public void removeTuioBlob(TuioBlob tblb) { }
        public void removeTuioCursor(TuioCursor tcur) { }
        public void removeTuioObject(TuioObject tobj) { }
        public void updateTuioBlob(TuioBlob tblb) { Console.WriteLine("blobl"); }

        /// <summary>
        /// Handles when an update cursor event is called
        /// </summary>
        /// <param name="tcur">Object containing information about the cursor touch event</param>
        public void updateTuioCursor(TuioCursor tcur)
        {
            var packet = new OSCMessage("/tuio/2Dcur");
            packet.Append(_panelNumber);
            packet.Append(tcur.X);
            packet.Append(tcur.Y);            
            _transmitter.Send(packet);
        }
        public void updateTuioObject(TuioObject tobj) { Console.WriteLine("obj"); }
    }
}


