// (c) Copyright 2021 HP Development Company, L.P.

using System;
using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace HP.Glia.Examples.Display
{
    public class FaceCameraDisplay : Graphic {
        private GliaBehaviour _gliaBehaviour;
        protected GliaBehaviour gliaBehaviour{
            get{
                if(_gliaBehaviour == null){
                    _gliaBehaviour = FindObjectOfType<GliaBehaviour>();
                }

                return _gliaBehaviour;
            }
        }


        public override Texture mainTexture
        {
            get
            {
                return cameraTexture;
            }
        }

        private Texture2D cameraTexture;

        public void Start()
        {
            base.Start();

            gliaBehaviour.OnCameraImage.AddListener(OnCameraImage);
            cameraTexture  =  new Texture2D(400, 400, TextureFormat.R8, false);
        }

        private void OnCameraImage(CameraImage cameraImage)
        {
            if (cameraImage != null && cameraImage.SensorInfo.Location.Contains("Mouth"))
            {
                cameraTexture.LoadRawTextureData(cameraImage.ImageData);
                cameraTexture.Apply();
            }

        }
    }
}