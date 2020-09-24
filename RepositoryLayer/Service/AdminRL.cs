using CommonLayer.Exceptions;
using CommonLayer.RequestModel;
using CommonLayer.ResponseModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Service
{
    public class AdminRL : IAdminRL
    {

        private UserManager<IdentityUser> _userManger;

        private RoleManager<IdentityRole> roleManager;

        //private DbSet<AdminRegisterModel> _adminmanager;
       

        /// <summary>
        /// create field for configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initialzes the memory and inject the configuration interface 
        /// </summary>
        /// <param name="configuration"></param>
        public AdminRL(IConfiguration configuration, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)//, DbSet<AdminRegisterModel> _adminmanager)
        {
            this.configuration = configuration;
            this._userManger = userManager;
            this.roleManager = roleManager;
            //this._adminmanager = _adminmanager;
        }


        /// <summary>
        /// Admin signup method
        /// </summary>
        /// <param name="adminRegisterModel"></param>
        /// <returns></returns>
        public async Task<RAdminRegisterModel> RegisterAdmin(AdminRegisterModel adminRegisterModel)
        {
            try
            {

                var identityUser = new IdentityUser
                {
                    Email = adminRegisterModel.EmailId,
                    UserName = adminRegisterModel.Name,
                };

                IdentityRole identityRole = new IdentityRole
                {
                    Name = adminRegisterModel.Role
                };

                var result = await roleManager.CreateAsync(identityRole);
                var RoleId = roleManager.GetRoleIdAsync(identityRole);

                if (!result.Succeeded)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_ROLE_EXCEPTION.ToString());
                }

                var model = new AdminRegisterModel
                {
                    Name = adminRegisterModel.Name,
                    EmailId = adminRegisterModel.EmailId,
                    Password = adminRegisterModel.Password,
                    Gender = adminRegisterModel.Gender,
                    CreatedDate = DateTime.Now.ToString(),
                    Role = adminRegisterModel.Role,
                };

                /*var results = _adminmanager.Add(model);

                if (results==null)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_ROLE_EXCEPTION.ToString());
                }*/

                string Password = adminRegisterModel.Password;
                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                adminRegisterModel.Password = Encrypt(adminRegisterModel.Password).ToString();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spAdminRegistration", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Name", adminRegisterModel.Name);
                sqlCommand.Parameters.AddWithValue("@EmailId", adminRegisterModel.EmailId);
                sqlCommand.Parameters.AddWithValue("@Password", adminRegisterModel.Password);
                sqlCommand.Parameters.AddWithValue("@Gender", adminRegisterModel.Gender);
                sqlCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString());
                sqlCommand.Parameters.AddWithValue("@Role", adminRegisterModel.Role);
                sqlCommand.Parameters.AddWithValue("@RoleId", RoleId.Id);

                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                var userData = new RAdminRegisterModel();
                while (sqlDataReader.Read())
                {
                    //userData = new RAdminLoginModel();
                    int status = sqlDataReader.GetInt32(0);
                    if (status == 0)
                    {
                        return null;
                    }

                    result = await _userManger.CreateAsync(identityUser, adminRegisterModel.Password);

                if (result.Succeeded)
                {
                    userData.AdminId = (int)sqlDataReader["ID"];
                    userData.AdminName = sqlDataReader["Name"].ToString();
                    userData.AdminEmailId = sqlDataReader["EmailId"].ToString();
                    userData.Role = sqlDataReader["Role"].ToString();
                    userData.RoleID = sqlDataReader["RoleId"].ToString();
                    userData.Gender = sqlDataReader["Gender"].ToString();
                    userData.CreatedDate = sqlDataReader["ModificateDate"].ToString();
                    //return new RAdminRegisterModel();
                }
                }

                if (userData.AdminEmailId != null)
                {
                    return userData;
                }
                else
                {
                    return null;
                }
                //return null;

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// Admin login method
        /// </summary>
        /// <param name="adminLoginModel"></param>
        /// <returns></returns>
        public async Task<RAdminLoginModel> AdminLogin(AdminLoginModel adminLoginModel)
        {
            try
            {
                var user = await _userManger.FindByEmailAsync(adminLoginModel.Email);

                if (user == null)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_EMAIL_IDENTITY.ToString());
                }
                var result = await _userManger.CheckPasswordAsync(user, adminLoginModel.Password);

                if (!result)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_PASSWORD_IDENTITY.ToString());
                }

                adminLoginModel.Password = Encrypt(adminLoginModel.Password).ToString();
                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spAdminLogin", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@EmailId", adminLoginModel.Email);
                sqlCommand.Parameters.AddWithValue("@Password", adminLoginModel.Password);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                var userData = new RAdminLoginModel();
                while (sqlDataReader.Read())
                {
                    int status = sqlDataReader.GetInt32(0);
                    if(status == 0)
                    {
                        return null;
                    }
                    userData.Id = (int)sqlDataReader["ID"];
                    userData.Name = sqlDataReader["Name"].ToString();
                    userData.EmailId = sqlDataReader["EmailId"].ToString();
                    userData.Role = sqlDataReader["Role"].ToString();
                    userData.RoleId = Convert.ToInt32(sqlDataReader["RoleId"]);
                    userData.Gender = sqlDataReader["Gender"].ToString();
                    userData.CreatedDate = sqlDataReader["ModificateDate"].ToString();
                }

                if (userData!= null)
                {
                    return userData;
                }
                return null;
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<RDeletedModel> DeleteAccount(String EmailId)
        {
            try
            {

                var user = await _userManger.FindByEmailAsync(EmailId);

                if (user == null)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_EMAIL_IDENTITY.ToString());
                }

                var result = await _userManger.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_PASSWORD_IDENTITY.ToString());
                }

                RDeletedModel rUser = new RDeletedModel();
                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spDeleteAccount", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@EmailId", EmailId);
                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    rUser.AdminId = sqlDataReader.GetInt32(0);
                    if (rUser.AdminId > 0)
                    {
                        rUser.AdminEmailId = EmailId;
                        rUser.Role = sqlDataReader["Role"].ToString();
                        rUser.AdminName = sqlDataReader["Name"].ToString();
                        rUser.Gender = sqlDataReader["Gender"].ToString();
                        rUser.CreatedDate = sqlDataReader["ModificateDate"].ToString();
                        return rUser;
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<RAdminLoginModel> UpdateModel(UpdateModel updateModel, string Email)
        {
            try
            {
               
                var user = await _userManger.FindByEmailAsync(Email);

                if (user == null)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_EMAIL_IDENTITY.ToString());
                }

                user.Email = updateModel.EmailId;
                user.UserName = updateModel.Name;

              
                var result = await _userManger.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_PASSWORD_IDENTITY.ToString());
                }

                RAdminLoginModel rUser = new RAdminLoginModel();
                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spUpdateModel", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@EmailId", updateModel.EmailId);
                sqlCommand.Parameters.AddWithValue("@Gender", updateModel.Gender);
                sqlCommand.Parameters.AddWithValue("@Name", updateModel.Name);
                //sqlCommand.Parameters.AddWithValue("@RoleId", RoleId);
                //sqlCommand.Parameters.AddWithValue("@Role", updateModel.Role);
                sqlCommand.Parameters.AddWithValue("@Email", Email);
                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    rUser.Id = sqlDataReader.GetInt32(0);
                    if (rUser.Id > 0)
                    {
                        rUser.CreatedDate = sqlDataReader["ModificateDate"].ToString();
                        rUser.Name = sqlDataReader["Name"].ToString();
                        rUser.Gender = sqlDataReader["Gender"].ToString();
                        rUser.Role = sqlDataReader["Role"].ToString();
                        rUser.RoleId = Convert.ToInt32(sqlDataReader["RoleId"]);
                        rUser.EmailId = sqlDataReader["EmailId"].ToString();
                        return rUser;
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<string> UpdateRole(string Role, int claimId , string Roles)
        {
            try
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = Role
                };

                IdentityRole role = await roleManager.FindByNameAsync(Roles);
                role.Name = Role;
                var result = await roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception(AdminExceptions.ExceptionType.INVALID_ROLE_EXCEPTION.ToString());
                }


                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spUpdateRole", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@claimId", claimId);
                sqlCommand.Parameters.AddWithValue("@Role", Role);
                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int Id = sqlDataReader.GetInt32(0);
                    if (Id > 0)
                    {
                        return "Success";
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

/*        public async Task<RAdminLoginModel> DeleteRole(string Role, int claimId)
        {
            try
            {
                RAdminLoginModel rUser = new RAdminLoginModel();
                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spDeleteRole", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@claimId", claimId);
                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int Id = sqlDataReader.GetInt32(0);
                    if (Id > 0)
                    {
                        rUser.CreatedDate = sqlDataReader["ModificateDate"].ToString();
                        rUser.Name = sqlDataReader["Name"].ToString();
                        rUser.Gender = sqlDataReader["Gender"].ToString();
                        rUser.Role = sqlDataReader["Role"].ToString();
                        rUser.RoleId = Convert.ToInt32(sqlDataReader["RoleId"]);
                        var result = roleManager.FindByIdAsync(rUser.RoleId.ToString());
                        if (result.Equals(null))
                        {
                            throw new Exception(AdminExceptions.ExceptionType.INVALID_ROLE_EXCEPTION.ToString());
                        }
                        IdentityRole identityRole = new IdentityRole
                        {
                            Name = rUser.Role
                        };
                        var result1 = await roleManager.DeleteAsync(identityRole);
                        if (!result1.Succeeded)
                        {
                            throw new Exception(AdminExceptions.ExceptionType.ERROR_ON_ROLE_DELETION.ToString());

                        }
                        return rUser;
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }*/

       /* public async Task<UserDetail> GetAllUser(string Email, string Role)
        {
            try
            {

                UserDetail Response = new UserDetail();
                var result = await _userManger.FindByEmailAsync(Email);
                Response.Id = result.Id;
                Response.EmailId = result.Email;
                Response.Name = result.UserName;
                IdentityRole result1 = await roleManager.FindByNameAsync(Role);
                Response.RoleId = Convert.ToInt32(result1.Id);
                Response.Role=result1.Name;

                return Response;

                *//*RAdminLoginModel rUser = new RAdminLoginModel();
                DatabaseConnection databaseConnection = new DatabaseConnection(this.configuration);
                SqlConnection sqlConnection = databaseConnection.GetConnection();
                SqlCommand sqlCommand = databaseConnection.GetCommand("spDeleteRole", sqlConnection);
                sqlConnection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int Id = sqlDataReader.GetInt32(0);
                    if (Id > 0)
                    {
                        rUser.CreatedDate = sqlDataReader["ModificateDate"].ToString();
                        rUser.Name = sqlDataReader["Name"].ToString();
                        rUser.Gender = sqlDataReader["Gender"].ToString();
                        rUser.Role = sqlDataReader["Role"].ToString();
                        rUser.RoleId = Convert.ToInt32(sqlDataReader["RoleId"]);
                        return rUser;
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;*//*
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }*/

        public static string Encrypt(string originalString)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes),
                CryptoStreamMode.Write);
            var writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// Declaration of decrypt method
        /// </summary>
        /// <param name="encryptedString">passing string</param>
        /// <returns>return string</returns>
        public static string Decrypt(string encryptedString)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
            if (String.IsNullOrEmpty(encryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedString));
            var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes),
                CryptoStreamMode.Read);
            var reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

    }
}
