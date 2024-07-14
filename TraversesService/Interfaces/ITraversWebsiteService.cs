using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraversesService.Interfaces
{
    public interface ITraversWebsiteService
    {
        Task<bool> StartAsync(string url);
    }
}
