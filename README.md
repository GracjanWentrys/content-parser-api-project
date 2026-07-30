\# Content Parser API



Web API napisane w technologii \*\*.NET 10 / C#\*\*, którego zadaniem jest dekodowanie oraz generyczne parsowanie danych przesyłanych przez API.



Aplikacja przyjmuje dane zakodowane w formacie \*\*Base64\*\*, następnie je dekoduje i przekazuje do odpowiedniego parsera na podstawie podanego typu zawartości.



Aktualnie obsługiwane formaty:



\- `CSV`

\- `INTERNAL\_JSON`



Projekt został przygotowany z myślą o łatwej rozbudowie o kolejne formaty danych bez konieczności zmiany istniejącej logiki endpointu.



\---



\# 🚀 Technologie



\- .NET 10

\- ASP.NET Core Minimal API

\- C#

\- OpenAPI

\- Scalar API Reference

\- Dependency Injection

\- Strategy Pattern

\- Factory Pattern



\---



\# 🏗️ Struktura projektu



```

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



\---



\# 🔄 Przepływ działania aplikacji



```

Request HTTP

&#x20;     |

&#x20;     v

Walidacja i dekodowanie Base64

&#x20;     |

&#x20;     v

ContentParserFactory

&#x20;     |

&#x20;     v

Odpowiedni parser danych

&#x20;     |

&#x20;     v

ParseResult

&#x20;     |

&#x20;     v

Response HTTP

```



\---



\# 🧩 Architektura



\## Content Decoder



Interfejs:



```

IContentDecoder

```



Odpowiada za zamianę danych Base64 na zwykły tekst.



Aktualna implementacja:



```

Base64ContentDecoder

```



Oddzielenie dekodowania od parsowania pozwala w przyszłości łatwo zmienić sposób dostarczania danych.



\---



\## Content Parser



Interfejs:



```

IContentParser

```



Definiuje wspólny kontrakt dla wszystkich parserów.



Każdy parser odpowiada za jeden konkretny typ danych.



Aktualne implementacje:



```

CsvContentParser

InternalJsonContentParser

```



Dzięki temu dodanie nowego formatu wymaga jedynie stworzenia nowej klasy implementującej `IContentParser`.



\---



\## Content Parser Factory



Klasa:



```

ContentParserFactory

```



Odpowiada za wybór odpowiedniego parsera na podstawie wartości `ContentType`.



Przykład:



```

ContentType.CSV



&#x20;       |

&#x20;       v



CsvContentParser

```



Endpoint API nie musi znać szczegółów implementacji parserów.



\---



\# 🎯 Zastosowane wzorce projektowe



\## Strategy Pattern



Każdy format danych posiada własną strategię parsowania.



Przykład:



```

&#x20;             IContentParser



&#x20;                   |

&#x20;       -------------------------

&#x20;       |                       |

&#x20;CsvContentParser    InternalJsonContentParser

```



Pozwala to dodawać kolejne formaty bez modyfikowania istniejącego kodu.



\---



\## Factory Pattern



Factory ukrywa logikę wyboru odpowiedniego parsera.



Dzięki temu endpoint pozostaje prosty i odpowiada jedynie za przepływ danych.



\---



\# 🚀 Wymagania



Do uruchomienia projektu wymagane jest:



\- \[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)



Sprawdzenie wersji:



```bash

dotnet --version

```



\---



\# 🛠️ Uruchomienie lokalne



\## 1. Sklonowanie repozytorium



```bash

git clone https://github.com/GracjanWentrys/content-parser-api.git



cd content-parser-api

```



\---



\## 2. Przywrócenie zależności



```bash

dotnet restore

```



\---



\## 3. Uruchomienie aplikacji



Z głównego katalogu rozwiązania:



```bash

dotnet run --project Api

```



lub będąc bezpośrednio w katalogu projektu:



```bash

dotnet run

```



\---



\# 📚 Dokumentacja API



Projekt wykorzystuje wbudowaną obsługę OpenAPI w ASP.NET Core oraz Scalar API Reference.



Po uruchomieniu aplikacji:



Dokument OpenAPI:



```

https://localhost:<port>/openapi/v1.json

```



Interaktywna dokumentacja Scalar:



```

https://localhost:<port>/scalar/v1

```



\---



\# 📌 Endpoint API



\## POST `/api/v1/parse-content`



Dekoduje zawartość Base64 i parsuje dane zgodnie z podanym typem.



\---



\# 📥 Przykładowy request



Nagłówki:



```http

Content-Type: application/json

```



\---



\## CSV



Request:



```json

{

&#x20; "type": "CSV",

&#x20; "content": "TmFtZSxBZ2UKSm9obiwzMA=="

}

```



Po dekodowaniu:



```csv

Name,Age

John,30

```



\---



\## INTERNAL\_JSON



Request:



```json

{

&#x20; "type": "INTERNAL\_JSON",

&#x20; "content": "W3sibmFtZSI6IkpvaG4iLCJhZ2UiOjMwfV0="

}

```



Po dekodowaniu:



```json

\[

&#x20; {

&#x20;   "name": "John",

&#x20;   "age": 30

&#x20; }

]

```



\---



\# ✅ Przykładowa odpowiedź poprawna



Status:



```

200 OK

```



Response:



```json

{

&#x20; "isSuccess": true,

&#x20; "recordCount": 1,

&#x20; "data": \[

&#x20;   {

&#x20;     "Name": "John",

&#x20;     "Age": "30"

&#x20;   }

&#x20; ],

&#x20; "errorMessage": null

}

```



\---



\# ❌ Obsługa błędów



API posiada globalną obsługę wyjątków poprzez middleware.



\---



\## Niepoprawny Base64



Status:



```

400 Bad Request

```



Przykład:



```json

{

&#x20; "isSuccess": false,

&#x20; "recordCount": 0,

&#x20; "data": null,

&#x20; "errorMessage": "Invalid Base64 string in content field."

}

```



\---



\## Niepoprawny JSON requestu



Status:



```

400 Bad Request

```



Przykład:



```json

{

&#x20; "isSuccess": false,

&#x20; "recordCount": 0,

&#x20; "data": null,

&#x20; "errorMessage": "Invalid JSON format."

}

```



\---



\## Nieobsługiwany typ danych



Status:



```

400 Bad Request

```



Przykład:



```json

{

&#x20; "isSuccess": false,

&#x20; "recordCount": 0,

&#x20; "data": null,

&#x20; "errorMessage": "Content type is not supported."

}

```



\---



\## Błąd parsowania danych



Status:



```

422 Unprocessable Entity

```



Przykład:



```json

{

&#x20; "isSuccess": false,

&#x20; "recordCount": 0,

&#x20; "data": null,

&#x20; "errorMessage": "Invalid JSON structure."

}

```



\---



\# ➕ Dodanie nowego parsera



Aby dodać nowy format danych:



Przykład: XML



\## 1. Utworzyć nową implementację:



```csharp

public class XmlContentParser : IContentParser

{

&#x20;   public ContentType SupportedType => ContentType.XML;



&#x20;   public IParseResult Parse(string rawContent)

&#x20;   {

&#x20;       // Logika parsowania XML

&#x20;   }

}

```



\---



\## 2. Dodać nową wartość enum:



```csharp

public enum ContentType

{

&#x20;   CSV,

&#x20;   INTERNAL\_JSON,

&#x20;   XML

}

```



\---



\## 3. Zarejestrować parser:



```csharp

builder.Services.AddSingleton<IContentParser, XmlContentParser>();

```



Istniejący endpoint nie wymaga żadnych zmian.



\---



\# 📝 Podjęte decyzje projektowe



\- Parsery zostały oddzielone od endpointu dzięki zastosowaniu interfejsu `IContentParser`.

\- Factory odpowiada za wybór odpowiedniego parsera.

\- Dependency Injection zarządza zależnościami aplikacji.

\- Globalny middleware obsługuje wyjątki i zapewnia spójny format błędów.

\- CSV jest parsowany przy użyciu `TextFieldParser`, aby poprawnie obsługiwać:

&#x20; - wartości w cudzysłowach,

&#x20; - przecinki w polach,

&#x20; - wartości wieloliniowe.

\- JSON jest obsługiwany dynamicznie poprzez `JsonElement`, ponieważ struktura danych nie jest wcześniej znana.



\---



\# 📄 Licencja



Projekt przygotowany jako implementacja generycznego parsera danych przesyłanych przez API w technologii .NET 10.

