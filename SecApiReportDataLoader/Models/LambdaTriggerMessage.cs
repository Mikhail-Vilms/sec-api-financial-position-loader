namespace SecApiReportDataLoader.Models
{
    public class LambdaTriggerMessage
    {
        public string CikNumber { get; set; }
        public string TickerSymbol { get; set; }
        public string FinancialStatement { get; set; }
        public string FinancialPosition { get; set; }
    }
}
