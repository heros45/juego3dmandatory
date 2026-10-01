namespace SciFiLab
{
    // Interfaz para objetos con los que el jugador puede interactuar con la tecla [E]
    public interface IInteractuable
    {
        string ObtenerTextoInteraccion();
        void Interactuar();
    }

    // Alias para compatibilidad
    public interface IInteractable : IInteractuable
    {
    }
}
