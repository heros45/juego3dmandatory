using UnityEngine;
using UnityEngine.Serialization;

namespace SciFiLab
{
    // Script para hacer que las monedas y orbes roten continuamente y leviten
    public class RotadorSimple : MonoBehaviour
    {
        [Header("Configuración de Rotación")]
        [FormerlySerializedAs("rotationSpeed")]
        public Vector3 velocidadRotacion = new Vector3(0f, 45f, 0f);

        [FormerlySerializedAs("useLocalSpace")]
        public bool usarEspacioLocal = true;

        [FormerlySerializedAs("bobbingEffect")]
        public bool efectoFlotacion = false;

        [FormerlySerializedAs("bobbingSpeed")]
        public float velocidadFlotacion = 2f;

        [FormerlySerializedAs("bobbingHeight")]
        public float alturaFlotacion = 0.15f;

        // Propiedades de retrocompatibilidad
        public Vector3 rotationSpeed { get => velocidadRotacion; set => velocidadRotacion = value; }
        public bool useLocalSpace { get => usarEspacioLocal; set => usarEspacioLocal = value; }
        public bool bobbingEffect { get => efectoFlotacion; set => efectoFlotacion = value; }
        public float bobbingSpeed { get => velocidadFlotacion; set => velocidadFlotacion = value; }
        public float bobbingHeight { get => alturaFlotacion; set => alturaFlotacion = value; }

        private Vector3 posicionInicial;

        private void Start()
        {
            posicionInicial = transform.localPosition;
        }

        private void Update()
        {
            float deltaTiempo = Time.deltaTime;

            // Rotar en su eje
            if (usarEspacioLocal)
            {
                transform.Rotate(velocidadRotacion * deltaTiempo, Space.Self);
            }
            else
            {
                transform.Rotate(velocidadRotacion * deltaTiempo, Space.World);
            }

            // Efecto de flotación suave arriba y abajo
            if (efectoFlotacion)
            {
                float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
                transform.localPosition = new Vector3(posicionInicial.x, nuevoY, posicionInicial.z);
            }
        }
    }

    // Alias para compatibilidad con el proyecto
    public class SimpleRotator : RotadorSimple { }
}
