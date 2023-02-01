using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ordenes.DTO;

namespace Ordenes.Repositorio
{
    public interface ILogin
    {
        Task<TokenDTO> Loguear(LoginDTO user) ;
    }


    public class Login : ILogin
    {

        private readonly IHttpClientFactory _httpClient;

        public Login(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenDTO> Loguear(LoginDTO user)
        {
            HttpClient cliente = _httpClient.CreateClient("UserManager");
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(user),Encoding.UTF8, "application/json");

            HttpResponseMessage response = await cliente.PostAsync("api/Login/Loguear",httpContent);

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            TokenDTO token = JsonConvert.DeserializeObject<TokenDTO>(response.Content.ReadAsStringAsync().Result);
            return token;

        }
    }
}