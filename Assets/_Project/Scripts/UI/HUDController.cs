using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Project.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Texto de objetivo (usa uno de los dos)")]
        public TMP_Text objectiveTMP;     // si usas TextMeshPro
        public Text objectiveUGUI;        // si usas el Text clásico

        /// <summary>Actualiza el texto de objetivo en el HUD.</summary>
        public void SetObjective(string text)
        {
            if (objectiveTMP) objectiveTMP.text = text;
            if (objectiveUGUI) objectiveUGUI.text = text;
        }
    }
}
