using System.Collections.Generic;
using Apps.Runtime.Data;
using Apps.Runtime.Movement;
using Apps.Runtime.Presentation;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.SceneManager
{
    public sealed class GameplayState : NetworkBehaviour
    {
        private string _statusText;

        // key: id, value: HP
        private readonly Dictionary<ulong, uint> _clientStats = new();
        [SerializeField] List<Transform> _linePositions;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnConnectionEvent += ServerOnConnectionEvent;
                SceneTransition.Instance.OnClientLoadedScene += ServerOnLoadedScene;
            }
        }

        private void ServerOnConnectionEvent(NetworkManager manager, ConnectionEventData data)
        {
            if (data.EventType != ConnectionEvent.ClientConnected) return;

            if (!_clientStats.ContainsKey(data.ClientId))
            {
                _clientStats.Add(data.ClientId, 0u);
            }

            OnStatusChanged();
        }

        private void ServerOnLoadedScene(ulong clientId)
        {
            if (!_clientStats.ContainsKey(clientId))
            {
                _clientStats.Add(clientId, 0u);
            }
            var connected = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);

            // status initialization
            var status = connected.GetComponent<Status>();
            status.Initialize(CreateUserName(clientId), 1000); // TODO configurable
            _clientStats[clientId] = status.HP.Value;

            // listen to hp changed
            status.HP.OnValueChanged += OnHPChanged;

            // place the connected player into appearance line.
            var transform = _linePositions[_clientStats.Count - 1];
            connected.GetComponent<ServerMover>().Teleport(transform.position, transform.rotation);
            connected.GetComponent<CharacterPresenter>().Initialize(Camera.main);
            //ClientOnInitializedRpc(clientId);
            OnStatusChanged();
        }

        private void OnHPChanged(uint _, uint __)
        {
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_clientStats.ContainsKey(id)) continue;
                // TODO this's expensive, but OK for a temporary test
                _clientStats[id] = NetworkManager.Singleton.SpawnManager
                    .GetPlayerNetworkObject(id)
                    .GetComponent<Status>().HP.Value;
            }
            OnStatusChanged();
        }

        private void OnStatusChanged()
        {
            if (IsServer)
            {
                GenerateStatsText();
            }

            foreach (var pair in _clientStats)
            {
                ClientOnStatusChangedRpc(pair.Key, pair.Value);
            }
        }


        private void GenerateStatsText()
        {
            _statusText = string.Empty;
            foreach (var kvp in _clientStats)
            {
                _statusText += CreateUserName(kvp.Key) + "   ";
                _statusText += "HP:" + kvp.Value;
                _statusText += "\n";
            }
        }

        private string CreateUserName(ulong id)
        {
            return "PLAYER_" + id;
        }

        // TODO menu creation
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.TextArea(_statusText);
            GUILayout.EndArea();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientOnStatusChangedRpc(ulong clientId, uint hp)
        {
            if (IsServer) return;
            _clientStats[clientId] = hp;
            GenerateStatsText();
        }

        // TODO where should this is called?
        public void Dispose()
        {
            NetworkManager.Singleton.OnConnectionEvent -= ServerOnConnectionEvent;
            SceneTransition.Instance.OnClientLoadedScene -= ServerOnLoadedScene;
        }
    }
}