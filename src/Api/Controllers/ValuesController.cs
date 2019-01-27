using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using OpenTracing.Tag;
using RestEase;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITracer _tracer;
        private readonly IService _serviiceA;
        private readonly IService _serviiceB;

        public ValuesController(ITracer tracer)
        {
            _tracer = tracer;
            _serviiceA = RestClient.For<IService>("http://localhost:5001");
            _serviiceB = RestClient.For<IService>("http://localhost:5002");
        }
        
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            using (var scope = BuildScope())
            {
                var span = scope.Span;
                
                var resultA = await _serviiceA.GetValuesAsync();
                span.Log($"service-A-fetch-completed: {String.Join(',', resultA)}");
                
                var resultB = await _serviiceB.GetValuesAsync();
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
