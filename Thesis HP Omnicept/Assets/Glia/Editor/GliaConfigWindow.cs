// (c) Copyright 2020-2021 HP Development Company, L.P.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Messaging;

namespace HP.Omnicept.Unity
{
    public class GliaConfigWindow : EditorWindow
    {
        protected GliaSettings m_settings;
        protected Editor m_settingsEditor;

        [MenuItem("HP Omnicept/Configure")]
        public static void CreateWindow()
        {
            GliaConfigWindow window = (GliaConfigWindow)EditorWindow.GetWindow(typeof(GliaConfigWindow), true);
            window.minSize = new Vector2(1000.0f, 175.0f);
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("HP Omnicept");
            m_settings = Resources.Load("GliaSettings") as GliaSettings;

            if(m_settings == null)
            {
                m_settings = CreateInstance<GliaSettings>();
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateAsset(m_settings, "Assets/Resources/GliaSettings.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void OnDisable()
        {
            if(m_settingsEditor != null)
            {
                DestroyImmediate(m_settingsEditor);
            }
        }

        void OnGUI()
        {
            if (m_settingsEditor == null)
            {
                m_settingsEditor = Editor.CreateEditor(m_settings) as GliaSettingsEditor;
            }
            m_settingsEditor.OnInspectorGUI();

            if(GUILayout.Button("Ok"))
            {
               Close();
            }
        }
    }

    [CustomEditor(typeof(GliaSettings))]
    public class GliaSettingsEditor : Editor
    {
        string clientIDField = "";
        string accessKeyField = "";
        bool subscriptionFoldout = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GliaSettings gliaSettings = (GliaSettings) target;

            clientIDField = EditorGUILayout.PasswordField(new GUIContent("Client ID", gliaSettings.ClientID), gliaSettings.ClientID);
            accessKeyField = EditorGUILayout.PasswordField(new GUIContent("Access Key", gliaSettings.AccessKey), gliaSettings.AccessKey);

            subscriptionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(subscriptionFoldout, "Subscriptions");
            Rect r = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Event", GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Sender", GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("ID", GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Sub ID", GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Location", GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Version Semantic", GUILayout.MinWidth(100));
            EditorGUILayout.EndHorizontal();
            foreach(var subItem in gliaSettings.UnitySubscriptions)
            {
                addSubscriptionField(subItem);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUI.changed)
            {
                gliaSettings.ClientID = clientIDField.Trim();
                gliaSettings.AccessKey = accessKeyField.Trim();
                EditorUtility.SetDirty(target);
            }
        }
        public static void addSubscriptionField(SubscriptionMenuItem menuItem)
        {
            Rect r = EditorGUILayout.BeginHorizontal();
            menuItem.m_enabled = EditorGUILayout.ToggleLeft(menuItem.m_label, menuItem.m_enabled, GUILayout.MinWidth(100));
            menuItem.m_sender = EditorGUILayout.TextField(menuItem.m_sender, GUILayout.MinWidth(100));
            menuItem.m_id = EditorGUILayout.TextField(menuItem.m_id, GUILayout.MinWidth(100));
            menuItem.m_subid = EditorGUILayout.TextField(menuItem.m_subid, GUILayout.MinWidth(100));
            menuItem.m_location = EditorGUILayout.TextField(menuItem.m_location, GUILayout.MinWidth(100));
            menuItem.m_versionSemantic = EditorGUILayout.TextField(menuItem.m_versionSemantic, GUILayout.MinWidth(100));
            EditorGUILayout.EndHorizontal();
        }

    }
}
