# slu-tentasal

En webbsida som visar klockan och dagens tentalösenord för ett campus.

```
https://<server>/?campus=Ultuna
```

- Klockan hämtas från servern varje sekund (htmx).
- Lösenordet hämtas med Dapper och pollas med jämna mellanrum (htmx). Det visas från 30 minuter före till 45 minuter efter tentastart.
- Utanför fönstret visas när nästa tentas lösenord blir synligt.

## Kör lokalt

Kräver .NET 10 SDK. Lokalt används SQLite: filen `tentasal.db` skapas automatiskt med exempeldata (Ultuna, Alnarp, Umea).

```
dotnet run --project src/SluTentasal.Web
```

Öppna sedan `https://localhost:7180/?campus=Ultuna`.

## Konfiguration

| Nyckel | Standard | Beskrivning |
|---|---|---|
| `Database:Provider` | `SqlServer` | `Sqlite`, `SqlServer` eller `Postgres` |
| `ConnectionStrings:Tentasal` | – | Connection string |
| `Tentasal:ShowBeforeStartMinutes` | `30` | Minuter före tentastart som lösenordet börjar visas |
| `Tentasal:ShowAfterStartMinutes` | `45` | Minuter efter tentastart som lösenordet slutar visas |
| `Tentasal:PasswordPollSeconds` | `60` | Hur ofta sidan hämtar lösenordet på nytt |
| `Tentasal:TimeZone` | `Europe/Stockholm` | Tidszon som `starttime` lagras i |

I test och produktion sätts värdena med miljövariabler, till exempel:

```
Database__Provider=Postgres
ConnectionStrings__Tentasal=Host=...;Database=...;Username=...;Password=...
```

## Healthcheck

| Endpoint | Kontrollerar | Svar |
|---|---|---|
| `/health/live` | Att processen är igång (inga beroenden) | `200 Healthy` |
| `/health` | Konfigurationen under `Tentasal` (tidszon, giltiga minuter/sekunder) och att databasen svarar och tabellen `Passwords` finns | JSON, `200` om allt är friskt annars `503` |

Databasfel loggas med detaljer, men svaret från `/health` innehåller bara en allmän beskrivning så att servernamn och liknande inte exponeras.

## Databas

Tabellen `Passwords` har kolumnerna `campus`, `password` och `starttime`. Skript för att skapa den finns i [sql/](sql/) för SQL Server, PostgreSQL och SQLite.
