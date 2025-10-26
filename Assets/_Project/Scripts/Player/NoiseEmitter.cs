using UnityEngine;


namespace Project.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class NoiseEmitter : MonoBehaviour
    {
        public float walkNoise = 1.0f;
        public float runNoise = 2.0f;
        public float crouchNoise = 0.3f;
        public float currentNoise { get; private set; }
        PlayerController _pc;


        void Awake() { _pc = GetComponent<PlayerController>(); }
        void Update()
        {
            // Simple heuristic: use speed state only
            currentNoise = _pc ? (_pc.enabled ? (_pc.GetComponent<CharacterController>().velocity.magnitude > 0.1f ? (_pc.GetComponent<CharacterController>().height < 1.6f ? crouchNoise : (_pc.enabled ? runNoise : walkNoise)) : 0f) : 0f) : 0f;
        }
    }
}