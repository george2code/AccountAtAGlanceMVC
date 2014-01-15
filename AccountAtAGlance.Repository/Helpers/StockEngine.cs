using System;
using System.Collections.Generic;
using System.Xml.Linq;
using AccountAtAGlance.Model;

namespace AccountAtAGlance.Repository.Helpers
{
    public class StockEngine
    {
        private const string BaseUrl = "http://www.google.com/ig/api?";
        private const string Separator = "&stock=";

        public List<Security> GetSecurityQuotes(params string[] symbols)
        {
            XDocument doc = CreateXDocument(symbols);
            return ParseSecurities(doc);
        }

        public List<MarketIndex> GetMarketQuotes(string[] symbols)
        {
            XDocument doc = CreateXDocument(symbols);
            return ParseMarketIndexes(doc);
        }

        private XDocument CreateXDocument(string[] symbols)
        {
            string symbolList = String.Join(Separator, symbols);
            string url = string.Concat(BaseUrl, Separator, symbolList, "&Ticks=", DateTime.Now.Ticks);

            try
            {
                XDocument doc = XDocument.Load(url);
                return doc;
            }
            catch {}
            return null;
        }

        private List<Security> ParseSecurities(XDocument doc)
        {
            if (doc == null || doc.Root == null) return null;
            List<Security> securities = new List<Security>();

            IEnumerable<XElement> quotes = doc.Root.Descendants("finance");

            foreach (var quote in quotes)
            {
                var symbol = GetAttributeData(quote, "symbol");
                var exchange = GetAttributeData(quote, "exchange");
                var last = GetDecimal(quote, "last");
                var change = GetDecimal(quote, "change");
                var percentChange = GetDecimal(quote, "perc_change");
                var company = GetAttributeData(quote, "company");

                if (exchange.ToUpper() == "MUTF") // Handle mutual fund
                {
                    var mf = new MutualFund();
                    mf.Symbol = symbol;
                    mf.Last = last;
                    mf.Change = change;
                    mf.PercentChange = percentChange;
                    mf.RetrievalDateTime = DateTime.Now;
                    mf.Company = company;
                    securities.Add(mf);
                }
                else // Handle stock
                {
                    var stock = new Stock();
                    stock.Symbol = symbol;
                    stock.Last = last;
                    stock.Change = change;
                    stock.PercentChange = percentChange;
                    stock.RetrievalDateTime = DateTime.Now;
                    stock.Company = company;
                    stock.Exchange = new Exchange { Title = exchange };
                    stock.DayHigh = GetDecimal(quote, "high");
                    stock.DayLow = GetDecimal(quote, "low");
                    stock.Volume = GetDecimal(quote, "volume");
                    stock.AverageVolume = GetDecimal(quote, "avg_volume");
                    stock.MarketCap = GetDecimal(quote, "market_cap");
                    stock.Open = GetDecimal(quote, "open");
                    securities.Add(stock);
                }
            }

            return securities;
        }

        private List<MarketIndex> ParseMarketIndexes(XDocument doc)
        {
            if (doc == null || doc.Root == null) return null;
            List<MarketIndex> marketIndexes = new List<MarketIndex>();

            IEnumerable<XElement> quotes = doc.Root.Descendants("finance");

            foreach (var quote in quotes)
            {
                var index = new MarketIndex();
                index.Symbol = GetAttributeData(quote, "symbol");
                index.Last = GetDecimal(quote, "last");
                index.Change = GetDecimal(quote, "change");
                index.PercentChange = GetDecimal(quote, "perc_change");
                index.RetrievalDateTime = DateTime.Now;
                index.Title = GetAttributeData(quote, "company");
                index.DayHigh = GetDecimal(quote, "high");
                index.DayLow = GetDecimal(quote, "low");
                index.Volume = GetDecimal(quote, "volume");
                index.Open = GetDecimal(quote, "open");
                marketIndexes.Add(index);
            }

            return marketIndexes;
        }

        private string GetAttributeData(XElement quote, string elemName)
        {
            var xElement = quote.Element(elemName);
            return (xElement != null) ? xElement.Attribute("data").Value : null;
        }

        private decimal GetDecimal(XElement quote, string elemName)
        {
            var input = GetAttributeData(quote, elemName);
            if (input == null) return 0.00M;

            decimal value;

            if (Decimal.TryParse(input, out value)) return value;
            return 0.00M;
        }

        private long GetLong(XElement quote, string elemName)
        {
            var input = GetAttributeData(quote, elemName);
            if (input == null) return 0L;

            long value;

            if (long.TryParse(input, out value)) return value;
            return 0L;
        }

        private DateTime GetDateTime(XElement quote, string elemName)
        {
            var input = GetAttributeData(quote, elemName);
            if (input == null) return DateTime.Now;

            DateTime value;

            if (DateTime.TryParse(input, out value)) return value;
            return DateTime.Now;
        }
    }
}
