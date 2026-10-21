using UnityEngine;

/// <summary>
/// Draws what each agent is thinking, on screen, while it runs.
///
/// Worth saying out loud: in the rover exercise you were told to hide all of
/// this. Here we show everything, because the goal is different. There you
/// were designing a surface for a player. Here you are reading a machine.
/// </summary>
public class Labels : MonoBehaviour
{
    //public BehaviourTreeAgent treeAgent;
    public UtilityAgent utilityAgent;

    GUIStyle title, body;

    void Build()
    {
        title = new GUIStyle(GUI.skin.label);
        title.fontSize = 20; title.fontStyle = FontStyle.Bold;
        title.normal.textColor = Color.white;

        body = new GUIStyle(GUI.skin.label);
        body.fontSize = 14;
        body.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
    }

    void OnGUI()
    {
        if (title == null) Build();
        float half = Screen.width * 0.5f;

/*
        if (treeAgent != null)
            Panel(new Rect(24, 20, half - 48, 200), "BEHAVIOUR TREE",
                  treeAgent.GetComponent<Needs>(),
                  treeAgent.GetComponent<Mover>().reportedAction,
                  treeAgent.reason,
                  "threshold " + treeAgent.threshold.ToString("0.00"));
*/

        if (utilityAgent != null)
            Panel(new Rect(half + 24, 20, half - 48, 200), "UTILITY",
                  utilityAgent.GetComponent<Needs>(),
                  utilityAgent.GetComponent<Mover>().reportedAction,
                  utilityAgent.reason,
                  "no threshold anywhere");
    }

    void Panel(Rect r, string heading, Needs n, string action, string why, string note)
    {
        GUILayout.BeginArea(r);
        GUILayout.Label(heading, title);
        GUILayout.Label("power  " + Bar(n.power), body);
        GUILayout.Label("heat   " + Bar(n.heat), body);
        GUILayout.Label("data   " + Bar(n.data), body);
        GUILayout.Space(6);
        GUILayout.Label("doing:  " + action, body);
        GUILayout.Label("because: " + why, body);
        GUILayout.Label(note, body);
        GUILayout.EndArea();
    }

    string Bar(float v)
    {
        int filled = Mathf.RoundToInt(v * 20f);
        return new string('=', filled) + new string('.', 20 - filled) + "  " + v.ToString("0.00");
    }
}
