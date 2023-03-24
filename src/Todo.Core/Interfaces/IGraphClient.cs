using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Core.Interfaces
{
    public interface IGraphClient
    {
        Task<string> RequestAsync(string uri);
    }
}
