using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Messages;
using Api.Services;
using DShop.Common.RabbitMq;
using Jaeger;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using OpenTracing.Propagation;
using RestEase;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITracer _tracer;
        private readonly IService _serviceA;
        private readonly IService _serviceB;
        private readonly IBusPublisher _busPublisher;
        
        public ValuesController(ITracer tracer, IBusPublisher busPublisher)
        {
            _tracer = tracer;
            _busPublisher = busPublisher;
            _serviceA = RestClient.For<IService>("http://localhost:5001");
            _serviceB = RestClient.For<IService>("http://localhost:5002");
        }
        
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            using (var scope = BuildScope())
            {
                var span = scope.Span;
                
                var resultA = await _serviceA.GetValuesAsync();
                span.Log($"service-A-fetch-completed: {String.Join(',', resultA)}");
                
                var resultB = await _serviceB.GetValuesAsync();
                span.Log($"service-B-fetch-completed: {String.Join(',', resultB)}");

                return resultA.ToList().Concat(resultB);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(GreetUser command)
        {
            await _busPublisher.SendAsync(command, GetContext());
            return Accepted();
        }

        private IScope BuildScope()
            => _tracer
                .BuildSpan("fetching-data-from-services")
                .WithTag("operation", "fetching")
                .StartActive(true);

        private ICorrelationContext GetContext()
            => CorrelationContext.Create<GreetUser>(   
                id: Guid.NewGuid(),
                userId: Guid.NewGuid(),
                resourceId: Guid.NewGuid(),
                origin: "api/values",
                traceId: _tracer.ActiveSpan.Context.TraceId.ToString(),
                spanContext: _tracer.ActiveSpan.Context.ToString(),
                connectionId: "",
                culture: "",
                resource: "");
    }
}
