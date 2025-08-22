var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ApiClient", client =>
{
    
    string baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

    // Asigna la URL base al cliente HTTP
    client.BaseAddress = new Uri(baseUrl);

    // (Opcional) Puedes añadir cabeceras por defecto si las necesitas
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});



// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
