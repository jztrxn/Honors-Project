using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;
using UnityEngine.SceneManagement;


public class ExperimentHandler : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public float cm_shift;
    public GameObject deviceGO;

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
            //Debug.LogFormat("i value: {0}", i);
            Trial currTrial = block1.GetRelativeTrial(trialNum);
            currTrial.settings.SetValue("marker_x_pos", 0f + ((cm_shift / 100f) * i));
            currTrial.settings.SetValue("calibration_distance", 1f);
            trialNum += 1;
            //Debug.LogFormat("Trial {0}, blockNum {2}, x_pos: {1}", currTrial.number, 
                              //currTrial.settings.GetFloat("marker_x_pos"), currTrial.block.number);
            if(i < 0)
            {
                currTrial.settings.SetValue("eye_tag", "Left Eye");
            }
            else
            {
                currTrial.settings.SetValue("eye_tag", "Right Eye");
            }
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
                trial.settings.SetValue("scene_distance", (0.3f + (((int)trial.number - numTrials) * (cm_shift / 100f))));
                //Debug.LogFormat("block {2},trial {1}, distance: {0}", trial.settings.GetFloat("scene_distance"), 
                                                                                    //trial.number, trial.block.number);
            }
        }

        //deviceGO = GameObject.Find("XRRig");
        //DataHolder.deviceHeight = deviceGO.transform.position.y;
        DataHolder.deviceHeight = 1.36144f;
        Debug.LogFormat("Device Height: {0}", DataHolder.deviceHeight);
    }


    // we will call this when the trial starts.
    public void SetupTrial(Trial trial)
    {
        // IMPORTANT
        // if this is the first trial in the block, we need to load a new scene.
        if (trial.numberInBlock == 1)
        {
            Debug.Log("Setting up trial");
            Debug.LogFormat("Trial Number: {0}, num in block: {1}, blocknum: {2}", 
                                            trial.number, trial.numberInBlock, trial.block.number);
            string scenePath = trial.settings.GetString("scene_name");

            // we'll load the scene asynchronously to avoid stutters
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(scenePath);

            // here we specify what we WILL do when the scene is loaded
            loadScene.completed += (op) => { SceneSpecificSetup(trial); };
            //SceneManager.LoadScene(scenePath);
            Debug.Log("loadScene completed");
            //SceneSpecificSetup(trial);
        }
        else
        {
            // if it is NOT the first trial in the block, we just do some scene specific stuff immediately, no need to wait for scene load
            SceneSpecificSetup(trial);
        }
    }
    public void SceneSpecificSetup(Trial trial)
    {
        Debug.Log("Scene Specific Setup");
        Debug.Log(trial);
        Debug.LogFormat("Trial Number: {0}, num in block: {1}, blocknum: {2}", trial.number, trial.numberInBlock, trial.block.number);
        // in order to perform scene-specific setup, we will find and our scene-specific scripts 
        // there are lots of ways to do this, but this works fine here
        if (trial.block.number == 1)
        {
            Debug.Log("start calib task");
            FindObjectOfType<CalibrationTask>().StartCalibrationTaskTrial(trial);
        }
        else if (trial.block.number == 2)
        {
            Debug.Log("start disp task");
            FindObjectOfType<DisparometerTask>().StartDisparometerTaskTrial(trial);
        }
    }

    public void CleanupTrial(Trial trial)
    {
        if (trial.block.number == 1)
        {
            Debug.Log("Ending Calibration Task");
            FindObjectOfType<CalibrationTask>().EndCalibrationTaskTrial(trial);
        }
        else if (trial.block.number == 2)
        {
            Debug.Log("Ending Disparometer Task");
            FindObjectOfType<DisparometerTask>().EndDisparometerTaskTrial(trial);
        }
    }
    
    void EndApp()
    {
        Debug.Log("this is ending and ran");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            Session.instance.BeginNextTrial();
        }
    }
}
