using System.Collections.Generic;

namespace MalumMenu.Utilities;

// Custom equality comparer for NetworkedPlayerInfo that uses ClientId
// Allows for reliable equality comparison in collections even if cosmetics, color, etc. change
public sealed class NetPlayerInfoCidComparer : IEqualityComparer<NetworkedPlayerInfo>
{
    public bool Equals(NetworkedPlayerInfo data1, NetworkedPlayerInfo data2)
    {
        return data1.ClientId == data2.ClientId;
    }

    public int GetHashCode(NetworkedPlayerInfo data)
    {
        return data.ClientId;
    }
}
