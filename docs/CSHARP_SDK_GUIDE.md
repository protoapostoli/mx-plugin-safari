# C# SDK Guide (.NET 8)

The C# SDK provides high performance, in-process execution within `LogiPluginService`, with direct access to dynamic LCD canvas drawing via `BitmapBuilder`.

---

## 1. Prerequisites

- **.NET 8 SDK**: Download from [Microsoft .NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0).
- Check installation:
  ```bash
  dotnet --version
  ```

---

## 2. Project Setup & Reference

The C# SDK is provided directly by your installed `LogiPluginService`:
- macOS API DLL: `/Applications/Utilities/LogiPluginService.app/Contents/MonoBundle/PluginApi.dll`

In your `.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <RootNamespace>Loupedeck.MyPlugin</RootNamespace>
    <PluginApiDir>/Applications/Utilities/LogiPluginService.app/Contents/MonoBundle/</PluginApiDir>
    <PluginDir>$(HOME)/Library/Application Support/Logi/LogiPluginService/Plugins/</PluginDir>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="PluginApi">
      <HintPath>$(PluginApiDir)PluginApi.dll</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>

  <!-- Auto-creates .link file on build -->
  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <WriteLinesToFile File="$(PluginDir)$(AssemblyName).link" Lines="$(TargetDir)" Overwrite="true" Encoding="Unicode" />
  </Target>
</Project>
```

---

## 3. Dynamic Keypad Display with `BitmapBuilder`

One of the most powerful features of the C# SDK is drawing dynamically on the MX Keypad's 80×80 LCD keys:

```csharp
namespace Loupedeck.MyPlugin
{
    using System;

    public class DynamicStatusButton : PluginDynamicCommand
    {
        private Int32 _cpuUsage = 24;

        public DynamicStatusButton()
            : base(displayName: "CPU Gauge", description: "Shows real-time CPU %", groupName: "Hardware")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            // Key was pressed on MX Keypad
            PluginLog.Info("CPU Button pressed");
            this.ActionImageChanged(); // Request screen redraw
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var builder = new BitmapBuilder(imageSize))
            {
                // Background
                builder.FillRectangle(0, 0, builder.Width, builder.Height, new BitmapColor(20, 20, 25));

                // Status text
                builder.DrawText($"CPU\n{this._cpuUsage}%", BitmapColor.Cyan, 18);

                return builder.ToImage();
            }
        }
    }
}
```

---

## 4. Building and Hot Reload

```bash
cd plugins/templates/csharp-plugin
dotnet build
```

For hot reloading upon saving code:
```bash
dotnet watch build
```
Logi Plugin Service will automatically reload the assembly.
