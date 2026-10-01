using UnityEngine;
using UnityEngine.Serialization;

namespace SciFiLab
{
    // Script para la moneda coleccionable que el jugador recoge con colisión Trigger
    public class MonedaColeccionable : MonoBehaviour
    {
        [Header("Efecto de Partículas")]
        [FormerlySerializedAs("collectEffectPrefab")]
        [SerializeField] private ParticleSystem prefabEfectoAlRecoger;

        private bool yaFueRecolectada = false;

        private void OnTriggerEnter(Collider otro)
        {
            if (yaFueRecolectada) return;

            // Detectamos si el que colisiona es el jugador
            if (otro.CompareTag("Player"))
            {
                yaFueRecolectada = true;

                // Sonido de recolección de moneda
                if (AudioJuego.Instancia != null)
                {
                    AudioJuego.Instancia.ReproducirSonidoMoneda();
                }

                // Actualizar interfaz sumando 1 moneda
                if (ControladorUI.Instancia != null)
                {
                    ControladorUI.Instancia.SumarMoneda();
                }

                // Efecto de partículas opcional al recoger
                ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.transform.SetParent(null);
                    var main = ps.main;
                    main.loop = false;
                    ps.Play();
                    Destroy(ps.gameObject, 1.5f);
                }

                // Destruir la moneda de la escena
                Destroy(gameObject);
            }
        }
    }

    // Alias para compatibilidad con el proyecto
    public class EnergyCorePickup : MonedaColeccionable { }
}
