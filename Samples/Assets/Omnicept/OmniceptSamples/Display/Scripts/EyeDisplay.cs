// (c) Copyright 2020 HP Development Company, L.P.

using System;
using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HP.Glia.Examples.Display
{
    public class EyeDisplay : UGUIBaseDisplay
    {
        private Vector2 leftGazeTarget;
        private float leftPupilSizeTarget = 0.5f;
        private Vector2 rightGazeTarget;
        private float rightPupilSizeTarget = 0.5f;

        

        private Material rightEyeMaterial;
        private Material leftEyeMaterial;

        [Header("References")]
        public GameObject rightEye;
        public GameObject leftEye;

        void Start()
        {
            gliaBehaviour.OnEyeTracking.AddListener(OnEyeTracking);

            if(rightEye.GetComponent<Graphic>()){
                rightEyeMaterial = new Material(rightEye.GetComponent<Graphic>().material);
                rightEye.GetComponent<Graphic>().material = rightEyeMaterial;
            }
            else if(rightEye.GetComponent<Renderer>()){
                rightEyeMaterial = rightEye.GetComponent<Renderer>().material;
            }
            else{
                Debug.LogWarning("No material?");
            }
            

            if(leftEye.GetComponent<Graphic>()){
                leftEyeMaterial  = new Material(leftEye.GetComponent<Graphic>().material);
                leftEye.GetComponent<Graphic>().material = leftEyeMaterial;
            }
            else if(leftEye.GetComponent<Renderer>()){
                leftEyeMaterial = leftEye.GetComponent<Renderer>().material;
            }
            else{
                Debug.LogWarning("No material?");
            }
            
        }

        void Update()
        {
            rightEyeMaterial.SetFloat("_EyePosX", rightGazeTarget.x);
            rightEyeMaterial.SetFloat("_EyePosY", rightGazeTarget.y);
            rightEyeMaterial.SetFloat("_PupilSize", rightPupilSizeTarget);

            
            leftEyeMaterial.SetFloat("_EyePosX", leftGazeTarget.x);
            leftEyeMaterial.SetFloat("_EyePosY", leftGazeTarget.y);
            leftEyeMaterial.SetFloat("_PupilSize",  leftPupilSizeTarget);
        }

        private void OnEyeTracking(EyeTracking eyeTracking)
        {
            if(eyeTracking != null){
                //nongazeLeft = new Vector2(eyeTracking.LeftEye.PupilPosition.X, eyeTracking.LeftEye.PupilPosition.Y);
                rightGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y) ;
                leftGazeTarget = new Vector2(eyeTracking.CombinedGaze.X, -eyeTracking.CombinedGaze.Y) ;
                rightPupilSizeTarget = eyeTracking.RightEye.PupilDilation/10f;
                leftPupilSizeTarget = eyeTracking.LeftEye.PupilDilation / 10f;
            }
        }

        private Vector2 nongazeLeft;
        private Vector2 rightEyeTarget;
        private Vector2 leftEyeTarget;
        public void EyeCoords(EyeTracking eyeTracking)
        {
            rightEyeTarget = new Vector2(eyeTracking.RightEye.PupilPosition.X, -eyeTracking.RightEye.PupilPosition.Y);
            leftEyeTarget = new Vector2(eyeTracking.LeftEye.PupilPosition.X, -eyeTracking.LeftEye.PupilPosition.Y);

        }
    }
}