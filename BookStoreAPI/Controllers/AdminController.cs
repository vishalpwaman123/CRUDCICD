using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Interface;
using CommonLayer;
using CommonLayer.Exceptions;
using CommonLayer.RequestModel;
using CommonLayer.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private ApplicationDbContext application;

        //UserBl Reference.
        private IAdminBL adminBL;

        //IConfiguration Reference for JWT.
        private IConfiguration configuration;

        /// <summary>
        /// Constructor for UserBL Reference.
        /// </summary>
        /// <param name="userBL"></param>
        public AdminController(IAdminBL userBL, IConfiguration configuration, ApplicationDbContext application)
        {
            this.adminBL = userBL;
            this.configuration = configuration;
            this.application = application;
        }



        /// <summary>
        /// Function For Resgister User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        //[Route("Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterModel adminRegisterModel)
        {
            try
            {
                //Throws Custom Exception When Fields are Null.
                if (adminRegisterModel.Name == null || adminRegisterModel.Password == null || adminRegisterModel.EmailId == null || adminRegisterModel.Gender == null )
                {
                    throw new Exception(AdminExceptions.ExceptionType.NULL_EXCEPTION.ToString());
                }

                //Throws Custom Exception When Fields are Empty Strings.
                if (adminRegisterModel.Name == "" || adminRegisterModel.Password == "" || adminRegisterModel.EmailId == "" || adminRegisterModel.Gender == "")
                {
                    throw new Exception(AdminExceptions.ExceptionType.EMPTY_EXCEPTION.ToString());
                }

                var data = await adminBL.RegisterAdmin(adminRegisterModel);

                if (data != null)
                {
                    return Ok(new { status = "True", message = "Register Successfully", data });
                }
                else
                {
                    return Conflict(new { status = "False", message = "Email Already Present" });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginModel adminLoginModel)
        {
            try
            {
                if (adminLoginModel.Password == null || adminLoginModel.Email == null )
                {
                    throw new Exception(AdminExceptions.ExceptionType.NULL_EXCEPTION.ToString());
                }

                //Throws Custom Exception When Fields are Empty Strings.
                if (adminLoginModel.Password == "" || adminLoginModel.Email == "")
                {
                    throw new Exception(AdminExceptions.ExceptionType.EMPTY_EXCEPTION.ToString());
                }

                RAdminLoginModel data = await this.adminBL.AdminLogin(adminLoginModel);
                if (data != null)
                {
                    data.Token = this.CreateToken(data, "authenticate role");
                    return this.Ok(new { status = "True", message = "Login Successfully", data });
                }
                else
                {
                    return this.NotFound(new { status = "False", message = "Login UnSuccessfully" });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteAccount(String EmailId)
        {
            try
            {
                RDeletedModel responseUser = null;

                if (!EmailId.Equals(null))
                {

                    //Throws Custom Exception When Fields are Null.
                    if (EmailId == null)
                    {
                        throw new Exception(AdminExceptions.ExceptionType.NULL_EXCEPTION.ToString());
                    }

                    //Throws Custom Exception When Fields are Empty Strings.
                    if (EmailId == "")
                    {
                        throw new Exception(AdminExceptions.ExceptionType.EMPTY_EXCEPTION.ToString());
                    }

                    responseUser = await adminBL.DeleteAccount(EmailId);
                }
                else
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_ROLE_EXCEPTION.ToString());
                }

                if (!responseUser.Equals(null))
                {
                  
                    bool Success = true;
                    var Message = "Delete User Successfull";
                    return Ok(new { Success, Message, });
                }
                else
                {
                    bool Success = false;
                    var Message = "Operation Unsucessfull";
                    return Conflict(new { Success, Message });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateModel(UpdateModel updateModel)
        {
            try
            {
                RAdminLoginModel responseUser = null;

                if (!updateModel.Equals(null))
                {

                    //Throws Custom Exception When Fields are Empty Strings.
                    if (updateModel.EmailId == "" || updateModel.Gender == "" || updateModel.Name == "")
                    {
                        throw new Exception(AdminExceptions.ExceptionType.EMPTY_EXCEPTION.ToString());
                    }

                    //Throws Custom Exception When Fields are Empty Strings.
                    if (updateModel.EmailId == null || updateModel.Gender == null || updateModel.Name == null)
                    {
                        throw new Exception(AdminExceptions.ExceptionType.NULL_EXCEPTION.ToString());
                    }

                    string Role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Role").Value;
                    string Email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Email").Value; 
                    //int RoleId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "RoleId").Value);

                    responseUser = await adminBL.UpdateModel(updateModel, Email);
                }
                else
                {
                    throw new Exception(AdminExceptions.ExceptionType.NULL_EXCEPTION.ToString());
                }

                if (!responseUser.Equals(null))
                {

                    bool Success = true;
                    var Message = "Model Update Successfull";
                    return Ok(new { Success, Message, });
                }
                else
                {
                    bool Success = false;
                    var Message = "Model Update UnSuccessfull";
                    return Conflict(new { Success, Message });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("Role")]
        public async Task<IActionResult> UpdateRole(string Role)
        {
            try
            {
                if(Role.Equals(null))
                {
                    throw new Exception(AdminExceptions.ExceptionType.NULL_EXCEPTION.ToString());
                }

                if(Role == "")
                {
                    throw new Exception(AdminExceptions.ExceptionType.EMPTY_EXCEPTION.ToString());
                }
                string Roles = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Role").Value;
                int claimId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

                var responseUser = await adminBL.UpdateRole(Role,claimId, Roles);
                
                if (!responseUser.Equals(null))
                {

                    bool Success = true;
                    var Message = "Role Update Successfull";
                    return Ok(new { Success, Message, });
                }
                else
                {
                    bool Success = false;
                    var Message = "Role Update UnSuccessfull";
                    return Conflict(new { Success, Message });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }

/*        [Authorize]
        [HttpDelete]
        [Route("Role")]
        public async Task<IActionResult> DeleteRole()
        {
            try
            {

                int claimId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

                string Role = (HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Role").Value).ToString();

                var responseUser = await adminBL.DeleteRole(Role,claimId);

                if (!responseUser.Equals(null))
                {

                    bool Success = true;
                    var Message = "Role Insert Successfull";
                    return Ok(new { Success, Message, });
                }
                else
                {
                    bool Success = false;
                    var Message = "Role Insert UnSuccessfull";
                    return Conflict(new { Success, Message });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }*/

       /* [Authorize]
        [HttpGet]
        [Route("Role")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {

                string Role = (HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Role").Value);

                string Email = (HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Email").Value).ToString();

                var responseUser = await adminBL.GetAllUser(Email, Role);

                if (!responseUser.Equals(null))
                {

                    bool Success = true;
                    var Message = "Role Insert Successfull";
                    return Ok(new { Success, Message, Data = responseUser });
                }
                else
                {
                    bool Success = false;
                    var Message = "Role Insert UnSuccessfull";
                    return Conflict(new { Success, Message });
                }
            }
            catch (Exception exception)
            {
                bool Success = false;
                return BadRequest(new { Success, Message = exception.Message });
            }
        }*/


        //Method to create JWT token
        private string CreateToken(RAdminLoginModel responseModel, string type)
        {
            try
            {
                var symmetricSecuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                var signingCreds = new SigningCredentials(symmetricSecuritykey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();
                claims.Add(new Claim("RoleId", responseModel.RoleId.ToString()));
                claims.Add(new Claim("Role", responseModel.Role.ToString()));
                claims.Add(new Claim("Email", responseModel.EmailId.ToString()));
                claims.Add(new Claim("Id", responseModel.Id.ToString()));
                claims.Add(new Claim("TokenType", type));

                var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                    configuration["Jwt:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCreds);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
