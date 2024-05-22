using CHNUCooin.Dtos;
using CHNUCooin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace CHNUCooin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpPost]
        public async Task<ActionResult<CreateUserResponseDto>> CreateUser()
        {
            var result = await _userService.CreateUser();
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<string>> GetListOfAddresses()
        {
            var addresses = _userService.GetListOfAddresses();
            return Ok(addresses);
        }
    }
}
