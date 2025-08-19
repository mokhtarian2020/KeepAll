#!/bin/bash

echo "🎯 KeepAll Quick Test"
echo "===================="

# Set dotnet alias
export PATH="/usr/share/dotnet:$PATH"
alias dotnet='/usr/share/dotnet/dotnet'

echo "📦 Building demo..."
/usr/share/dotnet/dotnet build src/KeepAll.Demo/KeepAll.Demo.csproj

echo ""
echo "🚀 Running demo..."
echo "This will open an interactive menu to test KeepAll functionality."
echo "You can:"
echo "  - Add items (Links, Books, Movies, Podcasts)"
echo "  - Search and filter items"
echo "  - Mark items as done"
echo "  - Export data"
echo "  - Simulate Android sharing"
echo ""
echo "Press Ctrl+C to stop anytime."
echo ""

# Run the demo
/usr/share/dotnet/dotnet src/KeepAll.Demo/bin/Debug/net8.0/KeepAll.Demo.dll
