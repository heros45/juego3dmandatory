using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SciFiLab.Editor
{
    public static class SciFiSceneBuilder
    {
        [MenuItem("Tools/Actualizar Monedas Doradas (5) y Victoria")]
        public static void UpdateCoinsInCurrentScene()
        {
            Debug.Log("--- ACTUALIZANDO MONEDAS DORADAS EN LA ESCENA ACTUAL ---");

            GameObject interactives = GameObject.Find("InteractiveObjects");
            if (interactives == null)
            {
                interactives = new GameObject("InteractiveObjects");
                GameObject env = GameObject.Find("--- 2. ENVIRONMENT ---");
                if (env != null) interactives.transform.SetParent(env.transform);
            }

            // Eliminar objetos antiguos de energía y monedas previas
            string[] toRemove = new string[] { "EnergyCore_1", "EnergyCore_2", "EnergyCore_3", "GoldCoin_1", "GoldCoin_2", "GoldCoin_3", "GoldCoin_4", "GoldCoin_5" };
            foreach (string objName in toRemove)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj != null) Object.DestroyImmediate(obj);
            }

            Material matCoinGold = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Coin_Gold.mat");
            if (matCoinGold == null)
            {
                matCoinGold = CreateMaterial("Mat_Coin_Gold", new Color(1f, 0.84f, 0.05f), 0.95f, 0.85f);
                matCoinGold.EnableKeyword("_EMISSION");
                matCoinGold.SetColor("_EmissionColor", new Color(1.3f, 0.95f, 0.1f));
            }

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0f, 1.1f, -9.5f),   // Moneda 1: Habitación inicial (Airlock)
                new Vector3(-6.5f, 1.4f, -2.5f), // Moneda 2: Mesa de laboratorio
                new Vector3(-2.0f, 1.2f, 2.5f),  // Moneda 3: Frente al reactor central
                new Vector3(-8.5f, 1.2f, 12.0f), // Moneda 4: Almacén de cajas y barriles
                new Vector3(8.5f, 3.4f, 13.0f)   // Moneda 5: Plataforma elevada de observación
            };

            for (int i = 0; i < positions.Length; i++)
            {
                CreateGoldCoinPickup($"GoldCoin_{i + 1}", positions[i], interactives, matCoinGold);
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            Debug.Log("¡5 Monedas Doradas actualizadas y guardadas con éxito en la escena!");
        }

        [MenuItem("Tools/Configurar Terreno y Vegetacion (Planeta Abandonado)")]
        public static void SetupAbandonedPlanetTerrain()
        {
            Debug.Log("--- CONFIGURANDO TERRENO Y AMBIENTACIÓN PLANETARIA ABANDONADA ---");

            string scenePath = "Assets/_Project/Scenes/SciFi_Lab_PA3.unity";
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.path != scenePath)
            {
                EditorSceneManager.OpenScene(scenePath);
            }

            GameObject rootEnv = GameObject.Find("--- 2. ENVIRONMENT ---");
            if (rootEnv == null) rootEnv = new GameObject("--- 2. ENVIRONMENT ---");

            // 1. Crear o cargar capas de textura (TerrainLayers) para evitar que se vea negro
            string matFolder = "Assets/_Project/Materials";
            if (!AssetDatabase.IsValidFolder(matFolder)) AssetDatabase.CreateFolder("Assets/_Project", "Materials");

            string dirtTexPath = "Assets/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Ground_Generic_03.png";
            string grassTexPath = "Assets/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Ground_Grass_Green_01.png";

            Texture2D dirtTex = AssetDatabase.LoadAssetAtPath<Texture2D>(dirtTexPath);
            Texture2D grassTex = AssetDatabase.LoadAssetAtPath<Texture2D>(grassTexPath);

            string dirtLayerPath = $"{matFolder}/TerrainLayer_AlienDirt.terrainlayer";
            TerrainLayer dirtLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(dirtLayerPath);
            if (dirtLayer == null)
            {
                dirtLayer = new TerrainLayer();
                dirtLayer.name = "TerrainLayer_AlienDirt";
                dirtLayer.diffuseTexture = dirtTex;
                dirtLayer.tileSize = new Vector2(6f, 6f);
                AssetDatabase.CreateAsset(dirtLayer, dirtLayerPath);
            }

            string grassLayerPath = $"{matFolder}/TerrainLayer_AlienGrass.terrainlayer";
            TerrainLayer grassLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(grassLayerPath);
            if (grassLayer == null)
            {
                grassLayer = new TerrainLayer();
                grassLayer.name = "TerrainLayer_AlienGrass";
                grassLayer.diffuseTexture = grassTex;
                grassLayer.tileSize = new Vector2(6f, 6f);
                AssetDatabase.CreateAsset(grassLayer, grassLayerPath);
            }

            // 2. Buscar o crear el GameObject del Terreno
            GameObject terrainObj = GameObject.Find("Terrain");
            Terrain terrain = null;
            TerrainCollider terrainCol = null;

            if (terrainObj != null)
            {
                terrain = terrainObj.GetComponent<Terrain>();
                terrainCol = terrainObj.GetComponent<TerrainCollider>();
                if (terrainCol == null) terrainCol = terrainObj.AddComponent<TerrainCollider>();
            }
            else
            {
                terrainObj = new GameObject("Terrain");
                terrain = terrainObj.AddComponent<Terrain>();
                terrainCol = terrainObj.AddComponent<TerrainCollider>();
            }

            terrainObj.transform.SetParent(rootEnv.transform);

            // 3. Crear o configurar TerrainData
            string terrainDataPath = $"{matFolder}/TerrainData_AlienPlanet.asset";
            TerrainData tData = AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataPath);
            if (tData == null)
            {
                tData = new TerrainData();
                tData.name = "TerrainData_AlienPlanet";
                AssetDatabase.CreateAsset(tData, terrainDataPath);
            }

            float terrainWidth = 140f;
            float terrainLength = 140f;
            float terrainHeight = 12f;
            tData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);

            // Se coloca la base del terreno a Y = -1.2f.
            // Así debajo del laboratorio (altura 0) la tierra queda 1.2m por debajo del piso metálico.
            // ¡Esto elimina 100% el z-fighting y evita que los suelos se crucen!
            float terrainBaseY = -1.2f;
            terrainObj.transform.position = new Vector3(-terrainWidth * 0.5f, terrainBaseY, -terrainLength * 0.5f);

            tData.terrainLayers = new TerrainLayer[] { dirtLayer, grassLayer };

            // 4. Esculpir alturas (desniveles suaves tipo campo, no montañas gigantescas)
            int hRes = 129;
            tData.heightmapResolution = hRes;
            float[,] heights = new float[hRes, hRes];

            for (int z = 0; z < hRes; z++)
            {
                float nz = (float)z / (hRes - 1);
                float worldZ = -terrainLength * 0.5f + nz * terrainLength;

                for (int x = 0; x < hRes; x++)
                {
                    float nx = (float)x / (hRes - 1);
                    float worldX = -terrainWidth * 0.5f + nx * terrainWidth;

                    // Interior del laboratorio (permanece 100% plano a nivel inferior, sin tocar el piso de la estación)
                    bool insideLab = (worldX >= -14f && worldX <= 14f && worldZ >= -16f && worldZ <= 18.5f);
                    // Rampa suave en la compuerta norte para conectar a nivel con el piso de la estación
                    bool exitThreshold = (worldX >= -4.5f && worldX <= 4.5f && worldZ > 18.5f && worldZ <= 24f);

                    if (insideLab)
                    {
                        heights[z, x] = 0f; // Superficie de tierra a Y = -1.2m en mundo (muy por debajo de las losas metálicas)
                    }
                    else if (exitThreshold)
                    {
                        // Sube suavemente para conectar con el umbral de la puerta norte a nivel Y = 0
                        float t = (worldZ - 18.5f) / 5.5f;
                        heights[z, x] = Mathf.Lerp(0f, 0.10f, t);
                    }
                    else
                    {
                        float dx = Mathf.Max(0f, Mathf.Abs(worldX) - 14f);
                        float dz = 0f;
                        if (worldZ > 18.5f) dz = worldZ - 18.5f;
                        else if (worldZ < -16f) dz = -16f - worldZ;

                        float distFromLab = Mathf.Sqrt(dx * dx + dz * dz);
                        float blendFactor = Mathf.Clamp01(distFromLab / 8f);

                        // Relieve natural tipo campo (colinas y lomas suaves de 2 a 4 metros)
                        float n1 = Mathf.PerlinNoise(nx * 3.8f + 1.2f, nz * 3.8f + 0.9f);
                        float n2 = Mathf.PerlinNoise(nx * 7.5f + 2.5f, nz * 7.5f + 1.7f) * 0.3f;

                        // Altura base exterior a nivel de suelo (0.10 = +1.2m relativo a baseY, es decir Y = 0.0m mundial)
                        float groundBase = 0.10f;
                        // Desniveles suaves y ondulaciones
                        float hillVariation = (n1 + n2) * 0.22f;

                        // Muro/borde perimetral suave (~4.5m) para delimitar el campo
                        float distFromEdge = Mathf.Min(Mathf.Min(nx, 1f - nx), Mathf.Min(nz, 1f - nz)) * 2f;
                        float perimeterRim = Mathf.Pow(1f - distFromEdge, 2f) * 0.25f;

                        float rawH = groundBase + (hillVariation + perimeterRim) * blendFactor;
                        heights[z, x] = Mathf.Clamp01(rawH);
                    }
                }
            }
            tData.SetHeights(0, 0, heights);

            // 5. Pintar texturas (Splatmap)
            int alphaRes = 128;
            tData.alphamapResolution = alphaRes;
            float[,,] alphas = new float[alphaRes, alphaRes, 2];

            for (int z = 0; z < alphaRes; z++)
            {
                float nz = (float)z / (alphaRes - 1);
                for (int x = 0; x < alphaRes; x++)
                {
                    float nx = (float)x / (alphaRes - 1);
                    float h = tData.GetHeight((int)(nx * (hRes - 1)), (int)(nz * (hRes - 1))) / terrainHeight;

                    float grassWeight = Mathf.Clamp01(h * 2.2f);
                    float dirtWeight = 1f - grassWeight;

                    alphas[z, x, 0] = dirtWeight;
                    alphas[z, x, 1] = grassWeight;
                }
            }
            tData.SetAlphamaps(0, 0, alphas);

            terrain.terrainData = tData;
            terrainCol.terrainData = tData;

            // 6. Asignar Material Template URP
            Material terrainMat = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/TerrainLit.mat");
            if (terrainMat != null)
            {
                terrain.materialTemplate = terrainMat;
            }

            // 7. Configurar la Luz Direccional (El Sol del Planeta)
            GameObject sunObj = GameObject.Find("DirectionalLight_Starlight");
            if (sunObj == null) sunObj = GameObject.Find("Directional Light");
            if (sunObj == null) sunObj = GameObject.Find("Directional Light (Sun)");
            if (sunObj == null)
            {
                sunObj = new GameObject("Directional Light (Sun)");
                sunObj.AddComponent<Light>();
                GameObject lightRoot = GameObject.Find("--- 3. LIGHTING & FX ---");
                if (lightRoot != null) sunObj.transform.SetParent(lightRoot.transform);
            }

            sunObj.name = "Directional Light (Sun)";
            Light sunLight = sunObj.GetComponent<Light>();
            if (sunLight != null)
            {
                sunLight.type = LightType.Directional;
                sunLight.color = new Color(1.0f, 0.96f, 0.88f); // Luz solar cálida
                sunLight.intensity = 1.25f; // Iluminación equilibrada
                sunLight.shadows = LightShadows.Soft;
                sunLight.shadowStrength = 0.80f;
            }
            sunObj.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

            // 8. Configurar el Cielo (Skybox de AllSky) y la Cámara
            Material skyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/AllSkyFree/Space_AnotherPlanet/AllSky_Space_AnotherPlanet.mat");
            if (skyMat == null) skyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/AllSkyFree/Epic_BlueSunset/Epic_BlueSunset.mat");
            if (skyMat != null)
            {
                RenderSettings.skybox = skyMat;
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
                DynamicGI.UpdateEnvironment();
            }

            // Configurar la cámara para que dibuje el Skybox en lugar de pantalla negra
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = GameObject.Find("FirstPersonCamera");
                if (camObj != null) mainCam = camObj.GetComponent<Camera>();
            }
            if (mainCam != null)
            {
                mainCam.clearFlags = CameraClearFlags.Skybox;
                mainCam.farClipPlane = 1000f;
            }

            RenderSettings.fog = false;

            // 9. Abrir salida en la pared Norte para conectar la estación con el exterior montañoso
            GameObject northWall = GameObject.Find("Wall_Main_North");
            if (northWall != null)
            {
                Transform parent = northWall.transform.parent;
                Object.DestroyImmediate(northWall);

                Material matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Wall_Titanium.mat");
                CreateBox("Wall_Main_North_L", new Vector3(-7f, 3f, 18f), new Vector3(10f, 6f, 0.6f), matWall, parent.gameObject);
                CreateBox("Wall_Main_North_R", new Vector3(7f, 3f, 18f), new Vector3(10f, 6f, 0.6f), matWall, parent.gameObject);
            }

            // 9. Colocar vegetación y rocas en laderas y colinas
            GameObject natureRoot = GameObject.Find("Nature_And_Rocks");
            if (natureRoot != null) Object.DestroyImmediate(natureRoot);
            natureRoot = new GameObject("Nature_And_Rocks");
            natureRoot.transform.SetParent(rootEnv.transform);

            string rockPrefabPath = "Assets/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_Generic_Rock_01.prefab";
            string rockMenhirPath = "Assets/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_Menhir_Rock_02.prefab";
            string shrubPrefabPath = "Assets/Polytope Studio/Lowpoly_Environments/Prefabs/Shrubs/PT_Generic_Shrub_01_green.prefab";
            string mushroomPrefabPath = "Assets/Polytope Studio/Lowpoly_Environments/Prefabs/Mushrooms/PT_Caesars_Mushroom_01.prefab";
            string treePrefabPath = "Assets/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_green.prefab";

            GameObject rockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rockPrefabPath);
            GameObject rockMenhir = AssetDatabase.LoadAssetAtPath<GameObject>(rockMenhirPath);
            GameObject shrubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(shrubPrefabPath);
            GameObject mushroomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mushroomPrefabPath);
            GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(treePrefabPath);

            // Rocas monumentales y de ladera
            Vector3[] rockPositions = new Vector3[]
            {
                new Vector3(-4f, 0f, 21f),
                new Vector3(4.5f, 0f, 21f),
                new Vector3(-14f, 0f, 28f),
                new Vector3(12f, 0f, 30f),
                new Vector3(-7f, 0f, 38f),
                new Vector3(8f, 0f, 44f),
                new Vector3(0f, 0f, 52f),
                new Vector3(-25f, 0f, 35f),
                new Vector3(25f, 0f, 35f)
            };
            foreach (var pos in rockPositions)
            {
                GameObject pref = (pos.z > 35f && rockMenhir != null) ? rockMenhir : rockPrefab;
                if (pref != null)
                {
                    GameObject r = (GameObject)PrefabUtility.InstantiatePrefab(pref, natureRoot.transform);
                    // Sumar la altura de la base del terreno (-1.2f) e incrustar ligeramente la roca en la tierra (-0.35f)
                    float y = terrain.SampleHeight(pos) + terrain.transform.position.y - 0.35f;
                    r.transform.position = new Vector3(pos.x, y, pos.z);
                    r.transform.localScale = Vector3.one * Random.Range(1.8f, 3.0f);
                    r.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                }
            }

            // Arbustos en laderas y cerca del sendero
            Vector3[] shrubPositions = new Vector3[]
            {
                new Vector3(-2.8f, 0f, 19f),
                new Vector3(3.2f, 0f, 19.5f),
                new Vector3(-9f, 0f, 25f),
                new Vector3(7f, 0f, 27f),
                new Vector3(-3.5f, 0f, 34f),
                new Vector3(4f, 0f, 39f),
                new Vector3(-1f, 0f, 46f)
            };
            foreach (var pos in shrubPositions)
            {
                if (shrubPrefab != null)
                {
                    GameObject s = (GameObject)PrefabUtility.InstantiatePrefab(shrubPrefab, natureRoot.transform);
                    float y = terrain.SampleHeight(pos) + terrain.transform.position.y - 0.1f;
                    s.transform.position = new Vector3(pos.x, y, pos.z);
                    s.transform.localScale = Vector3.one * Random.Range(1.0f, 1.5f);
                }
            }

            // Hongos alienígenas luminosos
            Vector3[] mushroomPositions = new Vector3[]
            {
                new Vector3(10f, 0f, 4f),
                new Vector3(-10.5f, 0f, 10f),
                new Vector3(-3.5f, 0f, 20f),
                new Vector3(3.5f, 0f, 22f),
                new Vector3(2f, 0f, 36f)
            };
            foreach (var pos in mushroomPositions)
            {
                if (mushroomPrefab != null)
                {
                    GameObject m = (GameObject)PrefabUtility.InstantiatePrefab(mushroomPrefab, natureRoot.transform);
                    // Si está en el interior de la estación (Z < 18), se asienta a Y = 0; si está fuera, en el terreno
                    float y = (pos.z < 18f) ? 0f : (terrain.SampleHeight(pos) + terrain.transform.position.y);
                    m.transform.position = new Vector3(pos.x, y, pos.z);
                    m.transform.localScale = Vector3.one * 1.5f;
                }
            }

            // Árboles en colinas
            Vector3[] treePositions = new Vector3[]
            {
                new Vector3(-20f, 0f, 32f),
                new Vector3(18f, 0f, 36f),
                new Vector3(-12f, 0f, 48f),
                new Vector3(14f, 0f, 50f),
                new Vector3(-28f, 0f, 55f),
                new Vector3(26f, 0f, 55f)
            };
            foreach (var pos in treePositions)
            {
                if (treePrefab != null)
                {
                    GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(treePrefab, natureRoot.transform);
                    // Asentar las raíces del árbol directamente en la superficie del terreno
                    float y = terrain.SampleHeight(pos) + terrain.transform.position.y - 0.2f;
                    t.transform.position = new Vector3(pos.x, y, pos.z);
                    t.transform.localScale = Vector3.one * Random.Range(1.4f, 2.0f);
                }
            }

            // 10. Actualizar las 5 monedas
            GameObject interactives = GameObject.Find("InteractiveObjects");
            if (interactives == null)
            {
                interactives = new GameObject("InteractiveObjects");
                interactives.transform.SetParent(rootEnv.transform);
            }

            string[] toRemove = new string[] { "EnergyCore_1", "EnergyCore_2", "EnergyCore_3", "GoldCoin_1", "GoldCoin_2", "GoldCoin_3", "GoldCoin_4", "GoldCoin_5" };
            foreach (string objName in toRemove)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj != null) Object.DestroyImmediate(obj);
            }

            Material matCoinGold = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Coin_Gold.mat");

            // Moneda 5 en la cima de la colina exterior
            Vector3 coin5Pos = new Vector3(0f, 0f, 38f);
            float coin5Y = terrain.SampleHeight(coin5Pos) + terrain.transform.position.y + 1.2f;
            coin5Pos.y = coin5Y;

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0f, 1.1f, -9.5f),   // Moneda 1: Habitación inicial (Airlock)
                new Vector3(-6.5f, 1.4f, -2.5f), // Moneda 2: Mesa de laboratorio
                new Vector3(-2.0f, 1.2f, 2.5f),  // Moneda 3: Estación central
                new Vector3(8.5f, 3.4f, 13.0f),  // Moneda 4: Plataforma elevada de observación
                coin5Pos                         // Moneda 5: En lo alto de la colina exterior (evidencia de desniveles)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                CreateGoldCoinPickup($"GoldCoin_{i + 1}", positions[i], interactives, matCoinGold);
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();

            Debug.Log("¡Terreno montañoso, Sol planetario, vegetación y 5 monedas configurados con éxito!");
        }

        [MenuItem("Tools/Generar Escenario Sci-Fi PA3")]
        public static void BuildCompleteSciFiScene()
        {
            Debug.Log("--- INICIANDO CONSTRUCCIÓN DEL ESCENARIO SCI-FI PA3 ---");

            // 1. Asegurar directorios
            EnsureFolders();

            // 2. Crear nueva escena
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 3. Crear materiales PBR Sci-Fi
            Material matFloor = CreateMaterial("Mat_Floor_Dark", new Color(0.12f, 0.14f, 0.16f), 0.7f, 0.4f);
            Material matFloorPlatform = CreateMaterial("Mat_Floor_Platform", new Color(0.18f, 0.22f, 0.26f), 0.8f, 0.5f);
            Material matWall = CreateMaterial("Mat_Wall_Titanium", new Color(0.35f, 0.38f, 0.42f), 0.5f, 0.3f);
            Material matWallDark = CreateMaterial("Mat_Wall_Dark", new Color(0.15f, 0.17f, 0.20f), 0.6f, 0.3f);
            Material matPipes = CreateMaterial("Mat_Pipes_Copper", new Color(0.7f, 0.45f, 0.25f), 0.85f, 0.7f);
            Material matCrate = CreateMaterial("Mat_Crate_Orange", new Color(0.85f, 0.45f, 0.1f), 0.3f, 0.4f);
            Material matCrateMetal = CreateMaterial("Mat_Crate_Dark", new Color(0.2f, 0.22f, 0.25f), 0.7f, 0.5f);
            Material matBarrel = CreateMaterial("Mat_Barrel_Steel", new Color(0.25f, 0.3f, 0.35f), 0.8f, 0.6f);
            Material matRailing = CreateMaterial("Mat_Railing_Yellow", new Color(0.95f, 0.8f, 0.1f), 0.4f, 0.5f);
            Material matGlass = CreateMaterial("Mat_Glass_Translucent", new Color(0.1f, 0.6f, 0.8f, 0.4f), 0.9f, 0.9f);

            // Materiales emisivos (Neón)
            Material matNeonCyan = CreateEmissiveMaterial("Mat_Neon_Cyan", new Color(0.1f, 0.8f, 1f), new Color(0.2f, 0.9f, 1f) * 2.5f);
            Material matNeonOrange = CreateEmissiveMaterial("Mat_Neon_Orange", new Color(1f, 0.5f, 0.1f), new Color(1f, 0.45f, 0.1f) * 2.5f);
            Material matNeonGreen = CreateEmissiveMaterial("Mat_Neon_Green", new Color(0.2f, 1f, 0.4f), new Color(0.2f, 1f, 0.4f) * 2.8f);
            Material matNeonRed = CreateEmissiveMaterial("Mat_Neon_Red", new Color(1f, 0.15f, 0.15f), new Color(1f, 0.15f, 0.15f) * 2.5f);
            Material matCoinGold = CreateMaterial("Mat_Coin_Gold", new Color(1f, 0.84f, 0.05f), 0.95f, 0.85f);
            matCoinGold.EnableKeyword("_EMISSION");
            matCoinGold.SetColor("_EmissionColor", new Color(1.3f, 0.95f, 0.1f));

            // 4. Crear estructura raíz de la jerarquía
            GameObject rootManagement = new GameObject("--- 1. MANAGEMENT ---");
            GameObject rootEnvironment = new GameObject("--- 2. ENVIRONMENT ---");
            GameObject rootArchitecture = new GameObject("Architecture");
            rootArchitecture.transform.SetParent(rootEnvironment.transform);
            GameObject rootDecorations = new GameObject("Decorations");
            rootDecorations.transform.SetParent(rootEnvironment.transform);
            GameObject rootInteractables = new GameObject("InteractiveObjects");
            rootInteractables.transform.SetParent(rootEnvironment.transform);

            GameObject rootLighting = new GameObject("--- 3. LIGHTING & FX ---");
            GameObject rootPlayer = new GameObject("--- 4. PLAYER ---");

            // 5. Configurar GameManager & UI
            GameObject uiObj = new GameObject("UIManager");
            uiObj.transform.SetParent(rootManagement.transform);
            uiObj.AddComponent<UIManager>();
            uiObj.AddComponent<ProceduralAudio>();

            // 6. Construir Arquitectura (Pisos, Paredes, Pasillo, Rampa, Mirador)
            // ==========================================
            // SALA DE ENTRADA / COMPUERTA (Z: -14 a -6, X: -4 a 4)
            CreateBox("Floor_Airlock", new Vector3(0f, -0.25f, -10f), new Vector3(8f, 0.5f, 8f), matFloor, rootArchitecture);
            CreateBox("Ceiling_Airlock", new Vector3(0f, 4.25f, -10f), new Vector3(8f, 0.5f, 8f), matWallDark, rootArchitecture);
            CreateBox("Wall_Airlock_Back", new Vector3(0f, 2f, -14f), new Vector3(8f, 4f, 0.5f), matWall, rootArchitecture);
            CreateBox("Wall_Airlock_Left", new Vector3(-4f, 2f, -10f), new Vector3(0.5f, 4f, 8f), matWall, rootArchitecture);
            CreateBox("Wall_Airlock_Right", new Vector3(4f, 2f, -10f), new Vector3(0.5f, 4f, 8f), matWall, rootArchitecture);

            // Marco de compuerta entre Airlock y Sala Principal (Z: -6)
            CreateBox("DoorFrame_Left", new Vector3(-2.75f, 2f, -6f), new Vector3(2.5f, 4f, 0.6f), matWallDark, rootArchitecture);
            CreateBox("DoorFrame_Right", new Vector3(2.75f, 2f, -6f), new Vector3(2.5f, 4f, 0.6f), matWallDark, rootArchitecture);
            CreateBox("DoorFrame_Top", new Vector3(0f, 3.6f, -6f), new Vector3(3f, 0.8f, 0.6f), matWallDark, rootArchitecture);

            // ==========================================
            // SALA PRINCIPAL DEL REACTOR (Z: -6 a 18, X: -12 a 12, Y: 0 a 6)
            // Piso principal
            CreateBox("Floor_MainRoom", new Vector3(0f, -0.25f, 6f), new Vector3(24f, 0.5f, 24f), matFloor, rootArchitecture);
            // Techo de la sala
            CreateBox("Ceiling_MainRoom", new Vector3(0f, 6.25f, 6f), new Vector3(24f, 0.5f, 24f), matWallDark, rootArchitecture);
            // Paredes perimetrales
            CreateBox("Wall_Main_South_L", new Vector3(-8f, 3f, -6f), new Vector3(8f, 6f, 0.6f), matWall, rootArchitecture);
            CreateBox("Wall_Main_South_R", new Vector3(8f, 3f, -6f), new Vector3(8f, 6f, 0.6f), matWall, rootArchitecture);
            CreateBox("Wall_Main_North", new Vector3(0f, 3f, 18f), new Vector3(24f, 6f, 0.6f), matWall, rootArchitecture);
            CreateBox("Wall_Main_West", new Vector3(-12f, 3f, 6f), new Vector3(0.6f, 6f, 24f), matWall, rootArchitecture);
            CreateBox("Wall_Main_East", new Vector3(12f, 3f, 6f), new Vector3(0.6f, 6f, 24f), matWall, rootArchitecture);

            // Columnas y vigas arquitectónicas Sci-Fi
            float[] colX = new float[] { -8f, 8f };
            float[] colZ = new float[] { -2f, 6f, 14f };
            foreach (float cx in colX)
            {
                foreach (float cz in colZ)
                {
                    GameObject col = CreateBox($"Column_{cx}_{cz}", new Vector3(cx, 3f, cz), new Vector3(1f, 6f, 1f), matWallDark, rootArchitecture);
                    // Anillo de neón en la columna
                    CreateBox($"ColNeon_{cx}_{cz}", new Vector3(cx, 2.5f, cz), new Vector3(1.05f, 0.15f, 1.05f), matNeonCyan, col);
                }
            }

            // ==========================================
            // DESNIVELES Y PLATAFORMA ELEVADA (Requisito mínimo 3: Plataformas o desniveles)
            // Plataforma de Observación / Sala de Control Elevada (X: 5 a 11.5, Z: 8 a 17, Y: 2.5f)
            CreateBox("Platform_Elevated", new Vector3(8.5f, 2.4f, 12.5f), new Vector3(6.5f, 0.4f, 10f), matFloorPlatform, rootArchitecture);

            // Barandillas de seguridad en la plataforma elevada
            CreateBox("Railing_Front", new Vector3(5.3f, 3.1f, 12.5f), new Vector3(0.15f, 1.0f, 10f), matRailing, rootArchitecture);
            CreateBox("Railing_Back", new Vector3(8.5f, 3.1f, 7.6f), new Vector3(6.2f, 1.0f, 0.15f), matRailing, rootArchitecture);

            // Rampa de acceso a la plataforma (de Z: 2 a Z: 7.5, sube de Y:0 a Y:2.5)
            GameObject ramp = CreateBox("Ramp_Access", new Vector3(8.5f, 1.2f, 4.8f), new Vector3(3.2f, 0.3f, 5.8f), matFloorPlatform, rootArchitecture);
            ramp.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);

            // Barandilla de rampa
            GameObject rampRailing = CreateBox("Ramp_Railing", new Vector3(6.8f, 1.8f, 4.8f), new Vector3(0.15f, 1f, 5.8f), matRailing, rootArchitecture);
            rampRailing.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);

            // ==========================================
            // 7. OBJETOS INTERACTIVOS (Requisito 8: Triggers / Colisiones)
            // ==========================================
            // INTERACTIVO 1: COMPUERTA CORREDIZA DE PRESIÓN
            GameObject doorObj = new GameObject("SlidingDoor_Security");
            doorObj.transform.SetParent(rootInteractables.transform);
            doorObj.transform.position = new Vector3(0f, 0f, -6f);

            // Panel móvil de la puerta
            GameObject doorPanel = CreateBox("DoorPanel", new Vector3(0f, 1.6f, -6f), new Vector3(2.8f, 3.2f, 0.25f), matCrateMetal, doorObj);
            // Franja de luz en la puerta
            GameObject doorLightStrip = CreateBox("LightStrip", new Vector3(0f, 1.6f, -5.85f), new Vector3(0.2f, 2.8f, 0.05f), matNeonRed, doorPanel);

            // Trigger de detección de proximidad
            BoxCollider doorTrigger = doorObj.AddComponent<BoxCollider>();
            doorTrigger.isTrigger = true;
            doorTrigger.size = new Vector3(4f, 3.5f, 5f);
            doorTrigger.center = new Vector3(0f, 1.8f, 0f);

            SlidingDoor slidingDoorScript = doorObj.AddComponent<SlidingDoor>();
            SerializedObject doorSO = new SerializedObject(slidingDoorScript);
            doorSO.FindProperty("doorPanel").objectReferenceValue = doorPanel.transform;
            doorSO.FindProperty("statusLightRenderer").objectReferenceValue = doorLightStrip.GetComponent<Renderer>();
            doorSO.FindProperty("lockedMaterial").objectReferenceValue = matNeonRed;
            doorSO.FindProperty("openMaterial").objectReferenceValue = matNeonGreen;
            doorSO.FindProperty("openOffset").vector3Value = new Vector3(0f, 3.3f, 0f);
            doorSO.FindProperty("slideSpeed").floatValue = 4.0f;
            doorSO.ApplyModifiedProperties();

            // ==========================================
            // INTERACTIVO 2: REACTOR CENTRAL Y CONSOLA DE CONTROL
            // Reactor Central (Cilindro con bobinas, núcleo brillante y partículas)
            GameObject reactorGroup = new GameObject("CentralReactor_Core");
            reactorGroup.transform.SetParent(rootDecorations.transform);
            reactorGroup.transform.position = new Vector3(-2f, 0f, 6f);

            // Base cilíndrica del reactor
            CreateCylinder("Reactor_Base", new Vector3(-2f, 0.4f, 6f), new Vector3(4.5f, 0.4f, 4.5f), matWallDark, reactorGroup);
            CreateCylinder("Reactor_SubBase", new Vector3(-2f, 1.0f, 6f), new Vector3(3.5f, 0.3f, 3.5f), matCrateMetal, reactorGroup);

            // Cilindro de contención transparente
            GameObject glassCylinder = CreateCylinder("Reactor_Glass", new Vector3(-2f, 2.6f, 6f), new Vector3(2.6f, 1.4f, 2.6f), matGlass, reactorGroup);

            // Núcleo brillante giratorio (Levitante y rotatorio)
            GameObject coreOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coreOrb.name = "Reactor_PlasmaCore";
            coreOrb.transform.SetParent(reactorGroup.transform);
            coreOrb.transform.position = new Vector3(-2f, 2.6f, 6f);
            coreOrb.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            coreOrb.GetComponent<Renderer>().material = matNeonCyan;
            SimpleRotator coreRotator = coreOrb.AddComponent<SimpleRotator>();
            SerializedObject rotSO = new SerializedObject(coreRotator);
            rotSO.FindProperty("rotationSpeed").vector3Value = new Vector3(30f, 75f, 15f);
            rotSO.FindProperty("bobbingEffect").boolValue = true;
            rotSO.FindProperty("bobbingSpeed").floatValue = 2.5f;
            rotSO.FindProperty("bobbingHeight").floatValue = 0.2f;
            rotSO.ApplyModifiedProperties();

            // Anillos magnéticos giratorios alrededor del núcleo
            GameObject ring1 = CreateCylinder("MagneticRing_1", new Vector3(-2f, 2.6f, 6f), new Vector3(3.1f, 0.1f, 3.1f), matWallDark, reactorGroup);
            CreateBox("Ring1_Neon", new Vector3(-2f, 2.6f, 6f), new Vector3(3.2f, 0.12f, 0.4f), matNeonCyan, ring1);
            SimpleRotator ringRot1 = ring1.AddComponent<SimpleRotator>();
            SerializedObject ring1SO = new SerializedObject(ringRot1);
            ring1SO.FindProperty("rotationSpeed").vector3Value = new Vector3(0f, -40f, 0f);
            ring1SO.ApplyModifiedProperties();

            // Cúpula superior del reactor
            CreateCylinder("Reactor_Cap", new Vector3(-2f, 4.3f, 6f), new Vector3(3.5f, 0.4f, 3.5f), matWallDark, reactorGroup);
            CreateBox("Reactor_TopPillar", new Vector3(-2f, 5.2f, 6f), new Vector3(1.2f, 0.8f, 1.2f), matPipes, reactorGroup);

            // SISTEMA DE PARTÍCULAS DEL REACTOR (Requisito 9)
            GameObject particleObj = new GameObject("Particle_ReactorEnergy");
            particleObj.transform.SetParent(reactorGroup.transform);
            particleObj.transform.position = new Vector3(-2f, 2.6f, 6f);
            ParticleSystem psReactor = SetupReactorParticleSystem(particleObj, Color.cyan);

            // Chispas del reactor
            GameObject sparkObj = new GameObject("Particle_ElectricSparks");
            sparkObj.transform.SetParent(reactorGroup.transform);
            sparkObj.transform.position = new Vector3(-2f, 1.5f, 6f);
            ParticleSystem psSparks = SetupSparkParticleSystem(sparkObj);

            // Luz central del reactor
            GameObject reactorLightObj = new GameObject("Light_ReactorCore");
            reactorLightObj.transform.SetParent(reactorGroup.transform);
            reactorLightObj.transform.position = new Vector3(-2f, 2.8f, 6f);
            Light reactorLight = reactorLightObj.AddComponent<Light>();
            reactorLight.type = LightType.Point;
            reactorLight.color = Color.cyan;
            reactorLight.range = 14f;
            reactorLight.intensity = 2.5f;
            PulsingLight pulseScript = reactorLightObj.AddComponent<PulsingLight>();
            SerializedObject pulseSO = new SerializedObject(pulseScript);
            pulseSO.FindProperty("minIntensity").floatValue = 1.5f;
            pulseSO.FindProperty("maxIntensity").floatValue = 3.5f;
            pulseSO.FindProperty("pulseSpeed").floatValue = 3f;
            pulseSO.ApplyModifiedProperties();

            // TERMINAL DECORATIVA (Ubicada en la plataforma elevada)
            GameObject consoleObj = new GameObject("LabConsole_Terminal");
            consoleObj.transform.SetParent(rootInteractables.transform);
            consoleObj.transform.position = new Vector3(7f, 2.6f, 12f);
            consoleObj.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

            // Mueble de la consola
            CreateBox("Console_Pedestal", new Vector3(7f, 3.1f, 12f), new Vector3(1.2f, 1.0f, 0.7f), matWallDark, consoleObj);
            // Pantalla inclinada
            GameObject screenObj = CreateBox("Console_Screen", new Vector3(7f, 3.75f, 12.05f), new Vector3(1.0f, 0.6f, 0.1f), matNeonOrange, consoleObj);
            screenObj.transform.localRotation = Quaternion.Euler(25f, 0f, 0f);

            // Teclado/Panel de botones
            CreateBox("Console_Keyboard", new Vector3(7f, 3.6f, 11.8f), new Vector3(0.9f, 0.08f, 0.35f), matCrateMetal, consoleObj);

            // Collider para interacción
            BoxCollider consoleCol = consoleObj.AddComponent<BoxCollider>();
            consoleCol.size = new Vector3(1.8f, 2.2f, 1.8f);
            consoleCol.center = new Vector3(0f, 0.8f, 0f);

            // ==========================================
            // INTERACTIVO 3: MONEDAS DORADAS COLECCIONABLES (5 MONEDAS EN EL ESCENARIO)
            CreateGoldCoinPickup("GoldCoin_1", new Vector3(0f, 1.1f, -9.5f), rootInteractables, matCoinGold);   // Habitación inicial (Airlock)
            CreateGoldCoinPickup("GoldCoin_2", new Vector3(-6.5f, 1.4f, -2.5f), rootInteractables, matCoinGold); // Mesa de laboratorio
            CreateGoldCoinPickup("GoldCoin_3", new Vector3(-2.0f, 1.2f, 2.5f), rootInteractables, matCoinGold);  // Frente al reactor central
            CreateGoldCoinPickup("GoldCoin_4", new Vector3(-8.5f, 1.2f, 12.0f), rootInteractables, matCoinGold); // Almacén de cajas y barriles
            CreateGoldCoinPickup("GoldCoin_5", new Vector3(8.5f, 3.4f, 13.0f), rootInteractables, matCoinGold);  // Plataforma elevada de observación

            // ==========================================
            // 8. OBJETOS DECORATIVOS (Requisito 3: Al menos 5 objetos decorativos)
            // ==========================================
            // Decorativo 1: Terminales y Servidores de datos en pared oeste
            for (int i = 0; i < 3; i++)
            {
                float zPos = -1f + (i * 2.8f);
                GameObject server = CreateBox($"ServerRack_{i}", new Vector3(-11.3f, 1.8f, zPos), new Vector3(0.8f, 3.6f, 1.8f), matWallDark, rootDecorations);
                // Luces de servidores
                CreateBox($"ServerLights_A_{i}", new Vector3(-10.85f, 2.5f, zPos), new Vector3(0.05f, 0.4f, 1.4f), matNeonCyan, server);
                CreateBox($"ServerLights_B_{i}", new Vector3(-10.85f, 1.4f, zPos), new Vector3(0.05f, 0.3f, 1.4f), matNeonOrange, server);
            }

            // Decorativo 2: Cajas de Carga Sci-Fi Apiladas (Zona de almacenamiento)
            CreateSciFiCrate(new Vector3(-9.5f, 0.6f, 13f), new Vector3(1.2f, 1.2f, 1.2f), matCrate, matCrateMetal, rootDecorations);
            CreateSciFiCrate(new Vector3(-8.2f, 0.6f, 13.2f), new Vector3(1.2f, 1.2f, 1.2f), matCrate, matCrateMetal, rootDecorations);
            CreateSciFiCrate(new Vector3(-8.8f, 1.8f, 13.1f), new Vector3(1.0f, 1.0f, 1.0f), matCrate, matCrateMetal, rootDecorations);
            CreateSciFiCrate(new Vector3(-9.2f, 0.5f, 15f), new Vector3(1.0f, 1.0f, 1.0f), matCrateMetal, matCrate, rootDecorations);

            // Decorativo 3: Barriles de Combustible Criogénico
            CreateSciFiBarrel(new Vector3(-7f, 0.8f, 15.5f), matBarrel, matNeonCyan, rootDecorations);
            CreateSciFiBarrel(new Vector3(-5.7f, 0.8f, 15.8f), matBarrel, matNeonCyan, rootDecorations);
            CreateSciFiBarrel(new Vector3(-6.4f, 0.8f, 14.5f), matBarrel, matNeonCyan, rootDecorations);

            // Decorativo 4: Tuberías Industriales en techo y paredes
            GameObject pipeObj = CreateCylinder("IndustrialPipe_Long", new Vector3(-11.5f, 4.5f, 6f), new Vector3(0.4f, 11f, 0.4f), matPipes, rootDecorations);
            pipeObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            GameObject pipeObj2 = CreateCylinder("IndustrialPipe_Ceiling", new Vector3(0f, 5.5f, 6f), new Vector3(0.35f, 10f, 0.35f), matPipes, rootDecorations);
            pipeObj2.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            // Decorativo 5: Mesa de Laboratorio con Holograma / Muestras
            GameObject labTable = CreateBox("LabWorkstation", new Vector3(-6.5f, 0.6f, -2.5f), new Vector3(3.2f, 1.2f, 1.4f), matWallDark, rootDecorations);
            CreateBox("LabScreen", new Vector3(-6.5f, 1.5f, -2.9f), new Vector3(1.4f, 0.6f, 0.1f), matNeonCyan, labTable);
            // Matraz / Contenedor químico cilíndrico en la mesa
            CreateCylinder("ChemicalVial", new Vector3(-7.2f, 1.45f, -2.4f), new Vector3(0.25f, 0.35f, 0.25f), matNeonGreen, labTable);

            // ==========================================
            // 9. ILUMINACIÓN Y ATMÓSFERA (Requisito 7: Atmósfera adecuada)
            // ==========================================
            // Luz direccional suave simulando radiación estelar tenue desde el exterior
            GameObject dirLightObj = new GameObject("DirectionalLight_Starlight");
            dirLightObj.transform.SetParent(rootLighting.transform);
            Light dirLight = dirLightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
            dirLight.color = new Color(0.2f, 0.3f, 0.45f);
            dirLight.intensity = 0.35f;
            dirLight.shadows = LightShadows.Soft;
            dirLightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Luz cenital de la entrada (Airlock)
            CreateCeilingLight("Lamp_Airlock", new Vector3(0f, 3.8f, -10f), new Color(0.2f, 0.7f, 1.0f), 2.2f, 9f, rootLighting, matNeonCyan);

            // Luces de techo de la sala principal
            CreateCeilingLight("Lamp_Main_1", new Vector3(-5f, 5.8f, 0f), new Color(0.3f, 0.8f, 1f), 2.0f, 14f, rootLighting, matNeonCyan);
            CreateCeilingLight("Lamp_Main_2", new Vector3(5f, 5.8f, 0f), new Color(0.3f, 0.8f, 1f), 2.0f, 14f, rootLighting, matNeonCyan);
            CreateCeilingLight("Lamp_Main_3", new Vector3(-5f, 5.8f, 12f), new Color(0.3f, 0.8f, 1f), 2.0f, 14f, rootLighting, matNeonCyan);
            CreateCeilingLight("Lamp_Main_4", new Vector3(8.5f, 5.8f, 12.5f), new Color(1.0f, 0.7f, 0.3f), 2.2f, 12f, rootLighting, matNeonOrange);

            // Configurar RenderSettings (Atmósfera nocturna sci-fi)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.10f, 0.14f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.04f, 0.06f, 0.10f);
            RenderSettings.fogDensity = 0.015f;
            RenderSettings.fogMode = FogMode.ExponentialSquared;

            // ==========================================
            // 10. PERSONAJE EN PRIMERA PERSONA (Requisitos 4 y 5)
            // ==========================================
            GameObject player = new GameObject("Player_FPS");
            player.tag = "Player";
            player.transform.SetParent(rootPlayer.transform);
            player.transform.position = new Vector3(0f, 1.0f, -12f); // Inicia en el Airlock mirando a la compuerta
            player.transform.rotation = Quaternion.identity;

            // Character Controller
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 1.9f;
            cc.radius = 0.45f;
            cc.center = new Vector3(0f, 0.95f, 0f);
            cc.stepOffset = 0.4f;

            // Cámara en primera persona
            GameObject camObj = new GameObject("FirstPersonCamera");
            camObj.transform.SetParent(player.transform);
            camObj.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            camObj.transform.localRotation = Quaternion.identity;

            Camera cam = camObj.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.fieldOfView = 70f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 200f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.02f, 0.03f, 0.05f);

            camObj.AddComponent<AudioListener>();

            // Script del jugador
            PlayerController pc = player.AddComponent<PlayerController>();
            SerializedObject pcSO = new SerializedObject(pc);
            pcSO.FindProperty("playerCamera").objectReferenceValue = cam;
            pcSO.ApplyModifiedProperties();

            // Modelo esquemático del jugador (oculto para primera persona, pero presente para colisiones y sombras)
            GameObject playerMesh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerMesh.name = "BodyVisual";
            playerMesh.transform.SetParent(player.transform);
            playerMesh.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            playerMesh.transform.localScale = new Vector3(0.8f, 0.95f, 0.8f);
            // Destruir collider del mesh para no chocar con el CharacterController
            Object.DestroyImmediate(playerMesh.GetComponent<Collider>());
            playerMesh.GetComponent<Renderer>().material = matCrateMetal;

            // Guardar Prefabs para cumplir Requisito 10 (GameObjects, Prefabs y jerarquía ordenada)
            SavePrefab(doorObj, "Assets/_Project/Prefabs/SlidingDoor.prefab");
            SavePrefab(consoleObj, "Assets/_Project/Prefabs/LabConsole.prefab");
            SavePrefab(labTable, "Assets/_Project/Prefabs/LabWorkstation.prefab");

            // Guardar Escena
            string scenePath = "Assets/_Project/Scenes/SciFi_Lab_PA3.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"Escena guardada exitosamente en: {scenePath}");

            // Agregar escena a EditorBuildSettings
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            // Convertir materiales a URP para evitar shader incompatible
            URPMaterialFixer.ConvertAllMaterialsToURP();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("--- CONSTRUCCIÓN DE ESCENARIO SCI-FI FINALIZADA CON ÉXITO ---");
        }

        private static void EnsureFolders()
        {
            string[] dirs = new string[]
            {
                "Assets/_Project",
                "Assets/_Project/Scenes",
                "Assets/_Project/Materials",
                "Assets/_Project/Prefabs",
                "Assets/_Project/Scripts"
            };

            foreach (string dir in dirs)
            {
                if (!AssetDatabase.IsValidFolder(dir))
                {
                    string parent = Path.GetDirectoryName(dir).Replace("\\", "/");
                    string folderName = Path.GetFileName(dir);
                    AssetDatabase.CreateFolder(parent, folderName);
                }
            }
        }

        private static GameObject CreateBox(string name, Vector3 pos, Vector3 scale, Material mat, GameObject parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = scale;
            if (parent != null) obj.transform.SetParent(parent.transform);
            if (mat != null) obj.GetComponent<Renderer>().material = mat;
            return obj;
        }

        private static GameObject CreateCylinder(string name, Vector3 pos, Vector3 scale, Material mat, GameObject parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = scale;
            if (parent != null) obj.transform.SetParent(parent.transform);
            if (mat != null) obj.GetComponent<Renderer>().material = mat;
            return obj;
        }

        private static GameObject CreateCeilingLight(string name, Vector3 pos, Color lightColor, float intensity, float range, GameObject parent, Material emissiveMat)
        {
            GameObject group = new GameObject(name);
            group.transform.SetParent(parent.transform);
            group.transform.position = pos;

            // Soporte físico
            CreateBox("Fixture_Base", pos + new Vector3(0f, 0.15f, 0f), new Vector3(1.2f, 0.1f, 0.4f), null, group);
            GameObject lamp = CreateBox("Fixture_Neon", pos, new Vector3(1.0f, 0.08f, 0.25f), emissiveMat, group);

            // Fuente de luz
            GameObject lightChild = new GameObject("PointLight");
            lightChild.transform.SetParent(group.transform);
            lightChild.transform.position = pos - new Vector3(0f, 0.2f, 0f);

            Light lightComp = lightChild.AddComponent<Light>();
            lightComp.type = LightType.Point;
            lightComp.color = lightColor;
            lightComp.intensity = intensity;
            lightComp.range = range;
            lightComp.shadows = LightShadows.Soft;

            return group;
        }

        private static void CreateSciFiCrate(Vector3 pos, Vector3 scale, Material bodyMat, Material trimMat, GameObject parent)
        {
            GameObject crate = CreateBox("Crate_Container", pos, scale, bodyMat, parent);
            // Marco o refuerzo metálico
            CreateBox("Trim_X", pos, new Vector3(scale.x * 1.02f, scale.y * 0.15f, scale.z * 1.02f), trimMat, crate);
            CreateBox("Trim_Y", pos, new Vector3(scale.x * 0.15f, scale.y * 1.02f, scale.z * 1.02f), trimMat, crate);
        }

        private static void CreateSciFiBarrel(Vector3 pos, Material bodyMat, Material neonMat, GameObject parent)
        {
            GameObject barrel = CreateCylinder("EnergyBarrel", pos, new Vector3(1.0f, 0.8f, 1.0f), bodyMat, parent);
            // Banda central de neón
            CreateCylinder("Barrel_NeonRing", pos, new Vector3(1.02f, 0.12f, 1.02f), neonMat, barrel);
        }

        private static void CreateGoldCoinPickup(string name, Vector3 pos, GameObject parent, Material goldMat)
        {
            GameObject coin = new GameObject(name);
            coin.transform.SetParent(parent.transform);
            coin.transform.position = pos;

            // 1. Disco principal de la moneda (Cilindro vertical plano)
            GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "CoinDisc";
            disc.transform.SetParent(coin.transform, false);
            disc.transform.localPosition = Vector3.zero;
            disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            disc.transform.localScale = new Vector3(0.75f, 0.04f, 0.75f);
            if (goldMat != null) disc.GetComponent<Renderer>().material = goldMat;
            Object.DestroyImmediate(disc.GetComponent<Collider>());

            // 2. Relieve interior de la moneda
            GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "CoinRim";
            rim.transform.SetParent(coin.transform, false);
            rim.transform.localPosition = Vector3.zero;
            rim.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            rim.transform.localScale = new Vector3(0.58f, 0.048f, 0.58f);
            if (goldMat != null) rim.GetComponent<Renderer>().material = goldMat;
            Object.DestroyImmediate(rim.GetComponent<Collider>());

            // 3. Estrella / Emblema central de la moneda
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Cube);
            star.name = "CoinStar";
            star.transform.SetParent(coin.transform, false);
            star.transform.localPosition = Vector3.zero;
            star.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            star.transform.localScale = new Vector3(0.24f, 0.24f, 0.056f);
            if (goldMat != null) star.GetComponent<Renderer>().material = goldMat;
            Object.DestroyImmediate(star.GetComponent<Collider>());

            // 4. Rotación continua y flotación suave (SimpleRotator)
            SimpleRotator rot = coin.AddComponent<SimpleRotator>();
            rot.rotationSpeed = new Vector3(0f, 150f, 0f);
            rot.bobbingEffect = true;
            rot.bobbingSpeed = 2.8f;
            rot.bobbingHeight = 0.12f;

            // 5. Luz dorada resplandeciente
            GameObject lightObj = new GameObject("CoinGlow");
            lightObj.transform.SetParent(coin.transform, false);
            Light pLight = lightObj.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(1f, 0.85f, 0.2f);
            pLight.intensity = 1.6f;
            pLight.range = 3.5f;

            // 6. Trigger Collider para detección física
            SphereCollider sc = coin.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.8f;

            coin.AddComponent<EnergyCorePickup>();
        }

        private static ParticleSystem SetupReactorParticleSystem(GameObject target, Color color)
        {
            ParticleSystem ps = target.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startSpeed = 3f;
            main.startSize = 0.25f;
            main.startLifetime = 2.0f;
            main.startColor = color;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 500;

            var emission = ps.emission;
            emission.rateOverTime = 30;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.8f;

            var velOverLife = ps.velocityOverLifetime;
            velOverLife.enabled = true;
            velOverLife.y = new ParticleSystem.MinMaxCurve(1.5f);

            var colOverLife = ps.colorOverLifetime;
            colOverLife.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            colOverLife.color = grad;

            return ps;
        }

        private static ParticleSystem SetupSparkParticleSystem(GameObject target)
        {
            ParticleSystem ps = target.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startSpeed = 6f;
            main.startSize = 0.12f;
            main.startLifetime = 0.6f;
            main.startColor = new Color(1f, 0.9f, 0.3f);
            main.gravityModifier = 1.2f;
            main.playOnAwake = false;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15, 30) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 35f;
            shape.radius = 0.2f;

            return ps;
        }

        private static Material CreateMaterial(string name, Color color, float metallic, float smoothness)
        {
            string path = $"Assets/_Project/Materials/{name}.mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Mobile/Diffuse");

            Material mat = new Material(shader);
            mat.name = name;

            // Compatibilidad Universal y Built-in
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static Material CreateEmissiveMaterial(string name, Color baseColor, Color emissiveColor)
        {
            string path = $"Assets/_Project/Materials/{name}.mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Mobile/Diffuse");

            Material mat = new Material(shader);
            mat.name = name;

            if (mat.HasProperty("_Color")) mat.SetColor("_Color", baseColor);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);

            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emissiveColor);

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static void SavePrefab(GameObject go, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(go, path);
        }
    }
}
