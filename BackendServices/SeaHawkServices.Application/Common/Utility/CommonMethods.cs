using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Common.Utility
{
    public class CommonMethods
    {
        public static string DatePicker(DateTime? DateTime)
        {
            if (DateTime == null)
                return "";
            else
            {
                var dt = (DateTime)DateTime;
                var date = dt;
                var year = date.Year.ToString();
                var month = "";
                if (date.Month < 10) { month = "0" + date.Month.ToString(); }
                else { month = date.Month.ToString(); }
                var day = "";
                if (date.Day < 10) { day = "0" + date.Day.ToString(); }
                else { day = date.Day.ToString(); }
                return year + "-" + month + "-" + day;
            }
        }
    }
}
