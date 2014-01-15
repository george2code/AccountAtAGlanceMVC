using System.Web.Mvc;
using AccountAtAGlance.Repository;

namespace AccountAtAGlance.Controllers
{
    public class DataServiceController : Controller
    {
        private IAccountRepository _accountRepository;
        private ISecurityRepository _securityRepository;
        private IMarketsAndNewsRepository _marketsAndNewsRepository;
        
        public DataServiceController(IAccountRepository accountRepo, ISecurityRepository secRepo, IMarketsAndNewsRepository marketRepo)
        {
            _accountRepository = accountRepo;
            _securityRepository = secRepo;
            _marketsAndNewsRepository = marketRepo;
        }

        public ActionResult GetAccount(string acctNumber)
        {
            return Json(_accountRepository.GetAccount(acctNumber), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuote(string symbol)
        {
            return Json(_securityRepository.GetSecurity(symbol), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMarketIndex()
        {
            return Json(_marketsAndNewsRepository.GetMarkets(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTickerQuotes()
        {
            var marketQuotes = _marketsAndNewsRepository.GetMarketTickerQuotes();
            var securityQuotes = _securityRepository.GetSecurityTickerQuotes();
            marketQuotes.AddRange(securityQuotes);
            var news = _marketsAndNewsRepository.GetMarketNews();

            return Json(new {Markets = marketQuotes, News = news}, JsonRequestBehavior.AllowGet);
        }
    }
}