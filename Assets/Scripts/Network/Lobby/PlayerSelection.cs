using System;
using Unity.Netcode;
using System.Linq;
using Unity.Collections;

public struct PlayerSelection : INetworkSerializable, IEquatable<PlayerSelection>
{
    public ulong ClientId;
    public FixedString64Bytes DisplayName;
    public bool HasPicked;
    public int PickedCharacterId;

    public bool Equals(PlayerSelection other) =>
        ClientId == other.ClientId
     && DisplayName.Equals(other.DisplayName)
     && HasPicked == other.HasPicked
     && PickedCharacterId == other.PickedCharacterId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref DisplayName);
        serializer.SerializeValue(ref HasPicked);
        serializer.SerializeValue(ref PickedCharacterId);
    }
}