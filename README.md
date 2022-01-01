

[![MIT License](https://img.shields.io/apm/l/atomic-design-ui.svg?)](https://github.com/tterb/atomic-design-ui/blob/master/LICENSEs)

# BreachedAccountInquiry On Azure

**In this sample we create a set of Azure Functions that receive HTTP requests from a client, interact with a third party REST API and communicate through Queues and write to Database.**

### Description:
We create Azure Functions to verify if user account has been involved in data breaches, according to the following requirements:

1. Azure Function 1: Receives a HTTP POST request containing a JSON payload as below:
```
{ account: "<ANY_ACCOUNT_OR_EMAIL_ADDRESS>" }
```
Then, the function add the received payload onto an Azure Queue.

2. Azure Function 2: Reads the item that Azure Function 1 put on the queue and verify the account against a Mock API which is built based on https://haveibeenpwned.com/API/v3. To learn about the signature of the Mock API endpoint see the section #3.
Once a response from the API has been received, the Function should store the result (API response) together with the account as a JSON object in a COSMOSDB instance (Ideally MongoDB).


3. Mock API endpoint: 
This enables us to get all breaches for an account which has a signature as below:  
```
GET /api/v3/breachedaccount/{account}   
hibp-api-key: [your key]
```
The API takes a single parameter which is the account to be searched for. The account is not case sensitive and will be trimmed of leading or trailing white spaces. The account should always be URL encoded. This is an authenticated API and an HIBP API key must be passed with the request.   
For more information see https://haveibeenpwned.com/API/v3#BreachesForAccount   
The API responds with a list of all breaches for the account:
```
[
 {
  "Name":"Adobe",
  "Title":"Adobe",
  "Domain":"adobe.com",
  "BreachDate":"2013-10-04",
  "AddedDate":"2013-12-04T00:00Z",
  "ModifiedDate":"2013-12-04T00:00Z"
 },
 ...
]
```
## What You Will Learn
- Creating Azure Functions
- Utilizing Azure CosmosDB 
- Utilizing Azure Queue
- Working with WireMock.Net
- Writing test for Azure Functions
## Prerequisites
- .Net 6
- VS 2022
## Acknowledgements

 - [Building a Web Application on Microsoft Azure ](https://www.linkedin.com/learning/building-a-web-application-on-microsoft-azure)
  
## Tech Stack

 .Net 6, Azure Functions, Azure Queue, Azure CosmosDB, XUnit
 , WireMock.Net


## Run Locally
To run locally, you need first to install an azure storage emulator.
 Follow this topic:
- [Install and use the Azure Cosmos DB Emulator for local development and testing ](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)
- [Install Azure Functions Core Tools](https://go.microsoft.com/fwlink/?linkid=2174087)

Clone the project

```bash
  git clone https://github.com/mohammad-niazmand/BreachedAccountInquiryOnAzure.git
```

Go to the project directory

```bash
  cd BreachedAccountInquiryOnAzure
```

Install dependencies

```bash
  dotnet build
```

Start function app

```bash
   cd src\Inquiry.FunctionApp\Inquiry.FunctionApp.Api
   func start    
```
Start BreachedAccountInquiry service

```bash
   dotnet run --project  .\src\StubBreach.API    
```

## Running Tests

To run tests, run the following command

```bash
  dotnet test
```

