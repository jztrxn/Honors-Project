using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;
using UnityEngine.SceneManagement;


public class ExperimentHandler : MonoBehaviour
{
    public GameObject GliaBehavior;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GliaBehavior);
    }

    public float cm_shift;
    public GameObject deviceGO;
    public int numBlocks;
    public void Generate(Session session)
    {
        // Calibration 7 points horizontal
        Block calBlock1 = session.CreateBlock(25);
        calBlock1.settings.SetValue("calibration_block", true);
        // Calibration 7 points horizontal
        Block calBlock2 = session.CreateBlock(25);
        calBlock2.settings.SetValue("calibration_block", true);

        getCalibrationPoints(calBlock1);
        getCalibrationPoints(calBlock2);
        

        for (int i = 0; i < numBlocks; i++)
        {
            session.CreateBlock(7);
        }
        Debug.LogFormat(session.blocks.ToString());
        

        int envCounter = 1;
        int dist_type = 1;
        foreach (Block block in session.blocks)
        {
            // set scene name
            block.settings.SetValue("scene_name", "Disparometer");

            //set environment
            if (envCounter == 1 || envCounter == 2)
            {
                block.settings.SetValue("environment", false);
                //Session.instance.CurrentTrial.result["Environment"] = block.settings.GetBool("environment").ToString();
                Debug.LogFormat("block number: {0}, env {1}", block.number, block.settings.GetBool("environment"));
            }
            else
            {
                block.settings.SetValue("environment", true);
                //Session.instance.CurrentTrial.result["Environment"] = block.settings.GetBool("environment").ToString();
                Debug.LogFormat("block number: {0}, env {1}", block.number, block.settings.GetBool("environment"));
            }
            envCounter++;
            if (envCounter > 4) { envCounter = 1; }

            // set distances
            string dist_str;
            if (dist_type == 1)
            {
                dist_str = "seq_norm";
                //Session.instance.CurrentTrial.result["seq_type"] = dist_str;
                
            }
            else if (dist_type == 2)
            {
                dist_str = "seq_rand";
                //Session.instance.CurrentTrial.result["seq_type"] = dist_str;
            }
            else if (dist_type == 3)
            {
                dist_str = "preset_norm";
                //Session.instance.CurrentTrial.result["seq_type"] = dist_str;
            }
            else
            {
                dist_str = "preset_rand";
                //Session.instance.CurrentTrial.result["seq_type"] = dist_str;
            }
            getDisparometerDistances(block, session.settings.GetBool("random_nonius", true), dist_str);
            dist_type++;
            if (dist_type > 4) { dist_type = 1; }
        }

        calBlock1.settings.SetValue("scene_name", "Calibration 1");
        calBlock2.settings.SetValue("scene_name", "Calibration 1");

    }

    public void getCalibrationPoints(Block block)
    {
        int trialNum = 1;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                //Debug.LogFormat("i value: {0}, j value: {1}", i, j);
                Trial currTrial = block.GetRelativeTrial(trialNum);
                currTrial.settings.SetValue("marker_x_pos", 0f + ((cm_shift / 100f) * i));
                currTrial.settings.SetValue("marker_y_pos", 0f + ((cm_shift / 100f) * j));
                currTrial.settings.SetValue("calibration_distance", 1f);
                trialNum += 1;
                //Debug.LogFormat("Trial {0}, blockNum {2}, x_pos: {1}, y_pos: {3}", currTrial.number, 
                //  currTrial.settings.GetFloat("marker_x_pos"), currTrial.block.number, currTrial.settings.GetFloat("marker_y_pos"));
            }
        }
        //make left or right eye
        foreach(Trial trial in block.trials)
        {
            if (trial.block.number == 1)
            {
                trial.settings.SetValue("eye_tag", "Left Eye");
            }
            else if (trial.block.number == 2)
            {
                trial.settings.SetValue("eye_tag", "Right Eye");
            }
        }
        
    }

    public void getDisparometerDistances(Block block, bool random_nonius, string dist_type)
    {
        //int numTrials = block.settings.GetInt("block3_numtrials");
        //int numTrials = 7;
        int i = 0;
        float shift = 0f;
        float[] set_dist = new float[] { 30, 35, 42, 63, 80, 100, 200};
        foreach (Trial trial in block.trials)
        {
            if (dist_type == "seq_norm")
            {
                i++;
                shift = i * (cm_shift / 100f);
            }
            else if (dist_type == "seq_rand")
            {
                i = Random.Range(0, 7);
                shift = i * (cm_shift / 100f);
            }
            else if (dist_type == "preset_norm")
            {
                shift = (set_dist[trial.numberInBlock - 1] / 100) - 0.3f;
            }
            else if (dist_type == "preset_rand")
            {
                i = Random.Range(0, 5);
                shift = (set_dist[i] / 100) - 0.3f;
            }
            
            trial.settings.SetValue("scene_distance", (0.3f + shift));
            Debug.LogFormat("Trial {0}, Block {1}, scene_distance: {2}", trial.number,
                block.number, trial.settings.GetFloat("scene_distance"));
        }

        
        foreach (Trial trial in block.trials)
        {
            if (random_nonius)
            {
                trial.settings.SetValue("nonius_start_pos", Random.Range(-0.1f, 0.1f));
            }
            else
            {
                trial.settings.SetValue("nonius_start_pos", 0.1f);
            }
        }
    }


    // we will call this when the trial starts.
    public void SetupTrial(Trial trial)
    {
        // IMPORTANT
        // if this is the first trial in the block, we need to load a new scene.
        if (trial.numberInBlock == 1)
        {
            //Debug.LogFormat("Trial Number: {0}, num in block: {1}, blocknum: {2}", 
            //                                trial.number, trial.numberInBlock, trial.block.number);
            
            string scenePath = trial.settings.GetString("scene_name");
            //Debug.LogFormat("Loading Scene: {0}", scenePath);
            // we'll load the scene asynchronously to avoid stutters
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(scenePath);

            // here we specify what we WILL do when the scene is loaded
            loadScene.completed += (op) => { SceneSpecificSetup(trial); };
            //SceneManager.LoadScene(scenePath);
            //Debug.Log("loadScene completed");
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
        //Debug.Log("Scene Specific Setup");
        //Debug.Log(trial);
        //Debug.LogFormat("Trial Number: {0}, num in block: {1}, blocknum: {2}", trial.number, trial.numberInBlock, trial.block.number);
        if (trial.block.number == 1 || trial.block.number == 2)
        {
            //Debug.Log("start calib task");
            FindObjectOfType<CalibrationTask>().StartCalibrationTaskTrial(trial);
        }
        else
        {
            //Debug.Log("start calib task 2");
            FindObjectOfType<DisparometerTask>().StartDisparometerTaskTrial(trial);
        }
    }

    public void CleanupTrial(Trial trial)
    {
        if (trial.block.number == 1 || trial.block.number == 2)
        {
            //Debug.Log("Ending Calibration Task");
            FindObjectOfType<CalibrationTask>().EndCalibrationTaskTrial(trial);
        }
        else
        {
            //Debug.Log("Ending Disparometer Task");
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
