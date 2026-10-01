using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SciFiLab
{
    // Administrador de la interfaz de usuario (HUD), contador de monedas y pantalla de victoria
    public class ControladorUI : MonoBehaviour
    {
        public static ControladorUI Instancia { get; private set; }
        public static ControladorUI Instance => Instancia;

        [Header("Datos de Monedas y Misión")]
        [FormerlySerializedAs("currentPrompt")]
        public string mensajeInteraccion = "";

        [FormerlySerializedAs("objectiveMessage")]
        public string mensajeObjetivo = "Misión: Explora el escenario y recolecta las 5 Monedas Doradas para ganar.";

        [FormerlySerializedAs("coinsCollected")]
        public int monedasRecolectadas = 0;

        [FormerlySerializedAs("totalCoins")]
        public int totalMonedas = 5;

        [FormerlySerializedAs("isGameWon")]
        public bool juegoGanado = false;

        // Propiedades de retrocompatibilidad
        public string currentPrompt { get => mensajeInteraccion; set => mensajeInteraccion = value; }
        public string objectiveMessage { get => mensajeObjetivo; set => mensajeObjetivo = value; }
        public int coinsCollected { get => monedasRecolectadas; set => monedasRecolectadas = value; }
        public int totalCoins { get => totalMonedas; set => totalMonedas = value; }
        public bool isGameWon { get => juegoGanado; set => juegoGanado = value; }
        public int energyCoresCollected { get => monedasRecolectadas; set => monedasRecolectadas = value; }
        public int totalEnergyCores { get => totalMonedas; set => totalMonedas = value; }

        private GUIStyle estiloPrompt;
        private GUIStyle estiloHUD;
        private GUIStyle estiloTitulo;
        private GUIStyle estiloTituloVictoria;
        private GUIStyle estiloSubtituloVictoria;
        private GUIStyle estiloTextoVictoria;
        private GUIStyle estiloTeclaVictoria;

        private Texture2D texturaMirilla;
        private Texture2D texturaFondoCaja;
        private Texture2D texturaFondoVictoria;

        private void Awake()
        {
            if (Instancia != null && Instancia != this)
            {
                Destroy(gameObject);
                return;
            }
            Instancia = this;
        }

        private void Start()
        {
            // Textura sólida para fondos semitransparentes en UI
            texturaFondoCaja = new Texture2D(1, 1);
            texturaFondoCaja.SetPixel(0, 0, new Color(0.05f, 0.1f, 0.15f, 0.88f));
            texturaFondoCaja.Apply();

            // Textura de fondo para panel de victoria
            texturaFondoVictoria = new Texture2D(1, 1);
            texturaFondoVictoria.SetPixel(0, 0, new Color(0.03f, 0.07f, 0.12f, 0.95f));
            texturaFondoVictoria.Apply();

            // Textura de punto de mira central (Crosshair)
            texturaMirilla = new Texture2D(1, 1);
            texturaMirilla.SetPixel(0, 0, new Color(0.2f, 0.9f, 1f, 0.9f));
            texturaMirilla.Apply();

            // Asegurar monedas y materiales
            AsegurarCincoMonedas();
            CorregirMaterialesParticulas();
        }

        private void Update()
        {
            // Reiniciar nivel con la tecla R
            bool teclaRPresionada = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) teclaRPresionada = true;
#endif
            if (Input.GetKeyDown(KeyCode.R)) teclaRPresionada = true;

            if (teclaRPresionada)
            {
                ReiniciarNivel();
            }
        }

        public void EstablecerPrompt(string mensaje)
        {
            mensajeInteraccion = mensaje;
        }

        public void LimpiarPrompt()
        {
            mensajeInteraccion = "";
        }

        public void SetPrompt(string prompt) => EstablecerPrompt(prompt);
        public void ClearPrompt() => LimpiarPrompt();

        public void SumarMoneda()
        {
            monedasRecolectadas++;

            if (monedasRecolectadas >= totalMonedas)
            {
                juegoGanado = true;
                mensajeObjetivo = "¡MISIÓN CUMPLIDA! Has recolectado todas las monedas y completado el juego.";

                if (AudioJuego.Instancia != null)
                {
                    AudioJuego.Instancia.ReproducirFanfarriaVictoria();
                }
            }
            else
            {
                mensajeObjetivo = $"Moneda obtenida ({monedasRecolectadas}/{totalMonedas}). ¡Sigue explorando el escenario y las colinas!";
            }
        }

        public void AddCoin() => SumarMoneda();
        public void AddEnergyCore() => SumarMoneda();

        public void ReiniciarNivel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnGUI()
        {
            InicializarEstilos();

            float anchoPantalla = Screen.width;
            float altoPantalla = Screen.height;

            // 1. DIBUJAR MIRA CENTRAL (CROSSHAIR)
            float tamanoMira = 6f;
            GUI.DrawTexture(new Rect((anchoPantalla - tamanoMira) * 0.5f, (altoPantalla - tamanoMira) * 0.5f, tamanoMira, tamanoMira), texturaMirilla);

            // Retícula sutil
            float distanciaReticula = 10f;
            float largoReticula = 8f;
            float grosorReticula = 2f;
            Color colorCyan = new Color(0.3f, 0.9f, 1f, 0.7f);
            GUI.color = colorCyan;
            GUI.DrawTexture(new Rect((anchoPantalla - grosorReticula) * 0.5f, (altoPantalla * 0.5f) - distanciaReticula - largoReticula, grosorReticula, largoReticula), texturaMirilla);
            GUI.DrawTexture(new Rect((anchoPantalla - grosorReticula) * 0.5f, (altoPantalla * 0.5f) + distanciaReticula, grosorReticula, largoReticula), texturaMirilla);
            GUI.DrawTexture(new Rect((anchoPantalla * 0.5f) - distanciaReticula - largoReticula, (altoPantalla - grosorReticula) * 0.5f, largoReticula, grosorReticula), texturaMirilla);
            GUI.DrawTexture(new Rect((anchoPantalla * 0.5f) + distanciaReticula, (altoPantalla - grosorReticula) * 0.5f, largoReticula, grosorReticula), texturaMirilla);
            GUI.color = Color.white;

            // 2. PANEL DE INFORMACIÓN SUPERIOR (HUD)
            GUILayout.BeginArea(new Rect(20, 20, 520, 115), texturaFondoCaja.name, GUI.skin.box);
            GUILayout.Label("ESTACIÓN CIENTÍFICA 3D // PA3", estiloTitulo);
            GUILayout.Space(4);
            GUILayout.Label(mensajeObjetivo, estiloHUD);
            GUILayout.Space(6);
            GUILayout.Label($"🪙 Monedas de Oro: [ <color=#FFD700><b>{monedasRecolectadas} / {totalMonedas}</b></color> ]", estiloHUD);
            GUILayout.EndArea();

            // 3. CONTROLES Y GUÍA RÁPIDA (ESQUINA INFERIOR IZQUIERDA)
            GUILayout.BeginArea(new Rect(20, altoPantalla - 130, 400, 110), texturaFondoCaja.name, GUI.skin.box);
            GUILayout.Label("CONTROLES DEL JUGADOR:", estiloTitulo);
            GUILayout.Label("• WASD: Moverse  |  Shift: Correr  |  Espacio: Saltar", estiloHUD);
            GUILayout.Label("• Mouse: Mirar alrededor  |  E: Interactuar con compuerta", estiloHUD);
            GUILayout.Label("• R: Reiniciar partida  |  Esc: Liberar cursor", estiloHUD);
            GUILayout.EndArea();

            // 4. AVISO DE INTERACCIÓN CENTRAL (si el jugador mira un objeto interactivo)
            if (!string.IsNullOrEmpty(mensajeInteraccion))
            {
                float anchoPrompt = 420f;
                float altoPrompt = 44f;
                Rect rectPrompt = new Rect((anchoPantalla - anchoPrompt) * 0.5f, altoPantalla * 0.62f, anchoPrompt, altoPrompt);

                GUI.Box(rectPrompt, GUIContent.none, GUI.skin.box);
                GUI.Label(rectPrompt, mensajeInteraccion, estiloPrompt);
            }

            // 5. PANTALLA / MODAL DE VICTORIA AL OBTENER LAS 5 MONEDAS
            if (juegoGanado)
            {
                float anchoVictoria = 600f;
                float altoVictoria = 280f;
                Rect rectVictoria = new Rect((anchoPantalla - anchoVictoria) * 0.5f, (altoPantalla - altoVictoria) * 0.5f, anchoVictoria, altoVictoria);

                Color colorOriginal = GUI.color;
                GUI.color = new Color(1f, 0.85f, 0.1f, 0.9f);
                GUI.Box(new Rect(rectVictoria.x - 3, rectVictoria.y - 3, rectVictoria.width + 6, rectVictoria.height + 6), GUIContent.none, GUI.skin.box);
                GUI.color = colorOriginal;

                GUI.DrawTexture(rectVictoria, texturaFondoVictoria);

                GUILayout.BeginArea(new Rect(rectVictoria.x + 20, rectVictoria.y + 20, rectVictoria.width - 40, rectVictoria.height - 40));
                GUILayout.Label("★ ★ ★ ¡JUEGO COMPLETADO! ★ ★ ★", estiloTituloVictoria);
                GUILayout.Space(6);
                GUILayout.Label("¡HAS GANADO EL JUEGO!", estiloSubtituloVictoria);
                GUILayout.Space(12);
                GUILayout.Label("Has encontrado y recolectado exitosamente las 5 Monedas Doradas.", estiloTextoVictoria);
                GUILayout.Label("El entorno tridimensional y el terreno montañoso han sido explorados.", estiloTextoVictoria);
                GUILayout.Space(16);
                GUILayout.Label("Presiona [ R ] para reiniciar el juego  •  [ Esc ] para liberar el cursor", estiloTeclaVictoria);
                GUILayout.EndArea();
            }
        }

        private void InicializarEstilos()
        {
            if (estiloPrompt == null)
            {
                estiloPrompt = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 18,
                    fontStyle = FontStyle.Bold
                };
                estiloPrompt.normal.textColor = new Color(1f, 0.95f, 0.4f);
            }

            if (estiloTitulo == null)
            {
                estiloTitulo = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold
                };
                estiloTitulo.normal.textColor = new Color(0.3f, 0.9f, 1f);
            }

            if (estiloHUD == null)
            {
                estiloHUD = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    richText = true,
                    wordWrap = true
                };
                estiloHUD.normal.textColor = new Color(0.9f, 0.95f, 1f);
            }

            if (estiloTituloVictoria == null)
            {
                estiloTituloVictoria = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 24,
                    fontStyle = FontStyle.Bold
                };
                estiloTituloVictoria.normal.textColor = new Color(1f, 0.85f, 0.1f);
            }

            if (estiloSubtituloVictoria == null)
            {
                estiloSubtituloVictoria = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 18,
                    fontStyle = FontStyle.Bold
                };
                estiloSubtituloVictoria.normal.textColor = new Color(0.2f, 1f, 0.5f);
            }

            if (estiloTextoVictoria == null)
            {
                estiloTextoVictoria = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    wordWrap = true
                };
                estiloTextoVictoria.normal.textColor = new Color(0.9f, 0.95f, 1f);
            }

            if (estiloTeclaVictoria == null)
            {
                estiloTeclaVictoria = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13,
                    fontStyle = FontStyle.Bold
                };
                estiloTeclaVictoria.normal.textColor = new Color(1f, 0.6f, 0.2f);
            }
        }

        private void AsegurarCincoMonedas()
        {
            GameObject moneda1 = GameObject.Find("GoldCoin_1");
            if (moneda1 != null) return; // Ya están en la escena

            Vector3[] posicionesMonedas = new Vector3[]
            {
                new Vector3(0f, 1.1f, -9.5f),   // Moneda 1: Habitación inicial (Airlock)
                new Vector3(-6.5f, 1.4f, -2.5f), // Moneda 2: Mesa de Laboratorio
                new Vector3(-2.0f, 1.2f, 2.5f),  // Moneda 3: Sala Central
                new Vector3(-8.5f, 1.2f, 12.0f), // Moneda 4: Almacén de Cajas y Barriles
                new Vector3(8.5f, 3.4f, 13.0f)   // Moneda 5: Plataforma Elevada de Observación
            };

            Transform padreTransform = null;
            GameObject interactivos = GameObject.Find("InteractiveObjects");
            if (interactivos != null) padreTransform = interactivos.transform;

            Material materialOro = Resources.Load<Material>("Mat_Coin_Gold");

            for (int i = 0; i < posicionesMonedas.Length; i++)
            {
                CrearMonedaVisual($"GoldCoin_{i + 1}", posicionesMonedas[i], padreTransform, materialOro);
            }
        }

        private GameObject CrearMonedaVisual(string nombre, Vector3 pos, Transform padre, Material matOro)
        {
            GameObject raizMoneda = new GameObject(nombre);
            if (padre != null) raizMoneda.transform.SetParent(padre);
            raizMoneda.transform.position = pos;

            // Disco cilíndrico
            GameObject disco = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disco.name = "CoinDisc";
            disco.transform.SetParent(raizMoneda.transform, false);
            disco.transform.localPosition = Vector3.zero;
            disco.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            disco.transform.localScale = new Vector3(0.75f, 0.04f, 0.75f);
            Collider colDisco = disco.GetComponent<Collider>();
            if (colDisco != null) Destroy(colDisco);
            if (matOro != null) disco.GetComponent<Renderer>().material = matOro;

            // Rotación y flotación
            RotadorSimple rotador = raizMoneda.AddComponent<RotadorSimple>();
            rotador.velocidadRotacion = new Vector3(0f, 150f, 0f);
            rotador.efectoFlotacion = true;
            rotador.velocidadFlotacion = 2.8f;
            rotador.alturaFlotacion = 0.12f;

            // Luz dorada
            GameObject luzObj = new GameObject("CoinGlow");
            luzObj.transform.SetParent(raizMoneda.transform, false);
            Light luzPuntual = luzObj.AddComponent<Light>();
            luzPuntual.type = LightType.Point;
            luzPuntual.color = new Color(1f, 0.85f, 0.2f);
            luzPuntual.intensity = 1.6f;
            luzPuntual.range = 3.5f;

            // Collider Trigger
            SphereCollider col = raizMoneda.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.8f;

            raizMoneda.AddComponent<MonedaColeccionable>();
            return raizMoneda;
        }

        private void CorregirMaterialesParticulas()
        {
            Material matParticula = Resources.Load<Material>("Mat_Neon_Cyan");
            if (matParticula == null) matParticula = Resources.Load<Material>("Mat_Coin_Gold");

            ParticleSystemRenderer[] renderers = FindObjectsByType<ParticleSystemRenderer>(FindObjectsSortMode.None);
            foreach (var psr in renderers)
            {
                if (psr.sharedMaterial == null || psr.sharedMaterial.name.Contains("Default"))
                {
                    if (matParticula != null) psr.material = matParticula;
                }
            }

            ParticleSystem[] sistemas = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
            foreach (var ps in sistemas)
            {
                var main = ps.main;
                main.startSize = 0.12f;
                main.startColor = new Color(0.2f, 0.9f, 1f, 0.85f);
            }
        }
    }

    // Alias para compatibilidad con el proyecto
    public class UIManager : ControladorUI { }
}
