﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Exceptions;

public class KeyVaultErrorException : Exception
{
    public KeyVaultErrorException(string message)
        : base(message)
    {
    }
}
