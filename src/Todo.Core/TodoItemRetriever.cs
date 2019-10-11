using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace Todo.Core
{
    public class TodoItemRetriever : ITodoItemRetriever
    {
        private IAuthenticationProvider AuthenticationProvider { get; }
        
        public TodoItemRetriever(IAuthenticationProvider authenticationProvider)
        {
            AuthenticationProvider = authenticationProvider;
        }

        public async Task<IEnumerable<OutlookTask>> ListAsync()
        {
            var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
            return await graphServiceClient.Me.Outlook.Tasks.Request().GetAsync();
        }
    }
}
