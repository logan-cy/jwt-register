using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ReregisterMVC.Models;
using ReregisterMVC.Viewmodels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ReregisterMVC.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(UserViewmodel model)
        {
            // Generate a token that holds our input (UserViewmodle in this case).
            var token = JwtTokenGenerator(model);

            // Generate the URL in the email link to the confirmation page.
            var uriBuilder = new UriBuilder("http://localhost:44385/Auth/Confirm");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();

            var emailBody = $"<p>Click on the link below to confirm your account.</p><p>{urlString}</p>";

            var emailService = new EmailService();
            await emailService.SendAsync(model.Email, emailBody, "Confirm your account", new EmailOptionsDTO());

            TempData["RegisterResult"] = true;
            return View();
        }

        public async Task<ActionResult> Confirm(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var securityToken = handler.ReadToken(token) as JwtSecurityToken;

                var user64 = securityToken.Claims.First().Value;
                var jsonBytes = Convert.FromBase64String(user64);
                var jsonString = Encoding.ASCII.GetString(jsonBytes);
                var model = JsonConvert.DeserializeObject<UserViewmodel>(jsonString);

                var user = new ApplicationUser
                {
                    Email = model.Email,
                    EmailConfirmed = true,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.Email
                };

                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(userStore);
                var result = await userManager.CreateAsync(user, model.Password);
                TempData["ConfirmResult"] = result.Succeeded;

                if (result.Errors.Count() > 0)
                {
                    TempData["ConfirmError"] = result.Errors.First();
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["ConfirmResult"] = false;
                TempData["ConfirmError"] = ex.ToString();
                return View();
            }
        }

        private string JwtTokenGenerator(UserViewmodel model)
        {
            // Site key should go in web.config file, but I put it here to be expedient.
            var secret = "ergwe98srg6KJG875jgjyrtyfderu65764uyJMHKOUKF";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var json = JsonConvert.SerializeObject(model);
            var jsonBytes = Encoding.ASCII.GetBytes(json);
            var user64 = Convert.ToBase64String(jsonBytes);
            var claims = new List<Claim> {
                new Claim(ClaimTypes.UserData, user64)
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}