using UnityEngine;

namespace SciFiLab
{
    // Generador de efectos de audio procedurales para el juego (sin requerir archivos externos)
    public class AudioJuego : MonoBehaviour
    {
        public static AudioJuego Instancia { get; private set; }
        public static AudioJuego Instance => Instancia;

        private AudioSource fuenteAudio;

        private void Awake()
        {
            if (Instancia != null && Instancia != this)
            {
                Destroy(gameObject);
                return;
            }
            Instancia = this;

            fuenteAudio = GetComponent<AudioSource>();
            if (fuenteAudio == null)
            {
                fuenteAudio = gameObject.AddComponent<AudioSource>();
            }
            fuenteAudio.spatialBlend = 0f; // Sonido 2D para interfaz
        }

        // Sonido de tono simple
        public void ReproducirBeep(float frecuencia = 880f, float duracion = 0.15f)
        {
            AudioClip clip = CrearClipTono("Beep", frecuencia, duracion, 0.3f);
            fuenteAudio.PlayOneShot(clip);
        }

        // Sonido brillante al recolectar moneda
        public void ReproducirSonidoMoneda()
        {
            AudioClip clip = CrearClipArpegio("Moneda", new float[] { 987.77f, 1318.51f }, 0.08f);
            fuenteAudio.PlayOneShot(clip, 0.6f);
        }

        // Fanfarria musical al completar las 5 monedas
        public void ReproducirFanfarriaVictoria()
        {
            AudioClip clip = CrearClipArpegio("Victoria", new float[] { 523.25f, 659.25f, 783.99f, 1046.50f, 1318.51f }, 0.16f);
            fuenteAudio.PlayOneShot(clip, 0.75f);
        }

        // Sonido neumático de apertura de compuerta
        public void ReproducirSonidoPuerta()
        {
            AudioClip clip = CrearClipRuido("PuertaMecanica", 0.4f);
            fuenteAudio.PlayOneShot(clip, 0.4f);
        }

        // Métodos en inglés para compatibilidad
        public void PlayBeep(float frequency = 880f, float duration = 0.15f) => ReproducirBeep(frequency, duration);
        public void PlayCoinSound() => ReproducirSonidoMoneda();
        public void PlayPickupSound() => ReproducirSonidoMoneda();
        public void PlayVictoryFanfare() => ReproducirFanfarriaVictoria();
        public void PlayDoorSound() => ReproducirSonidoPuerta();

        private AudioClip CrearClipTono(string nombre, float frecuencia, float duracion, float volumen)
        {
            int frecuenciaMuestreo = 44100;
            int totalMuestras = (int)(frecuenciaMuestreo * duracion);
            float[] muestras = new float[totalMuestras];

            for (int i = 0; i < totalMuestras; i++)
            {
                float t = (float)i / frecuenciaMuestreo;
                float envolvente = 1f - (t / duracion); // Desvanecimiento lineal
                muestras[i] = Mathf.Sin(2f * Mathf.PI * frecuencia * t) * volumen * envolvente;
            }

            AudioClip clip = AudioClip.Create(nombre, totalMuestras, 1, frecuenciaMuestreo, false);
            clip.SetData(muestras, 0);
            return clip;
        }

        private AudioClip CrearClipArpegio(string nombre, float[] frecuencias, float duracionNota)
        {
            int frecuenciaMuestreo = 44100;
            int totalMuestras = (int)(frecuenciaMuestreo * duracionNota * frecuencias.Length);
            float[] muestras = new float[totalMuestras];

            int muestrasPorNota = (int)(frecuenciaMuestreo * duracionNota);

            for (int n = 0; n < frecuencias.Length; n++)
            {
                float frec = frecuencias[n];
                int offset = n * muestrasPorNota;

                for (int i = 0; i < muestrasPorNota; i++)
                {
                    float t = (float)i / frecuenciaMuestreo;
                    float envolvente = Mathf.Exp(-t * 8f); // Decaimiento exponencial
                    int indice = offset + i;
                    if (indice < totalMuestras)
                    {
                        muestras[indice] = Mathf.Sin(2f * Mathf.PI * frec * t) * envolvente;
                    }
                }
            }

            AudioClip clip = AudioClip.Create(nombre, totalMuestras, 1, frecuenciaMuestreo, false);
            clip.SetData(muestras, 0);
            return clip;
        }

        private AudioClip CrearClipRuido(string nombre, float duracion)
        {
            int frecuenciaMuestreo = 44100;
            int totalMuestras = (int)(frecuenciaMuestreo * duracion);
            float[] muestras = new float[totalMuestras];

            for (int i = 0; i < totalMuestras; i++)
            {
                float t = (float)i / frecuenciaMuestreo;
                float envolvente = Mathf.Sin((t / duracion) * Mathf.PI);
                muestras[i] = (Random.value * 2f - 1f) * 0.25f * envolvente;
            }

            AudioClip clip = AudioClip.Create(nombre, totalMuestras, 1, frecuenciaMuestreo, false);
            clip.SetData(muestras, 0);
            return clip;
        }
    }

    // Alias para compatibilidad con el proyecto
    public class ProceduralAudio : AudioJuego { }
}
