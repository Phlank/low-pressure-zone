using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints((config) =>
{
    config.Errors.UseProblemDetails();
});

app.Run();
