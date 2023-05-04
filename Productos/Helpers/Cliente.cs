using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Productos.DTOS;
using Productos.Models;

namespace Productos.Helpers
{
    public interface ICLiente
    {
        public Logger ObtenerClaimsLogger(HttpContext httpContext,string servicio,string proceso);
    }
    public class ClienteRepositorio : ICLiente
    {

        public Logger ObtenerClaimsLogger(HttpContext httpContext,string servicio,string proceso)
        {
            Logger userClaims = new Logger();

           
                userClaims.Ip = httpContext.User.FindFirst("IP").Value;
                userClaims.Usuario = httpContext.User.FindFirst("USUARIO").Value;
                userClaims.Legajo = httpContext.User.FindFirst("LEGAJO").Value;
                userClaims.Servicio = servicio;
                userClaims.Proceso = proceso;
                userClaims.Fecha = DateTime.Now;
                if (userClaims is null)
                {
                    throw new Exception("Error en obtener los claims");
                }
                

                return userClaims;
            
           
                
            
        }
    }
}