using System;
using System.IO;
using System.Text;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

namespace Sky.PdfSharp
{
    public static class PdfSharpTextExtractor
    {
        public static string ExtractText(string pdfPath)
        {
            if (string.IsNullOrWhiteSpace(pdfPath))
                throw new ArgumentException("PDF path is required.", nameof(pdfPath));

            if (!File.Exists(pdfPath))
                throw new FileNotFoundException("PDF file not found.", pdfPath);

            using PdfDocument document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);

            StringBuilder fullText = new StringBuilder();

            for (int i = 0; i < document.PageCount; i++)
            {
                PdfPage page = document.Pages[i];
                string pageText = ExtractPageText(page);

                if (fullText.Length > 0)
                {
                    fullText.AppendLine();
                    fullText.AppendLine();
                }

                fullText.Append(pageText);
            }

            return fullText.ToString();
        }

        public static bool TryExtractText(string pdfPath, out string text, out string error)
        {
            try
            {
                text = ExtractText(pdfPath);
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                text = string.Empty;
                error = ex.Message;
                return false;
            }
        }

        private static string ExtractPageText(PdfPage page)
        {
            CSequence content = ContentReader.ReadContent(page);
            StringBuilder pageText = new StringBuilder();
            AppendSequenceText(content, pageText);
            return NormalizeWhitespace(pageText.ToString());
        }

        private static void AppendSequenceText(CSequence sequence, StringBuilder builder)
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                CObject item = sequence[i];

                if (item is COperator op)
                {
                    AppendOperatorText(op, builder);
                }
                else if (item is CSequence nestedSequence)
                {
                    AppendSequenceText(nestedSequence, builder);
                }
            }
        }

        private static void AppendOperatorText(COperator op, StringBuilder builder)
        {
            switch (op.Name)
            {
                case "Tj":
                case "'":
                case "\"":
                    AppendOperandsText(op.Operands, builder);
                    builder.AppendLine();
                    break;

                case "TJ":
                    AppendOperandsText(op.Operands, builder);
                    builder.AppendLine();
                    break;

                case "TD":
                case "Td":
                case "T*":
                    EnsureLineBreak(builder);
                    break;
            }
        }

        private static void AppendOperandsText(CSequence operands, StringBuilder builder)
        {
            for (int i = 0; i < operands.Count; i++)
            {
                AppendObjectText(operands[i], builder);
            }
        }

        private static void AppendObjectText(CObject obj, StringBuilder builder)
        {
            switch (obj)
            {
                case CString text:
                    builder.Append(text.Value);
                    break;

                case CArray array:
                    for (int i = 0; i < array.Count; i++)
                    {
                        AppendObjectText(array[i], builder);
                    }
                    break;

                case CSequence sequence:
                    AppendOperandsText(sequence, builder);
                    break;
            }
        }

        private static void EnsureLineBreak(StringBuilder builder)
        {
            if (builder.Length == 0)
                return;

            if (builder[builder.Length - 1] != '\n')
                builder.AppendLine();
        }

        private static string NormalizeWhitespace(string text)
        {
            string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            StringBuilder builder = new StringBuilder();
            bool previousLineWasBlank = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                bool isBlank = line.Length == 0;

                if (isBlank)
                {
                    if (!previousLineWasBlank && builder.Length > 0)
                    {
                        builder.AppendLine();
                    }

                    previousLineWasBlank = true;
                    continue;
                }

                if (builder.Length > 0 && !previousLineWasBlank)
                    builder.AppendLine();

                builder.Append(line);
                previousLineWasBlank = false;
            }

            return builder.ToString();
        }
    }
}
