using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();

services.AddSingleton<App>();
services.AddSingleton<Input>();
services.AddSingleton<Input_V>();
services.AddSingleton<CurrentDirectory>();
services.AddSingleton<CurrentDirectory_V>();
services.AddSingleton<SearchTerms>();
services.AddSingleton<SearchTerms_V>();
services.AddSingleton<Children>();
services.AddSingleton<Children_V>();
services.AddSingleton<History>();

using ServiceProvider provider = services.BuildServiceProvider();

App app = provider.GetRequiredService<App>();
app.Run();
