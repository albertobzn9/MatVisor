# MatVisor

<p align="center">
  <img src="src/MatVisor/Assets/matvisor-mark.png" alt="Logotipo de MatVisor" width="150" />
</p>

<p align="center">
  Visor ligero de matrices MATLAB <code>.mat</code>, creado con .NET y Avalonia.<br />
  <a href="README.md">English</a> · <a href="README.es.md">Español</a>
</p>

MatVisor abre matrices numéricas de MATLAB sin requerir MATLAB. Está pensado para inspeccionar datos rápidamente, copiarlos a Excel y preparar matrices conductuales de 9 columnas para su revisión.

## Funciones

- Abre matrices numéricas `.mat` desde la aplicación, la terminal o con doble clic en macOS cuando el tipo de archivo está asociado.
- Permite seleccionar rangos de celdas con el mouse y copiarlos con `Ctrl+C` o `⌘C`.
- Convierte una vista de 9 columnas a 8 columnas: elimina las filas cuya columna 9 es `2` y vuelve a numerar la columna 1.
- Guarda la matriz derivada de 8 columnas como un archivo `.mat` compatible.
- Genera una ventana independiente que divide los valores en tres conjuntos y muestra sus promedios.
- Funciona en macOS y Windows mediante lanzamientos autocontenidos.

## Descarga

Descarga el instalador o paquete portable para tu sistema operativo desde el [último lanzamiento](https://github.com/albertobzn9/MatVisor/releases/latest).

Los paquetes de macOS incluyen `.dmg` y `.zip`. Los de Windows incluyen un `.exe` independiente y un `.zip` portable.

## Ejecutar desde el código

Requisito: [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/albertobzn9/MatVisor.git
cd MatVisor
dotnet run --project src/MatVisor/MatVisor.csproj
```

Para abrir directamente un archivo:

```bash
dotnet run --project src/MatVisor/MatVisor.csproj -- /ruta/a/matriz.mat
```

## Estructura del repositorio

```text
src/MatVisor/       Aplicación Avalonia, recursos, vistas, modelos y servicios
docs/               Notas de arquitectura y comportamiento para contribuidores
packaging/          Metadatos de la aplicación para macOS
scripts/            Script local para empaquetar lanzamientos
.github/workflows/  Compilación continua del ejecutable para Windows
```

Consulta la [guía de arquitectura](docs/README.md) para conocer el papel de cada componente y las reglas de procesamiento de matrices.

## Crear un lanzamiento local

En macOS, crea los cuatro archivos distribuibles con:

```bash
./scripts/package-release.sh 1.3.0
```

El script genera un `.dmg` y un `.zip` de macOS, además de un `.exe` y un `.zip` de Windows, en `artifacts/release/<versión>/`.

## Tecnología

- [.NET 10](https://dotnet.microsoft.com/)
- [Avalonia UI](https://avaloniaui.net/)
- [MathNet.Numerics.Data.Matlab](https://www.nuget.org/packages/MathNet.Numerics.Data.Matlab)

## Licencia

Este repositorio no declara actualmente una licencia. Consulta con la persona propietaria antes de redistribuir o reutilizar el código.
