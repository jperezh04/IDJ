using UnityEngine;

public class TrajectoryHUD : MonoBehaviour {
    [Header("Tank References")]
    public FireShell aiTank;
    public Drive playerTank;

    void Start() {
        if (aiTank == null) {
            aiTank = Object.FindFirstObjectByType<FireShell>();
        }
        if (playerTank == null) {
            playerTank = Object.FindFirstObjectByType<Drive>();
        }
    }

    void OnGUI() {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(15, 15, 340, 310), GUI.skin.box);
        GUILayout.Label("<b><size=13>TUTORIAL: CÁLCULO DE TRAYECTORIAS</size></b>");
        GUILayout.Space(4);

        GUILayout.Label("<b>Controles Jugador (Tanque Rojo):</b>");
        GUILayout.Label(" • W / S / Flechas: Avanzar / Retroceder");
        GUILayout.Label(" • A / D / Flechas: Girar");
        GUILayout.Label(" • T / G: Ajustar cañón");
        GUILayout.Label(" • B: Disparar proyectil");
        GUILayout.Space(6);

        if (aiTank != null && playerTank != null) {
            float dist = Vector3.Distance(aiTank.transform.position, playerTank.transform.position);
            GUILayout.Label($"<b>Distancia al objetivo:</b> {dist:F2} m");

            string arcMode = aiTank.useLowAngle ? "Ángulo Bajo (Low Arc)" : "Ángulo Alto (High Arc)";
            GUILayout.Label($"<b>Trayectoria IA:</b> {arcMode}");

            if (GUILayout.Button(aiTank.useLowAngle ? "Cambiar a Ángulo Alto" : "Cambiar a Ángulo Bajo")) {
                aiTank.useLowAngle = !aiTank.useLowAngle;
            }

            GUILayout.Space(4);
            GUILayout.Label($"<b>Velocidad proyectil IA:</b> {aiTank.speed:F1} m/s");
            aiTank.speed = GUILayout.HorizontalSlider(aiTank.speed, 5.0f, 35.0f);

            GUILayout.Space(4);
            GUILayout.Label($"<b>Cadencia disparo IA:</b> {aiTank.delayReset:F2} s");
            aiTank.delayReset = GUILayout.HorizontalSlider(aiTank.delayReset, 0.1f, 2.0f);
        } else {
            GUILayout.Label("<i>(Tanques no asignados)</i>");
        }

        GUILayout.EndArea();
    }
}
