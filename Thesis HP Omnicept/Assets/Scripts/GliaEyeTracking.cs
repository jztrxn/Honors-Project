using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging.Messages;

public class GliaEyeTracking : MonoBehaviour
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
    
    public bool showEyeTrackingMessages = true;

    public void EyeTrackingHandler(EyeTracking eyeTracking)
    {
        if (showEyeTrackingMessages && eyeTracking != null)
        {
            Debug.Log(eyeTracking);
        }
    }

    public float[] getEyeTracking()
    {
        var eyeTracking = gliaBehaviour.GetLastEyeTracking();
        //Debug.Log(eyeTracking);

        

        float rPupilX = eyeTracking.RightEye.PupilPosition.X;
        float rPupilY = eyeTracking.RightEye.PupilPosition.Y;
        float lPupilX = eyeTracking.LeftEye.PupilPosition.X;
        float lPupilY = eyeTracking.LeftEye.PupilPosition.Y;
        

        //Coordinate Format: [LX, LY, RX, RY]
        float[] pupilCoords = new float[] { lPupilX, lPupilY, rPupilX, rPupilY };
        //Debug.LogFormat("pupilCoords: {0}", pupilCoords);
        return pupilCoords;

        //Session.instance.CurrentTrial.result["right Pupil X"] = rPupilX;
        //Session.instance.CurrentTrial.result["right Pupil Y"] = rPupilY;
        //Session.instance.CurrentTrial.result["left pupil X"] = lPupilX;
        //Session.instance.CurrentTrial.result["left Pupil Y"] = lPupilY;
    }
}
