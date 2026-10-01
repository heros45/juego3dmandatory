using System.IO;
using UnityEditor;
using UnityEngine;

namespace SciFiLab.Editor
{
    [InitializeOnLoad]
    public static class URPMaterialFixer
    {
        static URPMaterialFixer()
        {
            EditorApplication.delayCall += ConvertAllMaterialsToURP;
        }

        [MenuItem("Tools/Convertir Materiales a URP (Corregir Color Rosa)")]
        public static void ConvertAllMaterialsToURP()
        {
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");

            if (urpLit == null)
            {
                Debug.LogWarning("[URPMaterialFixer] No se encontró el shader Universal Render Pipeline/Lit.");
                return;
            }

            Debug.Log("[URPMaterialFixer] Convirtiendo materiales a Universal Render Pipeline...");

            ConfigureMat("Mat_Floor_Dark", urpLit, new Color(0.12f, 0.14f, 0.16f), 0.7f, 0.4f);
            ConfigureMat("Mat_Floor_Platform", urpLit, new Color(0.18f, 0.22f, 0.26f), 0.8f, 0.5f);
            ConfigureMat("Mat_Wall_Titanium", urpLit, new Color(0.35f, 0.38f, 0.42f), 0.5f, 0.3f);
            ConfigureMat("Mat_Wall_Dark", urpLit, new Color(0.15f, 0.17f, 0.20f), 0.6f, 0.3f);
            ConfigureMat("Mat_Pipes_Copper", urpLit, new Color(0.7f, 0.45f, 0.25f), 0.85f, 0.7f);
            ConfigureMat("Mat_Crate_Orange", urpLit, new Color(0.85f, 0.45f, 0.1f), 0.3f, 0.4f);
            ConfigureMat("Mat_Crate_Dark", urpLit, new Color(0.2f, 0.22f, 0.25f), 0.7f, 0.5f);
            ConfigureMat("Mat_Barrel_Steel", urpLit, new Color(0.25f, 0.3f, 0.35f), 0.8f, 0.6f);
            ConfigureMat("Mat_Railing_Yellow", urpLit, new Color(0.95f, 0.8f, 0.1f), 0.4f, 0.5f);
            ConfigureMat("Mat_Glass_Translucent", urpLit, new Color(0.1f, 0.6f, 0.8f, 0.6f), 0.9f, 0.9f);

            // Neones emisivos
            ConfigureEmissiveMat("Mat_Neon_Cyan", urpLit, new Color(0.1f, 0.8f, 1f), new Color(0.2f, 0.9f, 1f) * 3.5f);
            ConfigureEmissiveMat("Mat_Neon_Orange", urpLit, new Color(1f, 0.5f, 0.1f), new Color(1f, 0.45f, 0.1f) * 3.5f);
            ConfigureEmissiveMat("Mat_Neon_Green", urpLit, new Color(0.2f, 1f, 0.4f), new Color(0.2f, 1f, 0.4f) * 3.5f);
            ConfigureEmissiveMat("Mat_Neon_Red", urpLit, new Color(1f, 0.15f, 0.15f), new Color(1f, 0.15f, 0.15f) * 3.5f);
            ConfigureEmissiveMat("Mat_Coin_Gold", urpLit, new Color(1f, 0.84f, 0.05f), new Color(1.3f, 0.95f, 0.1f) * 2.0f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[URPMaterialFixer] ¡Todos los materiales han sido convertidos a URP con éxito!");
        }

        private static void ConfigureMat(string matName, Shader shader, Color color, float metallic, float smoothness)
        {
            string path = $"Assets/_Project/Materials/{matName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) return;

            mat.shader = shader;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

            EditorUtility.SetDirty(mat);
        }

        private static void ConfigureEmissiveMat(string matName, Shader shader, Color baseColor, Color emissiveColor)
        {
            string path = $"Assets/_Project/Materials/{matName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) return;

            mat.shader = shader;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", baseColor);

            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emissiveColor);

            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            EditorUtility.SetDirty(mat);
        }
    }
}
