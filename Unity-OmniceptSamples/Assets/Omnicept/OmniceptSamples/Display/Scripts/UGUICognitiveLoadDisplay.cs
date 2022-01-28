// (c) Copyright 2020 HP Development Company, L.P.

using HP.Omnicept.Messaging.Messages;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace HP.Glia.Examples.Display
{
    public class UGUICognitiveLoadDisplay : UGUIBaseDisplay
    {
        public TextMeshProUGUI cognitiveLoadText;
        private Material cogLoadMaterial;
        public Image cognitiveLoad;
        public Image noCLDataWarning;
        public bool showRawValue = false;
        private float targetCLConfidence;
        private float currentCL = 0f;
        private float targetCL = 0;
        
        void Start()
        {
            gliaBehaviour.OnCognitiveLoad.AddListener(OnCognitiveLoad);
            cogLoadMaterial = cognitiveLoad.material;
        }

        private void OnCognitiveLoad(CognitiveLoad cl)
        {
            if(cl != null){
                targetCL = cl.CognitiveLoadValue;
                targetCLConfidence = cl.StandardDeviation;

                if(!showRawValue){
                    if(cl.CognitiveLoadValue > 0.66){
                        cognitiveLoadText.text = "High";
                    }
                    else if(cl.CognitiveLoadValue > 0.33){
                        cognitiveLoadText.text = "Med";
                    }
                    else if(cl.CognitiveLoadValue > 0){
                        cognitiveLoadText.text = "Low";
                    }
                    else{
                        cognitiveLoadText.text = "N/A";
                    }
                }
                else{
                    cognitiveLoadText.text = targetCL.ToString("0.00");
                }
            }
        
            noCLDataWarning.gameObject.SetActive(cl == null);
        }

        void Update()
        {
            currentCL = Mathf.Lerp(currentCL, targetCL, 15f * Time.deltaTime);
            cogLoadMaterial.SetFloat("_FillAmmount", currentCL );

            var cl = gliaBehaviour.GetLastCognitiveLoad();
        }
    }
}