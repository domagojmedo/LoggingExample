using System;
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
                var gitHubApi = RestService.For<IGitHubApi>("https://api.github.com");
                var octocat = await gitHubApi.GetUser("nonexistinguser");
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
    }

    public interface IGitHubApi
    {
        [Get("/users/{user}")]
        Task<string> GetUser(string user);
    }
}
