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

    private bool boundsFound = false;

    public void Start()
    {
        boundsFound = false;
        centerLineMoveable = true;
        leftBoundFound = false;
        rightBoundFound = false;
        centerLineMoveable = false;

        GetBounds();
        Generate(session);
        BeginNext();
    }
    
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
        while (leftBoundFound == false) { yield return null;  }
        Debug.Log("getting bound right");
        findRightBound();
        while (rightBoundFound == false) { yield return null; }
        boundsFound = true;
    }

    private void findLeftBound()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            leftBoundLocation = centerLine.transform.position;
            Debug.LogFormat("leftBound Location is: {0}", leftBoundLocation);
            leftEyeBound = getEyeTracking();
            Debug.LogFormat("leftBound is: {0}", leftEyeBound);
            leftBoundFound = true;
        }
        
    }
    private void findRightBound()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rightBoundLocation = centerLine.transform.position;
            Debug.LogFormat("rightBound Location is: {0}", rightBoundLocation);
            rightEyeBound = getEyeTracking();
            Debug.LogFormat("rightBound is: {0}", rightEyeBound);
            rightBoundFound = true;
        }
        
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
        if (centerLineMoveable)
        {
            centerLine.transform.Translate(speed, 0f, 0f);
        }
        

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextTrialTimer && boundsFound)
        {
            getEyeTracking();
            if (boundsFound)
            {
                nextTrialTimer = Time.time + waitSeconds;
                EndAndPrepare();
            }
        }
    }

    private UXF.Session session;
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

        if (session.settings.GetBool("nine_point_calibration", true))
        {
            Debug.Log("nine_point_calibration bool true");
            //float cm_shift = session.settings.GetFloat("cm_shift", 0.1f);
            int trialNum = 1;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Trial currTrial = block1.GetRelativeTrial(trialNum);
                    currTrial.settings.SetValue("cube_x_pos", 0 + ((cm_shift/100f) * i));
                    currTrial.settings.SetValue("cube_y_pos", 0 + ((cm_shift/100f) * j));
                    trialNum += 1;
                    Debug.LogFormat("trial: {0}, x_pos: {1}, y_pos: {2}", currTrial.number, currTrial.settings.GetFloat("cube_x_pos"),
                                        currTrial.settings.GetFloat("cube_y_pos"));
                }
                
            }
        }
        if (session.settings.GetBool("calibration", true))
        {

            Debug.Log("calibration bool true");
            //float cm_shift = session.settings.GetFloat("cm_shift", 0.1f);
            int trialNum = 1;
            for (int i = 0; i < 10; i++)
            {
                Trial currTrial = block1.GetRelativeTrial(trialNum);
                currTrial.settings.SetValue("cube_x_pos", 0.3f + ((cm_shift/100f) * i));
                currTrial.settings.SetValue("cube_y_pos", 0);
                trialNum += 1;
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

        if (boundsFound)
        {
            Session.instance.CurrentTrial.result["right Pupil X"] = rPupilX;
            Session.instance.CurrentTrial.result["right Pupil Y"] = rPupilY;
            Session.instance.CurrentTrial.result["left Pupil X"] = lPupilX;
            Session.instance.CurrentTrial.result["left Pupil Y"] = lPupilY;
        }

        return bound;
    }

    public GameObject centerLine;
    public float stepSize;

    public void SetObjects(float distance, float cube_x_pos, float cube_y_pos)
    {
        centerLine.transform.localPosition = new Vector3(0f, 0.4f, cube_x_pos);
        Debug.Log(centerLine.transform.position);
    }

    public void PresentStimulus(Trial trial)
    {
        Debug.LogFormat("Running Calibration Trial {0}", trial.number);
        int distance = trial.settings.GetInt("distance");
        float cube_x_pos = trial.settings.GetFloat("cube_x_pos");
        float cube_y_pos = trial.settings.GetFloat("cube_y_pos");
        //int distance = trial.settings.GetInt("scene_distance");
        //float noniusStartPos = trial.settings.GetFloat("nonius_start_pos");
        //Debug.LogFormat("The distance for this trial is: {0}", distance);
        //Debug.LogFormat("Nonius Start Pos is: {0}", noniusStartPos);

        Debug.LogFormat("Cube Position is: {0}, {1}", cube_x_pos, cube_y_pos);
        Session.instance.CurrentTrial.result["target_x_pos"] = cube_x_pos;
        Session.instance.CurrentTrial.result["target_y_pos"] = cube_y_pos;
        //Session.instance.CurrentTrial.result["Scene Distance"] = distance;
        //Session.instance.CurrentTrial.result["Nonius Start"] = noniusStartPos;


        SetObjects(distance, cube_x_pos, cube_y_pos);
        

        //Invoke("EndAndPrepare", 1f);
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
