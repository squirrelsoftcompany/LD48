using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class AvoidEditor : EditorWindow
{
    public List<Terrain> m_terrainComp = null;
    public GameObject m_terrainRoot = null;
    public GameObject m_prefabMapCenter = null;
    public GameObject m_prefabAvoid = null;
    public GameObject m_prefabAvoidWall = null;
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
        window.m_terrainComp = new List<Terrain>();
        window.m_terrainRoot = null;
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
            PutErrorColor(m_terrainComp);
            m_terrainRoot = EditorGUILayout.ObjectField("Terrain Root", m_terrainRoot, typeof(GameObject), true) as GameObject;
            ResetErrorColor();
            m_terrainComp = m_terrainRoot ? m_terrainRoot.GetComponentsInChildren<Terrain>().ToList() : null;
            Bounds lTerrainBounds = DrawTerrainBounds.GetTerrainBounds(m_terrainComp, m_terrainRoot);

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("MAP CENTER", mainLabelStyle);
            PutErrorColor(m_prefabMapCenter);
            m_prefabMapCenter = EditorGUILayout.ObjectField("Prefab", m_prefabMapCenter, typeof(GameObject), false) as GameObject;
            ResetErrorColor();
            GUI.enabled = ValidList(m_terrainComp) && m_prefabMapCenter != null;
            if (GUILayout.Button("Generate Map Center"))
            {
                Instantiate(m_prefabMapCenter, m_terrainRoot.transform, lTerrainBounds.center);
            }
            GUI.enabled = true;

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("AVOID", mainLabelStyle);
            PutErrorColor(m_prefabAvoidWall);
            m_prefabAvoidWall = EditorGUILayout.ObjectField("prefab", m_prefabAvoidWall, typeof(GameObject), false) as GameObject;
            ResetErrorColor();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Relief", smallLabelStyle);
            PutErrorColor(m_stepsRelief);
            m_stepsRelief = EditorGUILayout.Vector3Field("Steps", m_stepsRelief);
            ResetErrorColor();
            GUI.enabled = ValidList(m_terrainComp) && m_prefabAvoidWall != null && ValidVector(m_stepsRelief);
            if (GUILayout.Button("Generate Avoid Relief"))
            {
                InstantiateRelief(m_terrainRoot.transform, m_stepsRelief, lTerrainBounds.center);
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

            GUI.enabled = ValidList(m_terrainComp) && m_prefabAvoidWall != null && ValidVector(m_stepsBounds)
                && (m_top || m_bottom || m_front || m_back || m_right || m_left) == true;
            if (GUILayout.Button("Generate Avoid Bound"))
            {
                InstantiateBounds(m_terrainRoot.transform, m_stepsBounds, lTerrainBounds.size, lTerrainBounds.center);
            }
            GUI.enabled = true;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    // WINDOW UTILS
    void PutErrorColor<T>(List<T> l)
    {
        GUI.backgroundColor = ValidList<T>(l) ? Color.white : Color.red;
    }

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

    bool ValidList<T>(List<T> l)
    {
        return l != null && l.Count > 0;
    }

    void ResetErrorColor()
    {
        GUI.backgroundColor = Color.white;
    }

    // INSTANTIATOR
    
    void InstantiateRelief(Transform parent, Vector3 steps, Vector3 origin)
    {
        // Generate wall
        GameObject wall = Instantiate(m_prefabAvoidWall, parent);
        wall.name += "Relief";
        wall.transform.localPosition = origin;
        wall.transform.localRotation = Quaternion.identity;
        AvoidList list = wall.GetComponent<AvoidList>();

        // for each terrain
        foreach (var terrain in m_terrainComp)
        {
            var size = terrain.terrainData.size;
            var terrainOrigin = terrain.GetPosition();

            // for each step on x and z axis
            for (float x = 0; x < size.x; x += steps.x)
            {
                for (float z = 0; z < size.z; z += steps.z)
                {
                    Vector2 interpolatedPosition = new Vector2(x / size.x, z / size.z);
                    float h = terrain.terrainData.GetInterpolatedHeight(interpolatedPosition.x, interpolatedPosition.y);
                    Vector3 N = terrain.terrainData.GetInterpolatedNormal(interpolatedPosition.x, interpolatedPosition.y);

                    // compute avoidData
                    Behaviour.EnemiesManager.BoidData avoidData;
                    avoidData.position = terrainOrigin + new Vector3(x, h, z);
                    avoidData.forward = N;

                    list.m_avoidDatum.Add(avoidData);
                }
            }
        }
    }

    void InstantiateBounds(Transform parent, Vector3 steps, Vector3 size, Vector3 origin)
    {
        if (m_bottom)
        {
            InstantiateAvoidWall(origin, Quaternion.identity, parent, new Vector2(steps.x, steps.z), new Vector2(size.x, size.z), size.y / 2, "Bottom");
        }

        if (m_top)
        {
            InstantiateAvoidWall(origin, Quaternion.Euler(180, 0, 0), parent, new Vector2(steps.x, steps.z), new Vector2(size.x, size.z), size.y / 2, "Top");
        }

        if (m_right)
        {
            InstantiateAvoidWall(origin, Quaternion.Euler(0, 0, 90), parent, new Vector2(steps.y, steps.z), new Vector2(size.y, size.z), size.x / 2, "Right");
        }

        if (m_left)
        {
            InstantiateAvoidWall(origin, Quaternion.Euler(0, 0, -90), parent, new Vector2(steps.y, steps.z), new Vector2(size.y, size.z), size.x / 2, "Left");
        }

        if (m_front)
        {
            InstantiateAvoidWall(origin, Quaternion.Euler(-90, 0, 0), parent, new Vector2(steps.x, steps.y), new Vector2(size.x, size.y), size.z / 2, "Front");
        }

        if (m_back)
        {
            InstantiateAvoidWall(origin, Quaternion.Euler(90, 0, 0), parent, new Vector2(steps.x, steps.y), new Vector2(size.x, size.y), size.z / 2, "Back");
        }
    }

    GameObject InstantiateAvoidWall(Vector3 origin, Quaternion rotation, Transform parent, Vector2 steps, Vector2 size, float heightOffset, string nameSuffix)
    {
        GameObject wall = Instantiate(m_prefabAvoidWall, parent);
        wall.name += nameSuffix;
        wall.transform.localPosition = origin;
        wall.transform.localRotation = rotation;
        AvoidList list = wall.GetComponent<AvoidList>();

        Vector3 wallPosition = wall.transform.position;
        Quaternion wallRotation = wall.transform.rotation;
        for (float x = -size.x/2; x < size.x/2; x+=steps.x)
        {
            for (float y = -size.y / 2; y < size.y / 2; y += steps.y)
            {
                Behaviour.EnemiesManager.BoidData boidData;
                boidData.position = wallPosition + (wallRotation * new Vector3(x, -heightOffset, y));
                boidData.forward = (wallPosition - boidData.position).normalized;
                list.m_avoidDatum.Add(boidData);
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
