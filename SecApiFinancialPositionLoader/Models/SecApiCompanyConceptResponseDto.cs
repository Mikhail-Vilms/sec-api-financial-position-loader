using System.Collections.Generic;

namespace SecApiFinancialPositionLoader.Models
{
    /// <summary>
    /// Class reflects response from SEC API when we request values for specific company for specific financial position
    /// </summary>
    /// <remarks>
    /// You can take a look at what this class represents by executing GET request against this endpoint:
    /// https://data.sec.gov/api/xbrl/companyconcept/CIK0000055067/us-gaap/NetCashProvidedByUsedInInvestingActivities.json
    /// </remarks>
    public class SecApiCompanyConceptResponseDto
    {
        public int Cik { get; set; }
        public string Taxonomy { get; set; }
        public string Tag { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string EntityName { get; set; }

        public Dictionary<string, List<Unit>> Units { get; set; }

        public class Unit
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

            /// <summary>
            /// Specific format that is used to define timeframe(quarter/annual) associated with current value.
            /// Examples: ["CY2020Q1", "CY2018Q2", "CY2017"]
            /// </summary>
            public string Frame { get; set; }
        }
    }
}
