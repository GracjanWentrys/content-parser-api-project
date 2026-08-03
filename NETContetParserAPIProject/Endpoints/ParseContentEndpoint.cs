using Api.Models.Dtos;
using Api.Services.Abstractions;
using Api.Services.Factories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class ParseContentEndpoint
{
    public static IEndpointRouteBuilder MapParseContentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/parse-content", HandleParseContent)
            .Accepts<ParseRequest>("application/json")
            .Produces<ParseResponse<object>>(StatusCodes.Status200OK)
            .Produces<ParseResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ParseResponse<object>>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ParseResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithName("ParseContent")
            .WithSummary("Parse Base64 encoded content")
            .WithDescription("Decodes Base64 content and parses CSV or INTERNAL_JSON data.");

        return endpoints;
    }

    private static IResult HandleParseContent(
        [FromBody] ParseRequest request,
        [FromServices] IContentDecoder decoder,
        [FromServices] ContentParserFactory parserFactory)
    {
        var rawContent = decoder.DecodeBase64(request.Content);
        var parser = parserFactory.GetParser(request.Type);
        var result = parser.Parse(rawContent);

        if (!result.IsSuccess)
        {
            return Results.UnprocessableEntity(
                ParseResponse<object>.Failure(result.ErrorMessage!));
        }

        return Results.Ok(
            ParseResponse<object>.Success(
                result.RawData!,
                result.Count));
    }
}