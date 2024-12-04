/// <see href="https://github.com/Roger-WIN/pangu.cs/blob/main/src/main/EncodingManager.cs" />

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PanguSpacing {
	internal static class EncodingManager {
		/// <summary>
		/// Check if the file that generated the byte stream is a text file?
		/// </summary>
		internal static bool IsTextFile(byte[] fileBytes) {
			List<byte> fileByteList = fileBytes.ToList(); // Create a byte list using a byte array.
			return !fileByteList.Contains(0); // If byte 0 exists, it is not a text file.
		}

		/// <summary>
		/// Analyze and get the encoding type of the byte stream.
		/// </summary>
		internal static Encoding GetEncodingType(byte[] fileBytes) {
			if (IsUTF8WithoutBom(fileBytes))
				return new UTF8Encoding(); // UTF-8 without BOM
			else if (fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF)
				return Encoding.UTF8; // UTF-8 with BOM
			else if (fileBytes[0] == 0xFE && fileBytes[1] == 0xFF && fileBytes[2] == 0x00)
				return Encoding.BigEndianUnicode; // Unicode (Big Endian)
			else if (fileBytes[0] == 0xFF && fileBytes[1] == 0xFE && fileBytes[2] == 0x41)
				return Encoding.Unicode; // Unicode
			else
				// TODO: .NET Framework and .NET Core will have different implementations for this,
				// and should be used on the basis of ensuring correctness.
				return Encoding.Default;
		}

		/// <summary>
		/// Check if it is UTF-8 without BOM encoding?
		/// </summary>
		/// <exception cref="FormatException">Unexpected byte format!</exception>
		private static bool IsUTF8WithoutBom(byte[] fileBytes) {
			int charByteCounter = 1; // Calculate the remaining number of bytes for the character being analyzed
			byte curByte; // Current analyzing bytes
			foreach (byte fileByte in fileBytes) {
				curByte = fileByte;
				if (charByteCounter == 1) {
					if (curByte >= 0x80) {
						while (((curByte <<= 1) & 0x80) != 0) // Check current byte
							charByteCounter++;
						if (charByteCounter == 1 || charByteCounter > 6)
							// If the first bit of the tag is non-zero, it should start with at least 2 1s, such as: 110XXXXX……1111110X
							return false;
					}
				} else {
					if ((curByte & 0xC0) != 0x80)
						// If it is UTF-8, the first bit must be 1 at this time
						return false;
					charByteCounter--;
				}
			}
			return charByteCounter > 1 ? throw new FormatException("Unexpected byte format") : true;
		}
	}
}
