using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ReactReduxApi.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }
    }
}
