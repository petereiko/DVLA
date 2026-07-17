// See https://aka.ms/new-console-template for more information
using DVLA.Business.EmailModule;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");


var services = new ServiceCollection();
services.AddTransient<IEmailService, EmailService>();
services.AddTransient<, EmailService>();

using var provider = services.BuildServiceProvider();

var runner = provider.GetRequiredService<IEmailService>();
bool isSent = runner.SendEmail("peterayebhere@gmail.com", "Test Subject", "Test Message");