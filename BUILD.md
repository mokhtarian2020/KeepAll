# Build and Run Instructions

## Prerequisites

1. Install .NET 8 SDK:
   ```bash
   sudo snap install dotnet-sdk
   # or
   sudo apt install dotnet-host-8.0 dotnet-sdk-8.0
   ```

2. Install Visual Studio 2022 with .NET MAUI workload, or use VS Code with C# extension

## Building the Solution

1. Open terminal in the project root directory
2. Restore dependencies:
   ```bash
   dotnet restore KeepAll.sln
   ```
3. Build the solution:
   ```bash
   dotnet build KeepAll.sln
   ```

## Running the Tests

```bash
dotnet test src/KeepAll.Tests/KeepAll.Tests.csproj
```

## Running the Android App

### Option 1: Visual Studio 2022
1. Open `KeepAll.sln` in Visual Studio 2022
2. Set `KeepAll.App` as startup project
3. Select Android emulator or connected device
4. Press F5 to run

### Option 2: Command Line
```bash
dotnet build src/KeepAll.App/KeepAll.App.csproj -f net8.0-android
# Deploy to emulator/device (requires Android SDK setup)
```

## Project Structure Overview

- `src/KeepAll.Core/` - Domain models and interfaces
- `src/KeepAll.Storage/` - SQLite repository implementation  
- `src/KeepAll.Metadata/` - Metadata enrichment service stubs
- `src/KeepAll.App/` - .NET MAUI application
- `src/KeepAll.Tests/` - Unit tests

## Features Implemented

✅ Clean MVVM architecture with dependency injection  
✅ SQLite local storage with repository pattern  
✅ Three-tab navigation (Inbox, Media, Search)  
✅ Android share target support  
✅ Privacy-first design (no external services)  
✅ Accessible UI with large tap targets  
✅ Search functionality  
✅ Export functionality  

## Testing Android Share

1. Build and install the app on Android device/emulator
2. Open any app (Chrome, etc.) and share text/URL
3. Select "Save to KeepAll" from share menu
4. Open KeepAll app - shared items will appear in Inbox

## Next Steps

- Add metadata enrichment APIs (Open Library, TMDb, iTunes)
- Implement swipe gestures for item actions
- Add more sophisticated UI themes
- iOS platform implementation
- Enhanced search with filters
