
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonPractices.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            (string Detail, string Title, int StatCode) dets = exception switch
            {
                NotFoundExc => 
                (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode=StatusCodes.Status404NotFound
                ),
                _=>
                (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
                )
            };

            ProblemDetails probdet = new ProblemDetails()
            {
                Title=dets.Title,
                Detail=dets.Detail,
                Status=dets.StatCode,
                Instance=httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(probdet);
            return true;
        }
    }
}
