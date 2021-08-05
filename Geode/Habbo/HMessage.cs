using Geode.Network;
using System.Diagnostics;

namespace Geode.Habbo
{
    [DebuggerDisplay("{Id,nq}")]
    public class HMessage
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public bool IsOutgoing { get; set; }
        public static implicit operator ushort(HMessage message) => message?.Id ?? ushort.MaxValue;
        public HMessage(ushort id, string name, bool isOutgoing)
        {
            Id = id;
            Name = name;
            IsOutgoing = isOutgoing;
        }
        public bool Match(DataInterceptedEventArgs dataInterceptedEventArgs)
        {
            if ((dataInterceptedEventArgs.IsOutgoing == IsOutgoing) && (dataInterceptedEventArgs.Packet.Id == Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}