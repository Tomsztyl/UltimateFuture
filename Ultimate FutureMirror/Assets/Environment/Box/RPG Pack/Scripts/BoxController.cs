using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BoxController : NetworkBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [SerializeField] private GameObject startPatricles;
    [SerializeField] private GameObject box001;
    [SerializeField] private GameObject destroy_box;

    //List Who is Instantiate GameObjcet from Box
    [SerializeField] private List<GameObject> objectInstantiateToDestroy = new List<GameObject>();

    //When Box is lover than -20f position y is destroy
    [SerializeField] private float rangeDestroyBoxWhenIsNoTerrain = -20f;

    //Radius Instantiate Object
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float chaseDistanceY = 10f;
    [SerializeField] private Vector3 sphareInstantiareObject;

    private void Update()
    {
        //update postion to instantiate object in sphare
        sphareInstantiareObject = new Vector3(transform.position.x, transform.position.y + chaseDistanceY, transform.position.z);
        if (transform.position.y < rangeDestroyBoxWhenIsNoTerrain)
        {
            Destroy(this.gameObject);
        }
    }
    #region Drop Mechanism Object from Box Pandora
    [ServerCallback]
    public void ExtractBoxPandora()
    {
        ChangeAnimation();
        DropRandomObjectFromList();
        DestoryObjectChange();
        DestroyObjectBoxPandora();
    }
    private void DropRandomObjectFromList()
    {
        //Instantiate count object of 1 to Object list count
        for (int i = 0; i < Random.Range(1, objectInstantiateToDestroy.Count); i++)
        {
            int RandomObject = Random.Range(0, objectInstantiateToDestroy.Count);
            var randomObjectInstantiate= Instantiate(objectInstantiateToDestroy[RandomObject], objectInstantiateToDestroy[RandomObject].transform.position = sphareInstantiareObject + Random.onUnitSphere * chaseDistance, Quaternion.identity);
            if (NetworkServer.active)
            {
                NetworkServer.Spawn(randomObjectInstantiate);
                PickUpMaterial pickUpMaterialObject = randomObjectInstantiate.GetComponent<PickUpMaterial>();
                if (pickUpMaterialObject!=null)
                {
                    pickUpMaterialObject.RandomCountObject();
                }
            }
        }
    }
    private void ChangeAnimation()
    {
        startPatricles.SetActive(false);
        box001.SetActive(false);
    }
    private void OnDrawGizmosSelected()
    {
        //select where in gizmos must have drop object
        sphareInstantiareObject = new Vector3(transform.position.x, transform.position.y + chaseDistanceY, transform.position.z);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(sphareInstantiareObject, chaseDistance);
    }
    #region Destoy Object Mechanism
    private void DestoryObjectChange()
    {
       var objectDestoyBoxPandora=Instantiate(destroy_box, transform.position, Quaternion.identity);
        if (NetworkServer.active) 
        {
            NetworkServer.Spawn(objectDestoyBoxPandora);
        }
    }
    private void DestroyObjectBoxPandora()
    {
        GetComponent<Rigidbody>().useGravity = false;
        Destroy(this.gameObject);
    }
    #endregion
    #endregion

}
