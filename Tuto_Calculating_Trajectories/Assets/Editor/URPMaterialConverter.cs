using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class URPMaterialConverter {
    static URPMaterialConverter() {
        EditorApplication.delayCall += ConvertMaterials;
    }

    [MenuItem("Tools/Convert Materials to URP")]
    public static void ConvertMaterials() {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        Shader urpParticle = Shader.Find("Universal Render Pipeline/Particles/Unlit");

        if (urpLit == null) {
            Debug.LogWarning("[URPMaterialConverter] Universal Render Pipeline/Lit shader not found. Ensure URP is installed.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        bool modified = false;

        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            if (mat.shader != null && (mat.shader.name == "Standard" || mat.shader.name.StartsWith("Hidden/InternalErrorShader"))) {
                if (mat.name.ToLower().Contains("explosion") && urpParticle != null) {
                    Texture mainTex = mat.mainTexture;
                    mat.shader = urpParticle;
                    if (mainTex != null) mat.mainTexture = mainTex;
                } else {
                    Color col = mat.HasProperty("_Color") ? mat.color : Color.white;
                    Texture mainTex = mat.mainTexture;
                    Texture normalMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;

                    mat.shader = urpLit;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", col);
                    if (mainTex != null && mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", mainTex);
                    if (normalMap != null && mat.HasProperty("_BumpMap")) mat.SetTexture("_BumpMap", normalMap);
                }
                EditorUtility.SetDirty(mat);
                modified = true;
                Debug.Log($"[URPMaterialConverter] Converted material to URP: {mat.name} ({path})");
            }
        }

        if (modified) {
            AssetDatabase.SaveAssets();
        }
    }
}
