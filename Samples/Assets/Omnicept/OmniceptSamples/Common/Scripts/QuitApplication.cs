// (c) Copyright 2020 HP Development Company, L.P.

using UnityEngine;
using NetMQ;
using HP.Omnicept.Unity;

namespace HP.Glia.Examples.Common
{
    public class QuitApplication : MonoBehaviour {
        public KeyCode quitKey = KeyCode.Escape;

        private void Update() {
            if(Input.GetKeyDown(quitKey)){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
            }
        }

    }
}