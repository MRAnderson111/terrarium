using UnityEngine;

/// <summary>
/// AGENT TWO. Decides by scoring.
///
/// Instead of a list of questions, it gives every station a score and goes to
/// the best one. This is how The Sims works, shrunk down as far as it goes.
///
/// Notice what it gains. There is no threshold to invent, it weighs everything
/// against everything, and adding a new consideration means adding a line to
/// the sum rather than restructuring anything.
///
/// Notice what it loses. You cannot look at this and say what it will do next.
/// You have to do the arithmetic.
/// </summary>
[RequireComponent(typeof(Needs))]
[RequireComponent(typeof(Mover))]
public class UtilityAgent : MonoBehaviour
{
    [Tooltip("How much being far away puts it off. Set to 0 to ignore distance.")]
    public float distanceFactor = 0.15f;

    public Station[] stations;

    Needs needs;
    Mover mover;
    Station chosen;

    // The two stations finished most recently. Index 0 is the newer one.
    // Scoring skips both of them, so they cannot win however high they score.
    Station[] lastUsed = new Station[2];

    [HideInInspector] public string reason = "";

    void Start()
    {
        needs = GetComponent<Needs>();
        mover = GetComponent<Mover>();
    }

    void Update()
    {
        // Pick again whenever we are not already busy with something.
        if (chosen == null) chosen = Score();//没有目标时看score，不会出现等待的情况

        // Every station is on the recently used list, so there is nothing left
        // to score. Wait until a trip finishes and frees one up.
        if (chosen == null) { mover.Idle(); return; }

        if (mover.GoToAndUse(chosen) == Status.Success)
        {
            // Recorded only now, when the visit is over. Any earlier and the
            // next frame would refuse the station we are still driving to.
            Remember(chosen);
            chosen = null; // 永远要重新选 
        }
    }

    void Remember(Station s)
    {
        lastUsed[1] = lastUsed[0];
        lastUsed[0] = s;
    }

    Station Score()
    {
        Station best = null;
        float bestScore = -1f;

        foreach (Station s in stations)
        {
            if (s == null) continue;

            // Out of the running if we just came from there.
            if (s == lastUsed[0] || s == lastUsed[1]) continue; //被跳过的站连分数都不算，不管它需求多急

            // How much I want it, times how good it says it is.
            float score = needs.Get(s.fixes) * s.advertises; ///????? what is advertises?

            // Then knock it down for being far away.
            float distance = Vector3.Distance(transform.position, s.transform.position);
            score /= 1f + distance * distanceFactor; //分数除于距离 距离越远，分数越低

            if (score > bestScore) { bestScore = score; best = s; } //如果有跟着急的需求，就选它
        }

        if (best != null) reason = "best score was " + best.label + " at " + bestScore.ToString("0.00");
        else reason = "the only stations left were just used";
        return best;
    }
}
