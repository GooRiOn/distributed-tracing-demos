using System;
using System.ComponentModel.Design;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace Shared
{
    public static class Extensions
    {
        public static IServiceCollection AddJeager(this IServiceCollection services, string name)
        {
            services.AddSingleton<ITracer>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                //var reporter = new RemoteReporter();
                var sampler = new ConstSampler(true);

                var tracerBuilder = new Tracer.Builder(name)
                    //.WithReporter()
                    .WithSampler(sampler);

                var tracer = tracerBuilder.Build();

                //GlobalTracer.Register(tracer);
                return tracer;
            });

            return services;
        }
    }
}
