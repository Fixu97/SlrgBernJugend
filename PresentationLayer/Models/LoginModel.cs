using System;

namespace PresentationLayer.Models
{
    public class LoginModel
    {
        private const string NotYetSentMessage = "";
        private const string ErrorMessage = "Username or password is incorrect!";
        private const string SuccessMessage = "You should have been redirected :/...";

        public enum PageMode { NotYetSent, Erroneous, Successful }

        public PageMode CurrentMode = PageMode.NotYetSent;

        public string GetStatusMessage()
        {
            switch (CurrentMode) {
                case PageMode.NotYetSent:
                    return NotYetSentMessage;
                case PageMode.Erroneous:
                    return ErrorMessage;
                case PageMode.Successful:
                    return SuccessMessage;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
