using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apps.Runtime.UI
{
    public sealed class CharacterUI : MonoBehaviour
    {
        public Image HPBarImage;
        public TMP_Text UserNameText;
        public Canvas Canvas;


        public void Update()
        {
            if (Canvas.worldCamera)
            {
                transform.forward = - Canvas.worldCamera.transform.forward;
            }
        }
    }
}
