using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;


public class ExperimentHandler : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public float cm_shift;
    public void Generate(Session session)
    {
        //int numTrials = session.settings.GetInt("trials_per_block", 10);

        Block block1 = session.CreateBlock(session.settings.GetInt("block1_numtrials", 7));
        Block block2 = session.CreateBlock(session.settings.GetInt("block2_numtrials", 7));

        block1.settings.SetValue("scene_name", "Calibration 1");
        block2.settings.SetValue("scene_name", "Disparometer");

        int trialNum = 1;
        int numTrials = session.settings.GetInt("block1_numtrials");
        for (int i = -numTrials/2; i < numTrials/2 + 1; i++)
        {
            Debug.LogFormat("i value: {0}", i);
            Trial currTrial = block1.GetRelativeTrial(trialNum);
            currTrial.settings.SetValue("marker_x_pos", 0f + ((cm_shift / 100f) * i));
            currTrial.settings.SetValue("distance", 1f);
            trialNum += 1;
            Debug.LogFormat("Trial {0}, x_pos: {1}", currTrial.number, currTrial.settings.GetFloat("marker_x_pos"));
        }
        


        foreach (Trial trial in block2.trials)
        {
            if (session.settings.GetBool("random_nonius", false))
            {
                trial.settings.SetValue("nonius_start_pos", Random.Range(-1f, 1f));
            }
            else
            {
                trial.settings.SetValue("nonius_start_pos", 0.5f);
            }

            if (session.settings.GetBool("random_distance", false))
            {
                trial.settings.SetValue("scene_distance", Random.Range(3, 10));
            }
            else
            {
                trial.settings.SetValue("scene_distance", (0.5f + ((int)trial.number * 0.01f * cm_shift)));
                Debug.LogFormat("trial distance: {0}", trial.settings.GetFloat("scene_distance"));
                // find better way to change the distances
            }
        }
    }

    public void startNextTrial()
    {
        Session.instance.BeginNextTrial();
    }

    public void SetupTrial(Trial trial)
    {
        if (trial.numberInBlock == 1)
        {
            string scenePath = trial.settings.GetString("scene_name");
            AsyncOperation loadScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath);
            loadScene.completed += (op) => { SceneSpecificSetup(trial); };
            //SceneSpecificSetup(trial);
            Debug.Log("SetUpTrial Successfully Ran");
        }
        else
        {
            SceneSpecificSetup(trial);
            Debug.Log("SceneSpecificSetup Ran");
        }
    }

    void SceneSpecificSetup(Trial trial)
    {
        if(trial.block.number == 1)
        {
            Debug.Log("Starting Calibration Trial");
            FindObjectOfType<CalibrationTask>().StartCalibrationTaskTrial(trial);
        }
        else if (trial.block.number == 2)
        {
            Debug.Log("Starting Disparometer Trial");
            FindObjectOfType<DisparometerTask>().StartDisparometerTaskTrial(trial);
        }
    }
   
    public void CleanupTrial(Trial trial)
    {
        if (trial.block.number == 1)
        {
            FindObjectOfType<CalibrationTask>().EndCalibrationTaskTrial(trial);
        }
        else if (trial.block.number == 2)
        {
            FindObjectOfType<DisparometerTask>().EndDisparometerTaskTrial(trial);
        }
    }
    
    void EndApp()
    {
        Debug.Log("this is ending and ran");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
