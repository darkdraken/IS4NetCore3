using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

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
        }
    }
}
