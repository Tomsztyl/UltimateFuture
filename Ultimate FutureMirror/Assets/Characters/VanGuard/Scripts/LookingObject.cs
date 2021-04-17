using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingObject : MonoBehaviour
{
    [SerializeField] private GameObject[] lookinObjectWithTag;

    private void Start()
    {
        
    }

    private void Update()
    {
        PlayerMirrorController playerMirrorController = GetComponent<PlayerMirrorController>();
        if (playerMirrorController!=null)
        {
            if (playerMirrorController.isLocalPlayer)
            {
                FindObjectLookingTag();
                LookAtCamera();
            }
        }
      
    }

    private void FindObjectLookingTag()
    {
        lookinObjectWithTag = GameObject.FindGameObjectsWithTag("LookToCamera");       
    }
    private void LookAtCamera()
    {

        if (lookinObjectWithTag.Length>0)
        {
            CameraControllerPlayer cameraControllerPlayer = GetComponent<CameraControllerPlayer>();

            if (cameraControllerPlayer != null)
            {
                foreach(GameObject lookinObject in lookinObjectWithTag)
                {
                    lookinObject.transform.LookAt(cameraControllerPlayer.GetChooseCamera().transform);
                }
                
            }
        }
       
    }
    
}
