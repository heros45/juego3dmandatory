using UnityEngine;
using UnityEngine.Serialization;

namespace SciFiLab
{
    // Controlador de compuerta corrediza con apertura automática y manual
    public class PuertaCorrediza : MonoBehaviour, IInteractuable, IInteractable
    {
        [Header("Partes de la Puerta")]
        [FormerlySerializedAs("doorPanel")]
        [SerializeField] private Transform panelPuerta;

        [FormerlySerializedAs("statusLightRenderer")]
        [SerializeField] private Renderer renderLuzEstado;

        [Header("Movimiento")]
        [FormerlySerializedAs("openOffset")]
        [SerializeField] private Vector3 desplazamientoApertura = new Vector3(0f, 3.2f, 0f);

        [FormerlySerializedAs("slideSpeed")]
        [SerializeField] private float velocidadDeslizamiento = 3.5f;

        [FormerlySerializedAs("openOnTrigger")]
        [SerializeField] private bool abrirPorProximidad = true;

        [Header("Materiales de Estado")]
        [FormerlySerializedAs("lockedMaterial")]
        [SerializeField] private Material materialCerrado;

        [FormerlySerializedAs("openMaterial")]
        [SerializeField] private Material materialAbierto;

        private Vector3 posicionCerrada;
        private Vector3 posicionObjetivo;
        private bool estaAbierta = false;

        private void Start()
        {
            if (panelPuerta == null)
            {
                panelPuerta = transform.Find("DoorPanel");
                if (panelPuerta == null) panelPuerta = transform;
            }

            posicionCerrada = panelPuerta.localPosition;
            posicionObjetivo = posicionCerrada;
            ActualizarVisuales();
        }

        private void Update()
        {
            if (panelPuerta != null)
            {
                panelPuerta.localPosition = Vector3.MoveTowards(
                    panelPuerta.localPosition, 
                    posicionObjetivo, 
                    velocidadDeslizamiento * Time.deltaTime
                );
            }
        }

        // Interfaz IInteractuable
        public string ObtenerTextoInteraccion()
        {
            return estaAbierta ? "[E] Cerrar Compuerta" : "[E] Abrir Compuerta";
        }

        public string GetInteractionPrompt() => ObtenerTextoInteraccion();

        public void Interactuar()
        {
            EstablecerEstadoPuerta(!estaAbierta);
        }

        public void Interact() => Interactuar();

        public void EstablecerEstadoPuerta(bool abrir)
        {
            estaAbierta = abrir;
            posicionObjetivo = estaAbierta ? (posicionCerrada + desplazamientoApertura) : posicionCerrada;

            if (AudioJuego.Instancia != null)
            {
                AudioJuego.Instancia.ReproducirSonidoPuerta();
            }

            ActualizarVisuales();
        }

        public void SetDoorState(bool open) => EstablecerEstadoPuerta(open);

        private void ActualizarVisuales()
        {
            if (renderLuzEstado != null)
            {
                if (estaAbierta && materialAbierto != null)
                {
                    renderLuzEstado.material = materialAbierto;
                }
                else if (!estaAbierta && materialCerrado != null)
                {
                    renderLuzEstado.material = materialCerrado;
                }
            }
        }

        // Detección automática por colisión de proximidad (Trigger)
        private void OnTriggerEnter(Collider otro)
        {
            if (abrirPorProximidad && otro.CompareTag("Player") && !estaAbierta)
            {
                EstablecerEstadoPuerta(true);
            }
        }

        private void OnTriggerExit(Collider otro)
        {
            if (abrirPorProximidad && otro.CompareTag("Player") && estaAbierta)
            {
                EstablecerEstadoPuerta(false);
            }
        }
    }

    // Alias para compatibilidad con el proyecto
    public class SlidingDoor : PuertaCorrediza { }
}
