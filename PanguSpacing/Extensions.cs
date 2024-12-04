using System.Text.RegularExpressions;

namespace PanguSpacing {
	internal static class Extensions {
		/// <inheritdoc cref="Regex.Replace(string, string, string)"/>
		internal static string Replace(this string input, Regex pattern, string replacement) =>
			pattern.Replace(input, replacement);

		/// <inheritdoc cref="Regex.Replace(string, string, MatchEvaluator)"/>
		internal static string Replace(this string input, Regex pattern, MatchEvaluator evaluator) =>
			pattern.Replace(input, evaluator);
	}
}
