using System.Collections;
using UnityEngine;

public class SimpleReleasePoint : MonoBehaviour, IReleasePoint
{
    public GameObject pointPrefab;

    public void ReleasePoint()
    {
        Instantiate(pointPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
    }
}
