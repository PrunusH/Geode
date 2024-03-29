﻿using Geode.Network.Protocol;

namespace Geode.Habbo.Packages.StuffData
{
    public class HCrackableStuffData : HStuffData
    {
        public string State { get; set; }
        public int Hits { get; set; }
        public int Target { get; set; }

        public HCrackableStuffData()
            : base(HStuffDataFormat.Crackable)
        { }
        public HCrackableStuffData(HPacket packet)
            : this()
        {
            State = packet.ReadUTF8();
            Hits = packet.ReadInt32();
            Target = packet.ReadInt32();
        }
    }
}
