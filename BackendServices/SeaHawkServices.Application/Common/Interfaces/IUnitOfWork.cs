using Microsoft.AspNetCore.Hosting;
using SeaHawkServices.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository User { get; }

        IUserLoginHistoryRepository UserLoginHistory { get; }
        IAccountHistoryRepository AccountHistory { get; }
        //ISampleCollectionRepository SampleCollection { get; }
        IXMLReportRepository XMLReport { get; }
        IRolesRepository Roles { get; }
        ICompanyRepository Companies { get; }
        IAnalysisResultRepository AnalysisResult { get; }
        IVesselDetailRepository VesselDetail { get; }
        ILiveDataRepository LiveData { get; }
        IContactUsRepository ContactUs { get; }
        INewsRepository News { get; }
        ISamplingKitRepository SamplingKit { get; }
        ICompanyUserRepository CompanyUser { get; }
        IVesselUserRepository VesselUser { get; }
        ISampleCollectionsRepository SampleCollections { get; }
        ICareerRepository Career { get; }
        IPDFRepository PDF { get; }
        ISmtpSettingRepository SmtpSettings { get; }
        Task<int> SaveAsync();
    }
}
