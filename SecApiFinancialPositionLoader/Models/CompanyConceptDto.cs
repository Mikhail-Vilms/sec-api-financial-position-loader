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
    public class CompanyConceptDto
    {
        public int Cik { get; set; }
        public string Taxonomy { get; set; }
        public string Tag { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string EntityName { get; set; }
        public Dictionary<string, List<CompanyConceptUnitDto>> Units { get; set; }
    }
}
