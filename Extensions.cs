using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PanguSpacing {
	internal static class Extensions {
		/// <inheritdoc cref="Regex.Replace(string, string, string)"/>
		internal static string Replace(this string input, Regex pattern, string replacement) =>
			pattern.Replace(input, replacement);

		/// <inheritdoc cref="Regex.Replace(string, string, MatchEvaluator)"/>
		internal static string Replace(this string input, Regex pattern, MatchEvaluator evaluator) =>
			pattern.Replace(input, evaluator);

		internal static void Deconstruct<T>(this IList<T> list, out T first, out T second, out T third, out IList<T> rest) {
			first = list.Count > 0 ? list[0] : default;
			second = list.Count > 1 ? list[1] : default;
			third = list.Count > 2 ? list[2] : default;
			rest = list.Skip(3).ToList();
		}

		/// <inheritdoc cref="Deconstruct{T}(IList{T}, out T, out T, out T, out IList{T})"/>
		internal static void Deconstruct<T>(this IList<T> list, out T first, out T second, out T third, out T fourth, out IList<T> rest) {
			first = list.Count > 0 ? list[0] : default;
			second = list.Count > 1 ? list[1] : default;
			third = list.Count > 2 ? list[2] : default;
			fourth = list.Count > 3 ? list[3] : default;
			rest = list.Skip(4).ToList();
		}
	}
}
