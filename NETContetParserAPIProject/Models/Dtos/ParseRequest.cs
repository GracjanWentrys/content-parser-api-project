using Api.Enums;

namespace Api.Models.Dtos;

public record ParseRequest(
    ContentType Type,
    string Content
);