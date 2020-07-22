using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace API_WMParking.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        public ProblemDetails ErrorEncountered()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            
            if (exceptionHandlerFeature == null)
            {
                return null;
            }

            ProblemDetails problem = ErrorDetails(exceptionHandlerFeature.Error);

            Response.StatusCode = (int)problem.Status;

            return problem;
        }

        private ProblemDetails ErrorDetails(Exception error)
        {
            if (error is InvalidOperationException)
            {
                var invalidOperationError = (InvalidOperationException)error;

                var errorMessages = new Dictionary<string, string[]>()
                {
                    { "failureReason", new string[] { invalidOperationError.Message } }
                };

                return new ValidationProblemDetails(errorMessages) {
                    Status = StatusCodes.Status400BadRequest
                };
            }

            return ProblemDetailsFactory.CreateProblemDetails(HttpContext);
        }
    }
}