using UnityEngine;

namespace Lab02Seeking
{
    public class SeekingDemoHUD : MonoBehaviour
    {
        public SeekingAgent agent;
        public TargetController target;
        public FarmCamera views;
        public static Rect PanelRect { get { return new Rect(12, 12, 335, 410); } }
        private SeekingAgent[] pigs;
        private bool paused;
        private bool vectors = true;

        private void Start() { pigs = FindObjectsOfType<SeekingAgent>(); }

        private void OnGUI()
        {
            if (agent == null || pigs == null) return;
            GUILayout.BeginArea(PanelRect, GUI.skin.box);
            GUILayout.Label("LAB 02 | CERDITOS Y GRANJERO");
            GUILayout.Label("Clic: caminar al punto | WASD / flechas");
            GUILayout.Label("Camara: " + (views != null ? views.view.ToString() : "General"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("General") && views != null) views.SetView(FarmCamera.ViewMode.General);
            if (GUILayout.Button("Desde arriba") && views != null) views.SetView(FarmCamera.ViewMode.Cenital);
            if (GUILayout.Button("Seguir") && views != null) views.SetView(FarmCamera.ViewMode.Seguir);
            GUILayout.EndHorizontal();
            GUILayout.Label("Controles para TODOS los chanchitos:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Seeking")) SetMode(SeekingAgent.MovementMode.Seeking);
            if (GUILayout.Button("Arrive")) SetMode(SeekingAgent.MovementMode.Arrive);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(paused ? "Continuar" : "Pausar"))
            {
                paused = !paused;
                foreach (SeekingAgent pig in pigs) if (pig != null) pig.SetPaused(paused);
                if (target != null) target.SetPaused(paused);
            }
            if (GUILayout.Button("Reiniciar"))
            {
                foreach (SeekingAgent pig in pigs) if (pig != null) pig.ResetAgent();
                if (target != null) target.ResetTarget();
            }
            GUILayout.EndHorizontal();
            bool show = GUILayout.Toggle(vectors, "Mostrar vectores");
            if (show != vectors)
            {
                vectors = show;
                foreach (SeekingAgent pig in pigs) if (pig != null) pig.showVectors = vectors;
            }
            GUILayout.Label("Chanchitos: " + pigs.Length + " | " + agent.mode + (paused ? " | PAUSA" : ""));
            GUILayout.Label("Agente principal - distancia: " + agent.DistanceToTarget.ToString("F2"));
            GUILayout.Label("Rapidez: " + agent.Velocity.magnitude.ToString("F2") + " u/s");
            GUILayout.Label("Aceleracion solicitada: " + agent.Acceleration.magnitude.ToString("F2") + " u/s2");
            LabelColor("Verde: velocidad | Rojo: velocidad deseada", Color.green);
            LabelColor("Amarillo: aceleracion solicitada", Color.yellow);
            LabelColor("Cian: objetivo y radio de Arrive", Color.cyan);
            GUILayout.Label("Hay colisiones; no hay busqueda de rutas.");
            GUILayout.EndArea();
        }

        private void SetMode(SeekingAgent.MovementMode mode)
        {
            foreach (SeekingAgent pig in pigs) if (pig != null) pig.mode = mode;
        }

        private static void LabelColor(string text, Color color)
        {
            Color previous = GUI.contentColor;
            GUI.contentColor = color;
            GUILayout.Label(text);
            GUI.contentColor = previous;
        }
    }
}
