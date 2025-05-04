using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpuInfrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new NpuInfrastructureStack(app, "NpuInfrastructureStack");
            app.Synth();
        }
    }
}
