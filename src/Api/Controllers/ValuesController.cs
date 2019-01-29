using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
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

        public ValuesController(ITracer tracer)
        {
            _tracer = tracer;
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

        private IScope BuildScope()
            => _tracer
                .BuildSpan("fetching-data-from-services")
                .WithTag("operation", "fetching")
                .StartActive(true);
    }
}
