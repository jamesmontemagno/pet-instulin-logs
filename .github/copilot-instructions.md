## Repo snapshot

This is a .NET MAUI single-project mobile app named `PetInsulinLogs` (see `PetInsulinLogs/PetInsulinLogs.csproj`). Targets: Android, iOS, MacCatalyst (and optionally Windows/Tizen). Keep changes small and platform-agnostic unless a platform file is explicitly edited under `Platforms/`.

## Big picture (what to know quickly)

- App entry and DI: `PetInsulinLogs/MauiProgram.cs` builds the MAUI app. Use `UseMauiApp<App>()` and register services here when adding app-level dependencies.
- UI layer: XAML pages live next to code-behind (e.g., `MainPage.xaml` / `MainPage.xaml.cs`, `AppShell.xaml` / `AppShell.xaml.cs`). Prefer modifying XAML for visual changes and code-behind for UI logic.
- Resources: Images, fonts, splash and icons are under `Resources/` and wired in the `.csproj` using `MauiImage`, `MauiFont`, `MauiSplashScreen`, `MauiIcon`.

## Project-specific patterns & conventions

- Single-project MAUI: The project uses <SingleProject>true</SingleProject> — do not create separate projects for platforms unless absolutely necessary.
- Platform code location: platform-specific code lives under `Platforms/{Android,iOS,MacCatalyst,Windows,Tizen}`. If you need native APIs, add code there (follow existing simple examples: `MainActivity.cs`, `AppDelegate.cs`).
- Fonts & images: Add new fonts to `Resources/Fonts/` and register in `MauiProgram.cs`. Add images to `Resources/Images/` and update .csproj only when special resizing is needed.
 - One type per file (important):
   - Models: Each model must reside in its own file under `PetInsulinLogs/Models` (e.g., `Pet.cs`, `LogEntry.cs`, `VacationPlan.cs`, `EntityValidation.cs`). Do not group multiple model classes in a single file.
   - Interfaces: Each service interface must reside in its own file under `PetInsulinLogs/Services` (e.g., `IPetRepository.cs`, `ILogRepository.cs`, `IVacationPlanRepository.cs`, `IIdService.cs`, `ITimeService.cs`). Avoid catch-all files like `Interfaces.cs`.

## Build / run / debug (developer workflow)

- Typical local debug (macOS with .NET SDK installed):
  - `dotnet build -f net9.0-ios` or `dotnet build` for all targets
  - Use `dotnet build -t:Run -f net9.0-android` or open in Visual Studio for Mac/Win and run the desired platform target.
- When adding native entitlements or capabilities modify the platform `Info.plist` (iOS/MacCatalyst) or `AndroidManifest.xml` (Android) under `Platforms/`.

## Tests & CI

- No test project detected. If adding tests prefer xUnit/NUnit and keep them as a separate project `tests/` alongside the main project (do not embed within the MAUI single-project).

## Integration points & external deps

- NuGet packages are referenced in the `.csproj`. Example packages already used: `Microsoft.Maui.Controls`, `Microsoft.Extensions.Logging.Debug`.
- For telemetry or cloud integration, add service registrations in `MauiProgram.CreateMauiApp()` and keep configuration in code or a new `appsettings.*` pattern if needed.

## Code examples to follow

- Register fonts in `MauiProgram.cs`:
  - See `builder.ConfigureFonts(...)` — add new fonts there and match resource name from `Resources/Fonts/`.
- Simple UI event pattern: `MainPage.xaml.cs` uses `InitializeComponent()` in constructor and event handlers like `OnCounterClicked` that update UI elements by name (e.g., `CounterBtn`). Keep UI logic minimal in code-behind; prefer MVU/MVVM for complex features.

## When you edit this repo (rules for AI agents)

- Preserve single-project structure; do not split into multiple projects unless asked.
- When changing resources update both the `Resources/` file and the `.csproj` only if the resource needs special metadata (resize, logical name, etc.).
- Keep platform-specific changes isolated to `Platforms/*` and call out in PR description which platform(s) you affected.

## PR & commit notes

- Small, focused PRs. Each PR should state which platform(s) were changed and include a short manual test matrix (e.g., Android emulator: pass, iOS simulator: pass).
