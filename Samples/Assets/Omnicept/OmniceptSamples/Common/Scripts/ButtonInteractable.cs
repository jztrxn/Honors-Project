// (c) Copyright 2020 HP Development Company, L.P.

using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HP.Glia.Examples.Common
{
    public class ButtonInteractable : MonoBehaviour
    {
        [System.Serializable]
        public class ButtonPressedEvent : UnityEvent { }
        [System.Serializable]
        public class ButtonReleasedEvent : UnityEvent { }

        public Vector3 Axis = new Vector3(0,-1,0 );
        public float MaxDistance;
        public float ReturnSpeed = 10.0f;

        
        public ButtonPressedEvent OnButtonPressed;
        public ButtonReleasedEvent OnButtonReleased;
        
        Vector3 m_StartPosition;
        Rigidbody m_Rigidbody;
        Collider m_Collider;

        [SerializeField] bool m_Pressed = false;

        protected void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponentInChildren<Collider>();
            m_StartPosition = transform.position;
        }

        void FixedUpdate()
        {
            Vector3 worldAxis = transform.TransformDirection(Axis);
            Vector3 end = transform.position + worldAxis * MaxDistance;
            
            float m_CurrentDistance = (transform.position - m_StartPosition).magnitude;
            RaycastHit info;

            float move = 0.0f;

            if (m_Rigidbody.SweepTest(-worldAxis, out info, ReturnSpeed * Time.deltaTime + 0.005f))
            {//hitting something, if the contact is < mean we are pressed, move downward
                move = (ReturnSpeed * Time.deltaTime) - info.distance;
            }
            else
            {
                move -= ReturnSpeed * Time.deltaTime;
            }

            float newDistance = Mathf.Clamp(m_CurrentDistance + move, 0, MaxDistance);

            m_Rigidbody.position = m_StartPosition + worldAxis * newDistance;

            if (!m_Pressed && Mathf.Approximately(newDistance, MaxDistance))
            {//was just pressed
                m_Pressed = true;
                OnButtonPressed.Invoke();
            }
            else if (m_Pressed && !Mathf.Approximately(newDistance, MaxDistance))
            {//was just released
                m_Pressed = false;
                OnButtonReleased.Invoke();
            }
        }
    }
}
