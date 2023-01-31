using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.DTO;

namespace UserManager.Helpers
{
    public interface IMapper
    {
        public EventoRegistrarUsuarioDTO MapCrearUsuarioAEventoRegistrarUsuario(CrearUsuarioDTO user);
    }

    public class Mapper : IMapper
    {

        public EventoRegistrarUsuarioDTO MapCrearUsuarioAEventoRegistrarUsuario(CrearUsuarioDTO user)
        {
            EventoRegistrarUsuarioDTO userEvento = new EventoRegistrarUsuarioDTO();
            userEvento.Mail = user.Mail;
            userEvento.Nombre = user.Nombre;
            userEvento.Usuario = user.Usuario;
            return userEvento;
        }
    }
}