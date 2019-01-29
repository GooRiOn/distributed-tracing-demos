using DShop.Common.Messages;

namespace ServiceA.Messages
{
    public class GreetUser : ICommand
    {
        public string Message { get; }   
        
        public GreetUser(string message)
        {
            Message = message;
        }
    }
}