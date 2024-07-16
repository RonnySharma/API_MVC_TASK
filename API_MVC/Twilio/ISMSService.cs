using Twilio.Rest.Api.V2010.Account;

namespace API_MVC.Twilio
{
    public interface ISMSService
    {
        MessageResource Send(string mobileNumber, string body);
    }
}
