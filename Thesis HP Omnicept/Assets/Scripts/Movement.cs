using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject noniusLine;
    public float acceleration;
    public float waitSeconds;

    private float speed;
    private float maxSpeed = 0.02f;
    private float nextTrialTimer = 0.0f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftShift))
        {
            speed = speed - acceleration * Time.deltaTime;
            if(Mathf.Abs(speed) > maxSpeed){
                speed = -maxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightShift))
        {
            speed = speed + acceleration * Time.deltaTime;
            if (Mathf.Abs(speed) > maxSpeed)
            {
                speed = maxSpeed;
            }
        }
        else
        {
            speed = 0;
        }
        noniusLine.transform.Translate(speed, 0f, 0f);
        /*
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextTrialTimer)
        {
            nextTrialTimer = Time.time + waitSeconds;
            //eyeTracking.getEyeTracking();
            this.GetComponent<GliaEyeTracking>().getEyeTracking();
            Debug.Log("Space Pressed");
        }*/
    }
}
