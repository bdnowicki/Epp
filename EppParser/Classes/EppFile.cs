using System;
using System.Collections.Generic;

namespace EppParser.Classes
{
    public class EppFile : List<EppHeader>
    {
        /// <summary>
        /// Info header with definition of file
        /// </summary>
        public EppHeaderInfo Info { get; private set; }


        public EppFile()
        {
            Info = new EppHeaderInfo();
        }


    }
}
