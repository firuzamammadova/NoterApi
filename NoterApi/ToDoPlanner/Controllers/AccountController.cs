using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using NoterApi.RequestModels;
using Services.Service;
using System.Reflection;


namespace NoterApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly JsonSerializerOptions _jsonSerializerSettings;
        private readonly UserManager<User> _userManager;
        // private readonly RoleManager<ApplicationRole> _roleManager;
        //private readonly IEmailService _emailService;
        //private readonly IStaffService _staffService;

        public AccountController(IConfiguration configuration, IUserService userService,
            ITokenService tokenService,
            //IEmailService emailService,
            UserManager<User> userManager
            //,IStaffService staffService, RoleManager<ApplicationRole> roleManager
            )
        {
            _userService = userService;
            //  _tokenService = tokenService;

            _configuration = configuration;
            _jsonSerializerSettings = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            _userManager = userManager;
            //_staffService = staffService;
            //_roleManager = roleManager;
            //_emailService = emailService;
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Put([FromBody] RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model == null) return new StatusCodeResult(500);

            var user = await _userService.FindByEmailAsync(model.Email);

            if (user != null)
            {
                return StatusCode(208, "User already exists");
            }

            user = new User()
            {
                Name = model.Name,
                Surname = model.Surname,
                GenderIdentity = model.GenderIdentity,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email.Split('@')[0],
                Email = model.Email,
                EmailConfirmed = true,
                LockoutEnabled = false,
              //  PhoneNumber = model?.PhoneNumber,
                BirthDate = model.BirthDate
            };


            var passwordErros = await _userService.ValidatePassword(model.Password);

            if (passwordErros != null)
            {
                return BadRequest(passwordErros);
            }
            var data = new Object();
            try
            {
                var result = await _userService.CreateAsync(user, model.Password);
                data = result;
                var errors = result.Errors;

                if (errors.ToList().FirstOrDefault() != null)
                {
                    return StatusCode(208, result);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }


            // await _userService.AddToRoleAsync(user, "RegisteredUser");

            return Json(JsonSerializer.Serialize(data));

        }
        [HttpPost("Auth")]
        public async Task<IActionResult>
        Jwt([FromBody] TokenRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);
            return model.grant_type switch
            {
                "password" => await GetToken(model),
                "refresh_token" => await RefreshToken(model),
                "sign_out" => await SignOut(),
                _ => new UnauthorizedResult()
            };
        }
        private async Task<IActionResult>
           GetToken(TokenRequestViewModel model)
        {
            try
            {
                //check if there's an user with the given username
                var user = await
                    _userService.FindByNameAsync(model.username);
                // fallback to support e-mail address instead of username
                if (user == null && model.username.Contains("@"))
                    user = await
                        _userService.FindByEmailAsync(model.username);

                if (user != null)
                {
                    var hasPassword = await _userService.CheckPasswordAsync(user,
                        model.password);
                    if (!hasPassword) return StatusCode(401, "not_data");
                }
                else
                {
                    return StatusCode(401, "not_data");
                }



                // username & password matches: create the refresh token
                var refreshToken = CreateRefreshToken(model.provider_id, user.Id.ToString(), model.username);

                // delete user token if it is exist in DB
                //_tokenService.Remove(new UserToken
                //{
                //    LoginProvider = model.provider_id,
                //    UserId = user.Id.ToString(),
                //    Name = model.username
                //});

                //// add the new refresh token to the DB
                //_tokenService.Add(refreshToken);

                // create & return the access token
                var token = CreateAccessToken(user, refreshToken.Value);


                return Json(token);
            }
            catch (Exception ex)
            {
                return new UnauthorizedResult();
            }
        }

        private async Task<IActionResult> RefreshToken(TokenRequestViewModel model)
        {
            try
            {
                var rt = _tokenService.FindByKeys(model.provider_id, model.refresh_token);

                if (rt == null)
                {
                    return new UnauthorizedResult();
                }

                var user = await _userService.FindByIdAsync(rt.UserId);

                if (user == null)
                {
                    return new UnauthorizedResult();
                }

                var rtNew = CreateRefreshToken(rt.LoginProvider, rt.UserId, user.UserName);

                _tokenService.Remove(rt);
                _tokenService.Add(rtNew);

                var response = CreateAccessToken(user, rtNew.Value);

                return Json(response);
            }
            catch (Exception ex)
            {
                return new UnauthorizedResult();
            }
        }
        private async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        [HttpGet("AuthWithGoogle")]
        public async Task<IActionResult> AuthWithGoogle()
        {
            string token = Request.Headers["Authorization"].ToString().Remove(0, 7); //remove Bearer 



            var data = await _userService.VerifyToken(token);
            if (data == null)
            {
                return BadRequest("Invalid token");
            }

            var user = await _userService.FindByEmailAsync(data.Email);

            if (user == null)
            {
                try
                {
                    user = new User()
                    {
                        Name = data.GivenName,
                        Surname = data.FamilyName,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = data.Email.Split('@')[0],
                        Email = data.Email,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        BirthDate = DateTime.Now
                    };
                    var result = await _userService.CreateAsync(user, data.Subject);

                }
                catch (Exception e)
                {

                    return StatusCode(500, e);
                }
            }




            try
            {

                var refreshToken = CreateRefreshToken("string", user.Id.ToString(), data.Email);


                var jwttoken = CreateAccessToken(user, refreshToken.Value);
                return Json(jwttoken);

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }


            return Ok();
        }

        private UserToken CreateRefreshToken(string clientId, string userId, string name)
        {
            return new UserToken()
            {
                LoginProvider = clientId,
                UserId = userId,
                Name = name,
                Type = 0,
                Value = Guid.NewGuid().ToString("N"),
                AddedDate = DateTime.UtcNow
            };
        }
        private TokenResponseViewModel CreateAccessToken(User user, string
           refreshToken)
        {
            var now = DateTime.UtcNow;

            // add the registered claims for JWT (RFC7519).
            // For more info, see https://tools.ietf.org/html/rfc7519#section-4.1
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
                // TODO: add additional claims here
            };

            // var roles = _userService.GetRolesAsync(user).Result;

            //if (roles != null && roles.Count > 0)
            //{
            //    claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));
            //}


            var tokenExpirationMins = _configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Auth:Jwt:Issuer"],
                audience: _configuration["Auth:Jwt:Audience"],
                claims: claims.ToArray(),
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                signingCredentials: new SigningCredentials(
                    issuerSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseViewModel()
            {
                token = encodedToken,
                expiration = tokenExpirationMins,
                refresh_token = refreshToken
            };
        }


        ////forgot password
        //[HttpPost("ResetPasswordWithConfirmation")]
        //public async Task<IActionResult> ResetPasswordWithoutOldPassword([FromBody] ChangePasswordModel requestModel)
        //{
        //    var currUser = await _userService.FindByEmailAsync(requestModel.Email);

        //    var passwordErros = await _userService.ValidatePassword(requestModel.Password);

        //    if (passwordErros != null)
        //    {
        //        return BadRequest(passwordErros);
        //    }
        //    try
        //    {
        //        if (requestModel.Password == requestModel.ConfirmPassword)
        //        {
        //            var res = await _userService.ResetPasswordWithoutOldPasswordAsync(currUser.Id.ToString(), requestModel.Password);

        //            await _userService.ValidateUser(currUser.Id.ToString());

        //            return Ok(res);
        //        }
        //        else return BadRequest();

        //    }
        //    catch (Exception ex)
        //    {
        //        return new UnprocessableEntityResult();
        //    }
        ////}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="confirmationCode"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        //[HttpPost("GetConfirmationCodeByEmail")]
        //public async Task<IActionResult> GetUserByEmail([FromBody] GetUserByEmailModel req)
        //{
        //    var currUser = await _userService.FindByEmailAsync(req.Email);

        //    if (currUser != null)
        //    {
        //        try
        //        {
        //            var res = await _userService.GetConfirmationCode(req.Email, currUser.Id.ToString());


        //            await _emailService.SendEmailAsync(currUser.Email, currUser.UserName, "Confirmation Code", @$"Hörmətli {currUser.UserName},sizin təsqidləmə kodunuz : {res}");
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(203, ex.Message);
        //            throw;
        //        }


        //        return Json(currUser.UserName);
        //    }
        //    else
        //    {
        //        return Ok("user_not_found");
        //    }
        //}
        [HttpPost("CheckConfirmationCode")]
        public async Task<IActionResult> CheckConfirmationCode([FromQuery] string confirmationCode, [FromQuery] string email)
        {
            var currUser = await _userService.FindByEmailAsync(email);

            var res = await _userService.CheckConfirmationCode(currUser.Id.ToString(), confirmationCode);

            return Ok(res);
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromQuery] long id)
        {
            var result = await _userService.DeleteUser(id);
            return Ok(result);
        }
    }
}