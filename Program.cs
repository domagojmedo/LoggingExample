using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Refit;
using Serilog;
using Serilog.Exceptions;

namespace LogginExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception} {Properties:j}")
                .CreateLogger();

            try
            {
                ThrowValidationApiException();

                //await ThrowApiException();

            }
            catch (ValidationApiException ex)
            {
                logger.Error(ex, "Exception occured");
            }
            catch (ApiException ex)
            {
                logger.Error(ex, "Exception occured");
            }

            Console.WriteLine("Hello World!");
        }

        static void ThrowValidationApiException()
        {
            var expectedProblemDetails = new ProblemDetails
            {
                Detail = "detail",
                Instance = "instance",
                Status = 1,
                Title = "title",
                Type = "type"
            };

            var exc = (ValidationApiException)FormatterServices.GetUninitializedObject(typeof(ValidationApiException));

            var prop = exc.GetType().GetProperty(nameof(ValidationApiException.Content), typeof(ProblemDetails));

            prop.SetValue(exc, expectedProblemDetails);

            throw exc;
        }

        static async Task ThrowApiException()
        {

            var gitHubApi = RestService.For<IGitHubApi>("https://api.github.com");
            var octocat = await gitHubApi.GetUser("nonexistinguser");
        }
    }

    public interface IGitHubApi
    {
        [Get("/users/{user}")]
        Task<string> GetUser(string user);
    }
}
