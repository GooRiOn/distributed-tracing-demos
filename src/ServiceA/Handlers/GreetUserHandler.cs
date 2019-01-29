using System;
using System.Threading.Tasks;
using DShop.Common.Handlers;
using DShop.Common.RabbitMq;
using ServiceA.Messages;

namespace ServiceA.Handlers
{
    public class GreetUserHandler : ICommandHandler<GreetUser>
    {
        public Task HandleAsync(GreetUser command, ICorrelationContext context)
        {
            Console.WriteLine(command.Message);
            return Task.CompletedTask;
        }
    }
}