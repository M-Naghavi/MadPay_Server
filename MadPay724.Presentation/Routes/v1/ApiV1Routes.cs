using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Routes.v1
{
    public static class ApiV1Routes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Site = "site";
        public const string App = "app";
        public const string Admin = "Admin";
        public const string BaseSiteAdmin = Root + "/" + Version + "/" + Site + "/" + Admin;
        public const string BaseSiteApp = Root + "/" + "/" + App + "/" + App;

        #region Users route
        public static class Users
        {
            //=>  /api/v1/site/admin/users
            // GET
            public const string GetUsers = BaseSiteAdmin + "/users";

            //=>  /api/v1/site/admin/users/{id}
            // GET
            public const string GetUser = BaseSiteAdmin + "/users/{id}";

            //=>  /api/v1/site/admin/users/{id}
            // PUT
            public const string UpdateUser = BaseSiteAdmin + "/users/{id}";

            //=>  /api/v1/site/admin/users/changeUserPassword/{id}
            // PUT
            public const string ChangeUserPassword = BaseSiteAdmin + "/users/changeUserPassword/{id}";
        }
        #endregion

        #region Photos route
        public static class Photos
        {
            //=>  /api/v1/site/admin/users/{userId}/photos
            // GET
            public const string GetPhoto = BaseSiteAdmin + "/users/{userId}/photos";

            //=>  /api/v1/site/admin/users/{userId}/photos/{id}
            // POST
            public const string ChangeUserPhoto = BaseSiteAdmin + "/users/{userId}/photos/{id}";
        }
        #endregion

        #region Auth route
        public static class Auth
        {
            //=>  /api/v1/site/admin/auth/register
            // POST
            public const string Register = BaseSiteAdmin + "/auth/register";

            //=>  /api/v1/site/admin/auth/login
            // POST
            public const string Login = BaseSiteAdmin + "/auth/login";
        } 
        #endregion
    }
}
