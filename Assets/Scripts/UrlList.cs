
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

#if UNITY_EDITOR
using System.Collections.Generic;
using UdonSharpEditor;
using UnityEditor;

#endif

public class UrlList : UdonSharpBehaviour
{
    public VRCUrl[] urls;
}

#if UNITY_EDITOR

[CustomEditor(typeof(UrlList))]
public class AccessControlUserListInspector : Editor
{
    SerializedProperty urlListProperty;

    string bulkText = "";
    // static bool showUserFoldout = true;

    private void OnEnable()
    {
        urlListProperty = serializedObject.FindProperty(nameof(UrlList.urls));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
            return;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Bulk Operations", EditorStyles.boldLabel);

        bulkText = EditorGUILayout.TextArea(bulkText, GUILayout.Height(70));
        Rect row = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Add URLs", "Add each URL from newline-separated list to the URL list if the URL is not already present")))
        {
            AppendNames(bulkText);
            bulkText = "";
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button(new GUIContent("Replace Existing", "Clears existing URL list and adds each URL from newline-separated list")))
        {
            ClearList();
            AppendNames(bulkText);
            bulkText = "";
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("URL List", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        int oldCount = urlListProperty.arraySize;
        int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", urlListProperty.arraySize));
        if (newCount != oldCount)
        {
            for (int i = oldCount; i < newCount; i++)
            {
                urlListProperty.InsertArrayElementAtIndex(i);
                SerializedProperty prop = urlListProperty.GetArrayElementAtIndex(i);
                prop.stringValue = "";
            }

            serializedObject.ApplyModifiedProperties();
        }

        for (int i = 0; i < urlListProperty.arraySize; i++)
        {
            SerializedProperty prop = urlListProperty.GetArrayElementAtIndex(i);
            Rect row2 = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prop, new GUIContent($"Element {i}"));
            if (GUILayout.Button(new GUIContent("X", "Remove Element"), GUILayout.Width(30)))
                RemoveName(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Element", GUILayout.Width(120)))
            AddElement();
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }

    void ClearList()
    {
        urlListProperty.ClearArray();
    }

    void AppendNames(string text)
    {
        UrlList data = serializedObject.targetObject as UrlList;
        if (data.urls == null)
            data.urls = new VRCUrl[0];

        HashSet<string> existing = new HashSet<string>();
        for (int i = 0; i < data.urls.Length; i++)
        {
            if (data.urls[i] == null || data.urls[i] == VRCUrl.Empty)
                continue;

            string name = data.urls[i].Get();
            if (name == null || name.Length == 0)
                continue;

            existing.Add(name);
        }

        string[] names = text.Split('\n');
        List<string> toAdd = new List<string>();
        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i].Trim();
            if (name.Length == 0)
                continue;

            if (existing.Contains(name))
                continue;

            toAdd.Add(name);
        }

        data.urls = new VRCUrl[existing.Count + toAdd.Count];
        string[] existingNames = new string[existing.Count];
        existing.CopyTo(existingNames);

        for (int i = 0; i < existingNames.Length; i++)
            data.urls[i] = new VRCUrl(existingNames[i]);
        for (int i = 0; i < toAdd.Count; i++)
            data.urls[existingNames.Length + i] = new VRCUrl(toAdd[i]);

        UdonSharpEditorUtility.CopyProxyToUdon(data);
    }

    void RemoveName(int index)
    {
        if (index < 0 || index >= urlListProperty.arraySize)
            return;

        urlListProperty.DeleteArrayElementAtIndex(index);
    }

    void AddElement()
    {
        int next = urlListProperty.arraySize;
        urlListProperty.InsertArrayElementAtIndex(next);
        SerializedProperty prop = urlListProperty.GetArrayElementAtIndex(next);
    }
}

#endif
