using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSU.NocSym.Core
{
    public interface IWireDistribution
    {
        bool IsAboveControl(double length);
    }
}
