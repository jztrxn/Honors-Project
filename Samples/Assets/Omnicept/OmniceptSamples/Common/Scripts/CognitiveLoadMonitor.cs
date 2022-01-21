// (c) Copyright 2020 HP Development Company, L.P.

using System.Collections.Generic;
using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Unity;
using UnityEngine;

namespace HP.Glia.Examples.Common
{
    //Monitor component for CL
    public class CognitiveLoadMonitor : MonoBehaviour {

        private GliaBehaviour _gliaBehaviour;
        private GliaBehaviour m_gliaBehaviour {
            get{
                if(_gliaBehaviour == null){
                    _gliaBehaviour = FindObjectOfType<GliaBehaviour>();
                }

                return _gliaBehaviour;
            }
        }

        
        private float m_cogLoadAcum = 0;
        
        [SerializeField] private List<float> m_cogLoadSamples = new List<float>(200);
        [SerializeField] private List<float> m_standardDeviation = new List<float>(200);

        
        List<float> cl;
        List<float> sd;

        public int sampleLimit =200; 

        bool shouldClear = false;
        bool resizing = false;

        public void StartMonitoring()
        {
            if(!resizing){
                m_cogLoadAcum = 0; 
                m_cogLoadSamples.Clear();
                m_standardDeviation.Clear();

                m_gliaBehaviour.OnCognitiveLoad.RemoveListener(RegisterCognitiveLoad);
                m_gliaBehaviour.OnCognitiveLoad.AddListener(RegisterCognitiveLoad);
            }
            else{
                shouldClear = true;
            }
        }


  

        public void StopMonitoring()
        {
            m_gliaBehaviour.OnCognitiveLoad.RemoveListener(RegisterCognitiveLoad);
        }

        public float GetMean(){
            float mean = (float) m_cogLoadAcum / (float) m_cogLoadSamples.Count;

            return float.IsNaN(mean) ? 0 : mean;
        }

     
        public ref List<float> GetData(){
            return ref  m_cogLoadSamples;
        } 

        public ref  List<float> GetSDData(){
            return ref  m_standardDeviation;
        }

        public void clearData()
        {
            m_cogLoadAcum = 0;
            m_cogLoadSamples.Clear();
            m_standardDeviation.Clear();
        }

        public void RegisterCognitiveLoad(CognitiveLoad cognitiveLoad)
        {           
            m_cogLoadAcum +=  cognitiveLoad.CognitiveLoadValue;
            m_cogLoadSamples.Add(cognitiveLoad.CognitiveLoadValue);
            m_standardDeviation.Add(cognitiveLoad.StandardDeviation);

            if(m_cogLoadSamples.Count == sampleLimit){
                resizing = true; 

               cl = new List<float>((int) sampleLimit/2);
               sd = new List<float>((int) sampleLimit/2);

                for(int i = 0; i < (int) sampleLimit/2; i++){
                    cl.Add( (m_cogLoadSamples[i*2] +  m_cogLoadSamples[i*2 + 1])/ 2.0f);
                    sd.Add( (m_standardDeviation[i*2] +  m_standardDeviation[i*2 + 1])/ 2.0f );
                }

               m_cogLoadSamples = cl;
               m_standardDeviation = sd;

               sd = cl = null;

               resizing = false;

               if(shouldClear){
                   shouldClear = false;
                   StartMonitoring();
               }
            }
        }
    }
}