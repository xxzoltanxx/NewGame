using UnityEngine;
using UnityEditor;

public class ModifiableListPropertyField
{
	private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
	private static GUILayoutOption miniButtonHeight = GUILayout.Height(16f);

	public delegate void OnAddAt(int index);
	public delegate void OnRemoveAt(int index);

    public static void Draw(SerializedProperty list, OnAddAt onAddAtCallback, OnRemoveAt onRemoveAtCallback)
    {
		if (!list.isArray)
			return;

        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;

        if (list.isExpanded)
        {
            EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);

            if (GUILayout.Button(new GUIContent("+")))
            {
				if (onAddAtCallback != null) 
					onAddAtCallback(-1);
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                GUILayout.Label(string.Format("Vertex {0}", i));

                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);

                if (GUILayout.Button(new GUIContent("+"), miniButtonWidth, miniButtonHeight))
                {
					if (onAddAtCallback != null)
						onAddAtCallback(i);
                }

                if (GUILayout.Button(new GUIContent("-"), miniButtonWidth, miniButtonHeight))
                {
					if (onRemoveAtCallback != null)
						onRemoveAtCallback(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUI.indentLevel -= 1;
    }
}
