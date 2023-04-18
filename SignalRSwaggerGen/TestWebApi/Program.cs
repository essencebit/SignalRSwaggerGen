var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    var apiInfo = new OpenApiInfo { Title = "TestWebApi", Version = "v1" };
    options.SwaggerDoc("controllers", apiInfo);
    options.SwaggerDoc("hubs", apiInfo);
    options.IncludeXmlComments("TestWebApi.xml", true);
    options.AddSignalRSwaggerGen();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/controllers/swagger.json", "REST API");
        options.SwaggerEndpoint("/swagger/hubs/swagger.json", "SignalR");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();