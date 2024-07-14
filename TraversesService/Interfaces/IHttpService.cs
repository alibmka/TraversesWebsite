using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TraversesService.Interfaces
{
    public interface IHttpService
    {
        Task<string> DownloadContentAsync(string url);
    }
}
