using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ISampleReportService
    {
        Task<(string FileName, Stream Content)?> GetPdfAsync(string sampleNumber);
    }
}
