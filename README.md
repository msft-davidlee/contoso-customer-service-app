# Disclaimer
The information contained in this README.md file and any accompanying materials (including, but not limited to, scripts, sample codes, etc.) are provided "AS-IS" and "WITH ALL FAULTS." Any estimated pricing information is provided solely for demonstration purposes and does not represent final pricing and Microsoft assumes no liability arising from your use of the information. Microsoft makes NO GUARANTEES OR WARRANTIES OF ANY KIND, WHETHER EXPRESSED OR IMPLIED, in providing this information, including any pricing information.

# Introduction 
This project show cases a solution called the Contoso Customer Service Rewards Lookup & Consumption Application. For the purpose of the discussion, we will simply refer to this application as the Customer Service App. 

## Background
Contoso is a fictious company that sells drinks and has chains all over North America. As part of building their customer base and customer loyalty, Contoso implemented a program where customers can earn points on their purchases. It has been several years since the program started and Contoso recently upgraded the system that Customer Service Rep is using to lookup and consume the points when the customer is at the store. A Partner company has been outsourced to help Contoso fulfill the order from Customers when they redeem their points for items.

## Requirements
1. The solution needs to be avaliable for over 1000 stores located in North America (US and Canada).
2. The solution needs to be avaliable during store operation hours which is between 6 AM to 11:00 PM CST.
3. The solution needs to have an SLA of 99.9% uptime.

## Architecture
1. The solution has a frontend component that requires login from the Customer Service Rep.
2. The solution has an API service that will allow users to lookup members and consume rewards.
3. When the Program changed, all members were assigned a new Member Id. However, not all members have transitioned to the new Program and may be using their old card. Hence, there is a Alternate Id Service that allows Customer Service Rep to search for the member using the old member Id. There will be a grace period and Contoso would like to retire the old member Id in 3 years.
4. Contoso IT has an agreement with the Partner IT to communicate requests via an Enterprise Service Bus (ESB) of their choosing. The Partner development team will subscribe request from the ESB.
5. In order to monitor the health of the system and ensure the SLA is met, there will be Application Monitoring created throughout the system.

![Architecture](/Architecture/Solution.png)

## SLA Details
A more concrete overview of the SLA uptime of how long the solution can be down in the context of the duration below.

1. Daily: 1m 26s
2. Weekly: 10m 4s
3. Monthly: 43m 49s
4. Quarterly: 2h 11m 29s
5. Yearly: 8h 45m 56s

# Demo
The Customer Service App can be hosted on many different platforms, which includes directly on a VM (VMSS), Container (AKS), and App Service. As such, this project by itself is not typically used independently. 

The technology stack used includes .NET 6, MS SSQL, Azure Storage Queue, Azure Service Bus, and Azure Functions. Most services can be run and debugged locally via emulators except for Azure Service Bus.

## Use this Project to showcase solutions
1. This solution employs a microservice architecture where we have the API and legacy API service in relation to the Member Id question. It may not make sense for the business to include the old Member Id as part of the new system as it will be retired in 3 years. As such, we are keeping the legacy API. Here, it may make sense to think about leveraging APIM so we have a consistent API experience for our microservice.
2. The Enterprise Service Bus approach is an interesting way for Contoso to "expose" their request to a third party because this is typically done by making a call to the third party endpoint. However, not all third party vendors will host their applications as services on the internet as their core business is not in technology but in fulfillment. Hence, the ESB approach will allow the most flexibility for any third party vendors. For example, if a vendor already has an API endpoint, they can simply create a proxy app to subscribe and push the request to their API. If they don't, they will create an app to subscribe to the ESB and consume the fulfillment request.
3. The security of the web app is taken care of using Identity Providers rather than a custom authentication approach (think asp.net forms authentication where we end up creating our own identity store in the olden days). A third party approach like AAD B2B or B2C means Contoso can get developers to focus on business problems and leave authentication and authorization to the experts.

# Get Started
If you are interested to run this Solution locally as a Developer, you will need to make sure you have the Azurite emulator, and a local instance of SQL Server running. I recommand installing Docker Desktop so you can install these dependencies which is the easist way to get started. Once you have configured Docker Desktop, you can run the following which uses Docker to install Storage and SQL dependencies.

```
.\LocalEnv\Install.ps1 -Password <Password>
```

## Prerequisite
If you have forked this project, this project will attempt to push to a shared container registery. Please follow the [instructions](https://github.com/msft-davidlee/az-internal-shared-services) to create the shared services environment. Note that this step is necessary if you are planning to create the [contoso customer service app service](https://github.com/msft-davidlee/contoso-customer-service-app-service) in your Subscription.

# DevOps Pipeline
There is a azure-pipelines.yml file which you can use if you want to demo a connection from GitHub Repository to Azure Pipeline. As such, you are not using Azure Repo, but GitHub to store your code which gives you an advantage of having the ability to do code scanning using GitHub CodeQL.

## Have an issue?
You are welcome to create an issue if you need help but please note that there is no timeline to answer or resolve any issues you have with the contents of this project. Use the contents of this project at your own risk! If you are interested to volunteer to maintain this, please feel free to reach out to be added as a contributor and send Pull Requests (PR).