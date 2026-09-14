using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lab02Seeking.Editor
{
    public static class CreateSeekingScene
    {
        [MenuItem("Laboratorio/Crear escena Seeking")]
        public static void Create()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Deten Play antes de crear la escena.");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            Shader surfaceShader = Shader.Find("Standard");
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null)
                surfaceShader = Shader.Find("Universal Render Pipeline/Lit");
            Shader lineShader = Shader.Find("Sprites/Default");
            if (surfaceShader == null || lineShader == null)
            {
                Debug.LogError("Usa un proyecto 3D Built-In o Universal 3D (URP). No se encontro el shader requerido.");
                return;
            }

            // CreateFolder devuelve un GUID, no una ruta. Validar antes de crear materiales.
            const string rootPath = "Assets/Lab02Seeking";
            if (!AssetDatabase.IsValidFolder(rootPath))
                AssetDatabase.CreateFolder("Assets", "Lab02Seeking");
            if (!AssetDatabase.IsValidFolder(rootPath))
            {
                Debug.LogError("No se pudo crear " + rootPath + ". Revisa la carpeta Assets.");
                return;
            }
            // Unity agrega un numero si ya existe Generado; conservar la ruta real creada.
            string folderGuid = AssetDatabase.CreateFolder(rootPath, "Generado");
            string folder = AssetDatabase.GUIDToAssetPath(folderGuid);
            if (string.IsNullOrEmpty(folder) || !folder.StartsWith(rootPath + "/")
                || !AssetDatabase.IsValidFolder(folder))
            {
                Debug.LogError("No se pudo crear la carpeta de salida. No se generaron materiales.");
                return;
            }
            Material floor = MaterialAsset(folder, "Suelo", surfaceShader, new Color(0.12f, 0.16f, 0.21f));
            Material lines = MaterialAsset(folder, "Vectores", lineShader, Color.white);
            Material fence = MaterialAsset(folder, "Cerca", surfaceShader, new Color(0.5f, 0.3f, 0.15f));

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.65f, 0.68f, 0.75f);
            GameObject ground = Primitive("Suelo", PrimitiveType.Cube, new Vector3(0f, -0.15f, 0f),
                new Vector3(24f, 0.3f, 24f), floor);
            ground.AddComponent<BoxCollider>();
            Wall("Cerca norte", new Vector3(0, 0.65f, 11.75f), new Vector3(24, 1.3f, 0.5f), fence);
            Wall("Cerca sur", new Vector3(0, 0.65f, -11.75f), new Vector3(24, 1.3f, 0.5f), fence);
            Wall("Cerca este", new Vector3(11.75f, 0.65f, 0), new Vector3(0.5f, 1.3f, 24), fence);
            Wall("Cerca oeste", new Vector3(-11.75f, 0.65f, 0), new Vector3(0.5f, 1.3f, 24), fence);
            for (int i = -12; i <= 12; i += 2)
            {
                GridLine(ground.transform, lines, new Vector3(i, 0.015f, -12f), new Vector3(i, 0.015f, 12f));
                GridLine(ground.transform, lines, new Vector3(-12f, 0.015f, i), new Vector3(12f, 0.015f, i));
            }

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.transform.position = new Vector3(0f, 23f, 26f);
            camera.transform.LookAt(Vector3.zero);
            camera.orthographic = true;
            camera.orthographicSize = 17f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.05f, 0.075f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            GameObject lightObject = new GameObject("Luz", typeof(Light));
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject target = FarmCharacters.Farmer(folder, surfaceShader);
            target.transform.position = new Vector3(5f, 0f, 3f);
            AddPhysics(target, 8f, new Vector3(0, 1.3f, 0), 0.68f, 2.6f, 1);
            GameObject actor = FarmCharacters.Pig(folder, surfaceShader);
            actor.transform.position = new Vector3(-5f, 0f, -3f);
            AddPhysics(actor, 1.5f, new Vector3(0, 0.95f, -0.02f), 0.64f, 2.75f, 2);
            SeekingAgent agent = actor.AddComponent<SeekingAgent>();
            agent.target = target.transform;
            actor.AddComponent<SteeringVisualizer>().lineMaterial = lines;
            GameObject second = Object.Instantiate(actor);
            second.name = "Cerdito 2 - Agente Seeking";
            second.transform.position = new Vector3(-8f, 0f, -6f);
            GameObject third = Object.Instantiate(actor);
            third.name = "Cerdito 3 - Agente Seeking";
            third.transform.position = new Vector3(-3f, 0f, -7f);
            TargetController controller = target.AddComponent<TargetController>();
            controller.sceneCamera = camera;
            controller.agent = agent;
            FarmCamera views = cameraObject.AddComponent<FarmCamera>();
            views.target = target.transform;
            SeekingDemoHUD hud = new GameObject("Panel del laboratorio").AddComponent<SeekingDemoHUD>();
            hud.agent = agent;
            hud.target = controller;
            hud.views = views;

            string path = folder + "/Seeking.unity";
            if (!EditorSceneManager.SaveScene(scene, path))
            {
                Debug.LogError("La escena fue creada pero no se pudo guardar en " + path);
                return;
            }
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = actor;
            Debug.Log("Escena creada en " + path + ". Abre Game y pulsa Play.");
        }

        private static Material MaterialAsset(string folder, string name, Shader shader, Color color)
        {
            if (string.IsNullOrEmpty(folder) || !AssetDatabase.IsValidFolder(folder))
                throw new System.InvalidOperationException("Carpeta de materiales no valida: " + folder);
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            AssetDatabase.CreateAsset(material, folder + "/" + name + ".mat");
            return material;
        }

        private static GameObject Primitive(string name, PrimitiveType type, Vector3 position,
            Vector3 scale, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.position = position;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            // El generador agrega explicitamente los colliders de suelo y cercas.
            Object.DestroyImmediate(obj.GetComponent<Collider>());
            return obj;
        }

        private static void AddPhysics(GameObject obj, float mass, Vector3 center,
            float radius, float height, int direction)
        {
            CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
            collider.center = center;
            collider.radius = radius;
            collider.height = height;
            collider.direction = direction;
            collider.isTrigger = false;
            Rigidbody body = obj.AddComponent<Rigidbody>();
            body.mass = mass;
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            obj.AddComponent<PlanarBody>();
        }

        private static void Wall(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject wall = Primitive(name, PrimitiveType.Cube, position, scale, material);
            wall.AddComponent<BoxCollider>();
        }

        private static void GridLine(Transform parent, Material material, Vector3 a, Vector3 b)
        {
            GameObject obj = new GameObject("Cuadricula");
            obj.transform.SetParent(parent, false);
            LineRenderer line = obj.AddComponent<LineRenderer>();
            line.sharedMaterial = material;
            line.useWorldSpace = true;
            line.startWidth = line.endWidth = 0.015f;
            line.startColor = line.endColor = new Color(0.22f, 0.3f, 0.38f);
            line.positionCount = 2;
            line.SetPosition(0, a);
            line.SetPosition(1, b);
        }
    }
}
