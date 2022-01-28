using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;


public class ExperimentGenerator : MonoBehaviour
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


    public void Generate(Session session)
    {
        int numTrials = session.settings.GetInt("trials_per_block", 10);

        Block block1 = session.CreateBlock(numTrials);
        //Block block2 = session.CreateBlock(numTrials);


        foreach (Trial trial in session.Trials)
        {
            //Do something
        }
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

    public void getEyeTracking()
    {
        var eyeTracking = gliaBehaviour.GetLastEyeTracking();
        Debug.Log(eyeTracking);


        rightGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y);
        leftGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y);

        float rPupilX = eyeTracking.RightEye.PupilPosition.X;
        float rPupilY = eyeTracking.RightEye.PupilPosition.Y;
        float lPupilX = eyeTracking.LeftEye.PupilPosition.X;
        float lPupilY = eyeTracking.LeftEye.PupilPosition.Y;
        Debug.Log(rPupilX);
        Debug.Log(rPupilY);
        rightPupilTarget = new Vector2(rPupilX, rPupilY);
        leftPupilTarget = new Vector2(lPupilX, lPupilY);
        Debug.LogFormat("RightGazeTarget is: {0}", rightGazeTarget);
        Debug.LogFormat("LeftGazeTarget is: {0}", leftGazeTarget);
        Debug.LogFormat("rightPupil is: {0}", rightPupilTarget);
        Debug.LogFormat("leftPupil is: {0}", leftPupilTarget);

        Session.instance.CurrentTrial.result["right eye pupil"] = (rPupilX, rPupilY);
        Session.instance.CurrentTrial.result["left eye pupil"] = (lPupilX, lPupilY);
        //rightPupilSizeTarget = eyeTracking.RightEye.PupilDilation / 10f;
        //leftPupilSizeTarget = eyeTracking.LeftEye.PupilDilation / 10f;
    }
    /*private void OnEyeTracking(EyeTracking eyeTracking)
    {
        if(eyeTracking != null)
        {
            rightGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y);
            leftGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y);
            //Debug.LogFormat("RightGazeTarget is: {0}", rightGazeTarget);
            //Debug.LogFormat("LeftGazeTarget is: {0}", leftGazeTarget);
            //rightPupilSizeTarget = eyeTracking.RightEye.PupilDilation / 10f;
            //leftPupilSizeTarget = eyeTracking.LeftEye.PupilDilation / 10f;
        }
    }*/

    public GameObject centerLine;
    public GameObject rightLine;
    public GameObject fusionLock;
    public float stepSize;

    public void SetObjects(float distance)
    {
        centerLine.transform.localPosition = new Vector3(0f, 0.5f, distance);
        rightLine.transform.localPosition = new Vector3(1.0f, 0f, distance);
        fusionLock.transform.localPosition = new Vector3(0f, 0f, distance);
    }

    public void PresentStimulus(Trial trial)
    {
        Debug.LogFormat("Running trial {0}", trial.number);

        float distance = (float)trial.number + 1f;
        Debug.LogFormat("The distance for this trial is: {0}", distance);

        SetObjects(distance);
        

        //Invoke("EndAndPrepare", 1f);
    }

    private void Update()
    {
        //Debug.LogFormat("RightGazeTarget is: {0}", rightGazeTarget);
        //Debug.LogFormat("LeftGazeTarget is: {0}", leftGazeTarget);
        if (Input.GetKey(KeyCode.A))
        {
            rightLine.transform.Translate(-stepSize, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rightLine.transform.Translate(+stepSize, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            EndAndPrepare();
            getEyeTracking();
        }
    }
    public void EndAndPrepare()
    {
        float discrepancy = rightLine.transform.position.x;
        Debug.LogFormat("Discrepancy between two eyes was {0}", discrepancy);

        Session.instance.CurrentTrial.result["discrepancy"] = System.Math.Round(discrepancy, 7);

        Debug.Log("Ending Trial");
        Session.instance.CurrentTrial.End();

        // if last trial, end session.
        if (Session.instance.CurrentTrial == Session.instance.LastTrial)
        {
            Session.instance.End();
            
        }
        else
        {
            // begin next after 2 second delay
            BeginNext();
        }
    }

    void BeginNext()
    {
        Session.instance.BeginNextTrial();
    }

    void EndApp()
    {
        Application.Quit();
    }
}
