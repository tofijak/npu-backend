using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.EC2;
using Constructs;
using Amazon.CDK.AWS.Lambda;
using System.Collections.Generic;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Events.Targets;

namespace NpuInfrastructure
{
    public class NpuInfrastructureStack : Stack
    {
        internal NpuInfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Vpc vpc = new(this, "NpuVpc");

            Bucket creationsBucket = CreateBucket();

            SecurityGroup dbSecurityGroup = CreateSecurityGroup(vpc);

            Credentials dbCredentials = CreateDatabaseInstance(vpc, dbSecurityGroup);

            Function lambdaFunction = CreateLambdaFunction(vpc, creationsBucket, dbSecurityGroup, dbCredentials);

            CreateAPIGateway(vpc, lambdaFunction);

        }

        private Bucket CreateBucket()
        {
            var creationsBucket = new Bucket(this, "CreationsBucket", new BucketProps
            {
                BucketName = "npu--creations",
                RemovalPolicy = RemovalPolicy.RETAIN,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                Versioned = true, 
            });
            return creationsBucket;
        }


        private SecurityGroup CreateSecurityGroup(Vpc vpc)
        {
            return new SecurityGroup(this, "DbSecurityGroup", new SecurityGroupProps
            {
                Vpc = vpc,
                Description = "Security group for NPU RDS instance",
                AllowAllOutbound = false
            });
        }

        private Credentials CreateDatabaseInstance(IVpc vpc, ISecurityGroup dbSecurityGroup)
        {
            dbSecurityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(5432), "Allow PostgreSQL connections");
            
            var credentials = Credentials.FromGeneratedSecret("postgres");

            new DatabaseInstance(this, "NpuDatabase", new DatabaseInstanceProps
            {
                Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
                {
                    Version = PostgresEngineVersion.VER_15
                }),
                InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.SMALL),
                Vpc = vpc,
                SecurityGroups = [dbSecurityGroup],
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
                DeletionProtection = true, 
                DatabaseName = "npudb",
                Credentials = credentials, 
                BackupRetention = Duration.Days(7),
                RemovalPolicy = RemovalPolicy.SNAPSHOT 
            });

            return credentials;
        }


        private Function CreateLambdaFunction(Vpc vpc, Bucket creationsBucket, SecurityGroup dbSecurityGroup, Credentials dbCredentials)
        {
            var npuLambda = new Function(this, "NpuFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_9,
                // TODO: Update hardcoded path when using pipeline
                Code = Code.FromAsset("src/NpuApi/bin/Debug/net9.0"),
                Handler = "NpuApi::Program::LambdaHandler",
                Vpc = vpc,
                SecurityGroups = [dbSecurityGroup],
                Environment = new Dictionary<string, string>
                {
                    { "BUCKET_NAME", creationsBucket.BucketName },
                    { "DB_SECRET_NAME", dbCredentials.SecretName },
                }
            });

            creationsBucket.GrantReadWrite(npuLambda);
            dbCredentials.Secret.GrantRead(npuLambda);

            return npuLambda;
        }


        private void CreateAPIGateway(Vpc vpc, Function npuLambda)
        {
            var api = new LambdaRestApi(this, "NpuApi", new LambdaRestApiProps
            {
                Handler = npuLambda,
                Description = "NPU API Gateway",
            });

            AddAuthHandlerToAPIGateway(api);

            LambdaIntegration integration = new(npuLambda);

            var creationsResource = api.Root.AddResource("creations");
            creationsResource.AddMethod("POST", integration); // POST method for uploading creations
            creationsResource.AddMethod("GET", integration); // GET for listing creations by searching

            var creationByIdResource = creationsResource.AddResource("{id}");
            var scoreResource = creationByIdResource.AddResource("score");
            scoreResource.AddMethod("POST", integration); // POST method for scoring a creation
        }

        private static void AddAuthHandlerToAPIGateway(LambdaRestApi api)
        {
            // TODO: Replace with actual authentication handler using IDP
            api.AddApiKey("ApiKey", new ApiKeyProps
            {
                ApiKeyName = "NpuApiKey",
            });
            api.AddUsagePlan("UsagePlan", new UsagePlanProps
            {
                Name = "NpuUsagePlan",
                Description = "Usage plan for NPU API",
                Quota = new QuotaSettings
                {
                    Limit = 1000,
                    Period = Period.MONTH,
                },
                Throttle = new ThrottleSettings
                {
                    BurstLimit = 5,
                    RateLimit = 10,
                }
            });
        }
    }
}
