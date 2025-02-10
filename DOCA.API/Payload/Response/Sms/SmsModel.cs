namespace DOCA.API.Payload.Response.Sms;

public class SmsModel
{
    public class SmsResponse
    {
        public string status { get; set; }
        public string code { get; set; }
        public SmsResponseData Data { get; set; }
    }

    public class SmsResponseData
    {
        public long TranId { get; set; }
        public string Name { get; set; }
        public int TotalSMS { get; set; }
        public int TotalPrice { get; set; }
        public string[] InvalidPhone { get; set; }
    }
}