using UnityEngine;
using UnityEngine.Serialization;

namespace SciFiLab
{
    // Script para hacer que la luz oscile armónicamente
    [RequireComponent(typeof(Light))]
    public class LuzPulsante : MonoBehaviour
    {
        [Header("Configuración de Intensidad")]
        [FormerlySerializedAs("minIntensity")]
        [SerializeField] private float intensidadMinima = 0.8f;

        [FormerlySerializedAs("maxIntensity")]
        [SerializeField] private float intensidadMaxima = 2.2f;

        [FormerlySerializedAs("pulseSpeed")]
        [SerializeField] private float velocidadPulso = 2.5f;

        // Propiedades de retrocompatibilidad
        public float minIntensity { get => intensidadMinima; set => intensidadMinima = value; }
        public float maxIntensity { get => intensidadMaxima; set => intensidadMaxima = value; }
        public float pulseSpeed { get => velocidadPulso; set => velocidadPulso = value; }

        [Header("Variación de Color Opcional")]
        [FormerlySerializedAs("changeColor")]
        [SerializeField] private bool cambiarColor = false;

        [FormerlySerializedAs("colorA")]
        [SerializeField] private Color colorA = Color.cyan;

        [FormerlySerializedAs("colorB")]
        [SerializeField] private Color colorB = Color.blue;

        private Light luzObjetivo;

        private void Awake()
        {
            luzObjetivo = GetComponent<Light>();
        }

        private void Update()
        {
            if (luzObjetivo == null) return;

            // Oscilación senoidal normalizada entre 0 y 1
            float onda = (Mathf.Sin(Time.time * velocidadPulso) + 1f) * 0.5f;
            luzObjetivo.intensity = Mathf.Lerp(intensidadMinima, intensidadMaxima, onda);

            if (cambiarColor)
            {
                luzObjetivo.color = Color.Lerp(colorA, colorB, onda);
            }
        }
    }

    // Alias para compatibilidad con el proyecto
    public class PulsingLight : LuzPulsante { }
}
