using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SeaHawkServices.Domain.Entities
{
    public static class StringFormating
    {
        public static string DatePicker(DateTime? DateTime)
        {
            if (DateTime == null)
                return string.Empty;
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

        public static string ShortDate(DateTime dt)
        {
            return dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString();
        }

        public static string ShortDateTime(DateTime dt)
        {
            return dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + (dt.Year - 2000).ToString() + " " + string.Format("{0:h:mm tt}", dt);
        }
    }
}

