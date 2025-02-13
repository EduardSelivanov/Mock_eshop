using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CommonPractices.CQRS.Behaviours
{
    public class LoggingBeh<TRequest, TResponse>(ILogger<LoggingBeh<TRequest, TResponse>> _logger) : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[START] Handle request={typeof(TRequest).Name} - Response={typeof(TResponse).Name} - RequestData={request}");

            var timer = new Stopwatch();
            timer.Start();

            var response = await next();

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3)
                _logger.LogWarning($"[PERFORMANCE] The request {typeof(TRequest).Name} took {timeTaken.Seconds} seconds.");

            _logger.LogInformation($"[END] Handled {typeof(TRequest).Name} with {typeof(TResponse).Name}");
            return response;
        }
    }
}
