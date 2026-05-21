using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;


namespace SeaHawkServices.Domain.Entities
{
    public static class CountryIsoHelper
    {
        private static readonly Dictionary<Country, string> IsoMap = new() {
    { Country.Afghanistan, "AF" },{ Country.Albania, "AL" },{ Country.Algeria, "DZ" },{ Country.AmericanSamoa, "AS" },{ Country.Andorra, "AD" },{ Country.Angola, "AO" },{ Country.Anguilla, "AI" },{ Country.Antarctica, "AQ" },{ Country.AntiguaAndBarbuda, "AG" },{ Country.Argentina, "AR" },
    { Country.Armenia, "AM" },    { Country.Aruba, "AW" },{ Country.Australia, "AU" },    { Country.Austria, "AT" },{ Country.Azerbaijan, "AZ" },{ Country.Bahamas, "BS" },{ Country.Bahrain, "BH" },{ Country.Bangladesh, "BD" },
    { Country.Barbados, "BB" },{ Country.Belarus, "BY" },{ Country.Belgium, "BE" },{ Country.Belize, "BZ" },{ Country.Benin, "BJ" },{ Country.Bermuda, "BM" },{ Country.Bhutan, "BT" },
    { Country.Bolivia, "BO" },{ Country.BosniaAndHerzegowina, "BA" },{ Country.Botswana, "BW" },{ Country.BouvetIsland, "BV" },{ Country.Brazil, "BR" },{ Country.BritishIndianOceanTerritory, "IO" },
    { Country.BruneiDarussalam, "BN" },{ Country.Bulgaria, "BG" },{ Country.BurkinaFaso, "BF" },{ Country.Burundi, "BI" },{ Country.Cambodia, "KH" },{ Country.Cameroon, "CM" },
    { Country.Canada, "CA" },{ Country.CapeVerde, "CV" },{ Country.CaymanIslands, "KY" },{ Country.CentralAfricanRepublic, "CF" },{ Country.Chad, "TD" },{ Country.Chile, "CL" },{ Country.China, "CN" },
    { Country.ChristmasIsland, "CX" },{ Country.CocosKeelingIslands, "CC" },{ Country.Colombia, "CO" },{ Country.Comoros, "KM" },
    { Country.Congo, "CG" }, // Republic of the Congo
    { Country.CookIslands, "CK" },
    { Country.CostaRica, "CR" },
    { Country.CoteDIvoire, "CI" },
    { Country.Croatia, "HR" },
    { Country.Cuba, "CU" },
    { Country.Cyprus, "CY" },
    { Country.CzechRepublic, "CZ" },

    { Country.Denmark, "DK" },
    { Country.Djibouti, "DJ" },
    { Country.Dominica, "DM" },
    { Country.DominicanRepublic, "DO" },

    { Country.EastTimor, "TL" },
    { Country.Ecuador, "EC" },
    { Country.Egypt, "EG" },
    { Country.ElSalvador, "SV" },
    { Country.EquatorialGuinea, "GQ" },
    { Country.Eritrea, "ER" },
    { Country.Estonia, "EE" },
    { Country.Ethiopia, "ET" },

    { Country.FalklandIslands, "FK" },
    { Country.FaroeIslands, "FO" },
    { Country.Fiji, "FJ" },
    { Country.Finland, "FI" },
    { Country.France, "FR" },
    { Country.FrenchGuiana, "GF" },
    { Country.FrenchPolynesia, "PF" },
    { Country.FrenchSouthernTerritories, "TF" },  { Country.Gabon, "GA" },
    { Country.Gambia, "GM" },
    { Country.Georgia, "GE" },
    { Country.Germany, "DE" },
    { Country.Ghana, "GH" },
    { Country.Gibraltar, "GI" },
    { Country.Greece, "GR" },
    { Country.Greenland, "GL" },
    { Country.Grenada, "GD" },
    { Country.Guadeloupe, "GP" },
    { Country.Guam, "GU" },
    { Country.Guatemala, "GT" },
    { Country.Guinea, "GN" },
    { Country.GuineaBissau, "GW" },
    { Country.Guyana, "GY" },

    { Country.Haiti, "HT" },
    { Country.HeardAndMcDonaldIslands, "HM" },
    { Country.HolySee, "VA" },
    { Country.Honduras, "HN" },
    { Country.HongKong, "HK" },
    { Country.Hungary, "HU" },

    { Country.Iceland, "IS" },
    { Country.India, "IN" },
    { Country.Indonesia, "ID" },
    { Country.Iran, "IR" },
    { Country.Iraq, "IQ" },
    { Country.Ireland, "IE" },
    { Country.Israel, "IL" },
    { Country.Italy, "IT" },

    { Country.Jamaica, "JM" },
    { Country.Japan, "JP" },
    { Country.Jordan, "JO" },

    { Country.Kazakhstan, "KZ" },
    { Country.Kenya, "KE" },
    { Country.Kiribati, "KI" },
    { Country.NorthKorea, "KP" },
    { Country.SouthKorea, "KR" },
    { Country.Kuwait, "KW" },
    { Country.Kyrgyzstan, "KG" },

    { Country.Laos, "LA" },
    { Country.Latvia, "LV" },
    { Country.Lebanon, "LB" },
    { Country.Lesotho, "LS" },
    { Country.Liberia, "LR" },
    { Country.Libya, "LY" },
    { Country.Liechtenstein, "LI" },
    { Country.Lithuania, "LT" },
    { Country.Luxembourg, "LU" },

    { Country.Macau, "MO" },
    { Country.Macedonia, "MK" }, // ISO still MK for North Macedonia
    { Country.Madagascar, "MG" },
    { Country.Malawi, "MW" },
    { Country.Malaysia, "MY" },
    { Country.Maldives, "MV" },
    { Country.Mali, "ML" },
    { Country.Malta, "MT" },
    { Country.MarshallIslands, "MH" },
    { Country.Martinique, "MQ" },
    { Country.Mauritania, "MR" },
    { Country.Mauritius, "MU" },
    { Country.Mayotte, "YT" },
    { Country.Mexico, "MX" },
    { Country.Micronesia, "FM" },
    { Country.Moldova, "MD" },
    { Country.Monaco, "MC" },
    { Country.Mongolia, "MN" },
    { Country.Montserrat, "MS" },
    { Country.Morocco, "MA" },
    { Country.Mozambique, "MZ" },
    { Country.Myanmar, "MM" },
     { Country.Namibia, "NA" },
    { Country.Nauru, "NR" },
    { Country.Nepal, "NP" },
    { Country.Netherlands, "NL" },
    { Country.NetherlandsAntilles, "AN" }, // historical, still AN
    { Country.NewCaledonia, "NC" },
    { Country.NewZealand, "NZ" },
    { Country.Nicaragua, "NI" },
    { Country.Niger, "NE" },
    { Country.Nigeria, "NG" },
    { Country.Niue, "NU" },
    { Country.NorfolkIsland, "NF" },
    { Country.NorthernMarianaIslands, "MP" },
    { Country.Norway, "NO" },

    { Country.Oman, "OM" },

    { Country.Pakistan, "PK" },
    { Country.Palau, "PW" },
    { Country.Panama, "PA" },
    { Country.PapuaNewGuinea, "PG" },
    { Country.Paraguay, "PY" },
    { Country.Peru, "PE" },
    { Country.Philippines, "PH" },
    { Country.Pitcairn, "PN" },
    { Country.Poland, "PL" },
    { Country.Portugal, "PT" },
    { Country.PuertoRico, "PR" },

    { Country.Qatar, "QA" },

    { Country.Reunion, "RE" },
    { Country.Romania, "RO" },
    { Country.Russia, "RU" },
    { Country.Rwanda, "RW" },

    { Country.SaintKittsAndNevis, "KN" },
    { Country.SaintLucia, "LC" },
    { Country.SaintVincentAndTheGrenadines, "VC" },
    { Country.Samoa, "WS" },
    { Country.SanMarino, "SM" },
    { Country.SaoTomeAndPrincipe, "ST" },
    { Country.SaudiArabia, "SA" },
    { Country.Senegal, "SN" },
    { Country.Seychelles, "SC" },
    { Country.SierraLeone, "SL" },
    { Country.Singapore, "SG" },
    { Country.Slovakia, "SK" },
    { Country.Slovenia, "SI" },
    { Country.SolomonIslands, "SB" },
    { Country.Somalia, "SO" },
    { Country.SouthAfrica, "ZA" },
    { Country.SouthGeorgiaAndSouthSandwichIslands, "GS" },
    { Country.Spain, "ES" },
    { Country.SriLanka, "LK" },
    { Country.SaintHelena, "SH" },
    { Country.SaintPierreAndMiquelon, "PM" },
    { Country.Sudan, "SD" },
    { Country.Suriname, "SR" },
    { Country.SvalbardAndJanMayenIslands, "SJ" },
    { Country.Swaziland, "SZ" }, // Eswatini → SZ
    { Country.Sweden, "SE" },
    { Country.Switzerland, "CH" },
    { Country.Syria, "SY" },{ Country.Taiwan, "TW" },
    { Country.Tajikistan, "TJ" },
    { Country.Tanzania, "TZ" },
    { Country.Thailand, "TH" },
    { Country.Togo, "TG" },
    { Country.Tokelau, "TK" },
    { Country.Tonga, "TO" },
    { Country.TrinidadAndTobago, "TT" },
    { Country.Tunisia, "TN" },
    { Country.Turkey, "TR" },
    { Country.Turkmenistan, "TM" },
    { Country.TurksAndCaicosIslands, "TC" },
    { Country.Tuvalu, "TV" },

    { Country.Uganda, "UG" },
    { Country.Ukraine, "UA" },
    { Country.UnitedArabEmirates, "AE" },
    { Country.UnitedKingdom, "GB" },
    { Country.UnitedStates, "US" },
    { Country.UnitedStatesMinorIslands, "UM" },
    { Country.Uruguay, "UY" },
    { Country.Uzbekistan, "UZ" },

    { Country.Vanuatu, "VU" },
    { Country.Venezuela, "VE" },
    { Country.Vietnam, "VN" },
    { Country.VirginIslandsBritish, "VG" },
    { Country.VirginIslandsUS, "VI" },

    { Country.WallisAndFutunaIslands, "WF" },
    { Country.WesternSahara, "EH" },

    { Country.Yemen, "YE" },
    { Country.Yugoslavia, "YU" }, // legacy ISO: YU
    { Country.Zaire, "ZR" },      // legacy ISO: ZR (now DR Congo CD)
    { Country.Zambia, "ZM" },
    { Country.Zimbabwe, "ZW" },
};

        private static readonly Dictionary<string, Country> IsoReverseMap = IsoMap.ToDictionary(x => x.Value, x => x.Key, StringComparer.OrdinalIgnoreCase);

        public static string ToIso2(Country country)
        {
            if (IsoMap.TryGetValue(country, out var code))
                return code;

            return "US"; // fallback
        }

        public static Country FromIso2(string iso2Code)
        {
            if (string.IsNullOrWhiteSpace(iso2Code))
                return Country.UnitedStates; // fallback

            if (IsoReverseMap.TryGetValue(iso2Code.Trim(), out var country))
                return country;

            return Country.UnitedStates; // fallback if unknown
        }
    }

}
