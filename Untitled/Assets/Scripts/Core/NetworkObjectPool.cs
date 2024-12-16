using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Apps.Runtime.Core
{
    /// <summary>
    /// Object Pool for networked objects, used for controlling how objects are spawned by Netcode. Netcode by default
    /// will allocate new memory when spawning new objects. With this Networked Pool, we're using the ObjectPool to reuse objects.
    /// </summary>
    public class NetworkObjectPool : NetworkBehaviour
    {
        // TODO DI
        public static NetworkObjectPool Singleton { get; private set; }

        [SerializeField] List<PoolConfigObject> _pooledPrefabsList;

        readonly HashSet<GameObject> _prefabs = new();
        readonly Dictionary<GameObject, ObjectPool<NetworkObject>> _pooledObjects = new();

        public void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Singleton = this;
            }
        }

        public override void OnNetworkSpawn()
        {
            // registers all objects in PooledPrefabsList to the cache.
            foreach (var configObject in _pooledPrefabsList)
            {
                RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
            }
        }

        public override void OnNetworkDespawn()
        {
            // Unregisters all objects in PooledPrefabsList from the cache.
            foreach (var prefab in _prefabs)
            {
                // Unregister Netcode Spawn handlers
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                _pooledObjects[prefab].Clear();
            }
            _pooledObjects.Clear();
            _prefabs.Clear();
        }

        public void OnValidate()
        {
            for (var i = 0; i < _pooledPrefabsList.Count; i++)
            {
                var prefab = _pooledPrefabsList[i].Prefab;
                if (prefab != null)
                {
                    Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
                }
            }
        }

        /// <summary>
        /// Gets an instance of the given prefab from the pool. The prefab must be registered to the pool.
        /// </summary>
        /// <remarks>
        /// To spawn a NetworkObject from one of the pools, this must be called on the server, then the instance
        /// returned from it must be spawned on the server. This method will then also be called on the client by the
        /// PooledPrefabInstanceHandler when the client receives a spawn message for a prefab that has been registered
        /// here.
        /// </remarks>
        /// <param name="prefab"></param>
        /// <param name="position">The position to spawn the object at.</param>
        /// <param name="rotation">The rotation to spawn the object with.</param>
        /// <returns></returns>
        public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var networkObject = _pooledObjects[prefab].Get();
            networkObject.transform.SetPositionAndRotation(position, rotation);
            return networkObject;
        }

        /// <summary>
        /// Return an object to the pool (reset objects before returning).
        /// </summary>
        public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
        {
            _pooledObjects[prefab].Release(networkObject);
        }

        /// <summary>
        /// builds up the cache for a prefab.
        /// </summary>
        void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
        {
            _prefabs.Add(prefab);

            // create the pool
            _pooledObjects[prefab] = new ObjectPool<NetworkObject>(
                createFunc: () => Instantiate(prefab).GetComponent<NetworkObject>(),
                actionOnGet: (p) => p.gameObject.SetActive(true),
                actionOnRelease: (p) => p.gameObject.SetActive(false),
                actionOnDestroy: (p) => Destroy(p.gameObject),
                defaultCapacity: prewarmCount) ;

            // populate the pool
            var prewarmNetworkObjects = new List<NetworkObject>();
            for (var i = 0; i < prewarmCount; i++)
            {
                prewarmNetworkObjects.Add(_pooledObjects[prefab].Get());
            }
            foreach (var networkObject in prewarmNetworkObjects)
            {
                _pooledObjects[prefab].Release(networkObject);
            }

            // register Netcode Spawn handlers
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
        }
    }

    [Serializable]
    struct PoolConfigObject
    {
        public GameObject Prefab;
        public int PrewarmCount;
    }

    sealed class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        readonly GameObject _prefab;
        readonly NetworkObjectPool _pool;

        public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
        {
            _prefab = prefab;
            _pool = pool;
        }

        NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return _pool.GetNetworkObject(_prefab, position, rotation);
        }

        void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject)
        {
            _pool.ReturnNetworkObject(networkObject, _prefab);
        }
    }
}