using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Todo.CLI;
using Todo.CLI.Auth;
using Todo.Core.Interfaces;

namespace MSTTool.Graph
{
    public class GraphClient : IGraphClient
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

        // TODO: to Util class
        public bool IsAbsoluteUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result);
        }

        // uri: is absolute or relative
        public async Task<string> RequestAsync(string uri)
        {
            var client = await HttpClientAsync;

            // TODO: use helper class/function to ensure is properly formatteds
            string requestUri = IsAbsoluteUrl(uri)
                ? uri
                : Config.BaseUri + uri;
            Debug.WriteLine("Request: {0}:", requestUri);
            var response = await client.GetAsync(requestUri);
            return await response.Content.ReadAsStringAsync();
        }

    }
}
