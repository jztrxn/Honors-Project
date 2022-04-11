using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class CalibrationTask : MonoBehaviour
{
    public GameObject marker;
    private float[] pupilCoords;
    private float xMarkerPos;

    public void SetMarker(float xMarkerPos, float distance)
    {
        marker.transform.position = new Vector3(xMarkerPos, 1.36144f, distance);
        Debug.LogFormat("Marker set at: {0}", marker.transform.position);
    }


    public void logData()
    {
        Session.instance.CurrentTrial.result["cal_x_pos"] = xMarkerPos;

        //Coordinate Format: [LX, LY, RX, RY]
        Session.instance.CurrentTrial.result["LX Pupil"] = pupilCoords[0];
        Session.instance.CurrentTrial.result["LY Pupil"] = pupilCoords[1];
        Session.instance.CurrentTrial.result["RX Pupil"] = pupilCoords[2];
        Session.instance.CurrentTrial.result["RY Pupil"] = pupilCoords[3];
    }

    public void StartCalibrationTaskTrial(Trial trial)
    {
        Debug.LogFormat("Running Calibration Trial {0}", trial.number);
        xMarkerPos = trial.settings.GetFloat("marker_x_pos");
        float distance = trial.settings.GetFloat("distance");
        SetMarker(xMarkerPos, distance);
        StartCoroutine(waitForSpacePress());
    }
    
    public void EndCalibrationTaskTrial(Trial trial)
    {
        Debug.Log("Ending Trial");
        NextTrial();
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

    
}
