# NPU API

## Table of Contents
- [NPU API](#npu-api)
  - [Table of Contents](#table-of-contents)
  - [API](#api)
    - [Prerequisites](#prerequisites)
    - [Run](#run)
  - [Infrastructure](#infrastructure)
    - [Deployment](#deployment)
    - [Services](#services)

## API

The API is built using ASP.NET Core 9.0 and provides endpoints for uploading, scoring, and searching creations.

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run

To run the API locally, use the following command:

```bash
dotnet run --project NpuApi/NpuApi.csproj
```

In your terminal you can see host and port where the API is running.


## Infrastructure

The API is designed to be deployed in an AWS environment.

The infrastructure is defined using [CDK](https://aws.amazon.com/cdk/).

The infrastructure is defined in the `NpuInfrastructure` directory.

### Deployment

AWS credentials are required to deploy the infrastructure.

To deploy the infrastructure, use the following command:

```bash
cdk deploy --all
```

### Services

- [S3](https://aws.amazon.com/s3/) - for storing creation images
- [PostgreSQL RDS](https://aws.amazon.com/rds/postgresql/) - for storing/searching creation, creation scores, and user data
- [API Gateway](https://aws.amazon.com/api-gateway/) - for exposing the API to the internet
- [Lambda](https://aws.amazon.com/lambda/) - for running the API

