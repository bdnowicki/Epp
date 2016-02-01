using System;
using System.Collections.Generic;
using System.Text;

namespace EppParser.Classes
{
    public class TowaryHeaderInfo : EppHeaderInfo
    {
        public EppContent GetTowarByKodDostawcy(string Kod)
        {
            foreach (EppContent c in Zawartosc)
                if (c[2].StringValue == Kod)
                    return c;

            return null;
        }

        public EppContent GetTowarByEAN(string Kod)
        {
            foreach (EppContent c in Zawartosc)
                if (c[3].StringValue == Kod)
                    return c;

            return null;
        }

        public EppContent GetTowarByKod(string Kod)
        {
            foreach (EppContent c in Zawartosc)
                if (c[1].StringValue == Kod)
                    return c;

            return null;
        }

        public override string ToString()
        {
            return "(TowaryHeaderInfo) " + base.ToString();
        }
    }
}
