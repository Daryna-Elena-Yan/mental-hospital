using Mental_Hospital;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


//Initiallizing DI container
var services = new ServiceCollection();
services.MentalHospitalSetup();
var provider = services.BuildServiceProvider();


var factory = provider.GetService<PersonFactory>();
var storage = provider.GetService<PersonStorage>();

factory.CreateNewPatient("aa", "aa", DateTime.Now, "asssa", "asads");
Console.WriteLine(storage.Count);
