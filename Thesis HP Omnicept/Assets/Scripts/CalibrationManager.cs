using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;


public class CalibrationManager : MonoBehaviour
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

    public float cm_shift;
    public void Generate(Session session)
    {
        int numTrials = session.settings.GetInt("trials_per_block", 10);

        Block block1 = session.CreateBlock(numTrials);
        //Block block2 = session.CreateBlock(numTrials);


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

        if (session.settings.GetBool("calibration", true))
        {
            Debug.Log("calibration bool true");
            //float cm_shift = session.settings.GetFloat("cm_shift", 0.1f);
            for (int i = -5; i < numTrials -5; i++)
            {
                Trial currTrial = block1.GetRelativeTrial(i + 6);
                currTrial.settings.SetValue("cube_x_pos", 0 + (cm_shift * i));
                Debug.LogFormat("trial: {0}, x_pos: {1}", currTrial.number, currTrial.settings.GetFloat("cube_x_pos"));
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
    public float stepSize;

    public void SetObjects(float distance, float cube_x_pos)
    {
        centerLine.transform.localPosition = new Vector3(cube_x_pos, 0.0f, distance);
    }

    public void PresentStimulus(Trial trial)
    {
        Debug.LogFormat("Running Calibration Trial {0}", trial.number);
        int distance = trial.settings.GetInt("distance");
        float cube_x_pos = trial.settings.GetFloat("cube_x_pos");
        //int distance = trial.settings.GetInt("scene_distance");
        //float noniusStartPos = trial.settings.GetFloat("nonius_start_pos");
        //Debug.LogFormat("The distance for this trial is: {0}", distance);
        //Debug.LogFormat("Nonius Start Pos is: {0}", noniusStartPos);

        Debug.LogFormat("Cube Position is: {0}", cube_x_pos);
        Session.instance.CurrentTrial.result["target_x_pos"] = cube_x_pos;
        //Session.instance.CurrentTrial.result["Scene Distance"] = distance;
        //Session.instance.CurrentTrial.result["Nonius Start"] = noniusStartPos;

        SetObjects(distance, cube_x_pos);
        

        //Invoke("EndAndPrepare", 1f);
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
            Debug.LogFormat("speed is {0}", speed);
        } else if (Input.GetKey(KeyCode.D) && (Mathf.Abs(speed) < maxSpeed))
        {
            speed = speed + acceleration * Time.deltaTime;
            Debug.LogFormat("speed is {0}", speed);
        } else
        {
            speed = 0;
        }
        //noniusLine.transform.Translate(speed, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextTrialTimer)
        {
            nextTrialTimer = Time.time + waitSeconds;
            EndAndPrepare();
            getEyeTracking();
        }
    }
    public void EndAndPrepare()
    {
        //float discrepancy = noniusLine.transform.position.x;
        //Debug.LogFormat("Discrepancy between two eyes was {0}", discrepancy);

        //Session.instance.CurrentTrial.result["Discrepancy"] = System.Math.Round(discrepancy, 7);
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
        Debug.Log("this is ending and ran");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
