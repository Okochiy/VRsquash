using UnityEngine;
using UnityEngine.UI;

public class MyDebugUI : MonoBehaviour
{
    bool inMenu;

    void Start()
    {
        DebugUIBuilder.instance.AddLabel("Debug Start", DebugUIBuilder.DEBUG_PANE_CENTER);
        DebugUIBuilder.instance.AddLabel("Debug Log", DebugUIBuilder.DEBUG_PANE_LEFT);
        DebugUIBuilder.instance.Show();
        inMenu = true;
    }

    void Update()
    {
        // Bボタンでデバッグディスプレイの表示・非表示
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }

        // Aボタンでデバッグログをクリア
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            DebugUIBuilder.instance.Clear();
            DebugUIBuilder.instance.AddLabel("Clear");
            DebugUIBuilder.instance.AddDivider();
        }
    }
}