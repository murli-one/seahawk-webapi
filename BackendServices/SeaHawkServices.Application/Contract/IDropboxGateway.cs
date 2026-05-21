using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Contract
{
    public interface IDropboxGateway
    {
        Task<Stream?> DownloadAsync(string dropboxPath);
        Task<string?> FindPdfPathForSampleAsync(string sampleNumber, string root);
    }
}
