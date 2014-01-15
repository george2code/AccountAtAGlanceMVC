using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountAtAGlance.Model;

namespace AccountAtAGlance.Repository
{
    public interface IAccountRepository
    {
        BrokerageAccount GetAccount(string acctNumber);
        BrokerageAccount GetAccount(int id);
        Customer GetCustomer(string custId);
    }
}
