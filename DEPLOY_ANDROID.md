# Android Deployment Guide

## Prerequisites (Windows)

1. **Install .NET 8 SDK**:
   ```cmd
   winget install Microsoft.DotNet.SDK.8
   ```

2. **Install MAUI Workload**:
   ```cmd
   dotnet workload install maui
   ```

3. **Install Android SDK** (via Visual Studio Installer or Android Studio)

## Build APK

1. **Navigate to your project**:
   ```cmd
   cd path\to\KeepAll
   ```

2. **Restore packages**:
   ```cmd
   dotnet restore
   ```

3. **Build for Android**:
   ```cmd
   dotnet build src/KeepAll.App/KeepAll.App.csproj -f net8.0-android -c Release
   ```

4. **Publish APK**:
   ```cmd
   dotnet publish src/KeepAll.App/KeepAll.App.csproj -f net8.0-android -c Release -p:AndroidPackageFormat=apk
   ```

## Install on Phone

### Method 1: USB Debugging (Recommended)
1. **Enable Developer Options** on your Android phone:
   - Go to Settings > About Phone
   - Tap "Build Number" 7 times
   - Go to Settings > Developer Options
   - Enable "USB Debugging"

2. **Connect phone** via USB

3. **Install ADB** (Android Debug Bridge) if not already installed

4. **Deploy directly**:
   ```cmd
   dotnet build src/KeepAll.App/KeepAll.App.csproj -f net8.0-android -c Release -t:Run
   ```

### Method 2: APK File Transfer
1. **Locate the APK**:
   - After publishing, find the APK in:
   - `src/KeepAll.App/bin/Release/net8.0-android/publish/`

2. **Transfer to phone**:
   - Copy APK to phone via USB, email, or cloud storage

3. **Install APK**:
   - On phone, enable "Install from Unknown Sources"
   - Tap the APK file to install

## Signing for Distribution

For distribution outside development:

1. **Create a keystore**:
   ```cmd
   keytool -genkey -v -keystore keepall-release-key.keystore -alias keepall -keyalg RSA -keysize 2048 -validity 10000
   ```

2. **Add to project file** (`KeepAll.App.csproj`):
   ```xml
   <PropertyGroup Condition="'$(Configuration)' == 'Release'">
     <AndroidKeyStore>true</AndroidKeyStore>
     <AndroidSigningKeyStore>keepall-release-key.keystore</AndroidSigningKeyStore>
     <AndroidSigningKeyAlias>keepall</AndroidSigningKeyAlias>
     <AndroidSigningKeyPass>your-key-password</AndroidSigningKeyPass>
     <AndroidSigningStorePass>your-store-password</AndroidSigningStorePass>
   </PropertyGroup>
   ```

3. **Build signed APK**:
   ```cmd
   dotnet publish src/KeepAll.App/KeepAll.App.csproj -f net8.0-android -c Release -p:AndroidPackageFormat=apk
   ```

## Troubleshooting

### Common Issues:
- **SDK not found**: Ensure Android SDK is installed and ANDROID_HOME is set
- **Device not detected**: Check USB debugging is enabled and drivers installed
- **Build errors**: Ensure all NuGet packages are restored and compatible

### Verify Installation:
- Check if Android SDK is detected: `dotnet workload list`
- Test device connection: `adb devices`

### Performance Tips:
- Use Release configuration for better performance
- Enable AOT compilation for faster startup (add to csproj):
  ```xml
  <PropertyGroup>
    <RunAOTCompilation>true</RunAOTCompilation>
  </PropertyGroup>
  ```

## Alternative: GitHub Actions (CI/CD)

You can also set up automated builds using GitHub Actions with Windows runners to build APKs automatically when you push code changes.
