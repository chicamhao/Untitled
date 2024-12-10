using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Apps.Runtime.SceneManager
{
    public sealed class MenuState : MonoBehaviour
    {
        // TODO input field
        [SerializeField] string _ip = "127.0.0.1​";
        [SerializeField] ushort _port = 7777;

        private void StartGame()
        {
            ConnectUnityTranport();
            NetworkManager.Singleton.StartHost();
            SceneTransition.Instance.RegisterCallbacks();
            SceneTransition.Instance.Switch(SceneTransition.SceneState.Lobby);
        }

        private void JoinGame()
        {
            ConnectUnityTranport();
            NetworkManager.Singleton.StartClient();
            // client scene transition notification is done by callback event
        }

        private void ConnectUnityTranport()
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            transport.SetConnectionData(Sanitize(_ip), _port);
        }

        // TODO menu creation
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("Start as Host"))
            {
                StartGame();
            }

            if (GUILayout.Button("Start as Client"))
            {
                JoinGame();
            }        
            GUILayout.EndArea();
        }

        static string Sanitize(string dirty)
        {
            return Regex.Replace(dirty, "[^A-Za-z0-9.]", "");
        }
    }
}