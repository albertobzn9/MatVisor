# VisorMat

Una herramienta de escritorio ultra rápida diseñada para investigadores que necesitan inspeccionar archivos `.mat` sin tener que abrir MATLAB.

> Estado actual: la rama `feature/avalonia-foundation` permite elegir un archivo `.mat` desde la aplicación o abrir el archivo entregado al iniciar; muestra su primera matriz numérica en una tabla de solo lectura. La asociación por doble clic se configurará con los instaladores en una etapa posterior.

## El Problema
Abrir MATLAB solo para revisar si los datos de un entrenamiento son correctos consume tiempo y memoria RAM. VisorMat resuelve esto permitiendo una inspección instantánea.

## Características
- **Apertura Instantánea:** Compilado de forma nativa para Windows (C#/.NET).
- **Interoperabilidad:** Permite seleccionar datos y copiarlos (`Cmd/Ctrl + C`) con formato de celdas para pegarlos directamente en Excel.
- **Visualización Limpia:** Formateo automático de decimales y resaltado de filas para mejorar la legibilidad de matrices grandes.
- **Portabilidad:** No requiere MATLAB instalado en la computadora.

## Instalación
Descarga el ejecutable desde la sección **[Releases]**. Es un archivo único "Self-Contained" que funciona en cualquier PC del laboratorio.

## Ejecutar la base Avalonia

Requiere el SDK de .NET 10. Desde la raíz del repositorio:

```bash
dotnet run -- /ruta/a/archivo.mat
```

Pulsa **Abrir archivo .mat…** y selecciona el archivo, o inicia la aplicación con su ruta. La ventana debe mostrar el nombre y las dimensiones de la primera matriz numérica, además de sus valores en una tabla. Esta versión aún no permite seleccionar otras variables del archivo.

Para seleccionar varias celdas, haz clic en la primera y arrastra el cursor hasta la última. `Cmd+A` selecciona toda la matriz en macOS y `Ctrl+A` hace lo mismo en Windows; `Cmd+C` o `Ctrl+C` copia el rango seleccionado para pegarlo en Excel.

## Publicación temporal para comprobar cada sistema

Para generar un ejecutable autocontenido de prueba para Windows:

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Para macOS con Apple Silicon:

```bash
dotnet publish -c Release -r osx-arm64 --self-contained true
```

Los paquetes de instalación y la asociación por doble clic se crearán después de que la tabla esté migrada.

## Archivo Paralelo En Drive

El snapshot usado como evidencia tutoral se conserva en:

```text
/Users/ab/Library/CloudStorage/GoogleDrive-jasjabs19@gmail.com/My Drive/workspace/04_doctorado/02_evaluacion-tutoral/01_sources/04_repos_del_proyecto/02_VisorMat-main
```

Es una copia histórica de referencia. El trabajo activo y las correcciones se
hacen únicamente en este repositorio.
