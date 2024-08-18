using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.SceneManager
{
    public sealed class LobbyState : NetworkBehaviour
    {
        private string _statusText;
        private readonly Dictionary<ulong, bool> _clientStatuses = new();

        public override void OnNetworkSpawn()
        {
            _clientStatuses.Add(NetworkManager.Singleton.LocalClientId, false);

            if (IsServer)
            {
                NetworkManager.Singleton.OnConnectionEvent += ServerOnConnectionEvent;
                SceneTransition.Instance.OnClientLoadedScene += ClientOnLoadedScene;
            }

            GenerateStatusText();
        }

        private void ServerOnConnectionEvent(NetworkManager manager, ConnectionEventData data)
        {
            Debug.Log("server on connection event, event type: " + data.EventType);
            if (data.EventType != ConnectionEvent.ClientConnected)
                return;

            if (!_clientStatuses.ContainsKey(data.ClientId))
            {
                _clientStatuses.Add(data.ClientId, false);
            }
            OnStatusChanged();
        }

        private void ClientOnLoadedScene(ulong clientId)
        {
            Debug.Log("client on loaded scene, server: " + IsServer);
            if (!IsServer) return;

            if (!_clientStatuses.ContainsKey(clientId))
            {
                _clientStatuses.Add(clientId, false);
            }

            OnStatusChanged();
        }

        private void OnStatusChanged()
        {
            if (IsServer)
            {
                GenerateStatusText();
            }

            // notice to all clients
            foreach (var status in _clientStatuses)
            {
                ClientNoticeReadyStatusRpc(status.Key, status.Value);
            }

            if (IsAllPlayersReady())
            {
                SwitchGameplayScene();
            }
        }

        private bool IsAllPlayersReady()
        {
            // at least 2 players to start multiplay mode
            if (_clientStatuses.Count < 2) return false;

            foreach (var status in _clientStatuses)
            {
                // some clients are still loading into the lobby scene
                if (!status.Value) return false;
                if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(status.Key)) return false;
            }

            return true;
        }

        private void SwitchGameplayScene()
        {
            NetworkManager.Singleton.OnConnectionEvent -= ServerOnConnectionEvent;
            SceneTransition.Instance.OnClientLoadedScene -= ClientOnLoadedScene;
            SceneTransition.Instance.Switch(SceneTransition.SceneState.Gameplay);
        }

        private void GenerateStatusText()
        {
            _statusText = string.Empty;
            foreach (var kvp in _clientStatuses)
            {
                _statusText += "PLAYER_" + kvp.Key + "   ";
                _statusText += kvp.Value ? "(READY)" : "(NOT READY)";
                _statusText += "\n";
            }
        }

        // TODO menu creation
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.TextArea(_statusText);

            var localClientId = NetworkManager.Singleton.LocalClientId;
            if (!_clientStatuses.ContainsKey(localClientId) || !_clientStatuses[localClientId])
            {
                if (GUILayout.Button("Ready"))
                {
                    OnReadyButtonClicked();
                }
            }
            else if (NetworkManager.Singleton.IsHost)
            {
                if (GUILayout.Button("Play"))
                {
                    SwitchGameplayScene();

                }
            }

            GUILayout.EndArea();
        }

        private void OnReadyButtonClicked()
        {
            _clientStatuses[NetworkManager.Singleton.LocalClientId] = true;
            if (IsServer) // clicked as host
            {
                OnStatusChanged();
            }
            else
            {
                ServerOnClientReadyRpc(NetworkManager.Singleton.LocalClientId);
            }
        }

        [Rpc(SendTo.Server)]
        private void ServerOnClientReadyRpc(ulong clientId)
        {
            if (_clientStatuses.ContainsKey(clientId))
            {
                _clientStatuses[clientId] = true;
                OnStatusChanged();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientNoticeReadyStatusRpc(ulong clientId, bool ready)
        {
            if (IsServer) return;
            _clientStatuses[clientId] = ready;
            GenerateStatusText();
        }
    }
}