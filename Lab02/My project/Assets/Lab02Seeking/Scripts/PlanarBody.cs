using UnityEngine;

namespace Lab02Seeking
{
    // Unity integra la aceleracion y resuelve los contactos en XZ.
    [RequireComponent(typeof(Rigidbody))]
    public class PlanarBody : MonoBehaviour
    {
        public Rigidbody Body { get; private set; }
        public bool IsPaused { get; private set; }
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 savedVelocity;
        private RigidbodyConstraints movingConstraints;

        public Vector3 Velocity
        {
            get
            {
                if (Body == null) return Vector3.zero;
#if UNITY_6000_0_OR_NEWER
                return Body.linearVelocity;
#else
                return Body.velocity;
#endif
            }
        }

        private void Awake()
        {
            Body = GetComponent<Rigidbody>();
            initialPosition = Body.position;
            initialRotation = Body.rotation;
            Body.useGravity = false;
            Body.isKinematic = false;
            movingConstraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            Body.constraints = movingConstraints;
            Body.interpolation = RigidbodyInterpolation.Interpolate;
            Body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void Accelerate(Vector3 acceleration)
        {
            if (IsPaused) return;
            acceleration.y = 0f;
            Body.AddForce(acceleration, ForceMode.Acceleration);
        }

        public void Face(Vector3 direction)
        {
            direction.y = 0f;
            if (IsPaused || direction.sqrMagnitude < 0.01f) return;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            Body.MoveRotation(Quaternion.RotateTowards(Body.rotation, rotation, 240f * Time.fixedDeltaTime));
        }

        public void SetPaused(bool value)
        {
            if (IsPaused == value) return;
            if (value)
            {
                savedVelocity = Velocity;
                WriteVelocity(Vector3.zero);
                Body.angularVelocity = Vector3.zero;
                Body.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                Body.constraints = movingConstraints;
                WriteVelocity(savedVelocity);
                Body.WakeUp();
            }
            IsPaused = value;
        }

        public void ResetBody()
        {
            WriteVelocity(Vector3.zero);
            Body.angularVelocity = Vector3.zero;
            savedVelocity = Vector3.zero;
            Body.position = initialPosition;
            Body.rotation = initialRotation;
        }

        private void WriteVelocity(Vector3 value)
        {
#if UNITY_6000_0_OR_NEWER
            Body.linearVelocity = value;
#else
            Body.velocity = value;
#endif
        }
    }
}
