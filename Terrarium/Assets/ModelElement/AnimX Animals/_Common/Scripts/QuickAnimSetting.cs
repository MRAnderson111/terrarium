using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuickAnimSetting : EditorWindow
{

    ClipSettings[] clipSettings = new ClipSettings[255];
    ModelImporter lastSelected;

    private Vector2 scrollPos;

    [MenuItem("Window/Quick Anim Setting")]
    static void Init()
    {
        QuickAnimSetting window = ScriptableObject.CreateInstance<QuickAnimSetting>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.Show();
    }

    [Serializable]
    struct ClipSettings
    {
        public bool loopTime;
        public bool loopPose;
        public bool lockRootHeightY;
    }

    void OnSelectionChange()
    {
        Repaint();
    }

    void OnGUI()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        ModelImporter model = AssetImporter.GetAtPath((AssetDatabase.GetAssetPath(obj.GetInstanceID()))) as ModelImporter;

        if (model == null)
        {
            lastSelected = null;
            return;
        }

        var clips = model.clipAnimations;
        if (clips.Length == 0)
        {
            clips = model.defaultClipAnimations;
        }

        if (lastSelected != model)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                clipSettings[i].loopTime = clips[i].loopTime;
                clipSettings[i].loopPose = clips[i].loopPose;
                clipSettings[i].lockRootHeightY = clips[i].lockRootHeightY;
            }
            lastSelected = model;
        }
        ClipsToggle(clips);
        

        ClipsButton(clips);

        EditorGUILayout.BeginHorizontal(); GUILayout.Space(wigthName);
        if (GUILayout.Button("Change Animation Settings", GUILayout.Width(wigthToggle*3)))
        {
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].loopTime = clipSettings[i].loopTime;
                clips[i].loopPose = clipSettings[i].loopPose;
                clips[i].lockRootHeightY = clipSettings[i].lockRootHeightY;
            }
            model.clipAnimations = clips;
            model.SaveAndReimport();
        }
        GUILayout.FlexibleSpace(); EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(); GUILayout.Space(wigthName);
        if (GUILayout.Button("Change all to Loop (wrap mode)", GUILayout.Width(300)))
        {
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].wrapMode = WrapMode.Loop;
            }
            model.clipAnimations = clips;
            model.SaveAndReimport();
        }
        GUILayout.FlexibleSpace(); EditorGUILayout.EndHorizontal();
    }
    int wigthToggle = 100;
    int wigthName = 210;
    void ClipsToggle(ModelImporterClipAnimation[] clips)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Clip Name", EditorStyles.boldLabel, GUILayout.Width(wigthName));

        EditorGUILayout.LabelField("Loop Time", EditorStyles.boldLabel, GUILayout.Width(wigthToggle));
        EditorGUILayout.LabelField("Loop Pose", EditorStyles.boldLabel, GUILayout.Width(wigthToggle));
        EditorGUILayout.LabelField("lock Root Height Y", EditorStyles.boldLabel, GUILayout.Width(wigthToggle));

        EditorGUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(600));
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < clips.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(clips[i].name, GUILayout.Width(wigthName));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(wigthToggle));
            clipSettings[i].loopTime = EditorGUILayout.Toggle(clipSettings[i].loopTime);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(wigthToggle));
            clipSettings[i].loopPose = EditorGUILayout.Toggle(clipSettings[i].loopPose);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(wigthToggle));
            clipSettings[i].lockRootHeightY = EditorGUILayout.Toggle(clipSettings[i].lockRootHeightY);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
        }

        GUI.EndScrollView();
        GUILayout.EndScrollView();
    }
    void ClipsButton(ModelImporterClipAnimation[] clips)
    {

        

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(wigthName);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("loopTime", EditorStyles.boldLabel, GUILayout.Width(wigthToggle));
        if (GUILayout.Button("Check All", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].loopTime = true;
        if (GUILayout.Button("Check None", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].loopTime = false;
        if (GUILayout.Button("Invert All", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].loopTime = !clipSettings[i].loopTime;
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("loopPose", EditorStyles.boldLabel, GUILayout.Width(wigthToggle));
        if (GUILayout.Button("Check All", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].loopPose = true;
        if (GUILayout.Button("Check None", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].loopPose = false;
        if (GUILayout.Button("Invert All", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].loopPose = !clipSettings[i].loopPose;

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("lockRootHeightY", EditorStyles.boldLabel, GUILayout.Width(wigthToggle));
        if (GUILayout.Button("Check All", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].lockRootHeightY = true;
        if (GUILayout.Button("Check None", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].lockRootHeightY = false;
        if (GUILayout.Button("Invert All", GUILayout.Width(wigthToggle)))
            for (int i = 0; i < clips.Length; i++)
                clipSettings[i].lockRootHeightY = !clipSettings[i].lockRootHeightY;

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}
