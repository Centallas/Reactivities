using System.Threading.Tasks;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username) 
        {                                             //Application.Profiles namespace
            return HandleResult(await Mediator.Send(new Details.Query{Username = username}));
        }        
    }
}