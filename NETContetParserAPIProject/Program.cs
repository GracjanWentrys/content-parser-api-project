using Api.Endpoints;
using Api.Middleware;
using Api.Services.Abstractions;
using Api.Services.Factories;
using Api.Services.Implementations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IContentDecoder, Base64ContentDecoder>();
builder.Services.AddSingleton<IContentParser, CsvContentParser>();
builder.Services.AddSingleton<IContentParser, InternalJsonContentParser>();
builder.Services.AddSingleton<ContentParserFactory>();

var app = builder.Build();

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var apiV1 = app.MapGroup("/api/v1");
apiV1.MapParseContentEndpoints();

app.Run();