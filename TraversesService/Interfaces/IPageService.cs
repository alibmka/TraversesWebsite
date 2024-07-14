using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraversesService.Interfaces
{
    public interface IPageService
    {
        Task ManagePageAsync(Uri baseUri, string url, bool processProductList = true);
    }
}
