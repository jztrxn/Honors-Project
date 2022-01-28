using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class ExperimentGenerator : MonoBehaviour
{
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

    public GameObject centerLine;
    public GameObject rightLine;
    public GameObject fusionLock;
    public float stepSize;

    public void SetObjects(float distance)
    {
        centerLine.transform.position = new Vector3(0f, 0.5f, distance);
        rightLine.transform.position = new Vector3(1.0f, 0f, distance);
        fusionLock.transform.position = new Vector3(0f, 0f, distance);
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
            //Application.Quit();
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
}
