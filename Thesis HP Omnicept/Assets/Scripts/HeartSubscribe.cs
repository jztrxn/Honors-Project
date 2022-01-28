using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;

public class HeartSubscribe : MonoBehaviour
{
    // Lazy cache of GliaBehaviour 
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

    /*// Subscribe to Heart Rate messages
    public void StartListeningToHeartRate()
    {
        gliaBehaviour.OnHeartRate.AddListener(AccumulateHeartRate);
    }


    // Unsubscribe to Heart Rate messages 
    public void StopListeningToHeartRate()
    {
        gliaBehaviour.OnHeartRate.RemoveListener(AccumulateHeartRate);
    }


    // Your code using heart rate messages
    private void AccumulateHeartRate(HeartRate heartRate)
    {
        
        Debug.Log("somethings happening");
        Debug.Log(heartRate.Rate);
    }*/
    
    public void Update()
    {
        var eyeTracking = gliaBehaviour.GetLastEyeTracking();
        Debug.Log(eyeTracking);

    }


}
