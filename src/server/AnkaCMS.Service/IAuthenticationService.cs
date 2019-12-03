using AnkaCMS.Service.Models;
using AnkaCMS.Core.Enums;

namespace AnkaCMS.Service
{
    public interface IAuthenticationService
    {
        void SignIn(SignInModel model);
        void SignOut(SignOutOption signOutOption);
        void SignUp(SignUpModel signUpModel);
        void ForgotPassword(string username);

    }
}
