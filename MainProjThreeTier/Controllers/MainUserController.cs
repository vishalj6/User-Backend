using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserProject_BAL.Interfaces;
using UserProject_DAL.Interfaces;
using UserProject_DAL.Models;

namespace MainProjThreeTier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainUserController : ControllerBase
    {

        private readonly ILogger<MainUserController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public MainUserController(ILogger<MainUserController> logger, IUserService userService, IEmailService emailService)
        {
            _logger = logger;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet("GetUsers")]
        [Produces(typeof(object))]
        public async Task<IActionResult> SearchUsers([FromQuery] string? allFilter)
        {
            //List<string> UserSearch = new List<string>() { firstName, lastName, phoneNo, email, country, state, city };
            try
            {
                FilterData allFilter2 = new FilterData(); ;
                if (allFilter != null)
                {
                    allFilter2 = JsonConvert.DeserializeObject<FilterData>(allFilter);
                }
                (List<User> users, int TotalPages, int TotalUsers) = await _userService.GetUsers(allFilter2, 0, null);
                var response = new
                {
                    Users = users,
                    totalPages = TotalPages,
                    totalUsers = TotalUsers
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching users.");
            }
        }

        [HttpGet("GetUserData")]
        [Produces(typeof(Task<IActionResult>))]

        public async Task<IActionResult> GetUserData([FromQuery] int UserId = 0, [FromQuery] string User_Password = "")
        {
            //List<string> UserSearch = new List<string>() { null, null, null, null, null, null, null };
            FilterData allFilter = new();
            try
            {
                (List<User> users, int TotalPages, int TotalUsers) = await _userService.GetUsers(allFilter, UserId, User_Password);
                return Ok(users[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching users.");
            }
        }


        [HttpPost]
        [Route("InsertUser")]
        [Produces(typeof(List<string>))]
        public async Task<List<string>> Insert_User([FromBody] User user)
        {
            return await _userService.Insert_User(user);
        }


        [HttpPost]
        [Route("UpdateUser")]
        [Produces(typeof(List<string>))]
        public async Task<List<string>> Update_User([FromBody] User? user = null, [FromQuery] int UserId = 0, [FromQuery] string? password = "")
        {
            return await _userService.Update_User(user, UserId, password);
        }

        [HttpPost]
        [Route("DeleteUser")]
        [Produces(typeof(List<string>))]
        public async Task<List<string>> DeleteUsers([FromBody] int UserId)
        {
            return await _userService.DeleteUsers(UserId);
        }

        [HttpPost]
        [Route("DeleteAllUser")]
        [Produces(typeof(bool))]
        public async Task<bool> DeleteAllPersons()
        {
            return await _userService.DeleteAllPersons();
        }

        [HttpPost]
        [Route("ValidateUser")]
        [Produces(typeof(IActionResult))]
        public async Task<IActionResult> ValidateUserAsync([FromBody] ValidateUserRequest ValidateUser)
        {
            try
            {
                (int UserId, string FirstName) = await _userService.ValidateUserAsync(ValidateUser.UserName, ValidateUser.Password);
                var response = new
                {
                    UserId,
                    FirstName
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching users.");
            }
        }

        [HttpPost]
        [Route("ValidateAdmin")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> ValidateAdminAsync([FromBody] ValidateUserRequest ValidateAdmin)
        {
            try
            {
                bool isAdmin = await _userService.ValidateAdminAsync(ValidateAdmin.UserName, ValidateAdmin.Password);
                //var response = new
                //{
                //    isAdmin
                //};
                return Ok(isAdmin);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching users.");
            }
        }

        [HttpGet]
        [Route("SendOTP")]
        [Produces(typeof(Task<IActionResult>))]
        public async Task<IActionResult> SendOTPEmail([FromQuery] string userEmail, [FromQuery] string? FirstName)
        {
            try
            {
                string OTP = await _emailService.SendOTPEmail(userEmail, FirstName);
                return Ok(OTP);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending OTP:", ex);
                return BadRequest("An error occurred while sending OTP.");
            }
        }

        [HttpGet("GetCountries")]
        [Produces(typeof(Task<IActionResult>))]

        public async Task<IActionResult> GetCountriesAsync()
        {
            try
            {
                List<string> Countries = await _userService.GetCountriesAsync();
                return Ok(Countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching Country Data.");
            }
        }


        [HttpGet("GetStates")]
        [Produces(typeof(Task<IActionResult>))]

        public async Task<IActionResult> GetStatesAsync()
        {
            try
            {
                List<string> States = await _userService.GetStatesAsync();
                return Ok(States);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching States Data.");
            }
        }

        [HttpGet("GetCities")]
        [Produces(typeof(Task<IActionResult>))]

        public async Task<IActionResult> GetCitiesAsync()
        {
            try
            {
                List<string> Cities = await _userService.GetCitiesAsync();
                return Ok(Cities);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users:", ex);
                return BadRequest("An error occurred while fetching Cities Data.");
            }
        }


    }
    public class ValidateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
