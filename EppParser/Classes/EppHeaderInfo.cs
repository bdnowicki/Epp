using System;
using System.Collections.Generic;
using System.Text;

namespace EppParser.Classes
{
    public class EppHeaderInfo : EppHeader
    {
        public EppContent Opis { get; set; }
        public EppHeaderContent Zawartosc { get; set; }

        public EppHeaderInfo() : base()
        {
        }

        public override string ToString()
        {
            string opis = "pusty";
            if (Opis != null && Opis.Count > 0)
                opis = Opis[0].StringValue;

            if (Zawartosc != null)
                opis += string.Format(" - {0}", Zawartosc.Count);

            return string.Format("{0} ({1})", Name, opis);
        }

        public override string GetString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine(string.Format("[{0}]", Name));
            sb.AppendLine(Opis.GetString());
            if (Zawartosc != null)
            {
                sb.AppendLine();
                sb.AppendLine(Zawartosc.GetString());
            }

            return sb.ToString();
        }
    }
}
