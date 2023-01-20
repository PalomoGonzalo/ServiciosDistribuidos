namespace UserManager.DTO
{
    public class CambiarContraseñaDTO
    {
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public int Legajo { get; set; }
        public string ContraseñaNueva { get; set; }
    }
}