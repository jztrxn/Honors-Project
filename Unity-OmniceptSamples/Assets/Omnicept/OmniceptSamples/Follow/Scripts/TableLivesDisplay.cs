// (c) Copyright 2020 HP Development Company, L.P.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HP.Glia.Examples.Follow
{
    [RequireComponent(typeof(AudioSource))]
    public class TableLivesDisplay : MonoBehaviour, ILivesDisplay
    {
        [SerializeField] private List<GameObject> Lives = new List<GameObject>();
        private AudioSource _audioSource = null;
        private AudioSource m_audioSource {
            get {
                if(_audioSource == null){
                    _audioSource = GetComponent<AudioSource>();
                }

                return _audioSource;
            }
        }

        public void SetRemainingLives(int remaingLives)
        {
            for(int l = 0; l < Lives.Count; l++){
                Lives[l].SetActive(l < remaingLives);
            }
            
            m_audioSource.Play();
        }
    }
}
