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
    public GameObject noniusLine;
    public GameObject environment;
    public void SetObjects(float distance, float noniusStart)
    {
        placeHolder.transform.localPosition = new Vector3(0, 0, distance);
        noniusLine.transform.localPosition = new Vector3(noniusStart, -0.035f, -0.0008000135f);
    }

    public void StartDisparometerTaskTrial(Trial trial)
    {
        Debug.LogFormat("Running Disparometer Trial {0}, Block Number {1}", trial.number, trial.block.number);

        float distance = trial.settings.GetFloat("scene_distance");
        float noniusStartPos = trial.settings.GetFloat("nonius_start_pos");
        Debug.LogFormat("The distance for this trial is: {0}", distance);
        //Debug.LogFormat("Nonius Start Pos is: {0}", noniusStartPos);

        Session.instance.CurrentTrial.result["Scene Distance"] = distance;
        Session.instance.CurrentTrial.result["Nonius Start"] = noniusStartPos;

        SetObjects(distance, noniusStartPos);
        if (trial.block.settings.GetBool("environment"))
        {
            environment.SetActive(true);
        }
        else
        {
            environment.SetActive(false);
        }
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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q)){

        }
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
