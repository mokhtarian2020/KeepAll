# KeepAll

A unified "save it for later" hub for links and media (Books/Movies/Podcasts) built with .NET MAUI.

## Features

- Mobile-first design (Android first, iOS-ready)
- Clean MVVM architecture
- SQLite local storage
- Share target support for Android
- Privacy-first (no ads, no tracking)
- Accessible UI with large tap targets

## Architecture

- **KeepAll.App**: .NET MAUI app with UI and dependency injection
- **KeepAll.Core**: Domain models and interfaces
- **KeepAll.Storage**: SQLite repository implementation
- **KeepAll.Metadata**: Metadata enrichment service stubs
- **KeepAll.Tests**: Unit tests

## Getting Started

1. Open the solution in Visual Studio 2022
2. Set KeepAll.App as the startup project
3. Run on Android emulator or device

## Privacy

KeepAll is private by default. No data is sent to external servers, no ads, no tracking.
