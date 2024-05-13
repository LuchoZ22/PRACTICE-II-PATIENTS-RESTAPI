using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using UPB.BussinessLogic.Managers.Exceptions;
using PacientesWebAPI.Middleware.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Text.Json;


namespace PacientesWebAPI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                
            }
            catch (Exception ex)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";
                var responseModel = new ResponseModel<string>() {Succed = false, Message = ex.Message};

                switch (ex)
                {
                    case NonFoundPatientException e:
                        //The patient was not found
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.Message = "The patient was not found";
                        break;
                    case CSVFileNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                    case FailedToGetDataException e:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = "The conection to the backing service was not possible";

                        break;
                        
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }


        
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
