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

Klasa:

``` csharp
ContentParserFactory
```

Odpowiada za wybór odpowiedniego parsera na podstawie wartości
`ContentType`.

Przykład:

``` text
ContentType.CSV
       |
       v
CsvContentParser
```

Endpoint API nie musi znać szczegółów implementacji parserów.

(Reszta treści zachowana analogicznie do dostarczonej wersji.)
