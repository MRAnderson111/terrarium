using UnityEngine;

/// <summary>
/// Three needs that get more urgent over time.
/// 0 means fine. 1 means desperate.
///
/// Both agents in this scene use this exact same script. The only thing that
/// differs between them is how they DECIDE what to do about it.
/// </summary>
public class Needs : MonoBehaviour
{
    [Range(0f, 1f)] public float power = 0.2f;
    [Range(0f, 1f)] public float heat  = 0.2f;
    [Range(0f, 1f)] public float data  = 0.2f;

    [Header("How fast each one climbs, per second")]
    public float powerRate = 0.10f;
    public float heatRate  = 0.13f;
    public float dataRate  = 0.07f;

    void Update()
    {
        power = Mathf.Clamp01(power + powerRate * Time.deltaTime);
        heat  = Mathf.Clamp01(heat  + heatRate  * Time.deltaTime);
        data  = Mathf.Clamp01(data  + dataRate  * Time.deltaTime);
    }

    /// Read one need by name.
    public float Get(Need n)
    {
        if (n == Need.Power) return power;
        if (n == Need.Heat)  return heat;
        return data;
    }

    /// Make one need less urgent.
    public void Relieve(Need n, float amount)
    {
        if (n == Need.Power) power = Mathf.Clamp01(power - amount);
        else if (n == Need.Heat) heat = Mathf.Clamp01(heat - amount);
        else data = Mathf.Clamp01(data - amount);
    }
}

public enum Need { Power, Heat, Data }
