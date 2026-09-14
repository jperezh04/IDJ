using UnityEditor;
using UnityEngine;

namespace Lab02Seeking.Editor
{
    // Modelos hechos con primitivas: todas las piezas quedan editables en Hierarchy.
    public static class FarmCharacters
    {
        public static GameObject Pig(string folder, Shader shader)
        {
            Material pink = Material(folder, "Cerdito_Rosa", shader, new Color(1f, 0.57f, 0.66f));
            Material blush = Material(folder, "Cerdito_Hocico", shader, new Color(0.9f, 0.32f, 0.46f));
            Material dark = Material(folder, "Cerdito_Pezunas", shader, new Color(0.24f, 0.12f, 0.17f));
            Material black = Material(folder, "Ojos", shader, new Color(0.055f, 0.045f, 0.055f));
            Material white = Material(folder, "Brillo_Ojos", shader, Color.white);
            GameObject root = new GameObject("Cerdito - Agente Seeking");
            Transform p = root.transform;
            Part(p, "Cuerpo redondito", PrimitiveType.Sphere, new Vector3(0, 0.94f, -0.13f),
                new Vector3(1.28f, 1.12f, 1.75f), pink);
            Part(p, "Cabeza", PrimitiveType.Sphere, new Vector3(0, 1.05f, 0.69f),
                new Vector3(1.1f, 1.02f, 0.98f), pink);
            Part(p, "Hocico", PrimitiveType.Sphere, new Vector3(0, 0.94f, 1.17f),
                new Vector3(0.68f, 0.44f, 0.25f), blush);

            for (int side = -1; side <= 1; side += 2)
            {
                string label = side < 0 ? " izquierda" : " derecha";
                Part(p, "Fosa nasal" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.15f, 0.95f, 1.285f), new Vector3(0.095f, 0.12f, 0.035f), dark);
                Part(p, "Ojo" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.32f, 1.25f, 1.08f), new Vector3(0.18f, 0.2f, 0.09f), black);
                Part(p, "Brillito" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.32f - 0.025f, 1.29f, 1.125f), Vector3.one * 0.05f, white);
                Part(p, "Mejilla" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.44f, 1.01f, 1.06f), new Vector3(0.17f, 0.11f, 0.055f), blush);
                Transform ear = Part(p, "Oreja" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.4f, 1.62f, 0.66f), new Vector3(0.33f, 0.65f, 0.18f), pink);
                ear.localRotation = Quaternion.Euler(-12f, 0, side * -25f);
                Transform inner = Part(p, "Interior oreja" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.4f, 1.63f, 0.76f), new Vector3(0.2f, 0.42f, 0.04f), blush);
                inner.localRotation = ear.localRotation;
                for (int front = -1; front <= 1; front += 2)
                {
                    Vector3 foot = new Vector3(side * 0.42f, 0.29f, front * 0.51f - 0.1f);
                    Part(p, "Patita " + side + " " + front, PrimitiveType.Capsule,
                        foot + Vector3.up * 0.09f, new Vector3(0.29f, 0.26f, 0.29f), pink);
                    Part(p, "Pezuna " + side + " " + front, PrimitiveType.Sphere,
                        new Vector3(foot.x, 0.13f, foot.z), new Vector3(0.33f, 0.26f, 0.37f), dark);
                }
            }
            // Cola rizada: esferas superpuestas formando una pequena espiral.
            for (int i = 0; i < 25; i++)
            {
                float t = i / 24f;
                float angle = t * Mathf.PI * 3f;
                float radius = 0.13f * t;
                Part(p, "Colita " + i, PrimitiveType.Sphere,
                    new Vector3(Mathf.Sin(angle) * radius, 1f + Mathf.Cos(angle) * radius,
                        -0.97f - 0.42f * t), Vector3.one * 0.085f, pink);
            }
            return root;
        }

        public static GameObject Farmer(string folder, Shader shader)
        {
            Material skin = Material(folder, "Granjero_Piel", shader, new Color(0.91f, 0.62f, 0.41f));
            Material shirt = Material(folder, "Granjero_Camisa", shader, new Color(0.8f, 0.25f, 0.16f));
            Material denim = Material(folder, "Granjero_Overol", shader, new Color(0.14f, 0.35f, 0.61f));
            Material straw = Material(folder, "Granjero_Sombrero", shader, new Color(0.94f, 0.73f, 0.32f));
            Material brown = Material(folder, "Granjero_Botas", shader, new Color(0.26f, 0.15f, 0.09f));
            Material black = Material(folder, "Granjero_Ojos", shader, new Color(0.06f, 0.045f, 0.035f));
            GameObject root = new GameObject("Granjero - Personaje controlable");
            Transform p = root.transform;
            Part(p, "Torso capsula", PrimitiveType.Capsule, new Vector3(0, 1.27f, 0),
                new Vector3(0.81f, 0.55f, 0.52f), shirt);
            Part(p, "Cintura overol", PrimitiveType.Sphere, new Vector3(0, 0.94f, 0),
                new Vector3(0.81f, 0.55f, 0.57f), denim);
            Part(p, "Pechera overol", PrimitiveType.Cube, new Vector3(0, 1.23f, 0.266f),
                new Vector3(0.48f, 0.5f, 0.065f), denim);
            Part(p, "Bolsillo", PrimitiveType.Cube, new Vector3(0, 1.18f, 0.31f),
                new Vector3(0.23f, 0.16f, 0.025f), shirt);
            Part(p, "Cuello", PrimitiveType.Capsule, new Vector3(0, 1.8f, 0),
                new Vector3(0.25f, 0.15f, 0.25f), skin);
            Part(p, "Cabeza", PrimitiveType.Sphere, new Vector3(0, 2.06f, 0),
                new Vector3(0.67f, 0.73f, 0.63f), skin);
            Part(p, "Nariz", PrimitiveType.Sphere, new Vector3(0, 2.03f, 0.33f),
                new Vector3(0.16f, 0.18f, 0.17f), skin);
            Part(p, "Bigote", PrimitiveType.Sphere, new Vector3(0, 1.92f, 0.302f),
                new Vector3(0.29f, 0.09f, 0.07f), brown);

            for (int side = -1; side <= 1; side += 2)
            {
                string label = side < 0 ? " izquierdo" : " derecho";
                Part(p, "Ojo" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.13f, 2.14f, 0.285f), new Vector3(0.07f, 0.095f, 0.045f), black);
                Part(p, "Oreja" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.33f, 2.06f, 0), new Vector3(0.15f, 0.23f, 0.17f), skin);
                Transform arm = Part(p, "Brazo" + label, PrimitiveType.Capsule,
                    new Vector3(side * 0.55f, 1.28f, 0), new Vector3(0.24f, 0.34f, 0.25f), shirt);
                arm.localRotation = Quaternion.Euler(0, 0, side * 15f);
                Part(p, "Mano" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.65f, 0.96f, 0.02f), Vector3.one * 0.25f, skin);
                Part(p, "Pierna" + label, PrimitiveType.Capsule,
                    new Vector3(side * 0.23f, 0.52f, 0), new Vector3(0.31f, 0.32f, 0.32f), denim);
                Part(p, "Bota" + label, PrimitiveType.Cube,
                    new Vector3(side * 0.23f, 0.14f, 0.1f), new Vector3(0.34f, 0.28f, 0.5f), brown);
                Part(p, "Tirante" + label, PrimitiveType.Cube,
                    new Vector3(side * 0.22f, 1.51f, 0.22f), new Vector3(0.095f, 0.42f, 0.11f), denim);
                Part(p, "Boton" + label, PrimitiveType.Sphere,
                    new Vector3(side * 0.21f, 1.37f, 0.307f), Vector3.one * 0.075f, straw);
            }
            Part(p, "Sombrero ala ancha", PrimitiveType.Cylinder, new Vector3(0, 2.39f, 0),
                new Vector3(1.25f, 0.055f, 1.1f), straw);
            Part(p, "Sombrero copa", PrimitiveType.Cylinder, new Vector3(0, 2.57f, 0),
                new Vector3(0.71f, 0.17f, 0.65f), straw);
            Part(p, "Cinta del sombrero", PrimitiveType.Cylinder, new Vector3(0, 2.46f, 0),
                new Vector3(0.735f, 0.055f, 0.675f), brown);
            return root;
        }

        private static Material Material(string folder, string name, Shader shader, Color color)
        {
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0.12f);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.12f);
            AssetDatabase.CreateAsset(material, folder + "/" + name + ".mat");
            return material;
        }

        private static Transform Part(Transform parent, string name, PrimitiveType type,
            Vector3 position, Vector3 scale, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(obj.GetComponent<Collider>());
            return obj.transform;
        }
    }
}
