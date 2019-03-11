using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Humanizer;
using Polyrific.Catapult.TaskProviders.Core;

namespace Polyrific.Catapult.Plugins.Angular
{
  class Program : CodeGeneratorProvider
  {
    private readonly CodeGenerator _codeGenerator;

    public Program(string[] args) : base(args)
    {
      _codeGenerator = new CodeGenerator(Logger);
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

      var error = await _codeGenerator.Generate(ProjectName, projectTitle, Config.OutputLocation, Models);

      if (!string.IsNullOrEmpty(error))
        return ("", null, error);

      return (Config.OutputLocation, null, "");        
    }
  }
}