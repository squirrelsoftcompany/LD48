using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidList : MonoBehaviour
{
    public List<Behaviour.EnemiesManager.BoidData> m_avoidDatum;

    private void OnDrawGizmosSelected()
    {
        Behaviour.EnemiesManager.BoidSettings settings = Behaviour.EnemiesManager.Get?.m_avoidSettings;

        foreach (var avoidData in m_avoidDatum)
        {
            if (settings != null)
            {
                Gizmos.color = settings.m_avoid ? Color.red : Color.green;
                Gizmos.DrawSphere(avoidData.position, settings.m_avoidanceRadius);
                Gizmos.DrawWireSphere(avoidData.position, settings.m_relevantRadius);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(avoidData.position, 15);
            }
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(avoidData.position, avoidData.position + avoidData.forward * 30);
        }
    }
}
