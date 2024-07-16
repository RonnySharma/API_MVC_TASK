namespace API_MVC.Twilio
{
    public class Types
    {
        public class PhoneNumber:global::Twilio.Types.PhoneNumber
        {
            public PhoneNumber(string number):base(number)
            {

            }
        }
    }
}
