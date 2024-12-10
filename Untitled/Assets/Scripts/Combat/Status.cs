using Unity.Netcode;
using System;

namespace Apps.Runtime.Combat
{
    public sealed class Status : NetworkBehaviour
    {
        public NetworkVariable<uint> Health { get; private set; } = new();
        public bool IsDead => Health.Value == 0;

        public void Initialize(uint hp)
        {
            Health.Value = hp;
        }

        public void Subtract(uint hp)
        {
            Health.Value = (uint)Math.Max((int)Health.Value - hp, 0);
        }
    }
}
