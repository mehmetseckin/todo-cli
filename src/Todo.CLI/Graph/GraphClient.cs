using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Todo.CLI;
using Todo.CLI.Auth;

namespace MSTTool.Graph
{
    public class GraphClient
    {
        public TodoCliConfiguration Config { get; }

        public IServiceProvider ServiceProvider { get; }

        public AsyncLazy<HttpClient> HttpClientAsync { get; }

        public GraphClient(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Config = ServiceProvider.GetService<TodoCliConfiguration>();

            HttpClientAsync = new AsyncLazy<HttpClient>(CreateHttpClientAsync);
        }

        protected virtual async Task<HttpClient> CreateHttpClientAsync()
        {
            var clientOptions = new GraphClientOptions();
            var client = Microsoft.Graph.GraphClientFactory.Create(clientOptions);
            var auth = ServiceProvider.GetService<InteractiveAuthenticator>();
            await auth.ConfigureClient(client);
            return client;
        }

        // fnord JsonRequest
        public async Task<string> RequestAsync(string relativeUri)
        {
            var client = await HttpClientAsync;

            // TODO: use helper class/function to ensure is properly formatteds
            var requestUri = Config.BaseUri + relativeUri;
            Console.WriteLine("Request: {0}:", requestUri);
            var response = await client.GetAsync(requestUri);
            return await response.Content.ReadAsStringAsync();
        }

    }
}
