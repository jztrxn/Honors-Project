
using UnityEngine;
using HP.Omnicept.Unity;
using System;
using UnityEngine.UI;

namespace  HP.Glia.Examples.Display{
    [RequireComponent(typeof(Image))]
    public class UGUIGliaStatus : UGUIBaseDisplay
    {
        public Color connectingColor = Color.yellow;
        public Color connectedColor = Color.blue;
        public Color disconnectedColor = Color.red;

        void Start()
        {
            GetComponent<Image>().color = connectingColor;
            gliaBehaviour.OnConnect.AddListener(OnConnect);
            gliaBehaviour.OnDisconnect.AddListener(OnDisconnect);
            gliaBehaviour.OnConnectionFailure.AddListener( e => { GetComponent<Image>().color = Color.red; });
        }

        private void OnConnect(GliaBehaviour arg0)
        {
            GetComponent<Image>().color = connectedColor;
        }

        private void OnDisconnect(string arg0)
        {
            GetComponent<Image>().color = disconnectedColor;
        }
    }
}