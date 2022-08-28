namespace SecApiFinancialPositionLoader.Models
{
    public class FinancialPostitionInfo
    {
        public string CikNumber { get; set; }
        public string TickerSymbol { get; set; }
        public string FinancialStatement { get; set; }
        public string FinancialPosition { get; set; }

        public override string ToString()
        {
            return $"[{TickerSymbol}/{CikNumber}/{FinancialStatement}/{FinancialPosition}]";
        }
    }
}
