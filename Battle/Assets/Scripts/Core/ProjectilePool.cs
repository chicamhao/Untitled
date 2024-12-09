using Apps.Runtime.Combat;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Core
{
    // TODO template class
    public sealed class ProjectilePool 
    {
        readonly List<Projectile> _projectilePool = new();

        public Projectile Spawn(Projectile prefab)
        {
            Projectile projectile;
            if (_projectilePool.Any())
            {
                projectile = _projectilePool.First();
                _projectilePool.RemoveAt(0);

            }
            else
            {
                projectile = Object.Instantiate(prefab);
                projectile.GetComponent<NetworkObject>().Spawn();
            }
            return projectile;
        }

        public void Release(Projectile projectile)
        {
            _projectilePool.Add(projectile);
        }
    }
}
