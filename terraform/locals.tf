locals {
    lambdaProjectName = "SecApiFinancialPositionLoader"
    lambdaName = "Sec-Api-Financial-Position-Loader"
    dynamoDbTableName = "Sec-Api-Financial-Data"
    sourceSns = "Sec-Api-Financial-Positions-To-Load" 
}
