using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PopulateCharacters : MonoBehaviour
{
    [Header("Character Database (SO)")]
    [SerializeField] private CharacterDatabase database;

    [Header("UI Prefab + Container")]
    [SerializeField] private GameObject characterCardPrefab;
    [SerializeField] private Transform characterCardContainer;

    // All character cards indexed by their ID
    public Dictionary<int, CharacterCard> Cards { get; } = new Dictionary<int, CharacterCard>();

    // Tracks the last picked characterId for each client
    private readonly Dictionary<ulong, int> lastPickByClient = new();

    private void Start()
    {
        InitializeCards();

        // When this client disconnects, clear local state
        NetworkManager.Singleton.OnClientConnectedCallback += OnAnyClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnAnyClientDisconnect;
        // Bootstrap picks and subscribe to lobby events
        InitPicked();
    }

    private void OnDestroy()
    {
        // Unsubscribe network callbacks
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnAnyClientDisconnect;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnAnyClientConnected;

        // Unsubscribe lobby events
        LobbyManager.Instance.PlayerJoined -= OnPlayerJoined;
        LobbyManager.Instance.PlayerLeft -= OnPlayerLeft;
        LobbyManager.Instance.PlayerPicked -= OnPlayerPicked;

        lastPickByClient.Clear();
    }

    private void OnAnyClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            foreach (var card in Cards.Values)
                card.SetSelectable(true);

            lastPickByClient.Clear();
            InitPicked();
        }
    }

    private void OnAnyClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            foreach (var kv in Cards)
                kv.Value.SetSelectable(true);

            LobbyManager.Instance.PlayerJoined -= OnPlayerJoined;
            LobbyManager.Instance.PlayerLeft -= OnPlayerLeft;
            LobbyManager.Instance.PlayerPicked -= OnPlayerPicked;
            lastPickByClient.Clear();
        }
    }

    private void InitializeCards()
    {
        foreach (var data in database.charactersData)
        {
            var go = Instantiate(characterCardPrefab, characterCardContainer);
            var card = go.GetComponent<CharacterCard>();
            card.SetCharacterData(data, OnCardClicked);
            Cards[data.Id] = card;
        }
    }

    private void InitPicked()
    {
        // Populate lastPickByClient from server state
        foreach (var sel in LobbyManager.Instance.PlayerSelections)
            lastPickByClient[sel.ClientId] = sel.PickedCharacterId;

        // Subscribe to lobby events
        LobbyManager.Instance.PlayerJoined += OnPlayerJoined;
        LobbyManager.Instance.PlayerLeft += OnPlayerLeft;
        LobbyManager.Instance.PlayerPicked += OnPlayerPicked;

        // Immediately disable already picked cards
        foreach (var kv in lastPickByClient)
        {
            var charId = kv.Value;
            if (charId >= 0 && Cards.TryGetValue(charId, out var card))
                card.SetSelectable(false);
        }
    }

    private void OnPlayerJoined(ulong clientId)
    {
        lastPickByClient[clientId] = -1;
    }

    private void OnPlayerLeft(ulong clientId)
    {
        if (lastPickByClient.TryGetValue(clientId, out var oldId) && oldId >= 0)
            Cards[oldId].SetSelectable(true);

        lastPickByClient.Remove(clientId);
    }

    private void OnPlayerPicked(ulong clientId, int newCharacterId)
    {
        if (lastPickByClient.TryGetValue(clientId, out var oldId) && oldId >= 0)
            Cards[oldId].SetSelectable(true);

        if (Cards.TryGetValue(newCharacterId, out var newCard))
            newCard.SetSelectable(false);

        lastPickByClient[clientId] = newCharacterId;
    }

    private void OnCardClicked(int characterId)
    {
        LobbyManager.Instance.PickCharacterServerRpc(characterId);
    }
}
