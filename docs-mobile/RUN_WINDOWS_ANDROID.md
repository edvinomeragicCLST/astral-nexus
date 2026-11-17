# Running Android on Windows

The root [README](../README.md) contains the complete current setup, including SQL credentials and the distinction between Compose `.env` and ASP.NET Core configuration.

## Prerequisites

- .NET 10 SDK and the MAUI Android workload.
- Android SDK/platform tools, compatible JDK and an emulator or development device.
- VS Code with C#/.NET MAUI extensions, or Visual Studio with MAUI tooling.

Install missing workloads deliberately:

```powershell
dotnet workload install maui-android
```

Run from the repository root. Restrict the framework evaluation to Android so Windows does not request the declared iOS target:

```powershell
dotnet restore src/AstralNexus.Mobile/AstralNexus.Mobile.csproj -p:TargetFrameworks=net10.0-android
dotnet build src/AstralNexus.Mobile/AstralNexus.Mobile.csproj -f net10.0-android -p:TargetFrameworks=net10.0-android --no-restore
```

Start an emulator, then install/run:

```powershell
dotnet build src/AstralNexus.Mobile/AstralNexus.Mobile.csproj -t:Run -f net10.0-android -p:TargetFrameworks=net10.0-android
```

Shared VS Code tasks use these framework restrictions. The local demo account is `demo@astralnexus.local` / `Demo123!`; web accounts are independent. Create a valid deck before using Match Lab.

## Optional REST catalog download

Follow the README's SQL setup, including the local `.env`, first:

```powershell
docker compose up -d sqlserver
.\scripts\start-web.ps1
```

Enter the current SQL password at the hidden prompt. The helper passes the web connection string through an environment variable. Compose `.env` does not automatically configure the web process. Alternatively, configure .NET user secrets as described in the README before using the VS Code web task.

If Windows disables script execution, use `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\start-web.ps1` for this invocation only, or use the README's user-secrets alternative.

Check `http://localhost:5092/api/cards` on the host. In mobile Settings, use `http://10.0.2.2:5092` for the standard Android emulator and select **Sinhronizuj katalog**. If needed, use `-ListenUrl 'http://0.0.0.0:5092'`; the README explains physical-device addressing, firewall access and network exposure.

Only existing cards' catalog fields are updated, matching `ExternalKey`. New cards, artwork and account/collection/deck/match data are not synchronized. Offline features do not require the web app.

## iOS and validation status

The source also declares `net10.0-ios`, requiring suitable Mac/Xcode tooling and signing configuration. iOS was not tested. Fresh Android builds and REST behavior also remain unverified in this publication review because the .NET SDK and Docker CLI were unavailable. See [the review](../docs/PUBLICATION_REVIEW.md).
