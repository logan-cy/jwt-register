using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReregisterCore.Helpers;
using ReregisterCore.Interfaces;
using ReregisterCore.Models;
using ReregisterCore.Viewmodels;

namespace ReregisterCore.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IEmail _emailService;
        private readonly IOptions<EmailOptionsDTO> _emailOptions;

        [BindProperty]
        public UserViewmodel Input { get; set; }
        public string ReturnUrl { get; set; }
        public bool Success { get; set; }

        public RegisterModel(IConfiguration config, IEmail emailService, IOptions<EmailOptionsDTO> emailOptions)
        {
            _config = config;
            _emailService = emailService;
            _emailOptions = emailOptions;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Generate a token that holds our input (ApplicationUser in this case).
            var token = JwtTokenGenerator(Input);

            // Generate the URL in the email link to the confirmation page.
            var uriBuilder = new UriBuilder("http://localhost:5000/Confirm");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();

            var emailBody = $"<p>Click on the link below to confirm your account.</p><p>{urlString}</p>";
            await _emailService.SendAsync(Input.Email, emailBody, "Confirm your account", _emailOptions.Value);

            Success = true;

            return Page();
        }

        private string JwtTokenGenerator(UserViewmodel model)
        {
            var secret = _config.GetSection("AppSettings:Key").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            // 1. Convert model to json.
            var json = JsonSerializer.Serialize(model);
            // 2. Get byte array from json.
            var jsonBytes = Encoding.ASCII.GetBytes(json);
            // 3. Convert json byte array to base64 text.
            var user64 = Convert.ToBase64String(jsonBytes);
            // 4. Add base64 text to claims.
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
