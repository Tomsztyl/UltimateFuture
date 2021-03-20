using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterManager : NetworkBehaviour
{
    [SerializeField] private GameObject selectCharacter = null;
    private Transform uIRoomScene = null;

    private void Start()
    {
        if (selectCharacter != null&&isLocalPlayer)
        {
            uIRoomScene=GameObject.FindGameObjectWithTag("UIRoomScene").GetComponent<Transform>();
            var selectCharacterInstantiare=Instantiate(selectCharacter, uIRoomScene);
            SetObjectInCharacterChooseManager(selectCharacterInstantiare);
        }

    }
    public void SelectChoosePlayerInRoom(GameObject characterObject)
    {
       GetComponent<NetworkRoomPlayerExt>().ExeciuteChangePlayerInstatiate(characterObject);
    }
    private void SetObjectInCharacterChooseManager(GameObject selectCharacterUI)
    {
        foreach (Transform objectCharacter in selectCharacterUI.transform)
        {

            CharacterChooseManager characterChooseManager = objectCharacter.GetComponent<CharacterChooseManager>();
            if (characterChooseManager != null)
            {
                characterChooseManager.SetObjectRoomSelect(this.gameObject);
            }
        }
    }
}
