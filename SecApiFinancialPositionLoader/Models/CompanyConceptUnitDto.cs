namespace SecApiFinancialPositionLoader.Models
{
    public class CompanyConceptUnitDto
    {
        // Start of filing period
        public string Start { get; set; }

        // End of filing period
        public string End { get; set; }

        /// <summary>
        /// Value in given currency.
        /// </summary>
        public long? Val { get; set; }

        // ???
        public string Accn { get; set; }

        // ??? Seems like year
        public int? Fy { get; set; }

        // ???
        public string Fp { get; set; }

        /// <summary>
        /// Type of the report value is taken from: "10-K" or "10-Q".
        /// </summary>
        public string Form { get; set; }

        // date
        public string Filed { get; set; }

        public string Frame { get; set; }
    }
}
