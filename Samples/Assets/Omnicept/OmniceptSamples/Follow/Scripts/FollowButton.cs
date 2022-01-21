// (c) Copyright 2020 HP Development Company, L.P.

using System;
using System.Collections;
using HP.Glia.Examples.Common;
using UnityEngine;

namespace HP.Glia.Examples.Follow
{
    public class FollowButton : ButtonInteractable {
        public Color color;
        private AudioSource audioSource;
        public int id;
        private Material _material;

        private Material material {
            get{
                if(_material == null){
                    _material = GetComponent<MeshRenderer>().material; //Creates a copy of the material
                }

                return _material;
            }
        }

        private Action<int> onClick;

        private bool interactable = false;

        public void SetData(int id, Color color)
        {
            this.color = color;
            this.id = id;
            
            material.color = color;
            material.DisableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color);
        }

        public void TurnOn(){
            material.EnableKeyword("_EMISSION");
            this.audioSource = GetComponent<AudioSource>();
            audioSource.Play();

        }

        public void TurnOff(){
            material.DisableKeyword("_EMISSION");
            //audioSource.Stop();
        }

        public void SetInteractable(bool interactable, Action<int> onClick = null)
        {
            TurnOff();
            StopAllCoroutines();
            this.interactable = interactable;
            this.onClick = onClick;
        }

        [ContextMenu("Click")]
        public void Click(){
            if(interactable && onClick != null){
                StartCoroutine(ClickFeedback());
            }
        }

        private IEnumerator ClickFeedback()
        {
            interactable = false;
            TurnOn();
            yield return new WaitForSeconds(0.5f);
            TurnOff();
            interactable = true;
            onClick.Invoke(id);
        }


        void Awake()
        {
            OnButtonPressed.AddListener(Click);
        }
        
        private void Update() {
            if(Input.GetKeyDown( (id+1).ToString())){
                Click();
            }
        }
    }
}