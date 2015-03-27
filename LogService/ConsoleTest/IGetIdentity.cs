using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ConsoleTest
{
    [ServiceContract]
    public interface IGetIdentity
    {
        [OperationContract]
        string Get(string ClientIdentity);

        [OperationContract]
        string Get1(string ClientIdentity);
    }
}
