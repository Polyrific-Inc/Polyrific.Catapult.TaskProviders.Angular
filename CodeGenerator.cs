using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.Logging;
using Polyrific.Catapult.TaskProviders.Angular.Helpers;
using Polyrific.Catapult.Shared.Dto.Constants;
using Polyrific.Catapult.Shared.Dto.ProjectDataModel;

namespace Polyrific.Catapult.TaskProviders.Angular
{
    public class CodeGenerator
    {
        private readonly ILogger _logger;

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public CodeGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> Generate(string projectName, string projectTitle, string outputLocation, List<ProjectDataModelDto> models)
        {
            try
            {
                // clean project name form space
                projectName = projectName.Replace(" ", "").Kebaberize();
                var projectFolder = Path.Combine(outputLocation, projectName);

                // 1. Generate the project
                await CreateAngularProject(projectName, outputLocation);
                await InitializeProject(projectFolder);

                // 2. Generate each model files
                await CreateHomeComponent(projectFolder, projectTitle, models);
                foreach (var model in models)
                {
                    await CreateModelRelatedFile(projectFolder, model);
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.Message;
            }
        }

        private async Task CreateAngularProject(string projectName, string outputLocation)
        {
            await CommandHelper.ExecuteShellCommand($"ng new {projectName} --routing=true --skipGit=true", outputLocation, _logger);
        }

        private async Task InitializeProject(string projectFolder)
        {
            // install angular material to project
            await CommandHelper.ExecuteShellCommand($"ng add @angular/material", projectFolder, _logger);
        }

        private async Task CreateHomeComponent(string projectFolder, string projectTitle, List<ProjectDataModelDto> models)
        {
            var appFolder = Path.Combine(projectFolder, "src/app");
            
            if (File.Exists(Path.Combine(appFolder, "app.component.ts")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app", "app.component.ts"));
                content = content.Replace("$Title$", projectTitle);
                await File.WriteAllTextAsync(Path.Combine(appFolder, "app.component.ts"), content);
            }
            
            if (File.Exists(Path.Combine(appFolder, "app.component.css")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app", "app.component.css"));
                await File.WriteAllTextAsync(Path.Combine(appFolder, "app.component.css"), content);
            }

            if (File.Exists(Path.Combine(appFolder, "app.component.html")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app", "app.component.html"));

                var sb = new StringBuilder();
                foreach (var model in models)
                {
                    sb.AppendLine($"<a mat-list-item routerLink=\"/{model.Name.Kebaberize()}\">{model.Label}</a>");
                }
                content = content.Replace("$navlist$", sb.ToString());

                await File.WriteAllTextAsync(Path.Combine(appFolder, "app.component.html"), content);
            }
            
            
            if (File.Exists(Path.Combine(appFolder, "app.module.ts")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app", "app.module.ts"));
                await File.WriteAllTextAsync(Path.Combine(appFolder, "app.module.ts"), content);
            }

            if (File.Exists(Path.Combine(appFolder, "app-routing.module.ts")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app", "app-routing.module.ts"));

                var sb = new StringBuilder();
                foreach (var model in models)
                {
                    var modelName = model.Name.Kebaberize();
                    sb.AppendLine($"import {{ {model.Name}Component }} from './{modelName}/{modelName}.component';");
                }
                
                content = content.Replace("$ImportComponents$", sb.ToString());
                
                sb = new StringBuilder();
                foreach (var model in models)
                {
                    var modelName = model.Name.Kebaberize();
                    sb.AppendLine($"{{path: '{modelName}', component: {model.Name}Component }},");
                }
                
                content = content.Replace("$RouteComponents$", sb.ToString());

                await File.WriteAllTextAsync(Path.Combine(appFolder, "app-routing.module.ts"), content);
            }

            
            await CommandHelper.ExecuteShellCommand($"ng generate component home", projectFolder, _logger);
            
            if (File.Exists(Path.Combine(appFolder, "home", "home.component.html")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app/home", "home.component.html"));
                await File.WriteAllTextAsync(Path.Combine(appFolder, "home", "home.component.html"), content);
            }
        }

        private async Task CreateModelRelatedFile(string projectFolder, ProjectDataModelDto model)
        {
            await CreateModelComponent(projectFolder, model);

            await CreateModelDataSource(projectFolder, model);
        }

        private async Task CreateModelComponent(string projectFolder, ProjectDataModelDto model)
        {
            var modelName = model.Name.Kebaberize();
            await CommandHelper.ExecuteShellCommand($"ng generate component {modelName}", projectFolder, _logger);

            string componentFolder = Path.Combine(projectFolder, "src/app", modelName);
            if (File.Exists(Path.Combine(componentFolder, $"{modelName}.component.ts")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app/model", "model.component.ts"));

                content = content.Replace("$ModelName$", modelName);
                content = content.Replace("$ModelClassName$", model.Name);
                
                var sb = new StringBuilder();
                foreach (var property in model.Properties)
                {
                    var propertyName = property.Name.Camelize();
                    sb.Append($"'{propertyName}', ");
                }

                content = content.Replace("$PropertyList$", sb.ToString());

                await File.WriteAllTextAsync(Path.Combine(componentFolder, $"{modelName}.component.ts"), content);
            }
            
            if (File.Exists(Path.Combine(componentFolder, $"{modelName}.component.css")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app/model", "model.component.css"));
                await File.WriteAllTextAsync(Path.Combine(componentFolder, $"{modelName}.component.css"), content);
            }

            if (File.Exists(Path.Combine(componentFolder, $"{modelName}.component.html")))
            {
                var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app/model", "model.component.html"));

                var sb = new StringBuilder();
                foreach (var property in model.Properties)
                {
                    var propertyName = property.Name.Camelize();
                    sb.AppendLine($"<!-- {property.Name} column -->");
                    sb.AppendLine($"<ng-container matColumnDef=\"{propertyName}\">");
                    sb.AppendLine($"<th mat-header-cell *matHeaderCellDef mat-sort-header>{property.Label}</th>");
                    sb.AppendLine($"<td mat-cell *matCellDef=\"let row\">{{{{row.{propertyName}}}}}</td>");
                    sb.AppendLine("</ng-container>");
                }
                content = content.Replace("$ColumnDefinition$", sb.ToString());

                await File.WriteAllTextAsync(Path.Combine(componentFolder, $"{modelName}.component.html"), content);
            }
        }

        private async Task CreateModelDataSource(string projectFolder, ProjectDataModelDto model)
        {
            var modelName = model.Name.Kebaberize();
            var componentFolder = Path.Combine(projectFolder, "src/app", modelName);
            var content = await LoadFile(Path.Combine(AssemblyDirectory, "Template/app/model", "model-datasource.ts"));

            var sb = new StringBuilder();
            foreach (var property in model.Properties)
            {
                var propertyName = property.Name.Camelize();

                string propertyType;
                switch (property.DataType)
                {
                    case PropertyDataType.String:
                        propertyType = "string";
                        break;
                    case PropertyDataType.Integer:
                    case PropertyDataType.Short:
                    case PropertyDataType.Float:
                    case PropertyDataType.Decimal:
                    case PropertyDataType.Double:
                        propertyType = "number";
                        break;
                    case PropertyDataType.Boolean:
                        propertyType = "boolean";
                        break;
                    default:
                        propertyType = "any";
                        break;
                }
                sb.AppendLine($"{propertyName}: {propertyType};");
            }
            
            content = content.Replace("$ModelDefinition$", sb.ToString());
            
            sb = new StringBuilder();
            for (var i = 0; i < 10; i++)
            {
                sb.Append("{");
                foreach (var property in model.Properties)
                {
                    var propertyName = property.Name.Camelize();
                    sb.Append($"{propertyName}: {GetRandomData(property.DataType)}, ");
                }

                sb.Append("},");
                sb.AppendLine();
            }
            
            content = content.Replace("$ModelDummyData$", sb.ToString());

            sb = new StringBuilder();
            sb.AppendLine("switch (this.sort.active) {");
            foreach (var property in model.Properties)
            {
                var propertyName = property.Name.Camelize();
                switch (property.DataType)
                {
                    case PropertyDataType.Boolean:
                    case PropertyDataType.String:
                        sb.AppendLine($"case '{propertyName}': return compare(a.{propertyName}, b.{propertyName}, isAsc);");
                        break;
                    case PropertyDataType.Integer:
                    case PropertyDataType.Short:
                    case PropertyDataType.Float:
                    case PropertyDataType.Decimal:
                    case PropertyDataType.Double:
                        sb.AppendLine($"case '{propertyName}': return compare(+a.{propertyName}, +b.{propertyName}, isAsc);");
                        break;
                    default:
                        break;
                }
            }
            sb.AppendLine("default: return 0;");
            sb.AppendLine("}");
            content = content.Replace("$ModelSort$", sb.ToString());
            
            content = content.Replace("$ModelName$", model.Name);

            await File.WriteAllTextAsync(Path.Combine(componentFolder, $"{modelName}-datasource.ts"), content);
        }

        private async Task<string> LoadFile(string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath);

            content = content.Replace("// @ts-ignore", "");

            return content;
        }

        private string GetRandomData(string propertyType)
        {            
            var rand = new Random();
            switch (propertyType)
            {
                case PropertyDataType.Integer:
                case PropertyDataType.Short:
                case PropertyDataType.Float:
                case PropertyDataType.Decimal:
                case PropertyDataType.Double:
                    return rand.Next(10).ToString();
                case PropertyDataType.Boolean:
                    return (rand.NextDouble() >= 0.5) ? "true" : "false";
                default:
                    return $"\"dummy {rand.Next(10)}\"";
            }
        }
    }
}