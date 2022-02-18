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
        int numTrials = session.settings.GetInt("trials_per_block", 5);

        Block block1 = session.CreateBlock(numTrials);
        Block block2 = session.CreateBlock(numTrials);


        foreach (Trial trial in session.Trials)
        {
            if (session.settings.GetBool("random_nonius", false))
            {
                trial.settings.SetValue("nonius_start_pos", Random.Range(-1f, 1f));
            }
            else
            {
                trial.settings.SetValue("nonius_start_pos", 1f);
            }

            if (session.settings.GetBool("random_distance", false))
            {
                trial.settings.SetValue("scene_distance", Random.Range(2, 10));
            }
            else
            {
                trial.settings.SetValue("scene_distance", (int)trial.number + 1);
            }
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

    public GameObject centerLine;
    public GameObject noniusLine;
    public GameObject fusionLock;
    public float stepSize;

    public void SetObjects(float distance, float noniusStart)
    {
        centerLine.transform.localPosition = new Vector3(0f, 0.5f, distance);
        noniusLine.transform.localPosition = new Vector3(noniusStart, 0f, distance);
        fusionLock.transform.localPosition = new Vector3(0f, 0f, distance);
    }

    public void PresentStimulus(Trial trial)
    {
        Debug.LogFormat("Running trial {0}", trial.number);

        int distance = trial.settings.GetInt("scene_distance");
        float noniusStartPos = trial.settings.GetFloat("nonius_start_pos");
        Debug.LogFormat("The distance for this trial is: {0}", distance);
        Debug.LogFormat("Nonius Start Pos is: {0}", noniusStartPos);

        Session.instance.CurrentTrial.result["Scene Distance"] = distance;
        Session.instance.CurrentTrial.result["Nonius Start"] = noniusStartPos;

        SetObjects(distance, noniusStartPos);
        

        //Invoke("EndAndPrepare", 1f);
    }
    
    private float totalTime;
    private void Update()
    {
        //Debug.LogFormat("RightGazeTarget is: {0}", rightGazeTarget);
        //Debug.LogFormat("LeftGazeTarget is: {0}", leftGazeTarget);
        if (Input.GetKey(KeyCode.A))
        {
            totalTime = Time.deltaTime * stepSize;
            noniusLine.transform.Translate(-totalTime, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            noniusLine.transform.Translate(+totalTime, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            EndAndPrepare();
            getEyeTracking();
        }
    }
    public void EndAndPrepare()
    {
        float discrepancy = noniusLine.transform.position.x;
        Debug.LogFormat("Discrepancy between two eyes was {0}", discrepancy);

        Session.instance.CurrentTrial.result["Discrepancy"] = System.Math.Round(discrepancy, 7);

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
            Invoke("BeginNext", 2.0f);
        }
    }

    void BeginNext()
    {
        Session.instance.BeginNextTrial();
    }

    void EndApp()
    {
        Debug.Log("this is ending and ran");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
