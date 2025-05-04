# NpuApiBackend - ASP.NET Core 8.0 Server

API for the NPU platform to upload, score, and search creations.

## Table of Contents
- [NpuApiBackend - ASP.NET Core 8.0 Server](#npuapibackend---aspnet-core-80-server)
  - [Table of Contents](#table-of-contents)
  - [API](#api)
    - [API First](#api-first)
    - [Prerequisites](#prerequisites)
    - [Run](#run)
  - [Infrastructure](#infrastructure)
    - [Services](#services)
    - [API Gateway](#api-gateway)

## API

The API is built using ASP.NET Core 8.0 and provides endpoints for uploading, scoring, and searching creations.
The API is designed to be simple and easy to use, with a focus on performance and scalability.

### API First

The API is designed using the OpenAPI specification, which allows for easy generation of client libraries and documentation. 

If you want to change the API, you can do so by modifying the OpenAPI specification file located in the `openapi` directory.

You can use the [OpenAPI Generator](https://openapi-generator.tech/) to regenerate the server library, and extend it with implementations for the endpoints.

Run the following command to generate the server library:

```bash
openapi-generator generate -i openapi/npu-api-spec.yml -g aspnetcore -o npu-api-dotnet --additional-properties=aspnetCoreVersion=8.0,packageName=NpuApi,buildTarget=library
```

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [OpenAPI Generator](https://openapi-generator.tech/docs/installation) (optional, for generating server library)

### Run

To run the API locally, use the following command:

```bash
dotnet run --project NpuApi/src/NpuApi.csproj
```

In your terminal you can see host and port where the API is running.


## Infrastructure

The API is designed to be deployed in an AWS environment.

The infrastructure is defined using [CDK](https://aws.amazon.com/cdk/).

The infrastructure is defined in the `NpuInfrastructure` directory.

### Services

- [S3](https://aws.amazon.com/s3/) - for storing creation images
- [PostgreSQL](https://aws.amazon.com/rds/postgresql/) - for storing/searching creation, creation scores, and user data
- [API Gateway](https://aws.amazon.com/api-gateway/) - for exposing the API to the internet
- [Lambda](https://aws.amazon.com/lambda/) - for running the API


### API Gateway

The API Gateway is used to expose the API to the internet. It utilizes the OpenAPI specification file in `openapi` folder to define the API endpoints and their parameters.