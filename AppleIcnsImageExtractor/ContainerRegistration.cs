using AppleIcnsImageExtractor.Abstract;
using AppleIcnsImageExtractor.Concrete;
using Autofac;

namespace AppleIcnsImageExtractor;

public static class ContainerRegistration
{
    public static IContainer RegisterDependencies()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<App>().As<IApp>().SingleInstance();
        builder.RegisterType<AppleIconsFileParser>().As<IAppleIconsFileParser>().SingleInstance();
        builder.RegisterType<BinaryStreamReader>().As<IBinaryStreamReader>().InstancePerDependency();
        builder.RegisterType<FileOperations>().As<IFileOperations>().SingleInstance();
        return builder.Build();
    }
}