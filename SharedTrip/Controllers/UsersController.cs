using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTrip.Controllers
{
    public class UsersController : Controller
    {
        public UsersController(Request request) 
            : base(request)
        {

        }

        public Response Login()
        {
            return View();
        }

        public Response Register()
        {
            return View();
        }
    }
}
