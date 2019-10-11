using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Core
{
    public interface ITodoItemRetriever
    {
        Task<IEnumerable<OutlookTask>> ListAsync();
    }
}
