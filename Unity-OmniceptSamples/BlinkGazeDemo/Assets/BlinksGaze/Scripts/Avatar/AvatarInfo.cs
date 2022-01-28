using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarInfo
{
    [Header("Model")]
    public GameObject model;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Bones")]
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject head;
    public GameObject bodyBase;

    private Dictionary<string, int> BlendShapeDict;

    private void GetBlendShapeNames()
    {
        if (skinnedMeshRenderer == null) return;

        BlendShapeDict = new Dictionary<string, int>();

        SkinnedMeshRenderer head = skinnedMeshRenderer;
        Mesh m = head.sharedMesh;
        for (int i = 0; i < m.blendShapeCount; i++)
        {
            string name = m.GetBlendShapeName(i);
            //print("Blend Shape: " + i + " " + name); // Blend Shape: 4 FightingLlamaStance

            BlendShapeDict[name] = i;
        }
    }

    public void SetBlendShapeWeight(string blendShapeName, float value)
    {
        if (BlendShapeDict == null)
            GetBlendShapeNames();

        int index = BlendShapeDict[blendShapeName];
        skinnedMeshRenderer.SetBlendShapeWeight(index, value);
    }
}
