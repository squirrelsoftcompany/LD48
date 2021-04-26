using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DrawTerrainBounds : MonoBehaviour
{
#if UNITY_EDITOR
    List<Terrain> _terrains;

    // Start is called before the first frame update
    void Start()
    {
        _terrains = GetComponentsInChildren<Terrain>().ToList();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Bounds b = GetTerrainBounds(_terrains, gameObject);
        Gizmos.DrawWireCube(b.center, b.size);
        Gizmos.DrawSphere(b.center, 15);

        Gizmos.color = Color.yellow;
        if (_terrains != null)
        {
            foreach (var terrain in _terrains)
            {
                Bounds b2 = terrain.terrainData.bounds;
                Gizmos.DrawSphere(b2.center + terrain.GetPosition(), 15);
            }
        }
    }

    public static Bounds GetTerrainBounds(List<Terrain> terrains, GameObject root)
    {
        Bounds totalB = new Bounds(root ? root.transform.position : Vector3.zero, Vector3.zero);
        if (terrains != null)
        {
            foreach (var terrain in terrains)
            {
                Bounds b = terrain.terrainData.bounds;
                b.center = b.center + terrain.GetPosition();
                totalB.Encapsulate(b);
            }
        }
        return totalB;
    }
#endif
}
