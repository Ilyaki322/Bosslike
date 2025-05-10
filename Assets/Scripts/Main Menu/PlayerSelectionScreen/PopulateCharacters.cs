using System.Collections.Generic;
using UnityEngine;

public class PopulateCharacters : MonoBehaviour
{
    [Header("Character Database (SO)")]
    [SerializeField] private CharacterDatabase database;

    [Header("UI Prefab + Container")]
    [SerializeField] private GameObject characterCardPrefab;
    [SerializeField] private Transform characterCardContainer;

    public Dictionary<int, CharacterCard> Cards { get; }
        = new Dictionary<int, CharacterCard>();

    private void Start()
    {
        // 1) Spawn all cards
        foreach (var data in database.charactersData)
        {
            var go = Instantiate(characterCardPrefab, characterCardContainer);
            var card = go.GetComponent<CharacterCard>();
            card.SetCharacterData(data, OnCardClicked);
            Cards[data.Id] = card;
        }

        LobbyManager.Instance.PlayerPicked += (_, characterId) =>
            Cards[characterId].SetSelectable(false);
    }

    private void OnCardClicked(int characterId)
    {
        LobbyManager.Instance.RequestPickCharacterServerRpc(characterId);

        if (Cards.TryGetValue(characterId, out var card))
            card.SetSelectable(false);
    }
}
