﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class ProviderLookUp
    {
        List<Provider> _providers;

        public ProviderLookUp(List<Provider> providers)
        {
            _providers = providers;
        }

        public Provider GetProvider(String providerCode)
        {
            Provider provider = null;

            if (_providers != null)
            {
                provider = (from p in _providers
                            where p.Code == providerCode
                            select p).SingleOrDefault();
            }

            return provider;
        }
    }
}
