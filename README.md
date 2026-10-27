# MatVisor

<p align="center">
  <img src="src/MatVisor/Assets/matvisor-mark.png" alt="MatVisor logo" width="150" />
</p>

<p align="center">
  A lightweight desktop viewer for MATLAB <code>.mat</code> matrices, built with .NET and Avalonia.<br />
  <a href="README.md">English</a> · <a href="README.es.md">Español</a>
</p>

MatVisor opens numeric MATLAB matrices without requiring MATLAB. It is designed for fast inspection, copying data to Excel, and preparing 9-column behavioral matrices for review.

## Features

- Open numeric `.mat` matrices from the app, command line, or by double-clicking an associated file on macOS.
- Select cell ranges with the mouse and copy them with `Ctrl+C` or `⌘C`.
- Switch a 9-column matrix to an 8-column view, removing rows where column 9 equals `2` and renumbering column 1.
- Save the derived 8-column matrix as a compatible `.mat` file.
- Generate a separate analysis window that splits values into three groups and displays their averages.
- Switch the application interface between Spanish and English without reloading the matrix.
- Run on macOS and Windows from self-contained releases.

## Download

Download the installer or portable package for your operating system from the [latest release](https://github.com/albertobzn9/MatVisor/releases/latest).

macOS packages include a `.dmg` and a portable `.zip`. Windows packages include a standalone `.exe` and a portable `.zip`.

## Run from source

Requirements: [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/albertobzn9/MatVisor.git
cd MatVisor
dotnet run --project src/MatVisor/MatVisor.csproj
```

To open a file directly:

```bash
dotnet run --project src/MatVisor/MatVisor.csproj -- /path/to/matrix.mat
```

## Repository layout

```text
src/MatVisor/       Avalonia application, assets, views, models, and services
docs/               Architecture and behavior notes for contributors
packaging/          macOS application metadata
scripts/            Local release-packaging script
.github/workflows/  Continuous build for the Windows executable
```

See [the architecture guide](docs/README.md) for the role of each component and the matrix-processing rules.

## Build a release locally

On macOS, create the four distributable files with:

```bash
./scripts/package-release.sh 1.3.0
```

The script produces a macOS `.dmg`, a macOS `.zip`, a Windows `.exe`, and a Windows `.zip` under `artifacts/release/<version>/`.

## Technology

- [.NET 10](https://dotnet.microsoft.com/)
- [Avalonia UI](https://avaloniaui.net/)
- [MathNet.Numerics.Data.Matlab](https://www.nuget.org/packages/MathNet.Numerics.Data.Matlab)

## License

This repository currently does not declare a license. Contact the repository owner before redistributing or reusing the code.
