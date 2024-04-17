
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Config;
public abstract class ConfigDtoBase<T>
{
    public abstract T GenerateValidConfigObject();
}
