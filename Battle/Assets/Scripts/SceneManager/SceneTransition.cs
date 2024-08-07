using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Apps.Runtime.SceneManager
{
    public sealed class SceneTransition : MonoBehaviour
    {
        public enum SceneState
        {
            Boot,
            Menu,
            Lobby,
            Gameplay
        }

        // TODO DI
        public static SceneTransition Instance { get; private set; }

        [SerializeField] NetworkManager _networkManager;

        public delegate void SceneChangedDelegate(SceneState state);
        public event SceneChangedDelegate OnSceneChanged;

        public delegate void ClientLoadedSceneDelegate(ulong clientId);
        public event ClientLoadedSceneDelegate OnClientLoadedScene;

        public SceneState State => _state;
        private SceneState _state = SceneState.Menu;

        private byte _loadCompletedCount;
        public bool AllLoadCompleted => _loadCompletedCount == NetworkManager.Singleton.ConnectedClients.Count;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            UnityEngine.SceneManagement.SceneManager.LoadScene(ToSceneName(_state));
        }

        public void Switch(SceneState state)
        {
            _state = state;
            if (NetworkManager.Singleton.IsListening)
            {
                _loadCompletedCount = 0;
                NetworkManager.Singleton.SceneManager.LoadScene(ToSceneName(state), LoadSceneMode.Single);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(ToSceneName(state));
            }
        }

        private static string ToSceneName(SceneState state) => state.ToString().ToLower();

        internal void RegisterCallbacks()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        }

        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            _loadCompletedCount++;
            OnClientLoadedScene?.Invoke(clientId);
            Debug.Log("load completed" + _loadCompletedCount);
        }
    }
}