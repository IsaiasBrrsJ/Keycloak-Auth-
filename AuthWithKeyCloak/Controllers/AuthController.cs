using AuthWithKeyCloak.InputModel;
using AuthWithKeyCloak.KeyCloak;
using AuthWithKeyCloak.ViewModel;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthWithKeyCloak.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<LoginOption> _loginOption;
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthController(IOptions<LoginOption> loginOption, IHttpClientFactory httpClientFactory)
        {
            _loginOption = loginOption;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("GetUserInfo")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetUser()
        {

            return Ok(new { message = "test ok autorizado " });
        }


        [HttpGet]
        [Route("GetView")]
        [Authorize(Roles = "View")]

        public IActionResult GetUserV()
        {

            return Ok(new { message = "test ok autorizado " });
        }

        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest request)
        {

            using (var real = _httpClientFactory.CreateClient())
            {
                var dict = new Dictionary<string, string>
                {
                    { "client_id", _loginOption.Value.clientId },
                    { "client_secret", _loginOption.Value.clientSecret },
                    { "grant_type", _loginOption.Value.grantType },
                    { "username", request.email },
                    { "password", request.password }
                };
                var content = new FormUrlEncodedContent(dict);


                var result = await _httpClientFactory.CreateClient("Keycloak")
                    .PostAsync("token", content);


                if (!result.IsSuccessStatusCode)
                    return BadRequest(new
                    {
                        error = result.Content.ReadAsStringAsync()

                    });


                var token = await result.Content.ReadFromJsonAsync<KeyCloakToken>();

                return Ok(token);


            }

        }

        [HttpPost("RefreshToken")]

        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            using (var real = _httpClientFactory.CreateClient())
            {
                var dict = new Dictionary<string, string>
                {
                    { "client_id", _loginOption.Value.clientId },
                    { "client_secret", _loginOption.Value.clientSecret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken },

                };
                var content = new FormUrlEncodedContent(dict);


                var result = await _httpClientFactory.CreateClient("Keycloak")
                    .PostAsync("token", content);


                if (!result.IsSuccessStatusCode)
                    return BadRequest(new
                    {
                        error = result.Content.ReadAsStringAsync()

                    });


                var token = await result.Content.ReadAsStringAsync();

                return Ok(token);


            }
        }


        [AllowAnonymous]
        [HttpPost("teste")]
        public async Task<IActionResult> gsqf()
        {
            string serverUrl = "http://localhost:8080";  
            string realm = "app-xpto";               
            string adminUsername = "admin";           
            string adminPassword = "admin";    
            var client  = new KeycloakClient(serverUrl, adminUsername, adminPassword);

           var token = await client.CreateUserAsync(realm, new Keycloak.Net.Models.Users.User
            {
                Credentials = new List<Keycloak.Net.Models.Users.Credentials>
                {
                    new Keycloak.Net.Models.Users.Credentials
                    {
                        Type = "password",
                        Value = "123456",
                        Temporary = false
                    }
                },
                Email = "teste@teste.com",
                EmailVerified = true,
                Groups = new List<string> { "Employee", "adminC1" },
                FirstName = "testebarros",
                LastName = "barros",

                UserName = "testebarros",
                Enabled = true,
               
            });
           
            return Ok(token);
        }
    }
}
