using AppleIcnsImageExtractor;
using AppleIcnsImageExtractor.Abstract;
using Autofac;

try
{
    using var container = ContainerRegistration.RegisterDependencies();
    var app = container.Resolve<IApp>();
    return app.Execute(args);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    return -1;
}