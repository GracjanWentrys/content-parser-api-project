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

---

# 🎯 Zastosowane wzorce projektowe

## Strategy Pattern

Każdy format danych posiada własną strategię parsowania.

Przykład:

```text
             IContentParser
                   │
        ┌──────────┴──────────┐
        │                     │
CsvContentParser   InternalJsonContentParser
```

Pozwala to dodawać kolejne formaty bez modyfikowania istniejącego kodu.

---

## Factory Pattern

Factory ukrywa logikę wyboru odpowiedniego parsera.

Dzięki temu endpoint pozostaje prosty i odpowiada jedynie za przepływ danych.

---

# 🚀 Wymagania

Do uruchomienia projektu wymagane jest:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

Sprawdzenie wersji:

```bash
dotnet --version
```

---

# 🛠️ Uruchomienie lokalne

## 1. Sklonowanie repozytorium

```bash
git clone https://github.com/GracjanWentrys/content-parser-api.git

cd content-parser-api
```

---

## 2. Przywrócenie zależności

```bash
dotnet restore
```

---

## 3. Uruchomienie aplikacji

Z głównego katalogu rozwiązania:

```bash
dotnet run --project Api
```

lub będąc bezpośrednio w katalogu projektu:

```bash
dotnet run
```

---

# 📚 Dokumentacja API

Projekt wykorzystuje wbudowaną obsługę OpenAPI w ASP.NET Core oraz Scalar API Reference.

Po uruchomieniu aplikacji:

Dokument OpenAPI:

```text
https://localhost:<port>/openapi/v1.json
```

Interaktywna dokumentacja Scalar:

```text
https://localhost:<port>/scalar/v1
```

---

# 📌 Endpoint API

## POST `/api/v1/parse-content`

Dekoduje zawartość Base64 i parsuje dane zgodnie z podanym typem.

---

# 📥 Przykładowy request

Nagłówki:

```http
Content-Type: application/json
```

## CSV

Request:

```json
{
  "type": "CSV",
  "content": "TmFtZSxBZ2UKSm9obiwzMA=="
}
```

Po dekodowaniu:

```csv
Name,Age
John,30
```

---

## INTERNAL_JSON

Request:

```json
{
  "type": "INTERNAL_JSON",
  "content": "W3sibmFtZSI6IkpvaG4iLCJhZ2UiOjMwfV0="
}
```

Po dekodowaniu:

```json
[
  {
    "name": "John",
    "age": 30
  }
]
```

---

# ✅ Przykładowa odpowiedź poprawna

Status:

```text
200 OK
```

Response:

```json
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

---

# ❌ Obsługa błędów

API posiada globalną obsługę wyjątków poprzez middleware.

---

## Niepoprawny Base64

Status:

```text
400 Bad Request
```

Przykład:

```json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Invalid Base64 string in content field."
}
```

---

## Niepoprawny JSON requestu

Status:

```text
400 Bad Request
```

Przykład:

```json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Invalid JSON format."
}
```

---

## Nieobsługiwany typ danych

Status:

```text
400 Bad Request
```

Przykład:

```json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Content type is not supported."
}
```

---

## Błąd parsowania danych

Status:

```text
422 Unprocessable Entity
```

Przykład:

```json
{
  "isSuccess": false,
  "recordCount": 0,
  "data": null,
  "errorMessage": "Invalid JSON structure."
}
```

---

# ➕ Dodanie nowego parsera

Aby dodać nowy format danych, np. XML:

## 1. Utworzyć nową implementację

```csharp
public class XmlContentParser : IContentParser
{
    public ContentType SupportedType => ContentType.XML;

    public IParseResult Parse(string rawContent)
    {
        // Logika parsowania XML
    }
}
```

---

## 2. Dodać nową wartość enum

```csharp
public enum ContentType
{
    CSV,
    INTERNAL_JSON,
    XML
}
```

---

## 3. Zarejestrować parser

```csharp
builder.Services.AddSingleton<IContentParser, XmlContentParser>();
```

Istniejący endpoint nie wymaga żadnych zmian.

---

# 📝 Podjęte decyzje projektowe

- Parsery zostały oddzielone od endpointu dzięki zastosowaniu interfejsu `IContentParser`.
- Factory odpowiada za wybór odpowiedniego parsera.
- Dependency Injection zarządza zależnościami aplikacji.
- Globalny middleware obsługuje wyjątki i zapewnia spójny format błędów.
- CSV jest parsowany przy użyciu `TextFieldParser`, aby poprawnie obsługiwać:
  - wartości w cudzysłowach,
  - przecinki w polach,
  - wartości wieloliniowe.
- JSON jest obsługiwany dynamicznie poprzez `JsonElement`, ponieważ struktura danych nie jest wcześniej znana.

---

# 📄 Licencja

Projekt przygotowany jako implementacja generycznego parsera danych przesyłanych przez API w technologii .NET 10.