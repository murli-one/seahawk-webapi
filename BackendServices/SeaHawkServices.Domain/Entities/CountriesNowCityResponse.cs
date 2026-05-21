using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    public class CountriesNowCityResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public List<string> Data { get; set; }
    }
    public class CountriesNowStatesResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public CountriesNowStatesData Data { get; set; }
    }

    public class CountriesNowStatesData
    {
        public string Name { get; set; }
        public string Iso3 { get; set; }
        public string Iso2 { get; set; }
        public List<CountriesNowStateItem> States { get; set; }
    }
    public class CountriesNowStateItem
    {
        public string Name { get; set; }

        [JsonPropertyName("state_code")]
        public string State_Code { get; set; }
    }

}
