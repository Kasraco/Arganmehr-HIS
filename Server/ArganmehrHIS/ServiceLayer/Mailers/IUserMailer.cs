using Mvc.Mailer;
using ViewModel.Account;

namespace ServiceLayer.Mailers
{
    public interface IUserMailer
    {
        MvcMailMessage ResetPassword(EmailViewModel resetPasswordEmail);
        MvcMailMessage ConfirmAccount(EmailViewModel confirmAccountEmail);
    }
}