﻿using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace SecApiFinancialPositionLoader.Models
{
    [DynamoDBTable("Sec-Api-Financial-Data")]
    public class FinancialPositionDynamoItem
    {
        [DynamoDBHashKey("PartitionKey")]
        public string PartitionKey { get; set; }

        [DynamoDBRangeKey("SortKey")]
        public string SortKey { get; set; }
 
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string Taxanomy { get; set; }
        public string TickerSymbol { get; set; }

        public IList<SecFact> Facts { get; set; }

        // https://www.sec.gov/edgar/sec-api-documentation
        // https://stackoverflow.com/questions/70950136/what-is-the-frame-attribution-in-xbrls-terminology
        // The company-concept API returns all the XBRL disclosures from a single company(CIK) and concept(a taxonomy and tag) into a single JSON file, with a separate array of facts for each units on measure that the company has chosen to disclose(e.g.net profits reported in U.S.dollars and in Canadian dollars).
        // https://data.sec.gov/api/xbrl/companyconcept/CIK##########/us-gaap/AccountsPayableCurrent.json
        public class SecFact
        {
            public string Form { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string Frame { get; set; }
            public string Value { get; set; }

            public string DisplayValue { get; set; }
            public string DisplayTimeFrame { get; set; }

            public bool IsAnnual { get; set; }
        }
    }
}
