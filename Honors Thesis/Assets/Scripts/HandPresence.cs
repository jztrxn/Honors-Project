using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandPresence : MonoBehaviour
{

    public GameObject leftObj;
    public GameObject rightObj;

    public float stepSize;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("started");
        Debug.Log(Input.GetJoystickNames());
    }

    // Update is called once per frame
    void Update()
    {
        
        // Left hand, left JS
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickLeft))
        {
            leftObj.transform.Translate(-stepSize, 0f, 0f);
        }

        // Left hand, right JS
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickRight))
        {
            leftObj.transform.Translate(stepSize, 0f, 0f);
        }

        // Right hand, left JS
        if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickLeft))
        {
            rightObj.transform.Translate(-stepSize, 0f, 0f);
        }

        // Right hand, right JS
        if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickRight))
        {
            rightObj.transform.Translate(stepSize, 0f, 0f);
        }
    }
}
