using SecApiFinancialPositionLoader.Models;
using System.Collections.Generic;
using System.Linq;

namespace SecApiFinancialPositionLoader.Heplers
{
    public static class CompanyConceptsHelper
    {
        public static IList<SecApiCompanyFact> FilterBalanceSheetNumbers(List<CompanyConceptUnitDto> companyFactDtos)
        {
            IList<SecApiCompanyFact> facts = new List<SecApiCompanyFact>();
            foreach (CompanyConceptUnitDto concept in companyFactDtos)
            {
                if (string.IsNullOrWhiteSpace(concept.Frame))
                {
                    continue;
                }

                facts.Add(new SecApiCompanyFact()
                {
                    Value = concept.Val.ToString(),
                    Form = concept.Form,
                    Frame = concept.Frame,
                    EndDate = concept.End,
                    StartDate = concept?.Start
                });
            }

            return facts
                .OrderBy(fact => fact.EndDate)
                .ToList();
        }
    }
}
