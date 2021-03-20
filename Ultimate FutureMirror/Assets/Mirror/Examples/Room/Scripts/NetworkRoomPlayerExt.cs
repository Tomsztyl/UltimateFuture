using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkRoomPlayerExt));

        public override void OnStartClient()
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnStartClient {0}", SceneManager.GetActiveScene().path);

            base.OnStartClient();
        }

        public override void OnClientEnterRoom()
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);
            GetComponent<SelectCharacterManager>().enabled = isLocalPlayer;
  
        }

        public override void OnClientExitRoom()
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
        }

        public override void ReadyStateChanged(bool _, bool newReadyState)
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "ReadyStateChanged {0}", newReadyState);
        }

        public bool IsFindObjectIndexInstantiate(GameObject objectCharacter)
        {
            foreach(GameObject objectFind in ((NetworkRoomManagerExt)NetworkManager.singleton).characterSpawnList)
            {
                if (objectFind == objectCharacter)
                {
                    return true;
                }
            }
            return false;
        }
        private int FindObjectIndexInstantiate(GameObject objectCharacter)
        {          
           return ((NetworkRoomManagerExt)NetworkManager.singleton).characterSpawnList.IndexOf(objectCharacter);
        }
        public void ExeciuteChangePlayerInstatiate(GameObject objectCharacter)
        {
            if (IsFindObjectIndexInstantiate(objectCharacter))
            {
                SelectCharacterVanguardServer(FindObjectIndexInstantiate(objectCharacter));
            }
            else
            {
                Debug.LogWarning("Add Object To Instantite");
            }
        }

        [Command]
        public void SelectCharacterVanguardServer(int characterSpawnIndex)
        {
            GetComponent<NetworkRoomPlayer>().choosePlayerRoom = ((NetworkRoomManagerExt)NetworkManager.singleton).characterSpawnList[characterSpawnIndex];
            SelectCharacterVanguardClient(characterSpawnIndex);
        }
        [ClientRpc]
        public void SelectCharacterVanguardClient(int characterSpawnIndex)
        {
            GetComponent<NetworkRoomPlayer>().choosePlayerRoom = ((NetworkRoomManagerExt)NetworkManager.singleton).characterSpawnList[characterSpawnIndex];
        }
    }
}
