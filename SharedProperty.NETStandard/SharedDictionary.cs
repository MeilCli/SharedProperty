using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public sealed class SharedDictionary : BaseSharedDictionary
    {
        public SharedDictionary(ISerializer serializer, IStorage storage, IConverter converter)
            : base(serializer, storage, converter)
        {
        }
    }
}