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

    public void Generate(Session session)
    {
        // Calibration 7 points horizontal
        Block block1 = session.CreateBlock(session.settings.GetInt("block1_numtrials", 25));

        // Calibration 7 points horizontal
        Block block2 = session.CreateBlock(session.settings.GetInt("block2_numtrials", 25));

        
        // Disparometer No Env Sequential 20cm apart
        Block block3 = session.CreateBlock(session.settings.GetInt("block3_numtrials", 7));
        // Disparometer No Env Random 20cm apart
        Block block4 = session.CreateBlock(session.settings.GetInt("block4_numtrials", 7));

        // Disparometer No Env Sequential Set Dist
        Block block5 = session.CreateBlock(session.settings.GetInt("block5_numtrials", 5));
        Block block6 = session.CreateBlock(session.settings.GetInt("block6_numtrials", 10));


        block1.settings.SetValue("scene_name", "Calibration 1");
        block2.settings.SetValue("scene_name", "Calibration 1");

        block3.settings.SetValue("scene_name", "Disparometer");
        block4.settings.SetValue("scene_name", "Disparometer");

        block5.settings.SetValue("scene_name", "Disparometer");
        block6.settings.SetValue("scene_name", "Disparometer");


        // For Calibration Tasks
        getCalibrationPoints(block1);
        getCalibrationPoints(block2);

        getDisparometerDistances(block3, session.settings.GetBool("random_nonius", false));
        getDisparometerDistances(block4, session.settings.GetBool("random_nonius", false));

        getDisparometerDistances(block5, session.settings.GetBool("random_nonius", false));
        getDisparometerDistances(block6, session.settings.GetBool("random_nonius", false));

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

    public void getDisparometerDistances(Block block, bool random_nonius)
    {
        //int numTrials = block.settings.GetInt("block3_numtrials");
        //int numTrials = 7;
        int i = 0;
        float shift = 0f;
        float[] set_dist = new float[] { 35, 42, 63, 100, 200};
        foreach (Trial trial in block.trials)
        {
            if (block.number == 3)
            {
                i++;
                shift = i * (cm_shift / 100f);
            }
            else if (block.number == 4)
            {
                i = Random.Range(0, 7);
                shift = i * (cm_shift / 100f);
            }
            else if (block.number == 5)
            {
                shift = (set_dist[trial.numberInBlock - 1] / 100) - 0.3f;
            }
            else if (block.number == 6)
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
                trial.settings.SetValue("nonius_start_pos", Random.Range(-1f, 1f));
            }
            else
            {
                trial.settings.SetValue("nonius_start_pos", 0.25f);
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
        else if (trial.block.number == 3 || trial.block.number == 4)
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
        else if (trial.block.number == 3 || trial.block.number == 4)
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
