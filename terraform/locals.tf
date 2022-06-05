locals {
    lambdaProjectName = "SecApiFinancialStatementDataLoader"
    lambdaName = "Sec-Api-Financial-Statement-Data-Loader"
    dynamoDbTableName = "Sec-Api-Data"
    sourceSns = "Sec-Api-Financial-Statement-Data-To-Process" 
}
