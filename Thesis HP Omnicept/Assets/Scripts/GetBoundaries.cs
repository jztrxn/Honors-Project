using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;

public class GetBoundaries : MonoBehaviour
{
    private GliaBehaviour _gliaBehaviour = null;
    private GliaBehaviour gliaBehaviour
    {
        get
        {
            if (_gliaBehaviour == null)
            {
                _gliaBehaviour = FindObjectOfType<GliaBehaviour>();
            }

            return _gliaBehaviour;
        }
    }
    public void Start()
    {
        boundsFound = false;
        centerLineMoveable = true;
        leftBoundFound = false;
        rightBoundFound = false;
        setTarget();
        StartCoroutine(GetBounds());
    }

    private bool boundsFound = false;
    public GameObject centerLine;
    private float[] leftEyeBound;
    private float[] rightEyeBound;
    private bool leftBoundFound;
    private bool rightBoundFound;
    private Vector3 leftBoundLocation;
    private Vector3 rightBoundLocation;
    //float[] = [rightX, rightY, leftX, leftY]
    private bool centerLineMoveable;
    IEnumerator GetBounds()
    {
        Debug.Log("getting bound left");
        findLeftBound();
        while (leftBoundFound == false) { yield return null; }
        Debug.Log("getting bound right");
        setTarget();
        findRightBound();
        while (rightBoundFound == false) { yield return null; }
        boundsFound = true;
    }

    private void findLeftBound()
    {
        Debug.Log("Finding Left Bound");

    }
    private void findRightBound()
    {
        Debug.Log("Finding Right Bound");
        

    }

    private float totalTime;
    private float speed;
    private float maxSpeed = 0.01f;
    public float acceleration;

    public float waitSeconds;
    private float nextTrialTimer = 0.0f;
    private void Update()
    {
        if (Input.GetKey(KeyCode.A) && (Mathf.Abs(speed) < maxSpeed))
        {
            speed = speed - acceleration * Time.deltaTime;
            //Debug.LogFormat("speed is {0}", speed);
        }
        else if (Input.GetKey(KeyCode.D) && (Mathf.Abs(speed) < maxSpeed))
        {
            speed = speed + acceleration * Time.deltaTime;
            //Debug.LogFormat("speed is {0}", speed);
        }
        else
        {
            speed = 0;
        }
        centerLine.transform.Translate(speed, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.Space) && leftBoundFound == false && boundsFound == false)
        {
            Debug.Log("Space key pressed");
            leftBoundLocation = centerLine.transform.position;
            Debug.LogFormat("leftBound Location is: {0}", leftBoundLocation);
            leftEyeBound = getEyeTracking();
            Debug.LogFormat("leftBound is: {0}", leftEyeBound);
            leftBoundFound = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && rightBoundFound == false && boundsFound == false && leftBoundFound == true)
        {
            Debug.Log("Space key pressed");
            rightBoundLocation = centerLine.transform.position;
            Debug.LogFormat("rightBound Location is: {0}", rightBoundLocation);
            rightEyeBound = getEyeTracking();
            Debug.LogFormat("rightBound is: {0}", rightEyeBound);
            rightBoundFound = true;
        }
        /*
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextTrialTimer && boundsFound)
        {
            getEyeTracking();
            if (boundsFound)
            {
                nextTrialTimer = Time.time + waitSeconds;
            }
        }*/
    }

    private Vector2 rightGazeTarget;
    private Vector2 leftGazeTarget;
    private Vector2 rightPupilTarget;
    private Vector2 leftPupilTarget;
    public bool showEyeTrackingMessages = true;
    public void EyeTrackingHandler(EyeTracking eyeTracking)
    {
        if (showEyeTrackingMessages && eyeTracking != null)
        {
            Debug.Log(eyeTracking);
        }
    }

    public float[] getEyeTracking()
    {
        var eyeTracking = gliaBehaviour.GetLastEyeTracking();
        Debug.Log(eyeTracking);


        rightGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y);
        leftGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y);

        float rPupilX = eyeTracking.RightEye.PupilPosition.X;
        float rPupilY = eyeTracking.RightEye.PupilPosition.Y;
        float lPupilX = eyeTracking.LeftEye.PupilPosition.X;
        float lPupilY = eyeTracking.LeftEye.PupilPosition.Y;
        //Debug.Log(rPupilX);
        //Debug.Log(rPupilY);
        rightPupilTarget = new Vector2(rPupilX, rPupilY);
        leftPupilTarget = new Vector2(lPupilX, lPupilY);
        //Debug.LogFormat("RightGazeTarget is: {0}", rightGazeTarget);
        //Debug.LogFormat("LeftGazeTarget is: {0}", leftGazeTarget);
        //Debug.LogFormat("rightPupil is: {0}", rightPupilTarget);
        //Debug.LogFormat("leftPupil is: {0}", leftPupilTarget);


        float[] bound = new float[4];
        bound[0] = rPupilX;
        bound[1] = rPupilY;
        bound[2] = lPupilX;
        bound[3] = lPupilY;

        /*if (boundsFound)
        {
            Session.instance.CurrentTrial.result["right Pupil X"] = rPupilX;
            Session.instance.CurrentTrial.result["right Pupil Y"] = rPupilY;
            Session.instance.CurrentTrial.result["left Pupil X"] = lPupilX;
            Session.instance.CurrentTrial.result["left Pupil Y"] = lPupilY;
        }*/

        return bound;
    }

    public void setTarget()
    {
        centerLine.transform.localPosition = new Vector3(0f, 0.4f, 1f);
        Debug.Log("Target Set");
    }
}
