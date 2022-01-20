using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGenerator : MonoBehaviour
{

    public GameObject leftObj;
    public GameObject rightObj;
    public GameObject centerObj;

    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        leftObj.transform.position = new Vector3(-1.0f, 0f, 4f);
        rightObj.transform.position = new Vector3(1.0f, 0f, 4f);
        centerObj.transform.position = new Vector3(1.0f, 0f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            float currDist = leftObj.transform.position.z;
            leftObj.transform.position = new Vector3(-1.0f, 0f, currDist + distance);
            rightObj.transform.position = new Vector3(1.0f, 0f, currDist + distance);
            centerObj.transform.position = new Vector3(0f, 0f, currDist + distance);
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            float currDist = leftObj.transform.position.z;
            leftObj.transform.position = new Vector3(-1.0f, 0f, currDist - distance);
            rightObj.transform.position = new Vector3(1.0f, 0f, currDist - distance);
            centerObj.transform.position = new Vector3(0f, 0f, currDist - distance);
        }

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            float currDist = leftObj.transform.position.z;
            leftObj.transform.position = new Vector3(-1.0f, 0f, currDist);
            rightObj.transform.position = new Vector3(1.0f, 0f, currDist);
            centerObj.transform.position = new Vector3(0f, 0f, currDist);
        }

        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            leftObj.transform.position = new Vector3(-1.0f, 0f, 4f);
            rightObj.transform.position = new Vector3(1.0f, 0f, 4f);
            centerObj.transform.position = new Vector3(0f, 0f, 4f);
        }
    }
}
