using Unity.Netcode;
using System;

namespace Apps.Runtime.Data
{
    public sealed class Status : NetworkBehaviour
    {
        public NetworkVariable<uint> HP { get; private set; } = new();
        public bool IsDead => HP.Value == 0;

        public string UserName => _userName;
        private string _userName;

        public uint MaxHP => _maxHp;
        private uint _maxHp;

        public void Initialize(string userName, uint maxHp)
        {
            _userName = userName;
            _maxHp = maxHp;

            if (IsServer)
            {
                HP.Value = maxHp;
            }
        }

        public void Subtract(uint hp)
        {
            HP.Value = (uint)Math.Max((int)HP.Value - hp, 0);
        }
    }
}
