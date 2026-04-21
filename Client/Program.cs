using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client; // Ensure this matches the namespace at the top of your App.razor

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// IMPORTANT: Replace 7148 with the port your Server project is actually running on!
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5240/") });

await builder.Build().RunAsync();