using UnityEngine;

namespace Lab02Seeking
{
    [RequireComponent(typeof(PlanarBody))]
    public class SeekingAgent : MonoBehaviour
    {
        public enum MovementMode { Seeking, Arrive }
        public Transform target;
        public MovementMode mode = MovementMode.Seeking;
        [Min(0.1f)] public float maxSpeed = 5f;
        [Min(0.1f)] public float maxAcceleration = 10f;
        [Min(0.01f)] public float responseTime = 0.3f;
        [Min(0.1f)] public float slowRadius = 4f;
        [Min(0.001f)] public float stopRadius = 2.2f;
        public bool paused;
        public bool showVectors = true;
        private PlanarBody motor;

        public Vector3 Velocity { get { return motor != null ? motor.Velocity : Vector3.zero; } }
        public Vector3 DesiredVelocity { get; private set; }
        public Vector3 Acceleration { get; private set; }
        public float DistanceToTarget
        {
            get
            {
                if (target == null) return 0f;
                Vector3 offset = target.position - transform.position;
                offset.y = 0f;
                return offset.magnitude;
            }
        }

        private void Awake() { motor = GetComponent<PlanarBody>(); }

        private void FixedUpdate()
        {
            motor.SetPaused(paused);
            if (paused) return;
            Vector3 offset = target != null ? target.position - motor.Body.position : Vector3.zero;
            offset.y = 0f;
            float distance = offset.magnitude;
            float speed = Mathf.Max(maxSpeed, 0.1f);
            if (mode == MovementMode.Arrive)
            {
                // Frenar antes del contacto: se considera el tamano de los personajes.
                float stop = Mathf.Max(stopRadius, 0.001f);
                float slow = Mathf.Max(slowRadius, stop + 0.1f);
                speed *= Mathf.Clamp01((distance - stop) / (slow - stop));
            }
            DesiredVelocity = distance > 0.0001f ? offset / distance * speed : Vector3.zero;
            Vector3 steering = DesiredVelocity - Velocity;
            Acceleration = Vector3.ClampMagnitude(steering / Mathf.Max(responseTime, 0.01f),
                Mathf.Max(maxAcceleration, 0.1f));
            // No escribir transform.position: el solver necesita resolver los choques.
            motor.Accelerate(Acceleration);
            motor.Face(Velocity);
        }

        public void SetPaused(bool value)
        {
            paused = value;
            if (motor != null) motor.SetPaused(value);
        }

        public void ResetAgent()
        {
            motor.ResetBody();
            DesiredVelocity = Vector3.zero;
            Acceleration = Vector3.zero;
        }
    }
}
