// See https://aka.ms/new-console-template for more information

using Grace.DependencyInjection;
using MindOnSite.CodeKata.Implementations.Services;

var container = new DependencyInjectionContainer();

container.Configure(c => c.Export<LrsService>());

var service = container.Locate<LrsService>();

Console.WriteLine($"result={service.GetStatementResultInCache}");
Console.WriteLine($"all done.");
