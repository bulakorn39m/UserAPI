using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserAPI.Models;
using UserAPI.Services;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<UsersController> _logger = logger;

        // GET api/users
        [HttpGet]
        public async Task<dynamic> Get()
        {
            var result = await _userService.GetAllUsers();
            if (result.Problem != null)
            {
                return Results.Problem(result.Problem);
            }
            return Results.Json(result.Result, statusCode: StatusCodes.Status200OK);
        }

        // GET api/users/1
        [HttpGet("{userId}")]
        public async Task<dynamic> Get(long userId)
        {
            var result = await _userService.GetUserById(userId);
            if (result.Problem != null)
            {
                return Results.Problem(result.Problem);
            }
            return Results.Json(result.Result, statusCode: StatusCodes.Status200OK);
        }

        // POST api/users
        [HttpPost]
        public async Task<dynamic> AddUser([FromBody] UserDo newUser)
        {
            var result = await _userService.AddUser(newUser);
            if (result.Problem != null)
            {
                return Results.Problem(result.Problem);
            }
            return Results.Json(result.Result,statusCode: StatusCodes.Status201Created);
        }

        // PUT api/users
        [HttpPut]
        public async Task<dynamic> UpdateUser([FromBody] UserDo updatedUser)
        {
            var result = await _userService.UpdateUser(updatedUser);
            if (result.Problem != null)
            {
                return Results.Problem(result.Problem);
            }
            return Results.Json(result.Result, statusCode: StatusCodes.Status200OK);
        }

        // DELETE api/users/5
        [HttpDelete("{userId}")]
        public async Task<dynamic> DeleteUser(long userId)
        {
            var result = await _userService.DeleteUser(userId);
            if (result.Problem != null)
            {
                return Results.Problem(result.Problem);
            }
            return Results.Json(result.Result, statusCode: StatusCodes.Status200OK);
        }
    }
}
