using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpMaterial : NetworkBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    [SerializeField] private Sprite spritePickUpObject = null;
    [SerializeField] private ScriptableObject prefabPickUp = null;
    [SyncVar]
    [SerializeField] private float countObject = 1;
    [SyncVar]
    [SerializeField] private bool isPistol = false;
    [SyncVar]
    [SerializeField] private bool isRifle = false;

    private void Start()
    {
        //Random coundObject
        RandomCountObject();
    }
    [ServerCallback]
    private void RandomCountObject()
    {
        countObject = Random.Range(1, 10);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inventoryManager = other.transform.Find("Canvas/InventoryManager").GetComponent<InventoryManager>();
            if (this.tag == "Gun")
            {

                CheckObjectEq();
                Destroy(this.gameObject);
            }
            else
            {
                CheckObjectEq();
                Destroy(this.gameObject);
            }
        }
    }
    public void CheckObjectEq()
    {
        if (inventoryManager.IsExistObject(prefabPickUp, spritePickUpObject))
        {
            //It is Exist in Equipment
            inventoryManager.SetObjectIsExist(prefabPickUp, spritePickUpObject, countObject);
        }
        else if (inventoryManager.IsHavePlaceInEq())
        {
            //Is Pleace In Equipment
            inventoryManager.SetObjectIsHavePlace(prefabPickUp, spritePickUpObject, countObject);
        }
        else
        {
            Debug.LogWarning("Object no exist and you don't have slot in EQ");
        }
    }
}
