using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace SeaHawkServices.Domain.Entities
{
    public static class Enums
    {
        public enum SampleType
        {
            BunkerFuel = 1,
            LubricatingOil = 2,
            TransformingInsulatingFuel = 3,
            PilotFuelMonitoring = 4,
            Urea = 5
        }

        public static string SampleTypeDesc(SampleType sampleType)
        {
            switch (sampleType)
            {
                case SampleType.BunkerFuel:
                    return "Bunker Fuel";
                case SampleType.LubricatingOil:
                    return "Lubricating Oil";
                case SampleType.TransformingInsulatingFuel:
                    return "Transforming Insulating Fuel";
                case SampleType.PilotFuelMonitoring:
                    return "Pilot Fuel Monitoring (Non-Standard!)";
                case SampleType.Urea:
                    return "UREA";
                default:
                    return string.Empty;
            }
        }


        public enum Country
        {
            Afghanistan = 1,
            Albania,
            Algeria,
            AmericanSamoa,
            Andorra,
            Angola,
            Anguilla,
            Antarctica,
            AntiguaAndBarbuda,
            Argentina,
            Armenia,
            Aruba,
            Australia,
            Austria,
            Azerbaijan,
            Bahamas,
            Bahrain,
            Bangladesh,
            Barbados,
            Belarus,
            Belgium,
            Belize,
            Benin,
            Bermuda,
            Bhutan,
            Bolivia,
            BosniaAndHerzegowina,
            Botswana,
            BouvetIsland,
            Brazil,
            BritishIndianOceanTerritory,
            BruneiDarussalam,
            Bulgaria,
            BurkinaFaso,
            Burundi,
            Cambodia,
            Cameroon,
            Canada,
            CapeVerde,
            CaymanIslands,
            CentralAfricanRepublic,
            Chad,
            Chile,
            China,
            ChristmasIsland,
            CocosKeelingIslands,
            Colombia,
            Comoros,
            Congo,
            CookIslands,
            CostaRica,
            CoteDIvoire,
            Croatia,
            Cuba,
            Cyprus,
            CzechRepublic,
            Denmark,
            Djibouti,
            Dominica,
            DominicanRepublic,
            EastTimor,
            Ecuador,
            Egypt,
            ElSalvador,
            EquatorialGuinea,
            Eritrea,
            Estonia,
            Ethiopia,
            FalklandIslands,
            FaroeIslands,
            Fiji,
            Finland,
            France,
            FrenchGuiana,
            FrenchPolynesia,
            FrenchSouthernTerritories,
            Gabon,
            Gambia,
            Georgia,
            Germany,
            Ghana,
            Gibraltar,
            Greece,
            Greenland,
            Grenada,
            Guadeloupe,
            Guam,
            Guatemala,
            Guinea,
            GuineaBissau,
            Guyana,
            Haiti,
            HeardAndMcDonaldIslands,
            HolySee,
            Honduras,
            HongKong,
            Hungary,
            Iceland,
            India,
            Indonesia,
            Iran,
            Iraq,
            Ireland,
            Israel,
            Italy,
            Jamaica,
            Japan,
            Jordan,
            Kazakhstan,
            Kenya,
            Kiribati,
            NorthKorea,
            SouthKorea,
            Kuwait,
            Kyrgyzstan,
            Laos,
            Latvia,
            Lebanon,
            Lesotho,
            Liberia,
            Libya,
            Liechtenstein,
            Lithuania,
            Luxembourg,
            Macau,
            Macedonia,
            Madagascar,
            Malawi,
            Malaysia,
            Maldives,
            Mali,
            Malta,
            MarshallIslands,
            Martinique,
            Mauritania,
            Mauritius,
            Mayotte,
            Mexico,
            Micronesia,
            Moldova,
            Monaco,
            Mongolia,
            Montserrat,
            Morocco,
            Mozambique,
            Myanmar,
            Namibia,
            Nauru,
            Nepal,
            Netherlands,
            NetherlandsAntilles,
            NewCaledonia,
            NewZealand,
            Nicaragua,
            Niger,
            Nigeria,
            Niue,
            NorfolkIsland,
            NorthernMarianaIslands,
            Norway,
            Oman,
            Pakistan,
            Palau,
            Panama,
            PapuaNewGuinea,
            Paraguay,
            Peru,
            Philippines,
            Pitcairn,
            Poland,
            Portugal,
            PuertoRico,
            Qatar,
            Reunion,
            Romania,
            Russia,
            Rwanda,
            SaintKittsAndNevis,
            SaintLucia,
            SaintVincentAndTheGrenadines,
            Samoa,
            SanMarino,
            SaoTomeAndPrincipe,
            SaudiArabia,
            Senegal,
            Seychelles,
            SierraLeone,
            Singapore,
            Slovakia,
            Slovenia,
            SolomonIslands,
            Somalia,
            SouthAfrica,
            SouthGeorgiaAndSouthSandwichIslands,
            Spain,
            SriLanka,
            SaintHelena,
            SaintPierreAndMiquelon,
            Sudan,
            Suriname,
            SvalbardAndJanMayenIslands,
            Swaziland,
            Sweden,
            Switzerland,
            Syria,
            Taiwan,
            Tajikistan,
            Tanzania,
            Thailand,
            Togo,
            Tokelau,
            Tonga,
            TrinidadAndTobago,
            Tunisia,
            Turkey,
            Turkmenistan,
            TurksAndCaicosIslands,
            Tuvalu,
            Uganda,
            Ukraine,
            UnitedArabEmirates,
            UnitedKingdom,
            UnitedStates,
            UnitedStatesMinorIslands,
            Uruguay,
            Uzbekistan,
            Vanuatu,
            Venezuela,
            Vietnam,
            VirginIslandsBritish,
            VirginIslandsUS,
            WallisAndFutunaIslands,
            WesternSahara,
            Yemen,
            Yugoslavia,
            Zaire,
            Zambia,
            Zimbabwe
        }

        //public enum Country
        //{
        //    UnitedStates,
        //    Canada,
        //    Mexico,
        //    Brazil,
        //    Argentina,
        //    Panama,
        //    Colombia,
        //    Chile,
        //    Germany,
        //    Netherlands,
        //    Belgium,
        //    UnitedKingdom,
        //    France,
        //    Spain,
        //    Italy,
        //    Greece,
        //    Turkey,
        //    Russia,
        //    SouthAfrica,
        //    Egypt,
        //    Nigeria,
        //    Kenya,
        //    Morocco,
        //    UnitedArabEmirates,
        //    SaudiArabia,
        //    India,
        //    Pakistan,
        //    Bangladesh,
        //    SriLanka,
        //    China,
        //    HongKong,
        //    Singapore,
        //    SouthKorea,
        //    Japan,
        //    Vietnam,
        //    Thailand,
        //    Malaysia,
        //    Indonesia,
        //    Philippines,
        //    Australia,
        //    NewZealand
        //}

        public enum MinuteEnum
        {
            M00 = 0,
            M15 = 15,
            M30 = 30,
            M45 = 45
        }

        public enum CourierProvider
        {
            FedEx = 1,
            DHL= 2
        }
        public enum HourEnum
        {
            H09 = 9,
            H10 = 10,
            H11 = 11,
            H12 = 12,
            H13 = 13,
            H14 = 14,
            H15 = 15,
            H16 = 16
        }


        public enum Role
        {
            Pending =0,
            SystemAdmin = 1,
            ManagementUser = 2,
            VesselUser = 3,
        }
        public enum FuelType
        {
            All = 0,
            IFO = 1,
            DO = 2,
        }
        public enum Specification
        {
            All = 0,
            Normal = 1,
            Caution = 2,
            Critical = 3,
            InProcess = 4,
            Completed = 5,
            OK = 6,
        }
        public enum Status
        {
            Active = 0,
            Inactive = 1
        }

        public enum AnalysisResultStatus
        {
            Preliminary = 0,
            Final = 1
        }
    }
}

