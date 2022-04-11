using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparometerObjHandler : MonoBehaviour
{
    public GameObject centerLine;
    public GameObject noniusLine;
    public GameObject fusionLock;
    public GameObject plane;
    public float lineDistance;
    public float scaleFactor;
    public float stepSize;

    public void SetObjects(float distance, float noniusStart)
    {
        //fusionLock.transform.localScale = new Vector3(scaleFactor * distance, scaleFactor * distance, 1f);
        plane.transform.localPosition = new Vector3(0f, 0.5f, distance);
        centerLine.transform.localPosition = new Vector3(0f, 0.25f, distance);
        noniusLine.transform.localPosition = new Vector3(noniusStart, 0f, distance);
        fusionLock.transform.localPosition = new Vector3(0f, 0f, distance);
        //Debug.LogFormat("fusionLock scale: {0}", fusionLock.transform.localScale);
        //Debug.LogFormat("fusionLock position: {0}", fusionLock.transform.position);
    }
}
