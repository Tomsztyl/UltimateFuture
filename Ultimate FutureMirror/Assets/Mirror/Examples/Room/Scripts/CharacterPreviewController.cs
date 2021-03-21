using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreviewController : MonoBehaviour
{
    [SerializeField] private GameObject prefabSpawnCharacter;

    [SerializeField] private GameObject[] ringsUnderPlayer;


    private bool IsCharacterInstantiate(GameObject prefabCharacter)
    {
        if (prefabCharacter == prefabSpawnCharacter)
            return true;
        else
           return false;
    }

    public void SetPrefabSpawnCharacter(GameObject character)
    {
        if (IsCharacterInstantiate(character)) return;
        prefabSpawnCharacter = character;
        ChcekIsExistCharacter();
    }
    private void ChcekIsExistCharacter()
    {
        foreach (Transform objectCharacterChild in this.gameObject.transform)
        {
            Destroy(objectCharacterChild.gameObject);
            SpawnCharacterPrefab();
            return;
        }
        SpawnCharacterPrefab();
    }
    private void SpawnCharacterPrefab()
    {
        var characterPrefabInstantiate= Instantiate(prefabSpawnCharacter, this.gameObject.transform);
        Instantiate(ringsUnderPlayer[Random.Range(0, ringsUnderPlayer.Length)], characterPrefabInstantiate.transform);
    }
}
