using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SciFiLab
{
    // Script de control de personaje en primera persona (FPS) y cámara
    [RequireComponent(typeof(CharacterController))]
    public class ControladorJugador : MonoBehaviour
    {
        [Header("Velocidades de Movimiento")]
        [FormerlySerializedAs("walkSpeed")]
        [SerializeField] private float velocidadCaminar = 5.0f;

        [FormerlySerializedAs("sprintSpeed")]
        [SerializeField] private float velocidadCorrer = 8.5f;

        [FormerlySerializedAs("jumpHeight")]
        [SerializeField] private float alturaSalto = 1.6f;

        [FormerlySerializedAs("gravity")]
        [SerializeField] private float gravedad = -18.0f;

        [Header("Sensibilidad de Cámara")]
        [FormerlySerializedAs("mouseSensitivity")]
        [SerializeField] private float sensibilidadMouse = 2.0f;

        [FormerlySerializedAs("minPitch")]
        [SerializeField] private float limiteMiradaAbajo = -80.0f;

        [FormerlySerializedAs("maxPitch")]
        [SerializeField] private float limiteMiradaArriba = 80.0f;

        [Header("Interacción con Teclado")]
        [FormerlySerializedAs("interactionDistance")]
        [SerializeField] private float distanciaInteraccion = 3.5f;

        [FormerlySerializedAs("interactionLayer")]
        [SerializeField] private LayerMask capaInteraccion = ~0;

        [Header("Referencias")]
        [FormerlySerializedAs("playerCamera")]
        [SerializeField] private Camera camaraJugador;

        private CharacterController controlador;
        private Vector3 velocidadVertical;
        private float rotacionVertical = 0f;
        private bool estaEnElSuelo;

        private IInteractuable interactuableActual;

        // Propiedades públicas para compatibilidad
        public float walkSpeed { get => velocidadCaminar; set => velocidadCaminar = value; }
        public float sprintSpeed { get => velocidadCorrer; set => velocidadCorrer = value; }
        public float mouseSensitivity { get => sensibilidadMouse; set => sensibilidadMouse = value; }

        private void Awake()
        {
            controlador = GetComponent<CharacterController>();
            if (camaraJugador == null)
            {
                camaraJugador = GetComponentInChildren<Camera>();
            }
        }

        private void Start()
        {
            BloquearCursor(true);
        }

        private void Update()
        {
            AlternarBloqueoCursor();
            ControlarMirada();
            ControlarMovimiento();
            DetectarInteracciones();
        }

        private void AlternarBloqueoCursor()
        {
            bool escapePresionado = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                escapePresionado = true;
#endif
            if (Input.GetKeyDown(KeyCode.Escape))
                escapePresionado = true;

            if (escapePresionado)
            {
                bool bloqueado = Cursor.lockState == CursorLockMode.Locked;
                BloquearCursor(!bloqueado);
            }

            // Volver a bloquear con click
            bool clickMouse = false;
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                clickMouse = true;
#endif
            if (Input.GetMouseButtonDown(0))
                clickMouse = true;

            if (clickMouse && Cursor.lockState != CursorLockMode.Locked)
            {
                BloquearCursor(true);
            }
        }

        private void BloquearCursor(bool bloquear)
        {
            if (bloquear)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void ControlarMirada()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            float deltaX = 0f;
            float deltaY = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                deltaX = delta.x * 0.1f * sensibilidadMouse;
                deltaY = delta.y * 0.1f * sensibilidadMouse;
            }
#else
            deltaX = Input.GetAxis("Mouse X") * sensibilidadMouse;
            deltaY = Input.GetAxis("Mouse Y") * sensibilidadMouse;
#endif

            // Rotación vertical de la cámara (arriba y abajo)
            rotacionVertical -= deltaY;
            rotacionVertical = Mathf.Clamp(rotacionVertical, limiteMiradaAbajo, limiteMiradaArriba);

            if (camaraJugador != null)
            {
                camaraJugador.transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
            }

            // Rotación horizontal del personaje (izquierda y derecha)
            transform.Rotate(Vector3.up * deltaX);
        }

        private void ControlarMovimiento()
        {
            estaEnElSuelo = controlador.isGrounded;
            if (estaEnElSuelo && velocidadVertical.y < 0)
            {
                velocidadVertical.y = -2f; // Mantener pegado al suelo
            }

            float entradaX = 0f;
            float entradaZ = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) entradaZ += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) entradaZ -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) entradaX += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) entradaX -= 1f;
            }
#else
            entradaX = Input.GetAxisRaw("Horizontal");
            entradaZ = Input.GetAxisRaw("Vertical");
#endif

            bool corriendo = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) corriendo = true;
#endif
            if (Input.GetKey(KeyCode.LeftShift)) corriendo = true;

            float velocidadActual = corriendo ? velocidadCorrer : velocidadCaminar;
            Vector3 direccion = (transform.right * entradaX + transform.forward * entradaZ).normalized;

            // Desplazamiento horizontal
            controlador.Move(direccion * velocidadActual * Time.deltaTime);

            // Salto con la barra espaciadora
            bool saltoPresionado = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) saltoPresionado = true;
#endif
            if (Input.GetKeyDown(KeyCode.Space)) saltoPresionado = true;

            if (saltoPresionado && estaEnElSuelo)
            {
                velocidadVertical.y = Mathf.Sqrt(alturaSalto * -2f * gravedad);
            }

            // Aplicar gravedad
            velocidadVertical.y += gravedad * Time.deltaTime;
            controlador.Move(velocidadVertical * Time.deltaTime);
        }

        private void DetectarInteracciones()
        {
            if (camaraJugador == null) return;

            Ray rayo = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
            RaycastHit hit;

            IInteractuable nuevoInteractuable = null;

            if (Physics.Raycast(rayo, out hit, distanciaInteraccion, capaInteraccion))
            {
                nuevoInteractuable = hit.collider.GetComponentInParent<IInteractuable>();
            }

            if (nuevoInteractuable != interactuableActual)
            {
                interactuableActual = nuevoInteractuable;
                if (ControladorUI.Instancia != null)
                {
                    if (interactuableActual != null)
                    {
                        ControladorUI.Instancia.EstablecerPrompt(interactuableActual.ObtenerTextoInteraccion());
                    }
                    else
                    {
                        ControladorUI.Instancia.LimpiarPrompt();
                    }
                }
            }

            // Si se presiona la tecla [E], interactuamos
            bool interactuarPresionado = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) interactuarPresionado = true;
#endif
            if (Input.GetKeyDown(KeyCode.E)) interactuarPresionado = true;

            if (interactuarPresionado && interactuableActual != null)
            {
                interactuableActual.Interactuar();
                if (ControladorUI.Instancia != null)
                {
                    ControladorUI.Instancia.EstablecerPrompt(interactuableActual.ObtenerTextoInteraccion());
                }
            }
        }
    }

    // Alias para compatibilidad con el proyecto
    public class PlayerController : ControladorJugador { }
}
