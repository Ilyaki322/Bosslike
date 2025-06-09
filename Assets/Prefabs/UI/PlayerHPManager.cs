using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class PlayerHPManager : NetworkBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private ScrollView hpList;

    // track bars + health‐change callbacks on *every* client
    private readonly Dictionary<ulong, ProgressBar> bars = new();
    private readonly Dictionary<ulong, NetworkVariable<float>.OnValueChangedDelegate> callbacks = new();

    // on the server only, remember which NetworkObjectReferences we’ve sent
    private readonly Dictionary<ulong, NetworkObjectReference> serverRefs = new();
    
    public override void OnNetworkSpawn()
    {
        // grab the UI on every peer
        hpList = uiDocument.rootVisualElement.Q<ScrollView>("hp-list");

        if (!IsServer)
        {
            bars.Clear();
            callbacks.Clear();
            return;
        }

        var nm = NetworkManager.Singleton;

        // 1) Hook up future joins/leaves
        nm.OnClientConnectedCallback += OnServerClientConnected;
        nm.OnClientDisconnectCallback += OnServerClientDisconnected;

        // 2) Send host’s own entry to itself
        OnServerClientConnected(OwnerClientId);
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnServerClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnServerClientDisconnected;
        }
    }

    private void OnServerClientConnected(ulong newId)
    {
        // A) first, send *all existing* entries to *just* the newcomer
        foreach (var kv in serverRefs)
        {
            var existingId = kv.Key;
            var objRef = kv.Value;
            var ctx = NetworkManager.Singleton
                             .ConnectedClients[existingId]
                             .PlayerObject.GetComponent<UnitContext>();

            // targeted RPC → only newId receives it
            AddEntryClientRpc(objRef,ctx.MaxHealth);
        }
        
        // B) now add & broadcast the newcomer to everyone
        SendAddToAllRpc(newId);
    }

    private void OnServerClientDisconnected(ulong clientId)
    {
        if (serverRefs.TryGetValue(clientId, out var objRef))
        {
            // broadcast removal
            RemoveEntryClientRpc(objRef);
            serverRefs.Remove(clientId);
        }
    }

    // helper to store & broadcast one ID → all clients
    private void SendAddToAllRpc(ulong clientId)
    {
        var netObj = NetworkManager.Singleton
                     .ConnectedClients[clientId]
                     .PlayerObject;
        var objRef = new NetworkObjectReference(netObj);
        serverRefs[clientId] = objRef;

        var healthNetwork = netObj.GetComponent<Healthbar_Network>();
        AddEntryClientRpc(objRef, healthNetwork.CurrHP.Value);
    }

    // this RPC will run on *all* clients, unless you override via ClientRpcParams
    [ClientRpc]
    private void AddEntryClientRpc(NetworkObjectReference objRef, float currHP, ClientRpcParams rpcParams = default)
    {
        if (!objRef.TryGet(out var netObj)) return;
        var id = netObj.OwnerClientId;

        if (bars.ContainsKey(id))
            return;

        var hb = netObj.GetComponent<Healthbar_Network>();
        if (hb == null) return;

        var entry = new VisualElement();
        entry.AddToClassList("player-entry");

        var nameLabel = new Label($"ID: {id}");
        nameLabel.AddToClassList("player-name");
        entry.Add(nameLabel);

        var bar = new ProgressBar();
        bar.lowValue = 0;
        bar.highValue = currHP;
        bar.value = currHP;
        bar.AddToClassList("hp-bar");
        entry.Add(bar);

        hpList.contentContainer.Add(entry);
        bars[id] = bar;

        // 3) Subscribe to real health changes
        NetworkVariable<float>.OnValueChangedDelegate cb = (_, newVal) =>
        {
            if (bars.TryGetValue(id, out var b))
                b.value = newVal;
        };
        hb.CurrHP.OnValueChanged += cb;
        callbacks[id] = cb;
    }

    [ClientRpc]
    private void RemoveEntryClientRpc(
        NetworkObjectReference objRef,
        ClientRpcParams rpcParams = default)
    {
        if (!objRef.TryGet(out var netObj)) return;
        var id = netObj.OwnerClientId;
        
        // destroy UI
        if (bars.TryGetValue(id, out var bar))
        {
            bar.parent.RemoveFromHierarchy();
            bars.Remove(id);
        }

        // unsubscribe
        var uc = netObj.GetComponent<Healthbar_Network>();
        if (uc != null && callbacks.TryGetValue(id, out var cb))
        {
            uc.CurrHP.OnValueChanged -= cb;
            callbacks.Remove(id);
        }
    }
}
