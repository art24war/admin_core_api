using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ReactReduxApi.Controllers
{
    [Route("api/[Controller]")]
    [Authorize]
    public class TestController: ControllerBase
    {
        [Route("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAwailData()
        {
            return await Task.FromResult(Ok("This is available data for ALL"));
        }

        [Route("admin")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetAdminData()
        {
            return await Task.FromResult(Ok("This is available data for Admin"));
        }

        [Route("man")]
        [Authorize(Roles = "ROLE_MANAGER")]
        public async Task<IActionResult> GetManadgerData()
        {
            return await Task.FromResult(Ok("This is available data for Manager"));
        }

        [Route("user")]
        public async Task<IActionResult> GetUserData()
        {
            return await Task.FromResult(Ok("This is available data for Auth user"));
        }
    }
}
