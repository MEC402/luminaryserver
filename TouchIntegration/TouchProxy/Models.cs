namespace TouchProxy
{
    /// <summary>
    /// Object containing Ip Address and Port that
    /// the TouchProxy needs to send the TUIO Events to.
    /// </summary>
    public class Receiver
    {
        public string Address { get; set; } = "";
        public int Port { get; set; }
    }

    /// <summary>
    /// Object acting as a model representation of an item in the config.json file. 
    /// Each item has the ports that a single wall will send events to as well as
    /// what ip-address and port the pc powering the wall is running on.
    /// </summary>
    public class WallConfig
    {
        public string ReceiverAddress { get; set; } = "";
        public int ReceiverPort { get; set; }
        public List<int> PanelPorts { get; set; } = new List<int>();
    }
}
