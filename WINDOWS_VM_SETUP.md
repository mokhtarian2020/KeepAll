# Windows VM Setup for .NET MAUI Development

## Option A: VirtualBox (Free)

### 1. Download and Install VirtualBox
```bash
sudo apt update
sudo apt install virtualbox virtualbox-ext-pack
```

### 2. Download Windows 11 Development VM
- Visit: https://developer.microsoft.com/en-us/windows/downloads/virtual-machines/
- Download "Windows 11 development environment" (VirtualBox version)
- This includes Visual Studio 2022 Community pre-installed

### 3. VM Configuration
- **RAM**: Minimum 8GB (16GB recommended)
- **Disk**: 100GB+ for development
- **CPU**: 4+ cores if available
- **Enable**: Virtualization features in BIOS

### 4. Setup Development Environment in VM

Once Windows VM is running:

```cmd
# Install .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Install MAUI workload
dotnet workload install maui

# Verify installation
dotnet workload list
```

## Option B: VMware (Better Performance)

### 1. Install VMware Workstation Pro
```bash
# Download from VMware website (60-day trial)
# Or use VMware Player (free for personal use)
```

### 2. Create Windows 11 VM
- Use Windows 11 ISO from Microsoft
- Allocate sufficient resources (8GB+ RAM, 100GB+ disk)

## Option C: QEMU/KVM (Advanced)

### 1. Install KVM
```bash
sudo apt install qemu-kvm libvirt-daemon-system libvirt-clients bridge-utils virt-manager
sudo usermod -aG libvirt $USER
sudo usermod -aG kvm $USER
```

### 2. Create Windows VM using virt-manager
```bash
virt-manager
```

## Transferring Your Project to VM

### Method 1: Shared Folder
1. Set up shared folder in VM settings
2. Point to your `/home/amir/Documents/amir/excellent/KeepAll` directory

### Method 2: Git Clone
```cmd
# In Windows VM
git clone https://github.com/yourusername/KeepAll.git
cd KeepAll
```

### Method 3: Copy via Network/USB
- Zip your project and transfer to VM

## Building in Windows VM

```cmd
# Navigate to project
cd C:\path\to\KeepAll

# Restore packages
dotnet restore

# Build for Android
dotnet build src/KeepAll.App/KeepAll.App.csproj -f net8.0-android -c Release

# Publish APK
dotnet publish src/KeepAll.App/KeepAll.App.csproj -f net8.0-android -c Release -p:AndroidPackageFormat=apk
```

## Tips for Better Performance

1. **Enable Hardware Acceleration**:
   - Enable VT-x/AMD-V in BIOS
   - Use hardware acceleration in VM settings

2. **Allocate Sufficient Resources**:
   - 8GB+ RAM for the VM
   - 4+ CPU cores
   - Enable 3D acceleration

3. **Use SSD Storage** for better I/O performance

4. **Close Unnecessary Services** in Windows to save resources
