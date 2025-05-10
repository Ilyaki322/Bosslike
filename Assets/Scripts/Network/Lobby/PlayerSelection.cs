using System;
using Unity.Netcode;
using System.Linq;

public struct PlayerSelection : INetworkSerializable, IEquatable<PlayerSelection>
{
    public ulong ClientId;
    public bool HasPicked;
    public int PickedCharacterId;

    public bool Equals(PlayerSelection other)
    {
        return ClientId == other.ClientId
            && HasPicked == other.HasPicked
            && PickedCharacterId == other.PickedCharacterId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref HasPicked);
        serializer.SerializeValue(ref PickedCharacterId);
    }
}