using System.Text.Json.Serialization;

namespace UserManager.DTO
{
    public class CrearUsuarioDTO
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Mail { get; set; }

    }

    public class CrearUsuarioDTOResponse
    {
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string Mail { get; set; }

    }

}