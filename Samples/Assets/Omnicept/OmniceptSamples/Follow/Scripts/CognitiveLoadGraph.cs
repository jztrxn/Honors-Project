// (c) Copyright 2020 HP Development Company, L.P.

using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using HP.Glia.Examples.Common;

namespace HP.Glia.Examples.Follow
{
    public class CognitiveLoadGraph : UIGraph
    {
        public TextMeshProUGUI AverageText;
        public TextMeshProUGUI ResultText;

        public bool showSD = true;


        public void ShowResult(bool roundPassed, CognitiveLoadMonitor clMonitor)
        {
            ShowResult(clMonitor);
            ResultText.gameObject.SetActive(true);
            AverageText.text = $"AVG: {clMonitor.GetMean()}";
            ResultText.text = roundPassed ? "Success" : "Failure";
            ResultText.color = roundPassed ? Color.green : Color.red;
        }

        public void ShowResult(CognitiveLoadMonitor clMonitor)
        {
            ResultText.gameObject.SetActive(false);
            Data = clMonitor.GetData();
            if (showSD)
            {
                SD = clMonitor.GetSDData();
            }

            Draw();
        }
    }


}