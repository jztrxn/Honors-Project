using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Unity;

namespace HP.Glia.Examples.Display
{
    public class EyesScript : UGUIBaseDisplay
    {
        private Vector2 leftGazeTarget;
        private float leftPupilSizeTarget = 0.5f;
        private Vector2 rightGazeTarget;
        private float rightPupilSizeTarget = 0.5f;

        // Start is called before the first frame update
        void Start()
        {
            //gliaBehaviour.OnEyeTracking.AddListener(OnEyeTracking);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

