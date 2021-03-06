﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServerFramework.Log
{
    public interface ILoggerLog
    {
        void Log(string logString);
        void Close();
    }

    public interface ILoggerBuilder
    {
        void UseLogger(ILoggerLog logger);
    }
}
