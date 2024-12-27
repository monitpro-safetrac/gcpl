using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace MonitPro.Models.Incident
{
    public class MyDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                DateTime d = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
