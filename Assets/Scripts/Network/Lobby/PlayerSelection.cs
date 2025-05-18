using System;
using Unity.Netcode;
using System.Linq;
using Unity.Collections;

public struct PlayerSelection : INetworkSerializable, IEquatable<PlayerSelection>
{
    public ulong ClientId;
    public FixedString64Bytes DisplayName;
    public bool isReady;
    public int PickedCharacterId;

    public bool Equals(PlayerSelection other) =>
        ClientId == other.ClientId
     && DisplayName.Equals(other.DisplayName)
     && isReady == other.isReady
     && PickedCharacterId == other.PickedCharacterId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref DisplayName);
        serializer.SerializeValue(ref isReady);
        serializer.SerializeValue(ref PickedCharacterId);
    }
}