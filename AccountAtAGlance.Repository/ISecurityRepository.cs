using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountAtAGlance.Model;

namespace AccountAtAGlance.Repository
{
    public interface ISecurityRepository
    {
        Security GetSecurity(string symbol);
        List<TickerQuote> GetSecurityTickerQuotes();
        OperationStatus UpdateSecurities();
    }
}
