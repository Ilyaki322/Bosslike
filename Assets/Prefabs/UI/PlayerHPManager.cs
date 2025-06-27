using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class PlayerHPManager : NetworkBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private ScrollView hpList;
    private readonly Dictionary<ulong, ProgressBar> bars = new();
    private readonly Dictionary<ulong, NetworkVariable<float>.OnValueChangedDelegate> callbacks = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // 1) Grab the UI ScrollView
        hpList = uiDocument.rootVisualElement.Q<ScrollView>("hp-list");
        if (hpList == null)
        {
            Debug.LogError("[PlayerHPManager] Couldn't find ScrollView named 'hp-list'");
            return;
        }

        // 2) Seed everyone already connected (host counts too)
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            AddPlayerEntry(client.ClientId);

        // 3) Watch for future joins / leaves
        NetworkManager.Singleton.OnClientConnectedCallback += AddPlayerEntry;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayerEntry;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.OnClientConnectedCallback -= AddPlayerEntry;
        NetworkManager.Singleton.OnClientDisconnectCallback -= RemovePlayerEntry;
    }

    private void AddPlayerEntry(ulong clientId)
    {
        // avoid duplicates
        if (bars.ContainsKey(clientId))
            return;

        // make sure their object has spawned
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var nc))
            return;
        var playerObj = nc.PlayerObject;
        if (playerObj == null)
            return;

        // grab its Healthbar_Network
        var hb = playerObj.GetComponent<Healthbar_Network>();
        if (hb == null)
            return;

        // build the UI row
        var container = new VisualElement();
        container.AddToClassList("player-entry");

        // name = client ID
        var nameLabel = new Label($"ID: {clientId}");
        nameLabel.AddToClassList("player-name");
        container.Add(nameLabel);

        // HP bar
        var bar = new ProgressBar
        {
            lowValue = 0,
            highValue = hb.MaxHealth,
            value = hb.CurrHP.Value
        };
        bar.AddToClassList("hp-bar");
        container.Add(bar);

        hpList.contentContainer.Add(container);
        bars[clientId] = bar;

        // subscribe to live updates
        NetworkVariable<float>.OnValueChangedDelegate cb = (_, newHP) =>
        {
            if (bars.TryGetValue(clientId, out var b))
                b.value = newHP;
        };
        hb.CurrHP.OnValueChanged += cb;
        callbacks[clientId] = cb;
    }

    private void RemovePlayerEntry(ulong clientId)
    {
        // tear down UI
        if (bars.TryGetValue(clientId, out var bar))
        {
            bar.parent.RemoveFromHierarchy();
            bars.Remove(clientId);
        }

        // unsubscribe
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var nc))
        {
            var hb = nc.PlayerObject.GetComponent<Healthbar_Network>();
            if (hb != null && callbacks.TryGetValue(clientId, out var cb))
            {
                hb.CurrHP.OnValueChanged -= cb;
                callbacks.Remove(clientId);
            }
        }
    }
}