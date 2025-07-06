using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RateConfig))]
public class RateConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        RateConfig config = (RateConfig)target;

        float total = 0f;
        foreach (var entry in config.entries)
        {
            total += entry.rate;
        }

        EditorGUILayout.LabelField("🎯 Total Rate: " + total.ToString("F2") + "%", EditorStyles.boldLabel);

        if (total > 100f)
        {
            EditorGUILayout.HelpBox("Le total dépasse 100% !", MessageType.Warning);
        }
        else if (total < 100f)
        {
            EditorGUILayout.HelpBox("Le total est inférieur à 100%.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Total parfait 👌", MessageType.None);
        }

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
