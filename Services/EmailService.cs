namespace Store_API.Services
{
    public interface IMessageService
    {
        void SendMessage(string message);
    }
    public class EmailService : IMessageService
    {
        void IMessageService.SendMessage(string message) =>  Console.WriteLine($"Message sent: {message}");
    }
}
