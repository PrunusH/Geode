using System;

namespace Geode.Habbo
{
    /// <summary>
    /// Represents all available StuffData flags.
    /// </summary>
    [Flags]
    public enum HStuffDataFlags
    {
        HasUniqueSerialNumber = 1 << 8
    }
}
