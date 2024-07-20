using Microsoft.Extensions.Logging;
using MindOnSite.CodeKata.Share.Constants;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MindOnSite.CodeKata.Implementations.Services
{
	/// <summary>
	/// Implementation of a LRS client that can connect to a remote
	/// LRS and send or query Statements.
	/// </summary>
	public class LrsService
    {
        public ILogger<LrsService> logger;

        /// <summary>
        /// Singleton instantiated HttpClient
        /// </summary>
        public HttpClient httpClient;

        /// <summary>
        /// The endpoint (URL) for the statement API endpoint.
        /// </summary>
        public string baseUrl = "http://localhost:5000";

        /// <summary>
        /// Initializes a new instance of the LrsService.
        /// </summary>            
        public LrsService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<LrsService>();
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };
            httpClient.DefaultRequestHeaders.Add("X-Experience-API-Version", ApiVersion.Version);
        }

        /// <summary>
        /// Configures the LRS client to use basic authentication using the passed username and password.
        /// <para>
        /// Both information will be encoded to base 64 and added to the authentication HTTP header.
        /// </para>
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        public void SetBasicAuthentication(string username, string password)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        }

        /// <summary>
        /// Returns the statement identified by the statement id passed in parameter.
        /// If there is no statement linked to that identifier, the method will return null.
        /// </summary>
        /// <param name="id">A unique identifier linked to the statement to retrieve</param>
        /// <returns>The statement linked to the passed identifier if found, otherwise null</returns>
        public async Task GetStatementAsync(string id)
        {
            var response = await httpClient.GetAsync($"{baseUrl}?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    GetStatementResultInCache = null;
                }

                throw new InvalidOperationException($"The statement with id {id} could not be found. {response.ReasonPhrase}");
            }

            var data = await response.Content.ReadAsStringAsync();

            GetStatementResultInCache = JsonConvert.DeserializeObject<StatementResult>(data);
        }

        public StatementResult GetStatementResultInCache { get; set; }

        /// <summary>
        /// Retrieves more statements from the same query that returned the given
        /// more statement Uri.
        /// </summary>
        /// <param name="moreStatementsUri">The URI returned by the LRS to retrieve more statements for the same query.</param>
        /// <returns>The resulting statements.</returns>
        public async Task<StatementResult> SearchStatementsAsync(SearchTypes searchTypes, string text)
        {
            string data = null;

            switch (searchTypes)
            {
                case SearchTypes.TypeA:
                    var responseA = await httpClient.GetAsync($"{baseUrl}/typeA?text={text}");
                    data = await responseA.Content.ReadAsStringAsync();
                    break;
                case SearchTypes.TypeB:
                    var responseB = await httpClient.GetAsync($"{baseUrl}/typeB/product?text={text}");
                    data = await responseB.Content.ReadAsStringAsync();
                    break;
                case SearchTypes.TypeC:
                    var responseC = await httpClient.GetAsync($"{baseUrl}/typeC/data?text={text}");
                    data = await responseC.Content.ReadAsStringAsync();
                    break;
                case SearchTypes.TypeD:
                    var responseD = await httpClient.GetAsync($"{baseUrl}/typeD/code/kata?text={text}");
                    responseD.EnsureSuccessStatusCode();
                    data = await responseD.Content.ReadAsStringAsync();
                    break;
				case SearchTypes.TypeE:
					var responseE = await httpClient.GetAsync($"{baseUrl}/typeE/code/kata?text={text}");
					responseE.EnsureSuccessStatusCode();
					data = await responseE.Content.ReadAsStringAsync();
					break;
				default:
                    return null;
            }
            return JsonConvert.DeserializeObject<StatementResult>(data);
        }
    }
}
