﻿using System;

namespace Chatanator.Core.Services
{
    public class AppService : IAppService
    {
        #region Properties

        public long LastActivity { get; set; } = 0;

        #endregion Properties
    }
}
