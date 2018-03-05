using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Debugger.Data
{
    public enum SourceLanguage
    {
        Other,
        Assembly,
        CSharp,
        Java,
        Python,
        Javascript
    }

    public enum DebugMode
    {
        Assembly,
        Source
    }
}
