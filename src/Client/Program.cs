using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Client
{
    internal class Program
    {
        /// <summary>
        /// The token endpoint at IdentityServer implements the OAuth 2.0 protocol, and you could use raw HTTP to access it. 
        /// However, we have a client library called IdentityModel, that encapsulates the protocol interaction in an easy to use API.
        /// </summary>
        /// <param name="args"></param>
        private static async Task Main(string[] args)
        {
            /*
                IdentityModel includes a client library to use with the discovery endpoint. 
                This way you only need to know the base-address of IdentityServer - the actual endpoint addresses can be read from the metadata.
             */

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            /*
                Next you can use the information from the discovery document to request a token to IdentityServer to access api1
             */

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            // await CallApi(tokenResponse);
        }

        private static async Task CallApi(TokenResponse tokenResponse)
        {
            Console.WriteLine("Call API:");
            Console.WriteLine();

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
            else
            {
                Console.WriteLine(response.StatusCode);
            }
        }
    }
}
