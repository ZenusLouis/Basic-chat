namespace ChatAppBE.Dto
{
    public class MessageRequest
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string? Content { get; set; }
    }


}
