using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ConsoleTest
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    //[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]

    public class GetIdentity : IGetIdentity
    {
        public string Get(string ClientIdentity)
        {
            return ClientIdentity;
        }
        public string Get1(string ClientIdentity)
        {
            return "1111";
        }
    }
}
