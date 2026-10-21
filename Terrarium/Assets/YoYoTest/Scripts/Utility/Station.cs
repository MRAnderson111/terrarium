using UnityEngine;

/// <summary>
/// A place in the world that fixes one need.
///
/// It also advertises how good it is at that, which only the utility agent
/// pays any attention to. The behaviour tree agent ignores this number
/// completely, and that difference is most of the point of this project.
/// </summary>
public class Station : MonoBehaviour
{
    public string label = "Station";
    public Need fixes = Need.Power;

    [Tooltip("How loudly it advertises. Only the utility agent reads this.")]
    [Range(0f, 2f)] public float advertises = 1f;

    [Tooltip("How much of the need it clears when used.")]
    [Range(0f, 1f)] public float relief = 0.7f;

    void Start()
    {
        // Colour-code so you can tell them apart without reading labels.
        Color c = fixes == Need.Power ? new Color(1f, 0.82f, 0.25f)
                : fixes == Need.Heat  ? new Color(0.95f, 0.35f, 0.3f)
                                      : new Color(0.35f, 0.7f, 1f);
        GetComponent<Renderer>().material.color = c;
    }
}
