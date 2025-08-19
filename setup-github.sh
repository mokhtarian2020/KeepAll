#!/bin/bash

echo "🚀 KeepAll GitHub Setup Script"
echo "=============================="
echo

# Check if git is initialized
if [ ! -d ".git" ]; then
    echo "📁 Initializing Git repository..."
    git init
    echo "✅ Git repository initialized"
else
    echo "✅ Git repository already exists"
fi

# Add all files
echo "📦 Adding files to git..."
git add .

# Check if there are changes to commit
if git diff --staged --quiet; then
    echo "ℹ️  No changes to commit"
else
    echo "💾 Committing changes..."
    git commit -m "Add KeepAll MAUI project with GitHub Actions build"
    echo "✅ Changes committed"
fi

echo
echo "🔗 Next steps:"
echo "1. Create a new repository on GitHub named 'KeepAll'"
echo "2. Run these commands:"
echo "   git remote add origin https://github.com/YOURUSERNAME/KeepAll.git"
echo "   git branch -M main"
echo "   git push -u origin main"
echo
echo "3. Go to GitHub Actions tab to see the build progress"
echo "4. Download the APK from the build artifacts once complete"
echo
echo "📱 The APK will be built automatically on every push!"
echo

# Show current git status
echo "📊 Current Git Status:"
git status --short
