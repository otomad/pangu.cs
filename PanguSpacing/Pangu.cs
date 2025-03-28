/// <see href="https://github.com/vinta/pangu.js/blob/master/src/shared/core.js">Convert from js to cs.</see>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace PanguSpacing {
	/// <summary>
	/// Automatically insert white spaces between all CJK characters and halfwidth letters,
	/// numbers, and symbols in the text, making the text look beautiful and attractive.
	/// </summary>
	/// <remarks>
	/// Sinologists refer to this space as the "Spacing of Pangu" because it splits the chaos
	/// between fullwidth and halfwidth characters.
	/// </remarks>
	public static class Pangu {
		/// <summary>
		/// CJK is an acronym for Chinese, Japanese, and Korean.
		/// </summary>
		/// <remarks>
		/// <para>CJK includes the following Unicode blocks:</para>
		/// <list type="bullet">
		/// <item><term><c>\u2e80-\u2eff</c></term><description>CJK Radicals Supplement</description></item>
		/// <item><term><c>\u2f00-\u2fdf</c></term><description>Kangxi Radicals</description></item>
		/// <item><term><c>\u3040-\u309f</c></term><description>Hiragana</description></item>
		/// <item><term><c>\u30a0-\u30ff</c></term><description>Katakana</description></item>
		/// <item><term><c>\u3100-\u312f</c></term><description>Bopomofo</description></item>
		/// <item><term><c>\u3200-\u32ff</c></term><description>Enclosed CJK Letters and Months</description></item>
		/// <item><term><c>\u3400-\u4dbf</c></term><description>CJK Unified Ideographs Extension A</description></item>
		/// <item><term><c>\u4e00-\u9fff</c></term><description>CJK Unified Ideographs</description></item>
		/// <item><term><c>\uf900-\ufaff</c></term><description>CJK Compatibility Ideographs</description></item>
		/// </list>
		///
		/// <para>For more information about Unicode blocks, see</para>
		/// <list type="bullet">
		/// <item><see href="http://unicode-table.com/en/" /></item>
		/// <item><see href="https://github.com/vinta/pangu" /></item>
		/// </list>
		///
		/// <para>all J below does not include <c>\u30fb</c></para>
		/// </remarks>
		// language=regex
		private static readonly string CJK = @"\p{IsCJKRadicalsSupplement}\p{IsKangxiRadicals}々-〇〡-〩〸-〺\p{IsHiragana}ァ-ヺー-ヿ\p{IsBopomofo}\p{IsKanbun}\p{IsBopomofoExtended}㇀-\u31ee\p{IsKatakanaPhoneticExtensions}\p{IsEnclosedCJKLettersandMonths}\p{IsCJKCompatibility}\p{IsCJKUnifiedIdeographsExtensionA}\p{IsCJKUnifiedIdeographs}\p{IsCJKCompatibilityIdeographs}";
		// Old: "\u2e80-\u2eff\u2f00-\u2fdf\u3040-\u309f\u30a0-\u30fa\u30fc-\u30ff\u3100-\u312f\u3200-\u32ff\u3400-\u4dbf\u4e00-\u9fff\uf900-\ufaff";
		// .NET Regex missing Unicode Block "IsCJKStrokes", this is because this block was released in Unicode 4.1, but .NET supports blocks based on Unicode 4.0.

		/// <summary>
		/// Math operators or math symbols, but except the midline horizontal ellipsis \u22ef.
		/// </summary>
		// language=regex
		private static readonly string SM = @"(?!⋯)\p{Sm}";

		/// <summary>
		/// Any variation selectors or nothing, but except variation selector 2.
		/// </summary>
		// language=regex
		private static readonly string NVS2 = @"(?!\ufe01)\p{IsVariationSelectors}?";

		private static readonly Regex
			// ANS is short for Alphabets, Numbers, and Symbols.
			//
			// A includes A-Za-z\u0370-\u03ff
			// N includes 0-9
			// S includes `~!@#$%^&*()-_=+[]{}\|;:'",<.>/?
			//
			// some S below does not include all symbols
			ANY_CJK = new Regex($@"[{CJK}]"),

			// the symbol part only includes ~ ! ; : , . ? but . only matches one character
			CONVERT_TO_FULLWIDTH_CJK_SYMBOLS_CJK = new Regex($@"([{CJK}])[ ]*([:]+|\.)[ ]*([{CJK}])"),
			CONVERT_TO_FULLWIDTH_CJK_SYMBOLS = new Regex($@"([{CJK}])[ ]*([~!;,\?]+)[ ]*"),
			DOTS_CJK = new Regex($@"([\.]{{2,}}|…)([{CJK}])"),
			FIX_CJK_COLON_ANS = new Regex($@"([{CJK}]):([A-Z0-9\(\)])"),

			// the symbol part does not include '
			CJK_QUOTE = new Regex($@"([{CJK}])([`""״])"),
			QUOTE_CJK = new Regex($@"([`""״])([{CJK}])"),
			FIX_QUOTE_ANY_QUOTE = new Regex($@"([`""״]+)[ ]* (.+?)[ ]* ([`""״]+)"),

			CJK_SINGLE_QUOTE_BUT_POSSESSIVE = new Regex($@"([{CJK}])('[^s])"),
			SINGLE_QUOTE_CJK = new Regex($@"(')([{CJK}])"),
			FIX_POSSESSIVE_SINGLE_QUOTE = new Regex($@"([A-Za-z0-9{CJK}])( )('s)"),

			HASH_ANS_CJK_HASH = new Regex($@"([{CJK}])(#)([{CJK}]+)(#)([{CJK}])"),
			CJK_HASH = new Regex($@"([{CJK}])(#([^ ]))"),
			HASH_CJK = new Regex($@"(([^ ])#)([{CJK}])"),

			// the symbol part only includes + - * / = & | < >
			CJK_OPERATOR_ANS = new Regex($@"([{CJK}])([\+\-\*\/=&\|<>]|{SM})([A-Za-z0-9])"),
			ANS_OPERATOR_CJK = new Regex($@"([A-Za-z0-9])([\+\-\*\/=&\|<>]|{SM})([{CJK}])"),

			FIX_SLASH_AS = new Regex(@"([/]) ([a-z\-_\./]+)"),
			FIX_SLASH_AS_SLASH = new Regex(@"([/\.])([A-Za-z\-_\./]+) ([/])"),

			// the bracket part only includes ( ) [ ] { } < > “ ” ‘ ’
			// except fullwidth quotes
			CJK_LEFT_BRACKET = new Regex($@"([{CJK}])([\(\[\{{<>]|[“‘]{NVS2})"),
			RIGHT_BRACKET_CJK = new Regex($@"([\)\]\}}<>]|[”’]{NVS2})([{CJK}])"),
			FIX_LEFT_BRACKET_ANY_RIGHT_BRACKET = new Regex($@"((?:[\(\[\{{<]|[“‘]{NVS2})+)[ ]*(.+?)[ ]*((?:[\)\]\}}>]|[”’]{NVS2})+)"),
			ANS_CJK_LEFT_BRACKET_ANY_RIGHT_BRACKET = new Regex($@"([A-Za-z0-9{CJK}])[ ]*([“‘]{NVS2})([A-Za-z0-9{CJK}\-_ ]+)([”’]{NVS2})"),
			LEFT_BRACKET_ANY_RIGHT_BRACKET_ANS_CJK = new Regex($@"([“‘]{NVS2})([A-Za-z0-9{CJK}\-_ ]+)([”’]{NVS2})[ ]*([A-Za-z0-9{CJK}])"),

			AN_LEFT_BRACKET = new Regex(@"([A-Za-z0-9])([\(\[\{])"),
			RIGHT_BRACKET_AN = new Regex(@"([\)\]\}])([A-Za-z0-9])"),

			CJK_ANS = new Regex($@"([{CJK}])([A-Za-zͰ-Ͽ0-9@\$%\^&\*\-\+\\=\|/¡-ÿ⅐-\u218f✀—➿]|{SM})"),
			ANS_CJK = new Regex($@"([A-Za-zͰ-Ͽ0-9~\$%\^&\*\-\+\\=\|/!;:,\.\?¡-ÿ⅐-\u218f✀—➿]|{SM})([{CJK}])"),

			S_A = new Regex(@"(%)([A-Za-z])"),

			MIDDLE_DOT = new Regex(@"([ ]*)([·•‧])([ ]*)");

		/// <summary>
		/// Unicode <b>Punctuation Space</b>
		/// </summary>
		/// <remarks>
		/// Space equal to narrow punctuation of a front.
		/// </remarks>
		public const char PUNCSP = '\u2008';

		/// <remarks>
		/// <para>By default, a punctuation space character (U+2008) is used that is narrower than THE SPACE character (U+0020) itself,
		/// to avoid making the space look too wide.</para>
		/// <para>If you don't like it, you can manually restore it to THE SPACE character itself, like this:</para>
		/// <code>
		/// Pangu.puncsp = " ";
		/// </code>
		/// </remarks>
		[SuppressMessage("Style", "IDE1006")]
		public static char puncsp { get; set; } = PUNCSP;

		private static string ConvertToFullwidth(string symbols) => symbols
			.Replace('~', '～')
			.Replace('!', '！')
			.Replace(';', '；')
			.Replace(':', '：')
			.Replace(',', '，')
			.Replace('.', '。')
			.Replace('?', '？');

		/// <summary>
		/// Spacing the text.
		/// </summary>
		public static string Spacing(string text) {
			if (text.Length <= 1 || !ANY_CJK.IsMatch(text))
				return text;

			string newText = text;

			newText = newText.Replace(CONVERT_TO_FULLWIDTH_CJK_SYMBOLS_CJK, match => {
				string leftCjk = match.Groups[1].Value,
					symbols = match.Groups[2].Value,
					rightCjk = match.Groups[3].Value;
				string fullwidthSymbols = ConvertToFullwidth(symbols);
				return $"{leftCjk}{fullwidthSymbols}{rightCjk}";
			});

			newText = newText.Replace(CONVERT_TO_FULLWIDTH_CJK_SYMBOLS, match => {
				string cjk = match.Groups[1].Value,
					symbols = match.Groups[2].Value;
				string fullwidthSymbols = ConvertToFullwidth(symbols);
				return $"{cjk}{fullwidthSymbols}";
			});

			newText = newText.Replace(DOTS_CJK, $"$1{puncsp}$2");
			newText = newText.Replace(FIX_CJK_COLON_ANS, "$1：$2");

			newText = newText.Replace(CJK_QUOTE, $"$1{puncsp}$2");
			newText = newText.Replace(QUOTE_CJK, $"$1{puncsp}$2");
			newText = newText.Replace(FIX_QUOTE_ANY_QUOTE, "$1$2$3");

			newText = newText.Replace(CJK_SINGLE_QUOTE_BUT_POSSESSIVE, $"$1{puncsp}$2");
			newText = newText.Replace(SINGLE_QUOTE_CJK, $"$1{puncsp}$2");
			newText = newText.Replace(FIX_POSSESSIVE_SINGLE_QUOTE, "$1's");

			newText = newText.Replace(HASH_ANS_CJK_HASH, $"$1{puncsp}$2$3$4{puncsp}$5");
			newText = newText.Replace(CJK_HASH, $"$1{puncsp}$2");
			newText = newText.Replace(HASH_CJK, $"$1{puncsp}$3");

			newText = newText.Replace(CJK_OPERATOR_ANS, $"$1{puncsp}$2{puncsp}$3");
			newText = newText.Replace(ANS_OPERATOR_CJK, $"$1{puncsp}$2{puncsp}$3");

			newText = newText.Replace(FIX_SLASH_AS, "$1$2");
			newText = newText.Replace(FIX_SLASH_AS_SLASH, "$1$2$3");

			newText = newText.Replace(CJK_LEFT_BRACKET, $"$1{puncsp}$2");
			newText = newText.Replace(RIGHT_BRACKET_CJK, $"$1{puncsp}$2");
			newText = newText.Replace(FIX_LEFT_BRACKET_ANY_RIGHT_BRACKET, "$1$2$3");
			newText = newText.Replace(ANS_CJK_LEFT_BRACKET_ANY_RIGHT_BRACKET, $"$1{puncsp}$2$3$4");
			newText = newText.Replace(LEFT_BRACKET_ANY_RIGHT_BRACKET_ANS_CJK, $"$1$2$3{puncsp}$4");

			newText = newText.Replace(AN_LEFT_BRACKET, "$1 $2");
			newText = newText.Replace(RIGHT_BRACKET_AN, "$1 $2");

			newText = newText.Replace(CJK_ANS, $"$1{puncsp}$2");
			newText = newText.Replace(ANS_CJK, $"$1{puncsp}$2");

			newText = newText.Replace(S_A, "$1 $2");

			newText = newText.Replace(MIDDLE_DOT, "・");

			return newText;
		}

		/// <summary>
		/// Spacing the byte stream.
		/// </summary>
		/// <exception cref="FormatException">Throws if the file is not a text file.</exception>
		public static byte[] Spacing(byte[] bytes) {
			if (!EncodingManager.IsTextFile(bytes)) // The file that generated the byte stream is not a text file.
				throw new FormatException("This file is not a text file and cannot be converted");
			Encoding encoding = EncodingManager.GetEncodingType(bytes); // Get the encoding of the byte stream.
			string originalText = encoding.GetString(bytes); // Convert byte stream to string.
			string targetText = Spacing(originalText); // Add Pangu to the string.
			return encoding.GetBytes(targetText); // Convert the converted string back into a byte stream.
		}
	}
}
