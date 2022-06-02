using OSC.NET;
using System.Text.Json;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Config
    {
        public string ProxyAddress { get; set; } = "";
        public int ProxyPort { get; set; }
        public int ExpirationTime { get; set; }
        public int HoldTime { get; set; }
        public int HoldNoise { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Console.WriteLine("Testing Proxy and Receiver");
            Console.WriteLine("Ensure they are both started\n");            
            var config = ReadConfig();

            Console.WriteLine("Testing Proxy can Recieve Message");
            Console.WriteLine("Enter any character to send a packet. Type q to move to next test");
            var transmitter = new OSCTransmitter(config.ProxyAddress, config.ProxyPort);
            transmitter.Connect();
            while (true)
            {
                if(Console.ReadLine() == "q")
                {
                    break;
                }
                
                var message = CreateMessage(.5f, .5f);
                transmitter.Send(message);
            }
            transmitter.Close();
        }

        public static OSCMessage CreateMessage(float x, float y)
        {
            /* Data must be in following format/order */
            //string command = (string)args[0];
            //long s_id = (int)args[1];
            //float xpos = (float)args[2];
            //float ypos = (float)args[3];
            //float xspeed = (float)args[4];
            //float yspeed = (float)args[5];
            //float maccel = (float)args[6];

            var msg = new OSCMessage("/tuio/2Dcur");
            msg.Append("set");
            msg.Append(0);
            msg.Append(x);
            msg.Append(y);
            msg.Append((float)0);
            msg.Append((float)0);
            msg.Append((float)0);
            return msg;
        }

        public static Config ReadConfig()
        {
            Config? config = null;
            try
            {
                using (StreamReader reader = new StreamReader("../../../config.json"))
                {
                    var json = reader.ReadToEnd();
                    config = JsonSerializer.Deserialize<Config>(json);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Failed to read config");
                Environment.Exit(1);
            }

            if (config == null)
            {
                Console.WriteLine("No Config");
                Environment.Exit(1);
            }

            return config;
        }
    }
}
