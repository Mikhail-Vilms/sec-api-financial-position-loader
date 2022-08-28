using SecApiFinancialPositionLoader.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SecApiFinancialPositionLoader.Heplers
{
    /// <summary>
    /// Static class for the mapping operation from the format of the response received from the SEC API to the format that reflects the structure of the dynamo item we need to save into the table
    /// </summary>
    public static class FinancialPositionMapper
    {
        // Examples: ["CY2015", "CY2016", "CY2019"]
        private static readonly Regex AnnualFrameRegex = new Regex(@"CY\d{4}");

        // Examples: ["CY2015Q3", "CY2016Q2", "CY2019Q1"]
        private static readonly Regex QuarterFrameRegex = new Regex(@"CY\d{4}Q\d{1}");

        // Examples: ["CY2015Q4", "CY2016Q4I", "CY2019Q4I"]
        private static readonly Regex FourthQuarterFrameRegex = new Regex(@"CY\d{4}Q4");

        /// <summary>
        /// Maps format of the response received from the SEC API to the format that reflects the structure of the dynamo item
        /// </summary>
        public static FinancialPositionDynamoItem Map(
            FinancialPostitionInfo financialPostitionInfo,
            SecApiCompanyConceptResponseDto companyConceptDto)
        {
            return new FinancialPositionDynamoItem()
            {
                PartitionKey = financialPostitionInfo.CikNumber,
                SortKey = $"{financialPostitionInfo.FinancialStatement}_{financialPostitionInfo.FinancialPosition}",
                TickerSymbol = financialPostitionInfo.TickerSymbol,

                CompanyName = companyConceptDto.EntityName,
                DisplayName = companyConceptDto.Label,
                Description = companyConceptDto.Description,
                Taxanomy = companyConceptDto.Taxonomy,

                Facts = companyConceptDto
                    .Units["USD"]
                    .Where(unit => !string.IsNullOrWhiteSpace(unit.Frame))
                    .Select(unit => MapFinancialFact(unit))
                    .OrderByDescending(secFact => secFact.DisplayTimeFrame)
                    .ToList()
            };
        }

        private static FinancialPositionDynamoItem.SecFact MapFinancialFact(SecApiCompanyConceptResponseDto.Unit unit)
        {
            string displayTimeFrame = GetDisplayTimeFrame(unit.Frame, out bool isAnnual);

            FinancialPositionDynamoItem.SecFact secFact = new FinancialPositionDynamoItem.SecFact()
            {
                Form = unit.Form,
                StartDate = unit.Start,
                EndDate = unit.End,
                Frame = unit.Frame,
                Value = unit.Val.ToString(),

                DisplayValue = GetDisplayValue(unit.Val.ToString()),
                DisplayTimeFrame = displayTimeFrame,
                IsAnnual = isAnnual
            };

            return secFact;
        }

        private static string GetDisplayTimeFrame(string frame, out bool isAnnual)
        {
            isAnnual = false;
            if (FourthQuarterFrameRegex.IsMatch(frame))
            {
                isAnnual = true;
                // "{year}-Y", for example: "2021-Y"
                return $"{frame.Substring(2, 4)}-Y";
            }

            if (QuarterFrameRegex.IsMatch(frame))
            {
                // "{year}-{quarter}", for example: "2021-Q2"
                return $"{frame.Substring(2, 4)}-{frame.Substring(6, 2)}";
            }

            if (AnnualFrameRegex.IsMatch(frame))
            {
                isAnnual = true;
                // "{year}-Y", for example: "2021-Y"
                return $"{frame.Substring(2, 4)}-Y";
            }

            return "N/A";
        }

        private static string GetDisplayValue(string value)
        {
            bool success = long.TryParse(value, out long valueInt);
            if (!success)
            {
                return value;
            }

            bool isPositive = valueInt >= 0;
            valueInt = Math.Abs(valueInt);

            string resultVal;
            if (valueInt < 1000)
            {
                resultVal = valueInt.ToString();
            }
            else if (1_000 <= valueInt && valueInt < 1_000_000)
            {
                resultVal = Math.Round((double)valueInt / 1_000, 2).ToString() + "K";
            }
            else if (1_000_000 <= valueInt && valueInt < 1_000_000_000)
            {
                resultVal = Math.Round((double)valueInt / 1_000_000, 2).ToString() + "M";
            }
            else
            {
                resultVal = Math.Round((double)valueInt / 1_000_000_000, 2).ToString() + "B";
            }

            return isPositive ? resultVal : $"({resultVal})";
        }
    }
}
