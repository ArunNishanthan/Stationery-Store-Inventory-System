using SSIS.Security;
using System.Web.Http;

namespace SSIS.Controllers.api
{
    /*
    * Arun Nishanthan Anbalagan
    */
    public class LoginController : ApiController
    {
        [HttpGet]
        public IHttpActionResult CheckUser(string email, string password)
        {
            var User = new UserManager().IsValid(email, password);
            return Ok(User);
        }
    }
}
