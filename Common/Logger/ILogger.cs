﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logger;
public interface ILogger
{
    void LogInfo(string message);
    void LogError(string message);
    void LogSuccess(string message);
}
