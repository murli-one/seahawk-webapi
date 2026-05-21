using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class XMLReportService : IXMLReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public XMLReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(byte[] Content, string FileName)> BuildDistillateXmlAsync(string sampleNumber)
        {

            var rows = await _unitOfWork.XMLReport.GetDistillateAsync(sampleNumber);

            if (rows == null || rows.Count == 0) return (Array.Empty<byte>(), string.Empty);

            var safeRows = rows.Where(r => r != null).ToList();
            if (safeRows.Count == 0) return (Array.Empty<byte>(), string.Empty);

            var first = safeRows[0];
            var lid = first.LID.HasValue
                ? first.LID.Value.ToString(CultureInfo.InvariantCulture)
                : "0";

            //var fileName = !string.IsNullOrWhiteSpace(first.FileName)
            //    ? $"{first.FileName.Trim()}.xml"
            //    : $"FO-{lid}-{DateTime.Now:yyyyMMdd}.xml";
            var guid6 = Guid.NewGuid().ToString("N").Substring(0, 6);
            var fileName = $"FO-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmssfff}-{guid6}.xml";

            using var ms = new MemoryStream();
            using (var w = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = System.Text.Encoding.UTF8, Indent = true, IndentChars = "  " }))
            {
                w.WriteStartElement("SAMPLES");
                foreach (var r in safeRows)
                {
                    w.WriteStartElement("SAMPLE");
                    w.WriteAttributeString("ID", r.SampleNumber ?? "");

                    // INFO
                    w.WriteStartElement("INFO");
                    E(w, "LID", F0(r.LID, "0")); E(w, "LNAME", S(r.Laboratory)); E(w, "VNAME", S(r.VesselName));
                    E(w, "PNAME", S(r.PortBunkered)); E(w, "BUNKTANKER", S(r.BunkerTanker)); E(w, "LOC", S(r.SampleLocation));
                    E(w, "SUP", S(r.Supplier)); E(w, "GRADE", S(r.Grade)); E(w, "SLATE", S(r.FuelType));
                    E(w, "BUNK", D(r.DateBunkered)); E(w, "RCVD", D(r.DateReceived)); E(w, "LOAD", D(r.SampleReportDate));
                    E(w, "SEAL", S(r.SealNumber)); E(w, "DENS", F0(r.SupplierDensity)); E(w, "VISC", F0(r.SupplierViscosity));
                    E(w, "SULF", F0(r.SupplierSulfurPPM)); E(w, "ASSESS", F0(r.ASSESS));
                    w.WriteEndElement(); // INFO

                    // Comment
                    E(w, "COM", S(r.Comment));

                    // RSLTS
                    w.WriteStartElement("RSLTS");
                    Res(w, "ASH", F3(r.Ash), F0(r.AshAss));
                    Res(w, "Flash Point", F0(r.FlashPoint), F0(r.FlashPointAss));
                    Res(w, "BIOFUEL SCAN", S0(r.FTIR), F0(r.FTIRAss));
                    Res(w, "VISCOSITY40C", F0(r.cstAt40), F0(r.cstAr40Ass));
                    Res(w, "DENSITY15C", F0(r.Density), F0(r.DensityAss));
                    Res(w, "CETANEIND", F0(r.Cetane), F0(r.CetaneAss));
                    Res(w, "SULFUR", F0(r.SulphurPPM), F0(r.SulphurPPMAss));
                    Res(w, "HYDROSULFIDE", F0(r.H2S), F0(r.H2SAss));
                    Res(w, "TAN", F0(r.TotalAcid), F0(r.TotalAcidAss));
                    Res(w, "PARTCONT", F0(r.ParticulateContamination), F0(r.ParticulateAss));
                    Res(w, "OXIDSTABILITY", F0(r.OxidationStability), F0(r.OxidationStabilityAss));
                    Res(w, "MCR", F0(r.MCR), F0(r.MCRAss));
                    Res(w, "CLOUDPT", F0(r.CloudPoint), F0(r.CloudPointAss));
                    Res(w, "POUR POINT", F0(r.PourPointSummStd), F0(r.PourPointAss));
                    // Appearance value is string; its *Ass is numeric
                    Res(w, "VIS APPEARANCE", S0(r.Appearance), F0(r.AppearanceAss));
                    Res(w, "WATER", F0(r.Water), F0(r.WaterAss));
                    Res(w, "WEARSCAR", F0(r.Lubricity), F0(r.LubricityAss));
                    w.WriteEndElement(); // RSLTS

                    w.WriteEndElement(); // SAMPLE
                }
                w.WriteEndElement(); // SAMPLES
                w.Flush();
            }

            return (ms.ToArray(), fileName);

            // ===== helpers =====
            static void E(XmlWriter w, string n, string v) => w.WriteElementString(n, v ?? "");
            static string S(string? s, string f = "") => string.IsNullOrWhiteSpace(s) ? f : s;
            static string S0(string? s) => string.IsNullOrWhiteSpace(s) ? "0" : s;

            static string F0(decimal? d, string fallback = "0") =>
                d.HasValue ? d.Value.ToString(CultureInfo.InvariantCulture) : fallback;

            static string F3(decimal? d, string fallback = "0") =>
                d.HasValue ? Math.Round(d.Value, 3).ToString(CultureInfo.InvariantCulture) : fallback;

            static string D(DateTime? dt, string fmt = "yyyy-MM-dd") =>
                dt.HasValue ? dt.Value.ToString(fmt, CultureInfo.InvariantCulture) : "";

            static void Res(XmlWriter w, string test, string val, string ass)
            {
                w.WriteStartElement("RES");
                E(w, "TEST", test); E(w, "VAL", string.IsNullOrWhiteSpace(val) ? "0" : val); E(w, "ASS", string.IsNullOrWhiteSpace(ass) ? "0" : ass);
                w.WriteEndElement();
            }
        }
        public async Task<(byte[] Content, string FileName)> BuildResidualXmlAsync(string sampleNumber)
        {
            var rows = await _unitOfWork.XMLReport.GetResidualAsync(sampleNumber);

            if (rows == null || rows.Count == 0) return (Array.Empty<byte>(), string.Empty);

            var safeRows = rows.Where(r => r != null).ToList();
            if (safeRows.Count == 0) return (Array.Empty<byte>(), string.Empty);

            var first = safeRows[0];
            //var lid = first.LID.HasValue
            //    ? first.LID.Value.ToString(CultureInfo.InvariantCulture)
            //    : "0";

            //var fileName = !string.IsNullOrWhiteSpace(first.FileName)
            //    ? $"{first.FileName.Trim()}.xml"
            //    : $"RO-{lid}-{DateTime.Now:yyyyMMdd}.xml"; // RO = residual oil

            var lid = first.LID?.ToString(CultureInfo.InvariantCulture) ?? "0";

            // Always make a unique RO filename: RO-YYYYMMDD-LID-HHMMSSfff-XXXXXX.xml
            var guid6 = Guid.NewGuid().ToString("N").Substring(0, 6);
            var fileName = $"RO-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmssfff}-{guid6}.xml";

            using var ms = new MemoryStream();
            using (var w = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = System.Text.Encoding.UTF8, Indent = true, IndentChars = "  " }))
            {
                w.WriteStartElement("SAMPLES");
                foreach (var r in safeRows)
                {
                    w.WriteStartElement("SAMPLE");
                    w.WriteAttributeString("ID", r.SampleNumber ?? "");

                    // INFO
                    w.WriteStartElement("INFO");
                    E(w, "LID", F0(r.LID, "0")); E(w, "LNAME", S(r.Laboratory)); E(w, "VNAME", S(r.VesselName));
                    E(w, "PNAME", S(r.PortBunkered)); E(w, "BUNKTANKER", S(r.BunkerTanker)); E(w, "LOC", S(r.SampleLocation));
                    E(w, "SUP", S(r.Supplier)); E(w, "GRADE", S(r.Grade)); E(w, "SLATE", S(r.FuelType));
                    E(w, "BUNK", D(r.DateBunkered)); E(w, "RCVD", D(r.DateReceived)); E(w, "LOAD", D(r.SampleReportDate));
                    E(w, "SEAL", S(r.SealNumber)); E(w, "DENS", F0(r.SupplierDensity)); E(w, "VISC", F0(r.SupplierViscosity));
                    E(w, "SULF", F0(r.SupplierSulphur)); E(w, "ASSESS", F0(r.ASSESS));
                    w.WriteEndElement(); // INFO

                    // Comment
                    E(w, "COM", S(r.Comment));

                    // RSLTS
                    w.WriteStartElement("RSLTS");
                    Res(w, "VISCOSITY50C", F0(r.cstAt50), F0(r.cstAt50Ass));
                    Res(w, "DENSITY15C", F0(r.Density), F0(r.DensityAss));
                    Res(w, "CCAI", F0(r.CCAI), F0(r.CCAIAss));
                    Res(w, "SULFUR", F0(r.Sulphur), F0(r.SulphurAss));
                    Res(w, "FLASH POINT", F0(r.FlashPoint), F0(r.FlashPointAss));
                    Res(w, "HYDROSULFIDE", F0(r.H2S), F0(r.H2SAss));
                    Res(w, "TAN", F0(r.TotalAcid), F0(r.TotalAcidAss));
                    Res(w, "SEDTOTAL", F0(r.TSE), F0(r.TSEAss));
                    Res(w, "MCR", F0(r.MCR), F0(r.MCRAss));
                    Res(w, "POUR POINT", F0(r.PourPointSummStd), F0(r.PourPointAss));
                    Res(w, "WATER", F0(r.Water), F0(r.WaterAss));
                    Res(w, "ASH", F3(r.Ash), F0(r.AshAss));
                    Res(w, "SODIUM", F0(r.Sodium), F0(r.SodiumAss));
                    Res(w, "VANADIUM", F0(r.Vanadium), F0(r.VanadiumAss));
                    Res(w, "ALSI", F0(r.AlSi), F0(r.AlSiAss));
                    Res(w, "ULO", F0(r.ULO), F0(r.ULOAss));
                    Res(w, "ZINC", F0(r.Zn), F0(r.ZnAss));
                    Res(w, "PHOSPHORUS", F0(r.P), F0(r.PAss));
                    Res(w, "CALCIUM", F0(r.Ca), F0(r.CaAss));
                    w.WriteEndElement(); // RSLTS

                    w.WriteEndElement(); // SAMPLE
                }
                w.WriteEndElement(); // SAMPLES
                w.Flush();
            }

            return (ms.ToArray(), fileName);

            // ===== helpers (local to this method, mirroring your distillate helpers) =====
            static void E(XmlWriter w, string n, string v) => w.WriteElementString(n, v ?? "");
            static string S(string? s, string f = "") => string.IsNullOrWhiteSpace(s) ? f : s;
            static string S0(string? s) => string.IsNullOrWhiteSpace(s) ? "0" : s;

            static string F0(decimal? d, string fallback = "0") =>
                d.HasValue ? d.Value.ToString(CultureInfo.InvariantCulture) : fallback;

            static string F3(decimal? d, string fallback = "0") =>
                d.HasValue ? Math.Round(d.Value, 3).ToString(CultureInfo.InvariantCulture) : fallback;

            static string D(DateTime? dt, string fmt = "yyyy-MM-dd") =>
                dt.HasValue ? dt.Value.ToString(fmt, CultureInfo.InvariantCulture) : "";

            static void Res(XmlWriter w, string test, string val, string ass)
            {
                w.WriteStartElement("RES");
                E(w, "TEST", test); E(w, "VAL", string.IsNullOrWhiteSpace(val) ? "0" : val); E(w, "ASS", string.IsNullOrWhiteSpace(ass) ? "0" : ass);
                w.WriteEndElement();
            }
        }
    }
}
