using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Lab02Seeking
{
    [RequireComponent(typeof(PlanarBody))]
    public class TargetController : MonoBehaviour
    {
        public Camera sceneCamera;
        public SeekingAgent agent;
        public float keyboardSpeed = 6f;
        public float arenaLimit = 10f;
        public float maxAcceleration = 40f;
        private PlanarBody motor;
        private Vector3 keyboardDirection;
        private Vector3 destination;
        private bool hasDestination;

        private void Awake() { motor = GetComponent<PlanarBody>(); }

        private void Update()
        {
            if (motor.IsPaused) return;
            Vector2 direction = Vector2.zero;
            Vector2 pointer = Vector2.zero;
            bool click = false;
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                direction.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
                direction.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
            }
            if (Mouse.current != null)
            {
                pointer = Mouse.current.position.ReadValue();
                click = Mouse.current.leftButton.wasPressedThisFrame;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            direction.x = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
            direction.y = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1f : 0f)
                - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
            pointer = Input.mousePosition;
            click = Input.GetMouseButtonDown(0);
#endif
            if (direction.sqrMagnitude > 1f) direction.Normalize();
            Vector3 right = sceneCamera != null ? sceneCamera.transform.right : Vector3.right;
            // camera.up proyectado sobre XZ funciona tambien en vista cenital exacta.
            Vector3 forward = sceneCamera != null ? sceneCamera.transform.up : Vector3.forward;
            right.y = forward.y = 0f;
            keyboardDirection = right.normalized * direction.x + forward.normalized * direction.y;
            if (keyboardDirection.sqrMagnitude > 0.001f) hasDestination = false;

            Vector2 guiPointer = new Vector2(pointer.x, Screen.height - pointer.y);
            if (click && sceneCamera != null && !SeekingDemoHUD.PanelRect.Contains(guiPointer))
            {
                Plane ground = new Plane(Vector3.up, Vector3.zero);
                Ray ray = sceneCamera.ScreenPointToRay(pointer);
                float distance;
                if (ground.Raycast(ray, out distance))
                {
                    destination = ray.GetPoint(distance);
                    destination.x = Mathf.Clamp(destination.x, -arenaLimit, arenaLimit);
                    destination.z = Mathf.Clamp(destination.z, -arenaLimit, arenaLimit);
                    hasDestination = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (motor.IsPaused) return;
            Vector3 desired = keyboardDirection * keyboardSpeed;
            if (hasDestination)
            {
                Vector3 offset = destination - motor.Body.position;
                offset.y = 0f;
                if (offset.magnitude < 0.12f) hasDestination = false;
                else desired = offset.normalized * Mathf.Min(keyboardSpeed, offset.magnitude / 0.2f);
            }
            motor.Accelerate(Vector3.ClampMagnitude((desired - motor.Velocity) / 0.12f, maxAcceleration));
            motor.Face(motor.Velocity);
        }

        public void SetPaused(bool value)
        {
            motor.SetPaused(value);
            keyboardDirection = Vector3.zero;
        }

        public void ResetTarget()
        {
            motor.ResetBody();
            hasDestination = false;
            keyboardDirection = Vector3.zero;
        }
    }
}
