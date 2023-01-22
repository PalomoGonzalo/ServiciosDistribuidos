namespace UserManager.DTO
{
    public class CambiarContraseñaDTO
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public int Legajo { get; set; }
        public string PasswordNueva { get; set; }
    }
}