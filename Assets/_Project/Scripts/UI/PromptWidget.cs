using UnityEngine;
using UnityEngine.UI;   // UI Text legacy
using TMPro;            // TMP

namespace Project.UI
{
    public class PromptWidget : MonoBehaviour
    {
        public CanvasGroup group;
        [Header("Asigna uno u otro")]
        public Text legacyText;        // UI → Text (legacy)
        public TMP_Text tmpText;       // TextMeshProUGUI

        void Awake()
        {
            if (!group) group = GetComponent<CanvasGroup>();
            Hide(); // iniciar oculto
        }

        public void Show(string msg)
        {
            if (tmpText) tmpText.text = msg;
            if (legacyText) legacyText.text = msg;
            if (group)
            {
                group.alpha = 1f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        public void Hide()
        {
            if (group) group.alpha = 0f;
        }
    }
}
