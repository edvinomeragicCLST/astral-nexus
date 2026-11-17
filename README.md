# Astral Nexus

Astral Nexus is a university project exploring collection and deck management for a collectible card game. This repository keeps two C# applications together: an ASP.NET Core web application and a .NET MAUI mobile application. Both use the same 124-card catalog and deck rules, with separate accounts and storage. The interface and much of the academic documentation are in Bosnian.

The project demonstrates authentication, CRUD operations, relational data modeling, business-rule validation, local mobile persistence and a small REST integration. Match Lab provides manual play assistance; a complete online game engine is outside this prototype's scope.

## Applications and implementation status

The following features were verified in source code. They were **not runtime-tested** during publication preparation; see [the validation report](docs/PUBLICATION_REVIEW.md).

| Area | Web application | Mobile application |
| --- | --- | --- |
| Accounts | Registration, cookie login/logout, user/admin roles | Local SQLite registration/login/logout; session in memory |
| Profile/dashboard | Display name, bio, coins and statistics | Display name, account details, coins and statistics |
| Collection | Search and type/value filters | Search and type filter |
| Decks | CRUD, ownership checks and deck validation | CRUD, selection from owned cards and deck validation |
| Packs | Six cards for 100 coins, duplicate refunds, per-card opening history | Six cards for 100 coins, duplicate refunds, opening summaries |
| Match Lab | Valid deck, random board card, draw/discard, HP, turns, dice/coin, recorded results/rewards | Valid deck, draw/discard, HP, turns, dice/coin, recorded results/rewards |
| Administration | Dashboard, card CRUD/image upload, user roles and coins | No administration module |
| Themes | Dark/light preference | System/light/dark preference |
| REST | Public card reads; admin card writes; authenticated collection/deck/statistics endpoints | Optional catalog-field download from `GET /api/cards` |

Both deck validators require exactly 20 distinct cards, with at most three value-3 cards, eight Mage cards, one Sentinel and five Effect cards.

Incomplete or limited functionality:

- Match Lab does not implement automatic combat, card-effect resolution, matchmaking or network multiplayer. Players manually record results.
- Mobile catalog synchronization updates existing cards matched by `ExternalKey`. It does not add new cards, remove inactive cards, download artwork or synchronize accounts, collections, decks, coins or matches. All 124 bundled card keys currently match the web seed.
- Mobile board-card JSON and artwork are bundled, but the mobile Match Lab does not consume that catalog.
- Android and iOS targets are declared. Earlier Android build output exists locally, but it does not establish that the current source passes a fresh build. iOS build/run behavior is **untested**.

## Technologies and prerequisites

These versions come from the project and Compose files; no dependency upgrades were performed.

| Component | Technology |
| --- | --- |
| Web | .NET **10**, ASP.NET Core MVC, Razor, C#, CSS |
| Web data | EF Core SQL Server and Design **10.0.0**; SQL Server **2022** (`2022-latest` image, Developer edition) |
| Mobile | .NET **10** MAUI, XAML/C#, Microsoft.Maui.Controls **10.0.20** |
| Mobile data | sqlite-net-pcl **1.9.172**, local SQLite |
| Targets | `net10.0-android` (minimum API 23); `net10.0-ios` (minimum iOS 15.0 declared) |
| Password storage | PBKDF2-SHA256 with random salts: web 120,000 iterations; mobile 100,000 |

For the web app, install the .NET 10 SDK and use an existing SQL Server or Docker with Compose v2. The provided SQL image uses Linux containers and `linux/amd64`; Docker Desktop must be running if used. There is no Node/npm frontend build.

For Android, also install the MAUI Android workload, Android SDK/platform tools, a compatible JDK and an emulator or development device. VS Code with C#/.NET MAUI extensions, or Visual Studio with MAUI tooling, can provide the development interface. iOS requires suitable Mac/Xcode tooling and signing configuration; it is outside the verified scope here.

## Repository structure

```text
AstralNexusWebPrototype.sln    Solution containing both applications
.env.example                 Public configuration template
docker-compose.yml          Local SQL Server and persistent named volume
.vscode/tasks.json           Shared development tasks
src/
  AstralNexus.Web/
    Controllers/             MVC and REST endpoints
    Data/                    EF model, initialization and seeding
    Models/ Services/ Views/ Web logic and Razor UI
    Seed/                    Required card/board-card JSON
    wwwroot/                 CSS and bundled artwork
  AstralNexus.Mobile/
    Data/ Models/ Services/  SQLite, accounts, rules and REST download
    Pages/                   XAML screens and code-behind
    Platforms/               Android/iOS source
    Resources/               JSON, artwork, icon and splash
scripts/                     Startup helpers; database reset disabled
docs/                        Web requirements, schema and manual test plan
docs-mobile/                 Mobile requirements, design and Android guide
```

## 1. Configure and start SQL Server

Run terminal commands from the repository root, containing the solution and Compose file. The examples use PowerShell on Windows.

1. If `.env` does not exist, run `Copy-Item .env.example .env`. Edit it locally and replace the placeholder with a strong, unique password that satisfies SQL Server's password policy. Keep the value single-quoted. Never commit this file. During publication preparation, the existing hard-coded SQL password was retained in an ignored local `.env`; keep that value for existing data until you deliberately rotate it.
2. For an **existing volume**, use its **current** SQL password. Changing the container's `MSSQL_SA_PASSWORD` does not rotate an existing database login's password.
3. Start the service and wait for the logs to report readiness:

   ```powershell
   docker compose up -d sqlserver
   docker compose ps
   docker compose logs --tail 40 sqlserver
   ```

SQL Server is available at `localhost,14333`, login `sa`. The host port binds to `127.0.0.1`, keeping the database off the LAN. The existing service retains container name `astral-nexus-sql` and volume declaration `astral_nexus_sql_data`.

**Moving/renaming this folder:** Compose project names affect volume names. Renaming the folder to `astral-nexus` can select a different volume. Before moving existing data, inspect `docker compose ls` and `docker volume ls`, then retain the original project name through `COMPOSE_PROJECT_NAME` in your local `.env` or `docker compose -p <original-project-name> ...`. Do not guess a volume name or delete the old one.

An existing non-Docker SQL Server is also usable: skip Compose and configure the web app for that server. Do not run the reference SQL file as an installation step.

## 2. Configure and start the web application

Compose reads `.env` to configure **the container**. A separately launched `dotnet run` process does **not** automatically read it. `Program.cs` reads `ConnectionStrings:DefaultConnection` through standard ASP.NET Core configuration. There is no embedded connection string; `appsettings.Local.json` is not loaded by this application.

Restore/build just the web project so the solution's mobile targets do not request extra tooling:

```powershell
dotnet --info
dotnet restore src/AstralNexus.Web/AstralNexus.Web.csproj
dotnet build src/AstralNexus.Web/AstralNexus.Web.csproj --no-restore
```

With SQL running, start the Windows helper:

```powershell
.\scripts\start-web.ps1
```

If Windows reports that scripts are disabled, run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\start-web.ps1` to allow this invocation only; it does not change the machine's execution policy. Organization-enforced policies may still apply, in which case use the user-secrets setup below.

Enter the **current SQL `sa` password**, matching `.env`, at its hidden prompt. The helper builds a correctly quoted connection string for `localhost,14333` / `AstralNexusDb`, passes it through `ConnectionStrings__DefaultConnection`, and restores the previous environment variable on exit. It does not read `.env`, rotate credentials or save the prompted password in source or command history.

Open `http://localhost:5092`. The launch profile sets `ASPNETCORE_ENVIRONMENT=Development`. Startup creates/seeds the selected database, so use a local development database you intend the app to initialize.

For a different server or shell, securely set `ConnectionStrings__DefaultConnection` before running the web project. The local connection-string template is:

```text
Server=localhost,14333;Database=AstralNexusDb;User ID=sa;Password=<CURRENT_SQL_PASSWORD>;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True
```

These certificate/encryption options are for local development. They are not a public deployment configuration.

Alternatively, the project has a `UserSecretsId`, allowing development configuration outside the repository:

```powershell
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' '<YOUR_COMPLETE_LOCAL_CONNECTION_STRING>' --project src/AstralNexus.Web/AstralNexus.Web.csproj
dotnet run --project src/AstralNexus.Web/AstralNexus.Web.csproj
```

Angle-bracket values are placeholders. Typing real credentials into a command can leave them in shell history; prefer the hidden-prompt helper on Windows. User secrets are local development storage, not an encrypted production secret store. Environment variables override user secrets. The VS Code web task and `bash scripts/start.sh` require independent web configuration first; the Bash helper also starts SQL and restores only the web project.

### Database initialization and intentional demo accounts

`DatabaseBootstrapper` calls `EnsureCreatedAsync()` and then `DatabaseSeeder`. There are **no EF migrations**. A new empty database receives the schema, 124 cards, 10 board cards and missing demo accounts. Missing cards are inserted by `ExternalKey`; existing card edits are not overwritten. The demo user receives a collection, a valid starter deck and sample matches when its collection is first created.

| Local web login | Password | Purpose |
| --- | --- | --- |
| `demo@astralnexus.local` or `demo` | `Demo123!` | User workflow demonstration |
| `admin@astralnexus.local` or `admin` | `Admin123!` | Administrator workflow demonstration |

These intentionally public demo credentials are separate from the SQL credential. The seeder creates the demo accounts regardless of hosting environment, without resetting existing account passwords. Do not expose a running instance with these accounts to the public internet.

`docs/AstralNexusDb.sql` is reference DDL, not a database dump or required migration. Preserve it, the seed JSON and artwork. `EnsureCreated` does not upgrade an old schema: back up existing data and plan schema changes explicitly.

To stop/start SQL while retaining data, use `docker compose stop sqlserver` and `docker compose start sqlserver`. **Do not use `docker compose down -v`**, which deletes the database volume. `scripts/reset-db.sh` now refuses to reset data.

## 3. Build and run Android

The mobile application can run offline without SQL Server, Docker or the web app.

1. Install the prerequisites above. If needed, deliberately install the Android workload:

   ```powershell
   dotnet workload install maui-android
   ```

2. Restore/build only the Android framework. Overriding `TargetFrameworks` avoids requesting the declared iOS target on Windows:

   ```powershell
   dotnet restore src/AstralNexus.Mobile/AstralNexus.Mobile.csproj -p:TargetFrameworks=net10.0-android
   dotnet build src/AstralNexus.Mobile/AstralNexus.Mobile.csproj -f net10.0-android -p:TargetFrameworks=net10.0-android --no-restore
   ```

3. Start an Android emulator or connect a development device, then run:

   ```powershell
   dotnet build src/AstralNexus.Mobile/AstralNexus.Mobile.csproj -t:Run -f net10.0-android -p:TargetFrameworks=net10.0-android
   ```

   Shared VS Code tasks provide the same commands. The workload-restore task can install missing tools; invoke it only when intended.

4. Continue from Welcome and use `demo@astralnexus.local` / `Demo123!`, or register locally. SQLite tables and bundled card data initialize on first database use. The database is `astral_nexus_mobile.db3` in `FileSystem.AppDataDirectory`, outside the repository. The mobile demo starts with 500 coins and 35 owned cards; create a valid 20-card deck before starting Match Lab. Web and mobile accounts are independent.

### Optional web catalog download

Start SQL and the configured web app, then check `http://localhost:5092/api/cards` on the computer. In mobile **Postavke** (Settings), use `http://10.0.2.2:5092` and select **Sinhronizuj katalog**. The standard Android emulator uses `10.0.2.2` to reach the host; `localhost` inside the emulator refers to the emulator itself.

Try the helper's default loopback URL first. If emulator networking requires a wider bind, use:

```powershell
.\scripts\start-web.ps1 -ListenUrl 'http://0.0.0.0:5092'
```

For a physical phone on the same trusted network, use this bind, allow the development port through the firewall and enter `http://<YOUR_COMPUTER_LAN_IP>:5092` in Settings. Keep the database port on loopback. The wider web bind can expose the demo to that network; return to the loopback default afterward.

The Android manifest permits cleartext HTTP for local demonstration. Catalog reads require no login. Other web APIs use web cookie authentication; there is no mobile backend-login/token integration. See the synchronization limits above.

## Validation and known limitations

- Only static checks completed during publication preparation. The .NET SDK and Docker CLI were unavailable, so restore, compilation, SQL startup, Android installation and end-to-end sync could not run. Existing binaries do not prove a fresh passing build.
- No automated test projects or runner configuration exist. [The web smoke test plan](docs/TEST_PLAN.md) is a manual checklist, not an executed test report.
- Active web matches and mobile login sessions are held in memory; restarting can lose them. Saved history stays in its database.
- Mobile multi-step writes are not a single transaction. Production concurrency, recovery, authentication hardening, HTTPS and deployment remain unvalidated.
- `2022-latest` is a moving image tag. Dependencies and image contents were not upgraded or audited for vulnerabilities during this review.

Before publication, review [the handoff](docs/PUBLICATION_REVIEW.md), complete available builds/smoke checks and confirm permission to publish the artwork. Never upload the entire working folder as an archive: ignored output may retain old configuration and private data. No open-source license has been selected or added.

## Further documentation

- [Web requirements](docs/SRS.md), [architecture](docs/CODE_ANALYSIS.md), [schema](docs/DATABASE_SCHEMA.md), [Unity-to-.NET mapping](docs/UNITY_TO_DOTNET_MAPPING.md).
- [Android guide](docs-mobile/RUN_WINDOWS_ANDROID.md), [mobile requirements](docs-mobile/SRS_MOBILE.md), [requirements mapping](docs-mobile/REQUIREMENTS_MAPPING.md), [UI proposal](docs-mobile/UI_PROPOSAL.md).

These documents preserve academic design context; requirements and proposals describe intent. Use this README and the source for current configuration and implementation status.
