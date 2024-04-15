using Blazored.LocalStorage;
using Blazored.SessionStorage;
using LDJam55.Game;
using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddSingleton<SettingsRepository>();
builder.Services.AddSingleton<SessionRepository>();
builder.Services.AddSingleton<WorldRepository>();

await builder.Build().RunAsync();
