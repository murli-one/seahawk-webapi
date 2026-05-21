using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;


namespace SeaHawkServices.Domain.Entities
{
    public static class GeneralEnumDDLs
    {
        public static List<SelectListItem> SampleTypeDDL()
        {
            var ddl = new List<SelectListItem>()
          {
               new SelectListItem() { Text = "Select Sample Type", Value = "0"},
               new SelectListItem() { Text = "BunkerFuel", Value = "1"},
               new SelectListItem() { Text = "LubricatingOil", Value = "2"},
               new SelectListItem() { Text = "TransformingInsulatingFuel", Value = "3"},
               new SelectListItem() { Text = "PilotFuelMonitoring", Value = "4"},
               new SelectListItem() { Text = "Urea", Value = "5"},
          };
            return ddl;
        }
        public static List<SelectListItem> StatusDDL()
        {
            var ddl = new List<SelectListItem>()
          {
               new SelectListItem() { Text = "Active", Value = "0"},
               new SelectListItem() { Text = "Inactive", Value = "1"},
          };
            return ddl;
        }


        public static List<SelectListItem> CountryListDDL()
        {
            var ddl = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Afghanistan", Value = "1" },
                new SelectListItem() { Text = "Albania", Value = "2" },
                new SelectListItem() { Text = "Algeria", Value = "3" },
                new SelectListItem() { Text = "American Samoa", Value = "4" },
                new SelectListItem() { Text = "Andorra", Value = "5" },
                new SelectListItem() { Text = "Angola", Value = "6" },
                new SelectListItem() { Text = "Anguilla", Value = "7" },
                new SelectListItem() { Text = "Antarctica", Value = "8" },
                new SelectListItem() { Text = "Antigua And Barbuda", Value = "9" },
                new SelectListItem() { Text = "Argentina", Value = "10" },
                new SelectListItem() { Text = "Armenia", Value = "11" },
                new SelectListItem() { Text = "Aruba", Value = "12" },
                new SelectListItem() { Text = "Australia", Value = "13" },
                new SelectListItem() { Text = "Austria", Value = "14" },
                new SelectListItem() { Text = "Azerbaijan", Value = "15" },
                new SelectListItem() { Text = "Bahamas", Value = "16" },
                new SelectListItem() { Text = "Bahrain", Value = "17" },
                new SelectListItem() { Text = "Bangladesh", Value = "18" },
                new SelectListItem() { Text = "Barbados", Value = "19" },
                new SelectListItem() { Text = "Belarus", Value = "20" },
                new SelectListItem() { Text = "Belgium", Value = "21" },
                new SelectListItem() { Text = "Belize", Value = "22" },
                new SelectListItem() { Text = "Benin", Value = "23" },
                new SelectListItem() { Text = "Bermuda", Value = "24" },
                new SelectListItem() { Text = "Bhutan", Value = "25" },
                new SelectListItem() { Text = "Bolivia", Value = "26" },
                new SelectListItem() { Text = "Bosnia And Herzegowina", Value = "27" },
                new SelectListItem() { Text = "Botswana", Value = "28" },
                new SelectListItem() { Text = "Bouvet Island", Value = "29" },
                new SelectListItem() { Text = "Brazil", Value = "30" },
                new SelectListItem() { Text = "British Indian Ocean Territory", Value = "31" },
                new SelectListItem() { Text = "Brunei Darussalam", Value = "32" },
                new SelectListItem() { Text = "Bulgaria", Value = "33" },
                new SelectListItem() { Text = "Burkina Faso", Value = "34" },
                new SelectListItem() { Text = "Burundi", Value = "35" },
                new SelectListItem() { Text = "Cambodia", Value = "36" },
                new SelectListItem() { Text = "Cameroon", Value = "37" },
                new SelectListItem() { Text = "Canada", Value = "38" },
                new SelectListItem() { Text = "Cape Verde", Value = "39" },
                new SelectListItem() { Text = "Cayman Islands", Value = "40" },
                new SelectListItem() { Text = "Central African Republic", Value = "41" },
                new SelectListItem() { Text = "Chad", Value = "42" },
                new SelectListItem() { Text = "Chile", Value = "43" },
                new SelectListItem() { Text = "China", Value = "44" },
                new SelectListItem() { Text = "Christmas Island", Value = "45" },
                new SelectListItem() { Text = "Cocos (Keeling) Islands", Value = "46" },
                new SelectListItem() { Text = "Colombia", Value = "47" },
                new SelectListItem() { Text = "Comoros", Value = "48" },
                new SelectListItem() { Text = "Congo", Value = "49" },
                new SelectListItem() { Text = "Cook Islands", Value = "50" },
                new SelectListItem() { Text = "Costa Rica", Value = "51" },
                new SelectListItem() { Text = "Cote D'Ivoire", Value = "52" },
                new SelectListItem() { Text = "Croatia (Local Name: Hrvatska)", Value = "53" },
                new SelectListItem() { Text = "Cuba", Value = "54" },
                new SelectListItem() { Text = "Cyprus", Value = "55" },
                new SelectListItem() { Text = "Czech Republic", Value = "56" },
                new SelectListItem() { Text = "Denmark", Value = "57" },
                new SelectListItem() { Text = "Djibouti", Value = "58" },
                new SelectListItem() { Text = "Dominica", Value = "59" },
                new SelectListItem() { Text = "Dominican Republic", Value = "60" },
                new SelectListItem() { Text = "East Timor", Value = "61" },
                new SelectListItem() { Text = "Ecuador", Value = "62" },
                new SelectListItem() { Text = "Egypt", Value = "63" },
                new SelectListItem() { Text = "El Salvador", Value = "64" },
                new SelectListItem() { Text = "Equatorial Guinea", Value = "65" },
                new SelectListItem() { Text = "Eritrea", Value = "66" },
                new SelectListItem() { Text = "Estonia", Value = "67" },
                new SelectListItem() { Text = "Ethiopia", Value = "68" },
                new SelectListItem() { Text = "Falkland Islands (Malvinas)", Value = "69" },
                new SelectListItem() { Text = "Faroe Islands", Value = "70" },
                new SelectListItem() { Text = "Fiji", Value = "71" },
                new SelectListItem() { Text = "Finland", Value = "72" },
                new SelectListItem() { Text = "France", Value = "73" },
                new SelectListItem() { Text = "French Guiana", Value = "74" },
                new SelectListItem() { Text = "French Polynesia", Value = "75" },
                new SelectListItem() { Text = "French Southern Territories", Value = "76" },
                new SelectListItem() { Text = "Gabon", Value = "77" },
                new SelectListItem() { Text = "Gambia", Value = "78" },
                new SelectListItem() { Text = "Georgia", Value = "79" },
                new SelectListItem() { Text = "Germany", Value = "80" },
                new SelectListItem() { Text = "Ghana", Value = "81" },
                new SelectListItem() { Text = "Gibraltar", Value = "82" },
                new SelectListItem() { Text = "Greece", Value = "83" },
                new SelectListItem() { Text = "Greenland", Value = "84" },
                new SelectListItem() { Text = "Grenada", Value = "85" },
                new SelectListItem() { Text = "Guadeloupe", Value = "86" },
                new SelectListItem() { Text = "Guam", Value = "87" },
                new SelectListItem() { Text = "Guatemala", Value = "88" },
                new SelectListItem() { Text = "Guinea", Value = "89" },
                new SelectListItem() { Text = "Guinea-Bissau", Value = "90" },
                new SelectListItem() { Text = "Guyana", Value = "91" },
                new SelectListItem() { Text = "Haiti", Value = "92" },
                new SelectListItem() { Text = "Heard And Mc Donald Islands", Value = "93" },
                new SelectListItem() { Text = "Holy See (Vatican City State)", Value = "94" },
                new SelectListItem() { Text = "Honduras", Value = "95" },
                new SelectListItem() { Text = "Hong Kong", Value = "96" },
                new SelectListItem() { Text = "Hungary", Value = "97" },
                new SelectListItem() { Text = "Iceland", Value = "98" },
                new SelectListItem() { Text = "India", Value = "99" },
                new SelectListItem() { Text = "Indonesia", Value = "100" },
                new SelectListItem() { Text = "Iran (Islamic Republic Of)", Value = "101" },
                new SelectListItem() { Text = "Iraq", Value = "102" },
                new SelectListItem() { Text = "Ireland", Value = "103" },
                new SelectListItem() { Text = "Israel", Value = "104" },
                new SelectListItem() { Text = "Italy", Value = "105" },
                new SelectListItem() { Text = "Jamaica", Value = "106" },
                new SelectListItem() { Text = "Japan", Value = "107" },
                new SelectListItem() { Text = "Jordan", Value = "108" },
                new SelectListItem() { Text = "Kazakhstan", Value = "109" },
                new SelectListItem() { Text = "Kenya", Value = "110" },
                new SelectListItem() { Text = "Kiribati", Value = "111" },
                new SelectListItem() { Text = "Korea, Dem People's Republic", Value = "112" },
                new SelectListItem() { Text = "Korea, Republic Of", Value = "113" },
                new SelectListItem() { Text = "Kuwait", Value = "114" },
                new SelectListItem() { Text = "Kyrgyzstan", Value = "115" },
                new SelectListItem() { Text = "Lao People's Dem Republic", Value = "116" },
                new SelectListItem() { Text = "Latvia", Value = "117" },
                new SelectListItem() { Text = "Lebanon", Value = "118" },
                new SelectListItem() { Text = "Lesotho", Value = "119" },
                new SelectListItem() { Text = "Liberia", Value = "120" },
                new SelectListItem() { Text = "Libyan Arab Jamahiriya", Value = "121" },
                new SelectListItem() { Text = "Liechtenstein", Value = "122" },
                new SelectListItem() { Text = "Lithuania", Value = "123" },
                new SelectListItem() { Text = "Luxembourg", Value = "124" },
                new SelectListItem() { Text = "Macau", Value = "125" },
                new SelectListItem() { Text = "Macedonia", Value = "126" },
                new SelectListItem() { Text = "Madagascar", Value = "127" },
                new SelectListItem() { Text = "Malawi", Value = "128" },
                new SelectListItem() { Text = "Malaysia", Value = "129" },
                new SelectListItem() { Text = "Maldives", Value = "130" },
                new SelectListItem() { Text = "Mali", Value = "131" },
                new SelectListItem() { Text = "Malta", Value = "132" },
                new SelectListItem() { Text = "Marshall Islands", Value = "133" },
                new SelectListItem() { Text = "Martinique", Value = "134" },
                new SelectListItem() { Text = "Mauritania", Value = "135" },
                new SelectListItem() { Text = "Mauritius", Value = "136" },
                new SelectListItem() { Text = "Mayotte", Value = "137" },
                new SelectListItem() { Text = "Mexico", Value = "138" },
                new SelectListItem() { Text = "Micronesia, Federated States", Value = "139" },
                new SelectListItem() { Text = "Moldova, Republic Of", Value = "140" },
                new SelectListItem() { Text = "Monaco", Value = "141" },
                new SelectListItem() { Text = "Mongolia", Value = "142" },
                new SelectListItem() { Text = "Montserrat", Value = "143" },
                new SelectListItem() { Text = "Morocco", Value = "144" },
                new SelectListItem() { Text = "Mozambique", Value = "145" },
                new SelectListItem() { Text = "Myanmar", Value = "146" },
                new SelectListItem() { Text = "Namibia", Value = "147" },
                new SelectListItem() { Text = "Nauru", Value = "148" },
                new SelectListItem() { Text = "Nepal", Value = "149" },
                new SelectListItem() { Text = "Netherlands", Value = "150" },
                new SelectListItem() { Text = "Netherlands Ant Illes", Value = "151" },
                new SelectListItem() { Text = "New Caledonia", Value = "152" },
                new SelectListItem() { Text = "New Zealand", Value = "153" },
                new SelectListItem() { Text = "Nicaragua", Value = "154" },
                new SelectListItem() { Text = "Niger", Value = "155" },
                new SelectListItem() { Text = "Nigeria", Value = "156" },
                new SelectListItem() { Text = "Niue", Value = "157" },
                new SelectListItem() { Text = "Norfolk Island", Value = "158" },
                new SelectListItem() { Text = "Northern Mariana Islands", Value = "159" },
                new SelectListItem() { Text = "Norway", Value = "160" },
                new SelectListItem() { Text = "Oman", Value = "161" },
                new SelectListItem() { Text = "Pakistan", Value = "162" },
                new SelectListItem() { Text = "Palau", Value = "163" },
                new SelectListItem() { Text = "Panama", Value = "164" },
                new SelectListItem() { Text = "Papua New Guinea", Value = "165" },
                new SelectListItem() { Text = "Paraguay", Value = "166" },
                new SelectListItem() { Text = "Peru", Value = "167" },
                new SelectListItem() { Text = "Philippines", Value = "168" },
                new SelectListItem() { Text = "Pitcairn", Value = "169" },
                new SelectListItem() { Text = "Poland", Value = "170" },
                new SelectListItem() { Text = "Portugal", Value = "171" },
                new SelectListItem() { Text = "Puerto Rico", Value = "172" },
                new SelectListItem() { Text = "Qatar", Value = "173" },
                new SelectListItem() { Text = "Reunion", Value = "174" },
                new SelectListItem() { Text = "Romania", Value = "175" },
                new SelectListItem() { Text = "Russian Federation", Value = "176" },
                new SelectListItem() { Text = "Rwanda", Value = "177" },
                new SelectListItem() { Text = "Saint K Itts And Nevis", Value = "178" },
                new SelectListItem() { Text = "Saint Lucia", Value = "179" },
                new SelectListItem() { Text = "Saint Vincent, The Grenadines", Value = "180" },
                new SelectListItem() { Text = "Samoa", Value = "181" },
                new SelectListItem() { Text = "San Marino", Value = "182" },
                new SelectListItem() { Text = "Sao Tome And Principe", Value = "183" },
                new SelectListItem() { Text = "Saudi Arabia", Value = "184" },
                new SelectListItem() { Text = "Senegal", Value = "185" },
                new SelectListItem() { Text = "Seychelles", Value = "186" },
                new SelectListItem() { Text = "Sierra Leone", Value = "187" },
                new SelectListItem() { Text = "Singapore", Value = "188" },
                new SelectListItem() { Text = "Slovakia (Slovak Republic)", Value = "189" },
                new SelectListItem() { Text = "Slovenia", Value = "190" },
                new SelectListItem() { Text = "Solomon Islands", Value = "191" },
                new SelectListItem() { Text = "Somalia", Value = "192" },
                new SelectListItem() { Text = "South Africa", Value = "193" },
                new SelectListItem() { Text = "South Georgia , S Sandwich Is.", Value = "194" },
                new SelectListItem() { Text = "Spain", Value = "195" },
                new SelectListItem() { Text = "Sri Lanka", Value = "196" },
                new SelectListItem() { Text = "St. Helena", Value = "197" },
                new SelectListItem() { Text = "St. Pierre And Miquelon", Value = "198" },
                new SelectListItem() { Text = "Sudan", Value = "199" },
                new SelectListItem() { Text = "Suriname", Value = "200" },
                new SelectListItem() { Text = "Svalbard, Jan Mayen Islands", Value = "201" },
                new SelectListItem() { Text = "Sw Aziland", Value = "202" },
                new SelectListItem() { Text = "Sweden", Value = "203" },
                new SelectListItem() { Text = "Switzerland", Value = "204" },
                new SelectListItem() { Text = "Syrian Arab Republic", Value = "205" },
                new SelectListItem() { Text = "Taiwan", Value = "206" },
                new SelectListItem() { Text = "Tajikistan", Value = "207" },
                new SelectListItem() { Text = "Tanzania, United Republic Of", Value = "208" },
                new SelectListItem() { Text = "Thailand", Value = "209" },
                new SelectListItem() { Text = "Togo", Value = "210" },
                new SelectListItem() { Text = "Tokelau", Value = "211" },
                new SelectListItem() { Text = "Tonga", Value = "212" },
                new SelectListItem() { Text = "Trinidad And Tobago", Value = "213" },
                new SelectListItem() { Text = "Tunisia", Value = "214" },
                new SelectListItem() { Text = "Turkey", Value = "215" },
                new SelectListItem() { Text = "Turkmenistan", Value = "216" },
                new SelectListItem() { Text = "Turks And Caicos Islands", Value = "217" },
                new SelectListItem() { Text = "Tuvalu", Value = "218" },
                new SelectListItem() { Text = "Uganda", Value = "219" },
                new SelectListItem() { Text = "Ukraine", Value = "220" },
                new SelectListItem() { Text = "United Arab Emirates", Value = "221" },
                new SelectListItem() { Text = "United Kingdom", Value = "222" },
                new SelectListItem() { Text = "United States", Value = "223" },
                new SelectListItem() { Text = "United States Minor Is.", Value = "224" },
                new SelectListItem() { Text = "Uruguay", Value = "225" },
                new SelectListItem() { Text = "Uzbekistan", Value = "226" },
                new SelectListItem() { Text = "Vanuatu", Value = "227" },
                new SelectListItem() { Text = "Venezuela", Value = "228" },
                new SelectListItem() { Text = "Viet Nam", Value = "229" },
                new SelectListItem() { Text = "Virgin Islands (British)", Value = "230" },
                new SelectListItem() { Text = "Virgin Islands (U.S.)", Value = "231" },
                new SelectListItem() { Text = "Wallis And Futuna Islands", Value = "232" },
                new SelectListItem() { Text = "Western Sahara", Value = "233" },
                new SelectListItem() { Text = "Yemen", Value = "234" },
                new SelectListItem() { Text = "Yugoslavia", Value = "235" },
                new SelectListItem() { Text = "Zaire", Value = "236" },
                new SelectListItem() { Text = "Zambia", Value = "237" },
                new SelectListItem() { Text = "Zimbabwe", Value = "238" }
            };

            return ddl;
        }


        public static List<SelectListItem> HourEnumDDL()
        {
            var ddl = new List<SelectListItem>()
          {
               new SelectListItem() { Text = "", Value = "0"},
               new SelectListItem() { Text = "09", Value = "9"},
               new SelectListItem() { Text = "10", Value = "10"},
               new SelectListItem() { Text = "11", Value = "11"},
               new SelectListItem() { Text = "12", Value = "12"},
               new SelectListItem() { Text = "13", Value = "13"},
               new SelectListItem() { Text = "14", Value = "14"},
               new SelectListItem() { Text = "15", Value = "15"},
               new SelectListItem() { Text = "16", Value = "16"}
         };
            return ddl;
        }

        public static List<SelectListItem> MinuteEnumDDL()
        {
            var ddl = new List<SelectListItem>()
          {
               new SelectListItem() { Text = "00", Value = "0"},
               new SelectListItem() { Text = "15", Value = "15"},
               new SelectListItem() { Text = "30", Value = "30"},
               new SelectListItem() { Text = "45", Value = "45"}
         };
            return ddl;
        }

        public static string FuelTypeDesc(FuelType type)
        {
            if (type == FuelType.All)
                return "All";
            if (type == FuelType.DO)
                return "DO";
            if (type == FuelType.IFO)
                return "IFO";
            return "";
        } 
        public static string SpecificationDesc(Specification specification)
        {
            if (specification == Specification.All)
                return "All";
            if (specification == Specification.Normal)
                return "Normal";
            if (specification == Specification.Critical)
                return "Critical";
            if (specification == Specification.Caution)
                return "Caution";
            if (specification == Specification.InProcess)
                return "In Process";
            if (specification == Specification.Completed)
                return "Completed";
            if (specification == Specification.OK)
                return "OK";
            return "";
        }
    }

}
