using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Humanizer;
using Polyrific.Catapult.Plugins.Core;

namespace Polyrific.Catapult.Plugins.Angular
{
  class Program : CodeGeneratorProvider
  {
    public Program(string[] args) : base(args)
    {
    }

    public override string Name => "Polyrific.Catapult.Plugins.Angular";

    static async Task Main(string[] args)
    {
      var app = new Program(args);
      
      var result = await app.Execute();
      app.ReturnOutput(result);
    }

    public override async Task<(string outputLocation, Dictionary<string, string> outputValues, string errorMessage)> Generate()
    {
      string projectTitle = ProjectName.Humanize(); // set the default title to project name
      if (AdditionalConfigs != null && AdditionalConfigs.ContainsKey("Title") && !string.IsNullOrEmpty(AdditionalConfigs["Title"]))
          projectTitle = AdditionalConfigs["Title"];
      
      Config.OutputLocation = Config.OutputLocation ?? Config.WorkingLocation;

      await GenerateCode(ProjectName, Config.OutputLocation);

      return (Config.OutputLocation, null, "");        
    }

    private Task GenerateCode(string projectName, string outputLocation)
    {
      // should you run t
      var info = new ProcessStartInfo("powershell")
      {
          UseShellExecute = false,
          Arguments = $"ng new {projectName.Kebaberize()} --skipGit=true --skipInstall=true",
          RedirectStandardInput = true,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          CreateNoWindow = true,
          WorkingDirectory = outputLocation
      };

      using (var process = Process.Start(info))
      {
        process.WaitForExit();
      }

      return Task.CompletedTask;
    }
  }
}