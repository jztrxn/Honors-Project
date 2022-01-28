using System.Collections;
using System.Collections.Generic;
using HP.Glia.Examples.Follow;
using UnityEngine.UI;
using UnityEngine.Events;

using UnityEngine;
using HP.Glia.Examples.Common;

namespace  HP.Glia.Examples.Display{
    public class TwoDGraph : UGUIBaseDisplay
    {
        private Material cogLoadMaterial;
        public bool showRawValue = false;
        public SDGraph graphing;
        public CognitiveLoadMonitor ClMonitor;
        public bool clearButton = false;

        public UnityEvent OnGameStart;
        public UnityEvent OnGameOver;


        void Start()
        {
            ClMonitor.StartMonitoring();
            OnGameStart.Invoke();
        }


        void Update()
        {

            graphing.ShowResult(ClMonitor);
            if (clearButton)
            {
                ClearGraph();
            }

        }
        public void ClearGraph()
        {
            ClMonitor.clearData();
            clearButton = false;
            OnGameOver.Invoke();
        }

    

    }
}