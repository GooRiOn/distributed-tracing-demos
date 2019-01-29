using DShop.Common.Messages;

namespace Api.Messages
{
    [MessageNamespace("A")]
    public class GreetUser : ICommand
    {
        public string Message { get; }   
        
        public GreetUser(string message)
        {
            Message = message;
        }
    }
}