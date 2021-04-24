using UnityEngine;

namespace Settings {
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject {
        public float maxHealth;
        public float maxDepth;
        public float velocity;
        public float angularVelocity;
        public float maxVelocity;
        public float minChangeDepth = 0.5f;
    }
}