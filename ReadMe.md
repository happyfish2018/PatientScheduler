# Patient Scheduler Simulator
The simulator provides a web service to allow medical practices to register patients and schedule a consultation with a doctor based on patient's medical condition.

## Introduction
The project was developed using Visual Studio 2013 and .NET framework 4.5.2. Services are self-hosted within the process (managed by Microsoft ASP.NET self hosting libriary).

## Getting started
Download the entire project from GitHub. Open PatientScheduler.sln with Visual Studio 2013, and build the solution.
Once the solution is built successfully, PatientScheduler.Service.exe and PatientScheduler.Client.exe will be generated respectively.

In order to start the services, run PatientScheduler.Service.exe as an administrator. That will open a command console which indicates where the services are hosted. For demonstration purpose,
the services are hosted on port 8002 (http://localhost:8002). Press Enter key to terminate the services.
Static data is seeded by provided Json file (DataResource.json) and available only in the during of the service session. Database persistence is not available.

## Available APIs

### GetRegisteredPatients
* **URL:** /patient
* **Method:** GET
* **Successful GET Response status:** OK or NoContent(if no registered patients found)


### RegisterPatient
* **URL:** /patient
* **Method:** POST
* **Successful POST Response status:** Created
* **Failed POST Response status:** BadRequest

### GetConsultations
* **URL:** /consultations
* **Method:** GET
* **Successful GET Response status:** OK or NoContent(if no consultations found)

## Implementations
There are two controllers created: PatientController and ConsultationsController.
PatientController is used for registering a patient, and retrieving registered patients.
ConsultatonController is used for retrieving all scheduled consultations.

## Testing
The APIs have been tested using PatientScheduler.Client.exe.
Unit tests were also developed using mock data on controllers and service business logic.

## Prerequisites
No special prerequisites that must be fulfilled in order to build the solution.
# PatientScheduler
