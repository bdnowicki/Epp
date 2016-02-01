using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EppParser.Classes
{
    public class EppItem : IEppElement
    {

        public int DecimalPlaces { get; set; }
        public EppItem()
        {
            StringValue = "";
            IsInt = false;
            IsDecimal = false;
            DecimalPlaces = 4;
        }

        public string StringValue { get; set; }
        public bool IsInt { get; set; }
        public bool IsDecimal { get; set; }
        public bool IsDateTime { get; set; }

        public bool Numeric { get { return IsDecimal || IsInt; } }

        private void ResetType()
        {
            IsDateTime = false;
            IsDecimal = false;
            IsInt = false;
        }

        public DateTime DateTimeValue
        {
            get { return (IsDateTime) ? DateTime.ParseExact(StringValue, "yyyyMMddhhmmss", CultureInfo.InvariantCulture) : DateTime.MinValue; }
            set
            {
                StringValue = value.ToString("yyyyMMddhhmmss");
                ResetType();
                IsDateTime = true;
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return (IsDecimal) ? decimal.Parse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture) : decimal.MinValue;
            }
            set
            {
                string format = "0.";
                for (int i = 0; i < DecimalPlaces; i++)
                    format += "0";
                string nVal = value.ToString(format, CultureInfo.InvariantCulture);
                StringValue = nVal;
                ResetType();
                IsDecimal = true;
            }
        }

        public int IntValue
        {
            get { return (IsInt) ? int.Parse(StringValue) : int.MinValue; }
            set
            {
                StringValue = value.ToString();
                ResetType();
                IsInt = true;
            }
        }

        public override string ToString()
        {
            return StringValue;
        }

        public string GetString()
        {
            if (string.IsNullOrEmpty(StringValue))
                return "";
            else if (IsInt)
                return StringValue;
            else if (IsDecimal)
                return StringValue;
            else if (IsDateTime)
                return StringValue;
            else
                return string.Format("\"{0}\"", StringValue);
        }
    }
}
