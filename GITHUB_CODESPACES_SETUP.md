# GitHub Codespaces Setup for .NET MAUI

## Overview
GitHub Codespaces provides cloud-based development environments. We can use Windows containers for MAUI development.

## Setup Steps

### 1. Create Codespaces Configuration

Create `.devcontainer/devcontainer.json` in your repository:

```json
{
  "name": "KeepAll MAUI Development",
  "image": "mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022",
  "hostRequirements": {
    "cpus": 4,
    "memory": "8gb",
    "storage": "32gb"
  },
  "features": {
    "ghcr.io/devcontainers/features/dotnet:1": {
      "version": "8.0"
    }
  },
  "postCreateCommand": [
    "powershell",
    "-Command",
    "dotnet workload install maui"
  ],
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csharp",
        "ms-dotnettools.vscode-dotnet-runtime"
      ]
    }
  }
}
```

### 2. Alternative: Use GitHub Actions + Download Artifacts

Since Windows Codespaces can be expensive, use the GitHub Actions workflow I created earlier:

1. **Push your code to GitHub**
2. **GitHub Actions will automatically build APK**
3. **Download APK from Actions artifacts**

## Quick Start with GitHub Actions

### 1. Initialize Git Repository
```bash
cd /home/amir/Documents/amir/excellent/KeepAll
git init
git add .
git commit -m "Initial KeepAll MAUI project"
```

### 2. Create GitHub Repository
- Go to github.com
- Create new repository named "KeepAll"
- Follow instructions to push existing repository

### 3. Push Code
```bash
git remote add origin https://github.com/yourusername/KeepAll.git
git branch -M main
git push -u origin main
```

### 4. Trigger Build
- GitHub Actions will automatically trigger
- Or manually trigger via Actions tab
- Download APK from Artifacts after build completes

## Cost Comparison

| Option | Cost | Setup Time | Performance |
|--------|------|------------|-------------|
| Local VM | Free | 2-4 hours | Good |
| GitHub Actions | Free (2000 min/month) | 10 minutes | Excellent |
| Codespaces | $0.18/hour | 5 minutes | Excellent |

## Recommendation

**Start with GitHub Actions** - it's free, fast, and requires minimal setup. The workflow I created will automatically build your APK whenever you push changes.
