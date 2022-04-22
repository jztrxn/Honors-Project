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
        Block calBlock1 = session.CreateBlock(25);
        calBlock1.settings.SetValue("calibration_block", true);
        // Calibration 7 points horizontal
        Block calBlock2 = session.CreateBlock(25);
        calBlock2.settings.SetValue("calibration_block", true);

        getCalibrationPoints(calBlock1);
        getCalibrationPoints(calBlock2);
        

        for (int i = 0; i < 14; i++)
        {
            session.CreateBlock(7);
        }
        Debug.LogFormat(session.blocks.ToString());
        /*
        // Disparometer No Env Sequential 20cm apart
        Block block3 = session.CreateBlock(7);
        block3.settings.SetValue("environment", false);
        getDisparometerDistances(block3, session.settings.GetBool("random_nonius", false), "seq_norm");
        // Disparometer No Env Random 20cm apart
        Block block4 = session.CreateBlock(7);
        block4.settings.SetValue("environment", false);
        getDisparometerDistances(block4, session.settings.GetBool("random_nonius", false), "seq_rand");

        // Disparometer Yes Env Sequential 20cm apart
        Block block5 = session.CreateBlock(7);
        block5.settings.SetValue("environment", true);
        getDisparometerDistances(block5, session.settings.GetBool("random_nonius", false), "seq_norm");
        // Disparometer Yes Env Random 20cm apart
        Block block6 = session.CreateBlock(7);
        block6.settings.SetValue("environment", true);
        getDisparometerDistances(block6, session.settings.GetBool("random_nonius", false), "seq_rand");

        // No Env, seq
        Block block7 = session.CreateBlock(7);
        block7.settings.SetValue("environment", false);
        getDisparometerDistances(block7, session.settings.GetBool("random_nonius", false), "seq_norm");
        // No Env, rand
        Block block8 = session.CreateBlock(7);
        block8.settings.SetValue("environment", false);
        getDisparometerDistances(block8, session.settings.GetBool("random_nonius", false), "seq_rand");

        //yes env, seq
        Block block9 = session.CreateBlock(7);
        block9.settings.SetValue("environment", true);
        getDisparometerDistances(block9, session.settings.GetBool("random_nonius", false), "seq_norm");
        //yes env, rand
        Block block10 = session.CreateBlock(7);
        block10.settings.SetValue("environment", true);
        getDisparometerDistances(block10, session.settings.GetBool("random_nonius", false), "seq_rand");

        //no env, seq
        Block block11 = session.CreateBlock(7);
        block11.settings.SetValue("environment", false);
        getDisparometerDistances(block11, session.settings.GetBool("random_nonius", false), "seq_norm");
        //no env, rand
        Block block12 = session.CreateBlock(7);
        block12.settings.SetValue("environment", false);
        getDisparometerDistances(block12, session.settings.GetBool("random_nonius", false), "seq_rand");

        //yes env, seq
        Block block13 = session.CreateBlock(7);
        block13.settings.SetValue("environment", true);
        getDisparometerDistances(block13, session.settings.GetBool("random_nonius", false), "seq_norm");
        //yes env, rand
        Block block14 = session.CreateBlock(7);
        block14.settings.SetValue("environment", true);
        getDisparometerDistances(block14, session.settings.GetBool("random_nonius", false), "seq_rand");
        */

        int envCounter = 1;
        int dist_type = 1;
        foreach (Block block in session.blocks)
        {
            /*
            //check if calibration block
            if (block.settings.GetBool("calibration_block", false)){
                Debug.Log("block skipped");
                break;
            }*/

            // set scene name
            block.settings.SetValue("scene_name", "Disparometer");

            //set environment
            if (envCounter == 1 || envCounter == 2)
            {
                block.settings.SetValue("environment", false);
                Debug.LogFormat("block number: {0}, env {1}", block.number, block.settings.GetBool("environment"));
            }
            else
            {
                block.settings.SetValue("environment", true);
                Debug.LogFormat("block number: {0}, env {1}", block.number, block.settings.GetBool("environment"));
            }
            envCounter++;
            if (envCounter > 4) { envCounter = 1; }

            // set distances
            if ((dist_type % 2) == 1)
            {
                string dist_str = "seq_norm";
                getDisparometerDistances(block, session.settings.GetBool("random_nonius", false), dist_str);
            }
            else
            {
                string dist_str = "seq_rand";
                getDisparometerDistances(block, session.settings.GetBool("random_nonius", false), dist_str);
            }
            dist_type++;
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
    
    /*public void generateBlocks(Session session, int num_blocks)
    {
        for(int i = 1; i <= num_blocks; i++)
        {
            string block_name = "block" + i.ToString();
            Block block_name = session.CreateBlock(session.settings.GetInt("block3_numtrials", 7));
        }
    }*/

    public void getDisparometerDistances(Block block, bool random_nonius, string dist_type)
    {
        //int numTrials = block.settings.GetInt("block3_numtrials");
        //int numTrials = 7;
        int i = 0;
        float shift = 0f;
        float[] set_dist = new float[] { 35, 42, 63, 100, 200};
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
            /*else if (block.number == 5)
            {
                shift = (set_dist[trial.numberInBlock - 1] / 100) - 0.3f;
            }
            else if (block.number == 6)
            {
                i = Random.Range(0, 5);
                shift = (set_dist[i] / 100) - 0.3f;
            }*/
            
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
