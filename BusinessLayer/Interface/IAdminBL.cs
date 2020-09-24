using CommonLayer.RequestModel;
using CommonLayer.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IAdminBL
    {

        /// <summary>
        /// Abstact Function For Register Admin.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<RAdminRegisterModel> RegisterAdmin(AdminRegisterModel adminRegisterModel);

        /// <summary>
        /// Abstact Function For Login Admin.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<RAdminLoginModel> AdminLogin(AdminLoginModel adminLoginModel);

        /// <summary>
        /// Abstact Function For Admin Login.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<RDeletedModel> DeleteAccount(String EmailId);

        /// <summary>
        /// Abstact Function For Admin Login.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<RAdminLoginModel> UpdateModel(UpdateModel updateModel, string Email);

        /// <summary>
        /// Abstact Function For Admin Login.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> UpdateRole(string Role, int claimId , string Roles);

        /// <summary>
        /// Abstact Function For Admin Login.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //Task<RAdminLoginModel> DeleteRole(string Role, int claimId);

        /// <summary>
        /// Abstract Function for Get all user
        /// </summary>
        /// <returns></returns>
        //Task<UserDetail> GetAllUser(string Email, string Role);
    }
}
