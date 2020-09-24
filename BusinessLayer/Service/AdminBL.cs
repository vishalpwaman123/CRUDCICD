using BusinessLayer.Interface;
using CommonLayer.RequestModel;
using CommonLayer.ResponseModel;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class AdminBL : IAdminBL
    {

        /// <summary>
        /// RL Reference.
        /// </summary>
        private IAdminRL adminRL;

        /// <summary>
        /// Constructor For Setting UserRL Instance.
        /// </summary>
        /// <param name="userRL"></param>
        public AdminBL(IAdminRL adminRL)
        {
            this.adminRL = adminRL;
        }

        /// <summary>
        /// Function For Register User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<RAdminRegisterModel> RegisterAdmin(AdminRegisterModel user)
        {
            try
            {
                return await this.adminRL.RegisterAdmin(user);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Function For Register User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<RAdminLoginModel> AdminLogin(AdminLoginModel adminLoginModel)
        {
            try
            {
                return await this.adminRL.AdminLogin(adminLoginModel);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<RDeletedModel> DeleteAccount(String EmailId)
        {
            try
            {
                return await this.adminRL.DeleteAccount(EmailId);
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
                return await this.adminRL.UpdateModel(updateModel, Email);
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
                return await this.adminRL.UpdateRole(Role, claimId , Roles);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /*public async Task<RAdminLoginModel> DeleteRole(string Role, int claimId)
        {
            try
            {
                return await this.adminRL.DeleteRole(Role, claimId);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }*/

        /*public async Task<UserDetail> GetAllUser(string Email, string Role)
        {
            try
            {
                return await this.adminRL.GetAllUser(Email, Role);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }*/
    }
}
