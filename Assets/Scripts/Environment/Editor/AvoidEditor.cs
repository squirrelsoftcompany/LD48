using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AvoidEditor : EditorWindow
{
    public Terrain m_terrain;
    public GameObject m_prefabMapCenter;
    public GameObject m_prefabAvoid;
    public Vector3 m_stepsRelief;
    public Vector3 m_stepsBounds;

    public bool m_top = true, m_bottom = false;
    public bool m_front = true, m_back = true;
    public bool m_right = true, m_left = true;

    public Vector2 m_scrollPosition;

    [MenuItem("Window/Avoid Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AvoidEditor window = (AvoidEditor)GetWindow(typeof(AvoidEditor));
        window.name = "Avoid Editor";
        window.position = new Rect(window.position.position, new Vector2(320, 430));
        window.Show();
    }

    void OnGUI()
    {
        var mainLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        mainLabelStyle.fontStyle = FontStyle.Bold;
        var smallLabelStyle = new GUIStyle(GUI.skin.label);
        smallLabelStyle.fontStyle = FontStyle.Bold;
        var blueprintStyle = new GUIStyle(GUI.skin.button);
        var spacerStyle = new GUIStyle(GUI.skin.label);
        var blueprintWidth = 65;

        m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
        EditorGUILayout.BeginVertical();
        {
            PutErrorColor(m_terrain);
            m_terrain = EditorGUILayout.ObjectField("Terrain", m_terrain, typeof(Terrain), true) as Terrain;
            ResetErrorColor();
            Vector3 lTerrainSize = m_terrain ? m_terrain.terrainData.size : Vector3.positiveInfinity;

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("MAP CENTER", mainLabelStyle);
            PutErrorColor(m_prefabMapCenter);
            m_prefabMapCenter = EditorGUILayout.ObjectField("Prefab", m_prefabMapCenter, typeof(GameObject), false) as GameObject;
            ResetErrorColor();
            GUI.enabled = m_terrain != null && m_prefabMapCenter != null;
            if (GUILayout.Button("Generate Map Center"))
            {
                Instantiate(m_prefabMapCenter, m_terrain.transform, lTerrainSize / 2);
            }
            GUI.enabled = true;

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("AVOID", mainLabelStyle);
            PutErrorColor(m_prefabAvoid);
            m_prefabAvoid = EditorGUILayout.ObjectField("Avoid", m_prefabAvoid, typeof(GameObject), false) as GameObject;
            ResetErrorColor();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Relief", smallLabelStyle);
            PutErrorColor(m_stepsRelief);
            m_stepsRelief = EditorGUILayout.Vector3Field("Steps", m_stepsRelief);
            ResetErrorColor();
            GUI.enabled = m_terrain != null && m_prefabAvoid != null && ValidVector(m_stepsRelief);
            if (GUILayout.Button("Generate Avoid Relief"))
            {
                InstantiateRelief(m_terrain.transform, m_stepsRelief, lTerrainSize);
            }
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bounds", smallLabelStyle);
            PutErrorColor(m_stepsBounds);
            m_stepsBounds = EditorGUILayout.Vector3Field("Steps", m_stepsBounds);
            ResetErrorColor();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Faces", GUILayout.Width(60));
                GUILayout.Label("", spacerStyle);
                GUI.color = Color.grey;
                EditorGUILayout.LabelField("released==activated", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight }, GUILayout.Width(120));
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("", spacerStyle);
                GUILayout.Label("", spacerStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle, GUILayout.Width(blueprintWidth));
                m_top = !GUILayout.Toggle(!m_top, "TOP", blueprintStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("", spacerStyle);
                m_back = !GUILayout.Toggle(!m_back, "BACK", blueprintStyle, GUILayout.Width(blueprintWidth));
                m_left = !GUILayout.Toggle(!m_left, "LEFT", blueprintStyle, GUILayout.Width(blueprintWidth));
                m_front = !GUILayout.Toggle(!m_front, "FRONT", blueprintStyle, GUILayout.Width(blueprintWidth));
                m_right = !GUILayout.Toggle(!m_right, "RIGHT", blueprintStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("", spacerStyle);
                GUILayout.Label("", spacerStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle, GUILayout.Width(blueprintWidth));
                m_bottom = !GUILayout.Toggle(!m_bottom, "BOTTOM", blueprintStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle, GUILayout.Width(blueprintWidth));
                GUILayout.Label("", spacerStyle);
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = m_terrain != null && m_prefabAvoid != null && ValidVector(m_stepsBounds)
                && (m_top || m_bottom || m_front || m_back || m_right || m_left) == true;
            if (GUILayout.Button("Generate Avoid Bound"))
            {
                InstantiateBounds(m_terrain.transform, m_stepsBounds, lTerrainSize);
            }
            GUI.enabled = true;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    // WINDOW UTILS

    void PutErrorColor(Object o)
    {
        GUI.backgroundColor = o ? Color.white : Color.red;
    }

    void PutErrorColor(Vector3 vec3)
    {
        GUI.backgroundColor = ValidVector(vec3) ? Color.white : Color.red;
    }

    bool ValidVector(Vector3 vec3)
    {
        return vec3.x != 0 && vec3.y != 0 && vec3.z != 0;
    }

    void ResetErrorColor()
    {
        GUI.backgroundColor = Color.white;
    }

    // INSTANTIATOR
    
    void InstantiateRelief(Transform parent, Vector3 steps, Vector3 size)
    {
        Vector3 origin = size / 2;
        GameObject wallBottom = InstantiateAvoidWall(origin, parent, new Vector2(steps.x, steps.z), new Vector2(size.x, size.z), size.y / 2, "Bottom");

        // apply height
        Vector3 lTerrainPosition = m_terrain.GetPosition();
        foreach (Transform avoid in wallBottom.transform)
        {
            float h = m_terrain.SampleHeight(avoid.position);
            avoid.position += Vector3.up * h;
            Vector3 N = m_terrain.terrainData.GetInterpolatedNormal((avoid.localPosition.x + origin.x) / size.x, (avoid.localPosition.y + origin.y) / size.y);
            avoid.LookAt(avoid.position + N);
        }
    }

    void InstantiateBounds(Transform parent, Vector3 steps, Vector3 size)
    {
        Vector3 origin = size/2;
        if (m_bottom)
        {
            GameObject wallBottom = InstantiateAvoidWall(origin, parent, new Vector2(steps.x, steps.z), new Vector2(size.x, size.z), size.y / 2, "Bottom");
        }

        if (m_top)
        {
            GameObject wallTop = InstantiateAvoidWall(origin, parent, new Vector2(steps.x, steps.z), new Vector2(size.x, size.z), size.y / 2, "Top");
            wallTop.transform.localRotation = Quaternion.Euler(180, 0, 0);
        }

        if (m_right)
        {
            GameObject wallRight = InstantiateAvoidWall(origin, parent, new Vector2(steps.y, steps.z), new Vector2(size.y, size.z), size.x / 2, "Right");
            wallRight.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }

        if (m_left)
        {
            GameObject wallLeft = InstantiateAvoidWall(origin, parent, new Vector2(steps.y, steps.z), new Vector2(size.y, size.z), size.x / 2, "Left");
            wallLeft.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        if (m_front)
        {
            GameObject wallFront = InstantiateAvoidWall(origin, parent, new Vector2(steps.x, steps.y), new Vector2(size.x, size.y), size.z / 2, "Front");
            wallFront.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }

        if (m_back)
        {
            GameObject wallBack = InstantiateAvoidWall(origin, parent, new Vector2(steps.x, steps.y), new Vector2(size.x, size.y), size.z / 2, "Back");
            wallBack.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }

    GameObject InstantiateAvoidWall(Vector3 origin, Transform parent, Vector2 steps, Vector2 size, float heightOffset, string nameSuffix)
    {
        GameObject wall = new GameObject("Wall" + nameSuffix);
        wall.transform.parent = parent;
        wall.transform.localPosition = origin;
        for (float x = -size.x/2; x < size.x/2; x+=steps.x)
        {
            for (float y = -size.y / 2; y < size.y / 2; y += steps.y)
            {
                GameObject avoid = Instantiate(m_prefabAvoid, wall.transform, new Vector3(x, -heightOffset, y));
            }
        }
        return wall;
    }

    GameObject Instantiate(GameObject prefab, Transform parent, Vector3 localPosition)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        go.transform.localPosition = localPosition;
        go.transform.LookAt(parent);
        return go;
    }
}
