using System.Text.Json.Serialization;

namespace UserManager.DTO
{
    public class LoginDTO
    {
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        
        [JsonIgnore]
        public int Legajo { get; set; }
    }
}