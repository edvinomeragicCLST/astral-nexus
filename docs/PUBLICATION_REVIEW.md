# Publication preparation review

Review date: 17 November 2026. Intended public repository name: `astral-nexus`.

## Status

The source/configuration cleanup is complete. Publication remains pending build and manual runtime verification on a machine with the required tools. No GitHub repository was created, no remote was configured, and nothing was staged, committed or pushed.

This folder was not a Git repository, and no nested Git repositories or applicable `AGENTS.md` files were found. There is therefore no local tracked-file list, commit history or remote configuration to clean or audit. Git metadata was created only in a temporary directory outside the project to evaluate the real ignore rules; the project itself was not initialized.

## Changes

- Replaced the embedded SQL Server Compose password with a required `MSSQL_SA_PASSWORD` reference. Added the public `.env.example` template. Retained the previous credential in an ignored local `.env` so existing credentials are not changed automatically.
- Removed the embedded web connection string. Added a development `UserSecretsId`, an early missing-configuration error, and a Windows helper that prompts for the current SQL password and supplies the connection string through the web process environment. Compose `.env` and web configuration are documented separately.
- Bound the SQL host port to loopback, preserving its port number, container name and named-volume declaration. No Docker commands that start, stop or modify services were executed.
- Disabled the old volume-deleting reset script. Scoped Bash web restore and Android VS Code tasks to their respective application/framework.
- Expanded `.gitignore` for build output, IDE caches, local configuration/environment files, signing material, runtime databases/uploads, logs and generated distribution packages. Shared VS Code tasks remain publishable.
- Rewrote the README and Android setup guide; corrected related documentation. No open-source license was added.

## Credential and content review

The original SQL credential is absent from candidate publishable text. The scan covered source, configuration, scripts and documentation, including common token/private-key patterns and credential assignments. No additional real credentials were identified. Pattern matches in the environment template, hidden-prompt code and XAML password-entry controls were reviewed as placeholders/code, not secrets.

The `.local` demo identities and seeded demo passwords are intentional example data, retained in source and documented. The web seeder creates both demo and administrator accounts in any hosting environment. A running instance must not be exposed publicly with these defaults; source publication and production deployment have different requirements.

The SQL file is reference schema DDL, not a user-data dump. The four bundled JSON catalogs and all 276 PNG assets are intentional resources and remain included. No local database files or signing keys were found outside generated directories during the filesystem inspection. Runtime card uploads are excluded separately from bundled artwork.

The candidate set is approximately 11.4 MiB, with no file over 0.4 MiB. No current required asset needs Git LFS. Local Android packages are approximately 87 MiB each, and thousands of generated files remain on disk under ignored output directories. Those outputs can contain copies of old configuration; do not upload an archive of the entire working folder.

## Validation

| Check | Result |
| --- | --- |
| Solution/project/dependency inspection | Both applications preserved; .NET 10 targets and package versions checked against project files |
| Git ignore evaluation | Real `.env`, generated output, signing files, runtime uploads/databases and packages excluded; `.env.example`, shared VS Code tasks, seed JSON, SQL reference and artwork retained |
| JSON/XML/project/XAML parsing | 26 files parsed successfully |
| Seed resources | 258 web/mobile card artwork references resolved; all 124 mobile card keys match web seed keys |
| PowerShell helper | Syntax passed; mocked prompt/launch checks passed for special-character quoting, expected target and environment cleanup after success/failure |
| Bash helpers | Syntax-only parsing passed; neither script was run |
| Documentation links | Relative links checked after adding this report |
| Secret review | No original SQL credential or additional real credential identified in candidate files; no secret values printed |
| Restore/build | Not runnable: `dotnet` unavailable on PATH or in checked standard installation locations |
| SQL/Compose validation | Not runnable: Docker CLI unavailable on PATH or in its checked standard installation location; no database was accessed |
| Automated tests | No test projects or runner configuration found; existing test plan is a manual checklist |
| Android/iOS runtime and REST integration | Not run; existing Android binaries were not treated as test evidence; iOS remains untested |

The SDK's absence also prevents verifying installed MAUI workloads and their Android SDK/JDK requirements. No SDK, workload or Docker installation was attempted. Windows script execution was disabled by default; the README documents a process-only invocation option and a user-secrets alternative without changing machine policy.

## Before publication

1. On a suitably configured machine, run the README's web restore/build and Android-only restore/build commands. Correct any failures before describing either app as build-verified.
2. Start the intended local SQL instance, configure the web connection independently, and complete the [manual web smoke plan](TEST_PLAN.md). Exercise mobile registration/login, collection, valid deck creation, pack opening, Match Lab/history, persistence after restart, theme selection and optional catalog download. Use a development database intended for these writes. Record actual results separately.
3. If the old SQL password was ever shared or used beyond a private development environment, rotate it before publication. Back up the existing database, connect with the current credential through a trusted SQL client, deliberately change the SQL login password, then update the ignored `.env` and any web environment/user-secrets configuration to match. Recheck connectivity. Changing the container environment alone does not rotate a persisted login. No rotation, reset or volume deletion was performed here.
4. When moving/renaming the folder, preserve the existing Compose project identity to reuse its volume; see the README. Never delete database volumes to fix a password/configuration issue.
5. Confirm permission to publish the bundled artwork and choose a license if desired. None was assumed. At the later Git initialization/publication step, inspect the actual candidate/staged file list again; keep `.env` and ignored outputs private.

Run terminal commands from the repository root containing the solution and Compose file, following the [README](../README.md). Creating the GitHub repository and pushing remain a separate next step.
