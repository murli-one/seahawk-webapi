using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IXMLReportService
    {
        //Task<(byte[] Content, string FileName)> BuildXmlAsync(string sampleNumber);
        Task<(byte[] Content, string FileName)> BuildDistillateXmlAsync(string sampleNumber);
        Task<(byte[] Content, string FileName)> BuildResidualXmlAsync(string sampleNumber);
    }

}