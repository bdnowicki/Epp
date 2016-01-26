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
            DecimalPlaces = 0;
        }

        public string StringValue { get; set; }
        public bool IsInt { get; set; }
        public bool IsDecimal { get; set; }
        public bool IsDateTime { get; set; }

        public bool Numeric { get { return IsDecimal || IsInt; } }

        public DateTime DateTimeValue
        {
            get { return (IsDateTime) ? DateTime.ParseExact(StringValue, "yyyyMMddhhmmss", CultureInfo.InvariantCulture) : DateTime.MinValue; }
        }

        public decimal DecimalValue
        {
            get { return (IsDecimal) ? decimal.Parse(StringValue, CultureInfo.InvariantCulture) : decimal.MinValue; }
            set { StringValue = value.ToString("n" + DecimalPlaces.ToString()); }
        }

        public int IntValue
        {
            get { return (IsInt) ? int.Parse(StringValue) : int.MinValue; }
            set { StringValue = value.ToString(); }
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
                return IntValue.ToString();
            else if (IsDecimal)
                return DecimalValue.ToString("n" + DecimalPlaces, CultureInfo.InvariantCulture);
            else if (IsDateTime)
                return StringValue;
            else
                return string.Format("\"{0}\"", StringValue);
        }
    }
}
