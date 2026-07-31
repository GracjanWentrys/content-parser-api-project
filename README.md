# Content Parser API

Web API napisane w technologii **.NET 10 / C#**, którego zadaniem jest
dekodowanie oraz generyczne parsowanie danych przesyłanych przez API.

Aplikacja przyjmuje dane zakodowane w formacie **Base64**, następnie je
dekoduje i przekazuje do odpowiedniego parsera na podstawie podanego
typu zawartości.

Aktualnie obsługiwane formaty:

-   `CSV`
-   `INTERNAL_JSON`

Projekt został przygotowany z myślą o łatwej rozbudowie o kolejne
formaty danych bez konieczności zmiany istniejącej logiki endpointu.

------------------------------------------------------------------------

# 🚀 Technologie

-   .NET 10
-   ASP.NET Core Minimal API
-   C#
-   OpenAPI
-   Scalar API Reference
-   Dependency Injection
-   Strategy Pattern
-   Factory Pattern

------------------------------------------------------------------------

# 🏗️ Struktura projektu

``` text
Api
│
├── Models
│   └── Models.cs
│
├── Services
│   └── Services.cs
│
└── Program.cs
```

------------------------------------------------------------------------

# 🔄 Przepływ działania aplikacji

``` text
Request HTTP
     |
     v
Walidacja i dekodowanie Base64
     |
     v
ContentParserFactory
     |
     v
Odpowiedni parser danych
     |
     v
ParseResult
     |
     v
Response HTTP
```

------------------------------------------------------------------------

## 🧩 Architektura

### Content Decoder

Interfejs:

``` csharp
IContentDecoder
```

Odpowiada za zamianę danych Base64 na zwykły tekst.

Aktualna implementacja:

``` csharp
Base64ContentDecoder
```

Oddzielenie dekodowania od parsowania pozwala w przyszłości łatwo
zmienić sposób dostarczania danych.

------------------------------------------------------------------------

### Content Parser

Interfejs:

``` csharp
IContentParser
```

Definiuje wspólny kontrakt dla wszystkich parserów.

Każdy parser odpowiada za jeden konkretny typ danych.

Aktualne implementacje:

``` csharp
CsvContentParser
InternalJsonContentParser
```

Dzięki temu dodanie nowego formatu wymaga jedynie stworzenia nowej klasy
implementującej `IContentParser`.

------------------------------------------------------------------------

### Content Parser Factory

``` csharp
ContentParserFactory
```

Odpowiada za wybór parsera na podstawie `ContentType`.

``` text
ContentType.CSV
      │
      ▼
CsvContentParser
```

------------------------------------------------------------------------

# 🎯 Zastosowane wzorce projektowe

## Strategy Pattern

``` text
             IContentParser
                   │
      ┌────────────┴────────────┐
      │                         │
CsvContentParser   InternalJsonContentParser
```

Każdy format danych posiada własną strategię parsowania.

## Factory Pattern

Factory ukrywa logikę wyboru parsera.

------------------------------------------------------------------------

# 🚀 Wymagania

-   .NET 10 SDK

``` bash
dotnet --version
```

------------------------------------------------------------------------

# 🛠️ Uruchomienie lokalne

## 1. Sklonowanie repozytorium

``` bash
git clone https://github.com/GracjanWentrys/content-parser-api.git
cd content-parser-api
```

## 2. Przywrócenie zależności

``` bash
dotnet restore
```

## 3. Uruchomienie

``` bash
dotnet run --project Api
```

lub

``` bash
dotnet run
```

------------------------------------------------------------------------

# 📚 Dokumentacja API

``` text
https://localhost:<port>/openapi/v1.json
https://localhost:<port>/scalar/v1
```

------------------------------------------------------------------------

# 📌 Endpoint API

## POST `/api/v1/parse-content`

Dekoduje zawartość Base64 i parsuje dane zgodnie z podanym typem.

------------------------------------------------------------------------

# 📥 Przykładowy request

``` http
Content-Type: application/json
```

## CSV

``` json
{
  "type": "CSV",
  "content": "TmFtZSxBZ2UKSm9obiwzMA=="
}
```

``` csv
Name,Age
John,30
```

## INTERNAL_JSON

``` json
{
  "type": "INTERNAL_JSON",
  "content": "W3sibmFtZSI6IkpvaG4iLCJhZ2UiOjMwfV0="
}
```

``` json
[
  {
    "name":"John",
    "age":30
  }
]
```

# ✅ Przykładowa odpowiedź

``` json
{
  "isSuccess": true,
  "recordCount": 1,
  "data": [
    {
      "Name": "John",
      "Age": "30"
    }
  ],
  "errorMessage": null
}
```

# ❌ Obsługa błędów

## Niepoprawny Base64

``` json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Invalid Base64 string in content field."
}
```

## Niepoprawny JSON requestu

``` json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Invalid JSON format."
}
```

## Nieobsługiwany typ danych

``` json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Content type is not supported."
}
```

## Błąd parsowania danych

``` json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Invalid JSON structure."
}
```

# ➕ Dodanie nowego parsera

``` csharp
public class XmlContentParser : IContentParser
{
    public ContentType SupportedType => ContentType.XML;

    public IParseResult Parse(string rawContent)
    {
        // Logika parsowania XML
    }
}
```

``` csharp
public enum ContentType
{
    CSV,
    INTERNAL_JSON,
    XML
}
```

``` csharp
builder.Services.AddSingleton<IContentParser, XmlContentParser>();
```

# 📝 Podjęte decyzje projektowe

-   Parsery zostały oddzielone od endpointu dzięki zastosowaniu
    interfejsu `IContentParser`.
-   Factory odpowiada za wybór parsera.
-   Dependency Injection zarządza zależnościami aplikacji.
-   Globalny middleware obsługuje wyjątki i zapewnia spójny format
    błędów.
-   CSV jest parsowany przy użyciu `TextFieldParser`, aby poprawnie
    obsługiwać:
    -   wartości w cudzysłowach,
    -   przecinki w polach,
    -   wartości wieloliniowe.
-   JSON jest obsługiwany dynamicznie poprzez `JsonElement`, ponieważ
    struktura danych nie jest wcześniej znana.

------------------------------------------------------------------------

# 📄 Licencja

Projekt przygotowany jako implementacja generycznego parsera danych
przesyłanych przez API w technologii .NET 10.
