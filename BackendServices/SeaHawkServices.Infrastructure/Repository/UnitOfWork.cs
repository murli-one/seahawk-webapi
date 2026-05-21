using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IApplicationUserRepository User { get; private set; }
        public IUserLoginHistoryRepository UserLoginHistory { get; private set; }
        public IAccountHistoryRepository AccountHistory { get; private set; }
        public IXMLReportRepository XMLReport { get; private set; }
        public IRolesRepository Roles { get; private set; }
        public ICompanyRepository Companies { get; private set; }
        public IAnalysisResultRepository AnalysisResult { get; private set; }
        public IVesselDetailRepository VesselDetail { get; private set; }
        public ILiveDataRepository LiveData { get; private set; }
        public IContactUsRepository ContactUs { get; private set; }
        public INewsRepository News { get; private set; }
        public ISamplingKitRepository SamplingKit { get; private set; }
        public ICompanyUserRepository CompanyUser { get; private set; }
        public ISampleCollectionsRepository SampleCollections { get; private set; }
        public ICareerRepository Career { get; private set; }
        public IPDFRepository PDF { get; private set; }
        public ISmtpSettingRepository SmtpSettings { get; private set; }
        public IVesselUserRepository VesselUser { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));

            User = new ApplicationUserRepository(_db);
            UserLoginHistory = new UserLoginHistoryRepository(_db);   // 👈 NEW
            AccountHistory = new AccountHistoryRepository(_db);
            XMLReport = new XMLReportRepository(_db);
            Roles = new RolesRepository(_db);
            Companies = new CompanyRepository(_db);
            AnalysisResult = new AnalysisResultRepository(_db);
            VesselDetail = new VesselDetailRepository(_db);
            LiveData = new LiveDataRepository(_db);
            ContactUs = new ContactUsRepository(_db);
            News = new NewsRepository(_db);
            SamplingKit = new SamplingKitRepository(_db);
            CompanyUser = new CompanyUserRepository(_db);
            VesselUser = new VesselUserRepository(_db);
            SampleCollections = new SampleCollectionsRepository(_db);
            Career = new CareerRepository(_db);
            PDF = new PDFRepository(_db);
            SmtpSettings = new SmtpSettingRepository(_db);
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
