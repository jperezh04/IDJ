using UnityEngine;

namespace Lab02Seeking
{
    [RequireComponent(typeof(SeekingAgent))]
    public class SteeringVisualizer : MonoBehaviour
    {
        public Material lineMaterial;
        private SeekingAgent agent;
        private LineRenderer actual;
        private LineRenderer desired;
        private LineRenderer acceleration;
        private LineRenderer direction;
        private LineRenderer radius;

        private void Start()
        {
            agent = GetComponent<SeekingAgent>();
            actual = CreateLine("Velocidad actual", Color.green, 0.075f);
            desired = CreateLine("Velocidad deseada", Color.red, 0.045f);
            acceleration = CreateLine("Aceleracion", Color.yellow, 0.035f);
            direction = CreateLine("Direccion al objetivo", Color.cyan, 0.018f);
            radius = CreateLine("Radio de desaceleracion", Color.cyan, 0.025f);
        }

        private LineRenderer CreateLine(string title, Color color, float width)
        {
            GameObject child = new GameObject(title);
            child.transform.SetParent(transform, false);
            LineRenderer line = child.AddComponent<LineRenderer>();
            line.sharedMaterial = lineMaterial;
            line.useWorldSpace = true;
            line.startColor = line.endColor = color;
            line.startWidth = line.endWidth = width;
            line.numCapVertices = 3;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            return line;
        }

        private void LateUpdate()
        {
            bool show = agent.showVectors && agent.target != null;
            actual.enabled = desired.enabled = acceleration.enabled = direction.enabled = show;
            radius.enabled = show && agent.mode == SeekingAgent.MovementMode.Arrive;
            if (!show) return;

            Vector3 origin = transform.position + Vector3.up * 2.1f;
            Arrow(actual, origin, agent.Velocity * 0.7f);
            Arrow(desired, origin + Vector3.up * 0.05f, agent.DesiredVelocity * 0.7f);
            Arrow(acceleration, origin + Vector3.up * 0.1f, agent.Acceleration * 0.3f);
            Vector3 end = agent.target.position;
            end.y = origin.y;
            direction.positionCount = 2;
            direction.SetPosition(0, origin);
            direction.SetPosition(1, end);

            if (radius.enabled)
            {
                const int segments = 96;
                radius.positionCount = segments;
                radius.loop = true;
                Vector3 center = agent.target.position;
                center.y = 0.06f;
                for (int i = 0; i < segments; i++)
                {
                    float angle = i * 2f * Mathf.PI / segments;
                    radius.SetPosition(i, center + new Vector3(Mathf.Cos(angle), 0f,
                        Mathf.Sin(angle)) * agent.slowRadius);
                }
            }
        }

        private static void Arrow(LineRenderer line, Vector3 start, Vector3 vector)
        {
            Vector3 end = start + vector;
            Vector3 forward = vector.normalized;
            Vector3 side = Vector3.Cross(Vector3.up, forward);
            float head = Mathf.Min(0.25f, vector.magnitude * 0.3f);
            line.positionCount = 5;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.SetPosition(2, end - forward * head + side * head * 0.5f);
            line.SetPosition(3, end);
            line.SetPosition(4, end - forward * head - side * head * 0.5f);
        }
    }
}
