using Apps.Runtime.Data;
using Apps.Runtime.UI;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Presentation
{
    public sealed class CharacterPresenter : NetworkBehaviour
    {
        [SerializeField] CharacterUI _UI;
        Status _status;

        public override void OnNetworkSpawn()
        {
            _status = GetComponent<Status>();

            if (!NetworkObject.IsPlayerObject)
            {
                OnClientChanged(_status.HP.Value, _status.HP.Value);
                _status.HP.OnValueChanged += OnClientChanged;
                _UI.Canvas.worldCamera = Camera.main;
            }
        }

        public void Initialize(Camera camera)
        {
            OnClientChanged(_status.HP.Value, _status.HP.Value);
            _status.HP.OnValueChanged += OnClientChanged;
            _UI.Canvas.worldCamera = camera;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient && _status)
            {
                _status.HP.OnValueChanged -= OnClientChanged;
            }
        }

        private void OnClientChanged(uint previousHealth, uint newHealth)
        {
            _UI.gameObject.SetActive(!_status.IsDead);

            if (!_status.IsDead)
            {
                if (_status.MaxHP != 0)
                {
                    float percent = (float)newHealth / _status.MaxHP;
                    _UI.HPBarImage.color = new Color(1 - percent, percent, 0);
                    _UI.HPBarImage.rectTransform.localScale = new Vector3(percent, 1);
                }

                if (!string.IsNullOrEmpty(_status.UserName))
                {
                    _UI.UserNameText.SetText(_status.UserName);
                }
            }
        }
    }
}