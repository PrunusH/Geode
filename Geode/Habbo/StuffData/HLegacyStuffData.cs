using Geode.Network.Protocol;

namespace Geode.Habbo.StuffData
{
    public class HLegacyStuffData : HStuffData
    {
        public string Data { get; set; }

        public HLegacyStuffData()
            : base(HStuffDataFormat.Legacy)
        { }
        public HLegacyStuffData(HPacket packet)
            : this()
        {
            Data = packet.ReadUTF8();
        }
    }
}
