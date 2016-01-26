using System.Collections.Generic;

namespace EppParser.Classes
{
    public abstract class EppHeader 
    {
        public string Name { get; set; }
        public abstract string GetString();
    }
}
