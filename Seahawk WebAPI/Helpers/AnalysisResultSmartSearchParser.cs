using Seahawk_WebAPI.Contracts.AnalysisResults;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Seahawk_WebAPI.Helpers;

public static class AnalysisResultSmartSearchParser
{
    private static readonly string[] StopWords =
    {
        "sample", "sampleno", "sample#",
        "vessel", "ship",
        "port",
        "fuel", "fueltype", "type",
        "comment", "remarks", "note", "notes",
        "from", "to", "between", "and", "in"
    };

    public static void Apply(AnalysisResultQueryRequest request)
    {
        var q = request.FilterAiQuery?.Trim();

        if (string.IsNullOrWhiteSpace(q))
            return;

        q = Regex.Replace(q, @"\s+", " ");

        var fromTo = Regex.Match(
            q,
            @"\bfrom\s+(?<d1>\d{4}-\d{2}-\d{2}|\d{1,2}[/-]\d{1,2}[/-]\d{2,4})\s+to\s+(?<d2>\d{4}-\d{2}-\d{2}|\d{1,2}[/-]\d{1,2}[/-]\d{2,4})\b",
            RegexOptions.IgnoreCase);

        var betweenAnd = Regex.Match(
            q,
            @"\bbetween\s+(?<d1>\d{4}-\d{2}-\d{2}|\d{1,2}[/-]\d{1,2}[/-]\d{2,4})\s+and\s+(?<d2>\d{4}-\d{2}-\d{2}|\d{1,2}[/-]\d{1,2}[/-]\d{2,4})\b",
            RegexOptions.IgnoreCase);

        if (fromTo.Success)
        {
            TrySetDates(request, fromTo.Groups["d1"].Value, fromTo.Groups["d2"].Value);
            q = q.Replace(fromTo.Value, "", StringComparison.OrdinalIgnoreCase).Trim();
        }
        else if (betweenAnd.Success)
        {
            TrySetDates(request, betweenAnd.Groups["d1"].Value, betweenAnd.Groups["d2"].Value);
            q = q.Replace(betweenAnd.Value, "", StringComparison.OrdinalIgnoreCase).Trim();
        }

        var sample = CaptureAfterKeyword(q, new[] { "sample", "sampleno", "sample#" }, singleToken: true);
        if (!string.IsNullOrWhiteSpace(sample))
            request.FilterSampleNumber = sample;

        var vessel = CaptureAfterKeyword(q, new[] { "vessel", "ship" }, singleToken: false);
        if (!string.IsNullOrWhiteSpace(vessel))
            request.FilterVessel = vessel;

        var port = CaptureAfterKeyword(q, new[] { "port" }, singleToken: false);

        if (string.IsNullOrWhiteSpace(port))
            port = CaptureAfterKeyword(q, new[] { "in" }, singleToken: false);

        if (!string.IsNullOrWhiteSpace(port))
            request.FilterPort = port;

        var fuel = CaptureAfterKeyword(q, new[] { "fuel", "fueltype", "type" }, singleToken: false);
        if (!string.IsNullOrWhiteSpace(fuel))
            request.FilterFuelType = fuel;

        var comment = CaptureAfterKeyword(q, new[] { "comment", "remarks", "note", "notes" }, singleToken: false);
        if (!string.IsNullOrWhiteSpace(comment))
            request.FilterComment = comment;

        if (string.IsNullOrWhiteSpace(sample) &&
            string.IsNullOrWhiteSpace(vessel) &&
            string.IsNullOrWhiteSpace(port) &&
            string.IsNullOrWhiteSpace(fuel) &&
            string.IsNullOrWhiteSpace(comment) &&
            q.Length >= 3)
        {
            request.FilterComment = q;
        }
    }

    private static void TrySetDates(AnalysisResultQueryRequest request, string d1, string d2)
    {
        if (TryParseDate(d1, out var from))
            request.FilterBunkerDateFrom = from;

        if (TryParseDate(d2, out var to))
            request.FilterBunkerDateTo = to;
    }

    private static bool TryParseDate(string input, out DateTime date)
    {
        input = input.Trim();

        string[] formats =
        {
            "yyyy-MM-dd",
            "dd-MM-yyyy", "d-M-yyyy",
            "dd/MM/yyyy", "d/M/yyyy",
            "MM/dd/yyyy", "M/d/yyyy"
        };

        if (DateTime.TryParseExact(
                input,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out date))
        {
            return true;
        }

        return DateTime.TryParse(
            input,
            CultureInfo.CurrentCulture,
            DateTimeStyles.AssumeLocal,
            out date);
    }

    private static string? CaptureAfterKeyword(string q, string[] keywords, bool singleToken)
    {
        var keyPattern = string.Join("|", keywords.Select(Regex.Escape));

        if (singleToken)
        {
            var m = Regex.Match(
                q,
                $@"\b({keyPattern})\b\s+(?<v>[^\s]+)",
                RegexOptions.IgnoreCase);

            return m.Success ? m.Groups["v"].Value.Trim() : null;
        }

        var m2 = Regex.Match(
            q,
            $@"\b({keyPattern})\b\s+(?<v>.+)$",
            RegexOptions.IgnoreCase);

        if (!m2.Success)
            return null;

        var value = m2.Groups["v"].Value.Trim();

        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var kept = new List<string>();

        foreach (var p in parts)
        {
            var clean = p.Trim().Trim(',', '.', ';', ':');

            if (StopWords.Contains(clean, StringComparer.OrdinalIgnoreCase))
                break;

            kept.Add(p);
        }

        return kept.Count == 0
            ? null
            : string.Join(" ", kept).Trim().Trim(',', '.', ';', ':');
    }
}