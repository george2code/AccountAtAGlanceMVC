using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using AccountAtAGlance.Model;

namespace AccountAtAGlance.Repository
{
    class AccountRepository : RepositoryBase<AccountAtAGlance>, IAccountRepository
    {
        private readonly bool _LocalDataOnly = Boolean.Parse(ConfigurationManager.AppSettings["LocalDataOnly"]);

        public BrokerageAccount GetAccount(string acctNumber)
        {
            return UpdateAccount(acctNumber);
        }

        public BrokerageAccount GetAccount(int id)
        {
            using (var context = DataContext)
            {
                return context.BrokerageAccounts
                    .Include("Orders")
                    .Include("WatchList")
                    .Include("Positions").SingleOrDefault(ba => ba.Id == id);
            }
        }

        public Customer GetCustomer(string custId)
        {
            using (var context = DataContext)
            {
                return context.Customers
                    .Include("BrokerageAccounts").SingleOrDefault(c => c.CustomerCode == custId);
            }
        }


        private BrokerageAccount UpdateAccount(string acctNumber)
        {
            //Force update of security values
            ISecurityRepository securityRepo = new SecurityRepository();
            securityRepo.UpdateSecurities();

            BrokerageAccount acct = null;
            acct = DataContext.BrokerageAccounts
                .Include("Orders")
                .Include("WatchList")
                .Include("WatchList.Securities")
                .Include("Positions")
                .Include("Positions.Security").SingleOrDefault(ba => ba.AccountNumber == acctNumber);

            if (acct != null && acct.Positions != null && !_LocalDataOnly)
            {
                acct.Positions = acct.Positions.OrderBy(p => p.Total).ToList();

                //Get account position securities
                var securities = acct.Positions.Select(p => p.Security).Distinct().ToList();

                var positions = acct.Positions;
                foreach (var pos in positions)
                {
                    pos.Total = pos.Shares * pos.Security.Last;
                    DataContext.Entry(pos).State = EntityState.Modified;
                    
                }
                acct.PositionsTotal = acct.Positions.Sum(p => p.Total);
                acct.Total = acct.PositionsTotal + acct.CashTotal;
                
                DataContext.Entry(acct).State = EntityState.Modified;

                try
                {
                    DataContext.SaveChanges();
                }
                catch
                {
                    //If we fail simply continue for sample app
                }
            }

            return acct;
        }
    }}
