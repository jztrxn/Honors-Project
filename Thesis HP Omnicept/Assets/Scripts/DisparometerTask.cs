using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class DisparometerTask : MonoBehaviour
{
    private float[] pupilCoords;

    public void logData()
    {
        float discrepancy = noniusLine.transform.position.x;
        Debug.LogFormat("Discrepancy between two eyes was {0}", discrepancy);
        Session.instance.CurrentTrial.result["Discrepancy"] = System.Math.Round(discrepancy, 7);

        Session.instance.CurrentTrial.result["LX Pupil"] = pupilCoords[0];
        Session.instance.CurrentTrial.result["LY Pupil"] = pupilCoords[1];
        Session.instance.CurrentTrial.result["RX Pupil"] = pupilCoords[2];
        Session.instance.CurrentTrial.result["RY Pupil"] = pupilCoords[3];
    }

    public GameObject placeHolder;
    public GameObject centerLine;
    public GameObject noniusLine;
    public GameObject fusionLock;
    public GameObject eyeMarker;
    public GameObject plane;
    //public float scaleFactor;

    public void SetObjects(float distance, float noniusStart)
    {
        placeHolder.transform.localPosition = new Vector3(0, 0, distance);
        //fusionLock.transform.localScale = new Vector3(scaleFactor * distance, scaleFactor * distance, 1f);
        plane.transform.localPosition = new Vector3(0f, 0f, 0f);
        centerLine.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        noniusLine.transform.localPosition = new Vector3(noniusStart, -0.12f, 0f);
        fusionLock.transform.localPosition = new Vector3(0f, 0f, 0f);
        //eyeMarker.transform.localPosition = new Vector3(0f,)
        //Debug.LogFormat("fusionLock scale: {0}", fusionLock.transform.localScale);
        //Debug.LogFormat("fusionLock position: {0}", fusionLock.transform.position);
    }

    public void StartDisparometerTaskTrial(Trial trial)
    {
        Debug.LogFormat("Running Disparometer Trial {0}", trial.number);

        float distance = trial.settings.GetFloat("scene_distance");
        float noniusStartPos = trial.settings.GetFloat("nonius_start_pos");
        Debug.LogFormat("The distance for this trial is: {0}", distance);
        Debug.LogFormat("Nonius Start Pos is: {0}", noniusStartPos);

        Session.instance.CurrentTrial.result["Scene Distance"] = distance;
        Session.instance.CurrentTrial.result["Nonius Start"] = noniusStartPos;

        SetObjects(distance, noniusStartPos);
        StartCoroutine(waitForSpacePress());
    }

    public void EndDisparometerTaskTrial(Trial trial)
    {
        Debug.Log("Ending Trial");
        NextTrial();
    }

    private IEnumerator waitForSpacePress()
    {
        yield return new WaitForSeconds(1f);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        pupilCoords = FindObjectOfType<GliaEyeTracking>().getEyeTracking();
        //nextTrialTimer = Time.time + waitSeconds;
        logData();
        Session.instance.CurrentTrial.End();
    }

    void NextTrial()
    {
        if (Session.instance.CurrentTrial == Session.instance.LastTrial)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            	Application.Quit();
#endif
        }
        else
        {
            Session.instance.BeginNextTrial();
        }
    }
}
