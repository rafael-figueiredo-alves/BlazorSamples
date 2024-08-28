using BlazorClientes;
using BlazorClientes.Auth;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.Toast;
using BlazorClientes.Services.Interfaces;
using System.Buffers.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes("cea7391443541fc0dabe35e2625c2530");
    string teste = Convert.ToBase64String(textBytes);
    var http = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7235/")
    };
    http.DefaultRequestHeaders.Add("API-Key", "cea7391443541fc0dabe35e2625c2530");
    return http;
 }); //builder.HostEnvironment.BaseAddress


builder.Services.AddScoped<IAuthServices, AuthServices>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<TokenAuthenticationProvider>();

builder.Services.AddScoped<IAuthToken, TokenAuthenticationProvider>(
    provider => provider.GetRequiredService<TokenAuthenticationProvider>()
    );

builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationProvider>(
  provider => provider.GetRequiredService<TokenAuthenticationProvider>());

builder.Services.AddScoped<ILocalStorage, LocalStorage>();

builder.Services.AddBlazoredToast();

builder.Services.AddScoped<IParamService, ParamService>();
builder.Services.AddSingleton<IUserData,  UserData>();

//Serviços relacionados a API
builder.Services.AddScoped<IClientes, ClientesService>();
builder.Services.AddScoped<IVendedores, VendedoresService>();
builder.Services.AddScoped<IProdutos, ProdutosService>();
builder.Services.AddScoped<IPedidos, PedidosService>();

await builder.Build().RunAsync();
