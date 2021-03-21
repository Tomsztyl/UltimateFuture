using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChooseManager : MonoBehaviour
{
    [Header("Variable set in RoomPlayer")]
    [Tooltip("This variable is a controll who set instantite object in RoomPlayer")]
    [SerializeField] private GameObject objectCharacterChoose = null;
    [Tooltip("This variable set object in Room Scene ")]
    [SerializeField] private GameObject objectCharacterChoosePreview = null;
    [Tooltip("This variable select if gameobject is First Choose Default")]
    [SerializeField] private bool isFirstChooseDefault = false;

    [Header("Variable select RoomPlayer")]
    [Tooltip("This variable set a room Player")]
    [SerializeField] private GameObject roomSelect = null;
    //public ControllRoom selectCharacter = null;

    private void Start()
    {
        if (isFirstChooseDefault)
        {
            SendToRoomObjectCharacter();
        }
    }

    private GameObject GetObjectCharacterChoose()
    {
        return objectCharacterChoose;
    }
    public void SetObjectRoomSelect(GameObject roomSelect)
    {
        this.roomSelect = roomSelect;
    }
    public void SendToRoomObjectCharacter()
    {
        roomSelect.GetComponent<SelectCharacterManager>().SelectChoosePlayerInRoom(objectCharacterChoose);
        CharacterPreviewController characterPreviewController = GameObject.FindObjectOfType<CharacterPreviewController>().GetComponent<CharacterPreviewController>();

        if (characterPreviewController!=null)
        {
            characterPreviewController.SetPrefabSpawnCharacter(objectCharacterChoosePreview);
        }
    }
}
