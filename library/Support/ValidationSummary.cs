using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public struct ValidationSummary
    {
        public bool IsValid { get; set; }

        // todo: Put reasons validation failed here, such as out-of-range values, bad egg, EVs > 510, etc.
        // public bool TooMuchEvs {get; set;}
    }
}
