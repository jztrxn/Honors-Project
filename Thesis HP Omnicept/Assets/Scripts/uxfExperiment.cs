using HP.Omnicept.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class uxfExperiment : MonoBehaviour
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

    public void Update()
    {
        gliaBehaviour.GetLastEyeTracking();
    }


    public void GenerateExperiment(Session session)
    {
        int numMainTrials = session.settings.GetInt("n_trials", valueIfNotFound: 5);

        Block mainBlock = session.CreateBlock(numMainTrials);


    }

    public void PresentStimulus(Trial trial)
    {
        Debug.LogFormat("Running trial {0}", trial.number);

        float thrust = trial.settings.GetFloat("thrust");

        
    }
}
