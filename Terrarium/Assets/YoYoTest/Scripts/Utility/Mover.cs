using UnityEngine;

/// <summary>
/// Drives to a station and uses it. Both agents share this, so movement is
/// never the thing that differs between them.
///
/// Call GoToAndUse every frame with the station you want. It reports back
/// Running while it is still busy, and Success on the frame it finishes.
/// </summary>
public class Mover : MonoBehaviour
{
    public float speed = 3f;
    public float arriveDistance = 0.6f;
    public float useSeconds = 1.2f;

    [HideInInspector] public string reportedAction = "thinking";

    Station current;
    float useTimer;
    Needs needs;

    void Awake() { needs = GetComponent<Needs>(); }

    public Status GoToAndUse(Station station)
    {
        if (station == null) return Status.Failure;

        // Switched target mid-job, so start again.
        if (current != station) { current = station; useTimer = 0f; }

        Vector3 to = station.transform.position - transform.position;
        to.z = 0f;

        if (to.magnitude > arriveDistance)
        {
            reportedAction = "driving to " + station.label;
            transform.position += to.normalized * speed * Time.deltaTime;
            return Status.Running;
        }

        reportedAction = "using " + station.label;
        useTimer += Time.deltaTime;
        if (useTimer < useSeconds) return Status.Running;

        needs.Relieve(station.fixes, station.relief);
        current = null;
        useTimer = 0f;
        return Status.Success;
    }

    public Status Idle()
    {
        reportedAction = "nothing to do";
        return Status.Running;
    }
}
