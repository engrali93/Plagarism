using System.Threading;
using System.Threading.Tasks;
using RestSharp;


namespace Plagarism.REST_GET
{
    public class GET_Request
    {
        private readonly CancellationToken callback;
        private string getUrl = "https://customsearch.googleapis.com/customsearch/v1?cx="; // google Url

        public async Task<IRestResponse> GetAsync(string cx, string API_key, string line_)
        {
           
            string final_url = getUrl + cx + "&exactTerms=" + line_ + "&num=1&key=" + API_key;   // building the complete Url

            IRestClient restClient = new RestClient();                                          //creating the client
            IRestRequest restRequest = new RestRequest(final_url);
            restRequest.AddHeader("Accept", "application/json");                                //setting the header
            IRestResponse restResponse = await restClient.ExecuteAsync(restRequest, callback);

            return restResponse ;
        }
    }
}
