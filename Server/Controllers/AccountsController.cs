using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlazorDemoUdemy.Server.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController: ControllerBase{
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager, 
        IConfiguration configuration)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model){
            var user = new IdentityUser(){UserName = model.Email, Email = model.Email};
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded){
                return await BuildToken(model);

            }else {
                return BadRequest("User name or password invalid");
            }
            
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo){
            var result = await _signInManager.PasswordSignInAsync(userInfo.Email,
            userInfo.Password, isPersistent: false, lockoutOnFailure : false);
            if (result.Succeeded){
                return await BuildToken(userInfo);
            } else {
                return BadRequest("Invaliud login attempt");
            }
        }

        private async Task<UserToken> BuildToken(UserInfo userInfo){
            var claims = new List<Claim>(){
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim("myvalue","whatever I want")
            };
            var identityUser = await _userManager.FindByEmailAsync(userInfo.Email);
            var claimsDB = await _userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var expiration = DateTime.UtcNow.AddYears(1);
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer :null,
                    audience: null,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds
            );
            return new UserToken{
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };

        }


    }

}