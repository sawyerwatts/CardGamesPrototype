namespace CardGamesPrototype.Cli;

public static class ServiceRegistration
{
    public static void RegisterCliServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<Driver>();
        builder.Services.AddSingleton<CliPlayerInterfaceFactory>();
    }
}
