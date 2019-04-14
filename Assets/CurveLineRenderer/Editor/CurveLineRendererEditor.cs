//#define CURVE_LINE_RENDERER_DEBUG
#undef  CURVE_LINE_RENDERER_DEBUG
//#define CURVE_LINE_RENDERER_DEBUG_SHOW_NORMALS
#undef  CURVE_LINE_RENDERER_DEBUG_SHOW_NORMALS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

[CustomEditor(typeof(CurveLineRenderer))]
public class CurveLineRendererEditor : Editor
{
	private const float epsilon = 0.0001f;
	
    private CurveLineRenderer curveLineRenderer;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private MeshFilter handleMeshFilter;
	private MeshCollider handleMeshCollider;

    private float vertexButtonSize = 0.04f;
    private float vertexButtonPickSize = 0.06f;

    private int selectedIndex = -1;

    void OnEnable()
	{
		curveLineRenderer = (CurveLineRenderer)target;

        handleTransform = curveLineRenderer.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? handleTransform.rotation : Quaternion.identity;
        handleMeshFilter = curveLineRenderer.GetComponent<MeshFilter>();
		handleMeshCollider = curveLineRenderer.GetComponent<MeshCollider>();

        SceneView.onSceneGUIDelegate = OnSceneDraw;

		RebuildMesh();
    }

    void OnDisable()
    {
		curveLineRenderer = null;
        SceneView.onSceneGUIDelegate -= OnSceneDraw;
    }

    void OnSceneDraw(SceneView sceneView)
    {
        List<Vector3> vertices = curveLineRenderer.vertices;
        if (vertices.Count == 0)
        {
            return;
        }

        Handles.color = Color.white;

        Vector3 prev = ShowVertex(0);
        for (int i = 1; i < vertices.Count; ++i)
        {
            Vector3 cur = ShowVertex(i);
            Handles.DrawLine(prev, cur);

            prev = cur;
        }
#if (CURVE_LINE_RENDERER_DEBUG_SHOW_NORMALS)
        Mesh mesh = handleMeshFilter.sharedMesh;
        Handles.color = Color.green;
        for (int i = 0; i < mesh.normals.Length; i+=1
            )
        {
            Vector3 startPoint = handleTransform.TransformPoint(mesh.vertices[i]);
            Vector3 normal = mesh.normals[i];
            Handles.DrawLine(startPoint, startPoint + normal);
        }
#endif

        if (GUI.changed)
        {
			if (target != null)
            	EditorUtility.SetDirty(target);

            RebuildMesh();
        }


        Vector3 pos = curveLineRenderer.transform.position;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("meshBuildMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roundedAngle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("normal"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("reverseSideEnabled"));
#if (CURVE_LINE_RENDERER_DEBUG)
        if (GUILayout.Button("Rebuild"))
            GUI.changed = true;
#endif
		ModifiableListPropertyField.Draw(serializedObject.FindProperty("vertices"), OnAddVertexAtIndex, OnRemoveVertexAtIndex);
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
			if (target != null)
            	EditorUtility.SetDirty(target);

            RebuildMesh();
        }
    }

    void RebuildMesh()
    {
        if (handleMeshFilter == null || curveLineRenderer == null)
            return;

        Mesh mesh = handleMeshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Curve Line Mesh";
            handleMeshFilter.sharedMesh = mesh;
        }

        Material[] materials = curveLineRenderer.GetComponent<MeshRenderer>().sharedMaterials;
        for (int i = 0; i < materials.Length; ++i)
        {
            if (materials[i].mainTexture.wrapMode != TextureWrapMode.Repeat)
            {
                Debug.LogWarning("Main texture of " + materials[i].name + " should have 'Repeat' wrap mode");
            }
        };
	    
        curveLineRenderer.Rebuild(mesh, handleMeshCollider);
    }

    Vector3 ShowVertex(int index)
    {
        Vector3 vertex = handleTransform.TransformPoint(curveLineRenderer.vertices[index]);
        float size = HandleUtility.GetHandleSize(vertex);

        if (Handles.Button(vertex, handleRotation, vertexButtonSize * size, vertexButtonPickSize * size, Handles.DotCap))
        {
            selectedIndex = index;
            Repaint();
        }
        Handles.Label(vertex, String.Format("Vertex {0}", index));

        if (selectedIndex != index)
            return vertex;

        EditorGUI.BeginChangeCheck();
        vertex = Handles.DoPositionHandle(vertex, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curveLineRenderer, "Move vertex");
            curveLineRenderer.vertices[index] = handleTransform.InverseTransformPoint(vertex);
        }

        return vertex;
    }

	/**
     * Adding new vertex in line
     */
	void AddVertexAtPosition(int position)
	{
		List<Vector3> vertices = curveLineRenderer.vertices;

		Vector3 point;
		if (position < 0)
		{
			if (vertices.Count > 0) 
				point = vertices[0] - CalculateDirection(position);
			else 
				point = Vector3.zero;
		}
		else if (position >= vertices.Count - 1)
		{
			point = vertices[vertices.Count - 1] + CalculateDirection(position);
		}
		else
		{
			point = (vertices[position] + vertices[position + 1] ) * 0.5f;
		}

		curveLineRenderer.InsertVertex(position + 1, point);
	}
	
	/**
     * Calculating direction for next curve segment
     */
	Vector3 CalculateDirection(int newVertexIndex)
	{
		List<Vector3> vertices = curveLineRenderer.vertices;

		Vector3 direction;
		if (newVertexIndex < 0)
		{
			if (vertices.Count >= 2)
			{
				Vector3 firstPoint = vertices[0];
				int wayVertexIndex = FirstDifferentVertexIndex(1);
				if (wayVertexIndex > 0)
				{
					direction = vertices[wayVertexIndex] - firstPoint;
				}
				else
				{
					direction = NormalOrthogonalVector(curveLineRenderer.normal).normalized;
				}
			}
			else
			{
				direction = NormalOrthogonalVector(curveLineRenderer.normal).normalized;
			}
		}
		else if (newVertexIndex >= vertices.Count - 1)
		{
			if (vertices.Count >= 2)
			{
				Vector3 lastPoint = vertices[vertices.Count - 1];
				int wayVertexIndex = FirstDifferentVertexIndex(-1);
				if (wayVertexIndex >= 0)
				{
					Vector3 pointPrevLast = vertices[wayVertexIndex];
					direction = lastPoint - pointPrevLast;
				}
				else
				{
					direction = NormalOrthogonalVector(curveLineRenderer.normal).normalized;
				}
				
			}
			else
			{
				direction = NormalOrthogonalVector(curveLineRenderer.normal).normalized;
			}	
		}
		else 
		{
			direction = Vector3.zero;
		}

		return direction;
	}
	
	/**
     * First different vertex
     */
	int FirstDifferentVertexIndex(int direction)
	{
		List<Vector3> vertices = curveLineRenderer.vertices;

		if (direction == 0)
		{
			return -1;
		}

		int startIndex = direction > 0 ? 1 : vertices.Count - 2;
		int endIndex = direction > 0 ? vertices.Count : -1;
		Vector3 point = direction > 0 ? vertices[0] : vertices[vertices.Count - 1];
		for (int i = startIndex; i != endIndex; i += direction)
		{
			if (Mathf.Abs(point.x - vertices[i].x) > epsilon ||
			    Mathf.Abs(point.y - vertices[i].y) > epsilon ||
			    Mathf.Abs(point.z - vertices[i].z) > epsilon)
			{
				return i;
			}
		}

		return -1;
	}
	
	/**
     * Vector orthogonal to normal
     */
	Vector3 NormalOrthogonalVector(Vector3 normal)
	{
		Vector3 orthogonalVector;
		if (Mathf.Abs(normal.x) < epsilon || Mathf.Abs(normal.y) < epsilon || Mathf.Abs(normal.z) < epsilon)
		{
			orthogonalVector = new Vector3(Mathf.Abs(normal.x) < epsilon ? 1 : 0,
			                               Mathf.Abs(normal.y) < epsilon ? 1 : 0,
			                               Mathf.Abs(normal.z) < epsilon ? 1 : 0);
		}
		else
		{
			orthogonalVector = new Vector3(0, -normal.z, normal.y);
		}

		return orthogonalVector.normalized;
	}

	void OnAddVertexAtIndex(int index)
	{
		AddVertexAtPosition(index);
	}

	void OnRemoveVertexAtIndex(int index)
	{
		curveLineRenderer.RemoveVertexAt(index);
	}
}
