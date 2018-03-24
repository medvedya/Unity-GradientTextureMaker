﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GradientTextureGeneration
{
    [CreateAssetMenu]
    public class GradientTextureMaker : ScriptableObject
    {

        [SerializeField]
        private Gradient gradient = new Gradient();
        [SerializeField]
        private int width = 256;
        [SerializeField]
        private int height = 2;
        [SerializeField]
        Texture2D targetTexture;
        public Texture2D Make()
        {
            var t2d = new Texture2D(width, height);
            for (int i = 0; i < width; i++)
            {
                var c = gradient.Evaluate(((float)i) / width);
                for (int j = 0; j < height; j++)
                {
                    t2d.SetPixel(i, j, c);
                }
            }
            t2d.Apply();
            return t2d;
        }
        [ContextMenu("Apply")]
        public void Apply()
        {
            string path;
            if (targetTexture == null)
            {
                var assetPath = AssetDatabase.GetAssetPath(this);
                path = assetPath.Substring(0, assetPath.Length - 5) + "png";
            }
            else
            {
                path = AssetDatabase.GetAssetPath(targetTexture);
            }
            var tex = Make();
            var data =  tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, data);
            AssetDatabase.ImportAsset(path);
            DestroyImmediate(tex);
            targetTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
    [CustomEditor(typeof(GradientTextureMaker))]
    public class GradientTextureMakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gradient"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTexture"));
            if (GUILayout.Button("Apply"))
            {
                foreach (var item in targets)
                {
                    (item as GradientTextureMaker).Apply();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
