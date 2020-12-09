using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReregisterCore.Helpers;
using ReregisterCore.Models;
using ReregisterCore.Viewmodels;

namespace ReregisterCore.Pages
{
    public class ConfirmModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public bool Success { get; set; }
        public string Error { get; set; }

        public ConfirmModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var securityToken = handler.ReadToken(token) as JwtSecurityToken;

                var user64 = securityToken.Claims.First().Value;

                // 1. Convert base64 text to byte array.
                var jsonBytes = Convert.FromBase64String(user64);

                // 2. Convert byte array to json string.
                var jsonString = Encoding.ASCII.GetString(jsonBytes);

                // 3. Convert json string to model.
                var model = JsonSerializer.Deserialize<UserViewmodel>(jsonString);

                var user = new ApplicationUser
                {
                    Email = model.Email,
                    EmailConfirmed = true,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                Success = result.Succeeded;

                if (result.Errors.Count() > 0)
                {
                    Error = result.Errors.First().Description;
                }
            }
            catch (Exception ex)
            {
                Success = false;
                Error = ex.ToString();
            }
        }
    }
}
