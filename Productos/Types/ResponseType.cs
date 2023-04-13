using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.Types
{
public class HttpResponseOk
    {
        public String status { get; set; }
        public String msg {get; set; }
        public Object data { get; set; }
        
        public HttpResponseOk()
        {
            status = Constants.Ok;
        }
        public HttpResponseOk(string msg)
        {
            this.msg = msg;
            status = Constants.Ok;
        }
    }

    public static class Constants
    {
        public const String Ok = "ok";
        public const String Nok = "nok";
    }

    public class HttpBadResponse
    {
        public String status { get; set; }
        public ErrorMessage oError { get; set; }
        public HttpBadResponse()
        {

        }
        public HttpBadResponse(System.Exception e)
        {
            status = Constants.Nok;
            oError = new ErrorMessage() { code = e.GetHashCode().ToString(), errorMessage = e.Message };
        }
        public HttpBadResponse(String errorMessage)
        {
            status = Constants.Nok;
            oError = new ErrorMessage() { code = "1000", errorMessage = errorMessage };
        }
    }

    public class ErrorMessage
    {
        public String code { get; set; }
        public String errorMessage { get; set; }
    }
}