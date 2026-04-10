# PDFSharp

`com.sky.pdfsharp` is a Unity package that exposes itself as `PDFSharp` and packages the PDFsharp Core build with a small convenience API for lightweight PDF text extraction.

This repository currently bundles PDFsharp `7.0.0-preview-1` from the upstream `PDFsharp` NuGet package:

- upstream package: https://www.nuget.org/packages/PDFsharp/7.0.0-preview-1
- upstream repository: https://github.com/empira/PDFsharp

## What This Package Provides

- bundled `PDFsharp` assemblies under `Runtime/Plugins/`
- a tiny text extraction API in the `Sky.PdfSharp` namespace
- a single public entry point: `PdfSharpTextExtractor`

The wrapper API currently provides:
- extracting lightweight text from page content streams
- an exception-based `ExtractText(...)` API
- a failure-safe `TryExtractText(...)` API

The bundled upstream assemblies remain available to consumers through Unity's normal assembly reference flow.

## Package Layout

- `Runtime/Plugins/` - bundled upstream PDFsharp assemblies
- `Runtime/` - wrapper API under the `Sky.PdfSharp` namespace
- `LICENSE` - license for this repository's wrapper/source files
- `THIRD_PARTY_NOTICES.md` - notices for bundled third-party binaries

## Quick Start

Add the package to a Unity project, then call:

```csharp
using Sky.PdfSharp;

string text = PdfSharpTextExtractor.ExtractText("/absolute/path/to/document.pdf");
UnityEngine.Debug.Log(text);
```

If you prefer not to throw on failure, use:

```csharp
using Sky.PdfSharp;

if (PdfSharpTextExtractor.TryExtractText("/absolute/path/to/document.pdf", out string text, out string error))
{
    UnityEngine.Debug.Log(text);
}
else
{
    UnityEngine.Debug.LogError(error);
}
```

## Compatibility Notes

- This package targets Unity `2021.3` or newer.
- The bundled PDFsharp binaries are based on the upstream `netstandard2.0` build of `7.0.0-preview-1`.
- The wrapper in this repository is aimed at lightweight text-reading workflows. Broader PDFsharp usage in Unity should be validated in your target project before shipping.
- This package bundles the additional dependency assemblies required by upstream preview `7.0.0-preview-1`, including `Microsoft.Extensions.Logging.Abstractions`, `System.Security.Cryptography.Pkcs`, `System.Memory`, `System.Buffers`, `System.Numerics.Vectors`, and related support libraries.

## Scope Notes

- This package exposes lightweight content-stream text extraction, not full document reconstruction.
- The package no longer includes the earlier document-inspection wrapper or editor sample.
- Scanned PDFs still require OCR-oriented tooling.
- Text extraction quality depends on the PDF's content streams and may miss layout-heavy or unusual documents.
- If you need full document generation or modification features, refer to the upstream PDFsharp API and documentation.

## Licensing

This repository contains:

- original wrapper/sample code licensed under MIT in `LICENSE`
- bundled PDFsharp and supporting dependency binaries covered by the notices in `THIRD_PARTY_NOTICES.md`
