using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraversesService.Interfaces
{
    public interface IFileServices
    {
        Task SaveFileAsync(string filename, string content, string filePath);

        void SaveFileFromUrlAsync(Uri address);
    }
}
