// Assets/_Project/Scripts/Debug/DevHotkeys.cs
using UnityEngine;

public class DevHotkeys : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            var hud =
#if UNITY_2023_1_OR_NEWER
                FindFirstObjectByType<Project.UI.InventoryHUD>();
#else
                FindObjectOfType<Project.UI.InventoryHUD>();
#endif
            if (hud) hud.AddItem("Key_Ritual", 1, "Llave del ritual", null);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            var mc =
#if UNITY_2023_1_OR_NEWER
                FindFirstObjectByType<Project.Missions.MissionController>();
#else
                FindObjectOfType<Project.Missions.MissionController>();
#endif
            mc?.SetStepById("ve_al_waypoint"); // enciende waypoint sin la llave
        }
    }
}
