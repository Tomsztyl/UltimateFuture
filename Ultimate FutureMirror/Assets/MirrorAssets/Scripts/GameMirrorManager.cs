using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMirrorManager : NetworkBehaviour
{
    [SerializeField] private Camera cameraServer = null;
    private void Start()
    {
        if (cameraServer == null)
        {
            cameraServer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

           
        if (cameraServer!=null)
            TriggerCameraServer();
    }
    private void TriggerCameraServer()
    {
        cameraServer.GetComponent<AudioListener>().enabled = isServerOnly;
        cameraServer.enabled= isServerOnly;
    }
}
