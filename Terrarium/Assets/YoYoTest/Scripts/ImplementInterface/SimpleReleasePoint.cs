using System.Collections;
using UnityEngine;

public class SimpleReleasePoint : MonoBehaviour, IReleaseResearchPoint
{
    public GameObject pointPrefab;

    public void ReleaseResearchPoint()
    {
        Instantiate(pointPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
    }
}
