using System.Collections.Generic;
using System.Text;

/// <summary>
/// Copyright 2005 Bytecode Pty Ltd.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
/// http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </summary>

namespace org.applied_geodesy.util.io.csv
{

	/// <summary>
	/// A very simple CSV parser released under a commercial-friendly license.
	/// This just implements splitting a single line into fields.
	/// 
	/// @author Glen Smith
	/// @author Rainer Pruy
	/// </summary>
	public class CSVParser
	{

		internal readonly char separator;

		internal readonly char quotechar;

		internal readonly char escape;

		internal readonly bool strictQuotes;

		private string pending;
		private bool inField = false;

		internal readonly bool ignoreLeadingWhiteSpace;

		internal readonly bool ignoreQuotations;

		/// <summary>
		/// The default separator to use if none is supplied to the constructor.
		/// </summary>
		public const char DEFAULT_SEPARATOR = ',';

		public const int INITIAL_READ_SIZE = 128;

		/// <summary>
		/// The default quote character to use if none is supplied to the
		/// constructor.
		/// </summary>
		public const char DEFAULT_QUOTE_CHARACTER = '"';


		/// <summary>
		/// The default escape character to use if none is supplied to the
		/// constructor.
		/// </summary>
		public const char DEFAULT_ESCAPE_CHARACTER = '\\';

		/// <summary>
		/// The default strict quote behavior to use if none is supplied to the
		/// constructor
		/// </summary>
		public const bool DEFAULT_STRICT_QUOTES = false;

		/// <summary>
		/// The default leading whitespace behavior to use if none is supplied to the
		/// constructor
		/// </summary>
		public const bool DEFAULT_IGNORE_LEADING_WHITESPACE = true;

		/// <summary>
		/// I.E. if the quote character is set to null then there is no quote character.
		/// </summary>
		public const bool DEFAULT_IGNORE_QUOTATIONS = false;

		/// <summary>
		/// This is the "null" character - if a value is set to this then it is ignored.
		/// </summary>
		public const char NULL_CHARACTER = '\0';

		/// <summary>
		/// Constructs CSVParser using a comma for the separator.
		/// </summary>
		public CSVParser() : this(DEFAULT_SEPARATOR, DEFAULT_QUOTE_CHARACTER, DEFAULT_ESCAPE_CHARACTER)
		{
		}

		/// <summary>
		/// Constructs CSVParser with supplied separator.
		/// </summary>
		/// <param name="separator"> the delimiter to use for separating entries. </param>
		public CSVParser(char separator) : this(separator, DEFAULT_QUOTE_CHARACTER, DEFAULT_ESCAPE_CHARACTER)
		{
		}


		/// <summary>
		/// Constructs CSVParser with supplied separator and quote char.
		/// </summary>
		/// <param name="separator"> the delimiter to use for separating entries </param>
		/// <param name="quotechar"> the character to use for quoted elements </param>
		public CSVParser(char separator, char quotechar) : this(separator, quotechar, DEFAULT_ESCAPE_CHARACTER)
		{
		}

		/// <summary>
		/// Constructs CSVReader with supplied separator and quote char.
		/// </summary>
		/// <param name="separator"> the delimiter to use for separating entries </param>
		/// <param name="quotechar"> the character to use for quoted elements </param>
		/// <param name="escape">    the character to use for escaping a separator or quote </param>
		public CSVParser(char separator, char quotechar, char escape) : this(separator, quotechar, escape, DEFAULT_STRICT_QUOTES)
		{
		}

		/// <summary>
		/// Constructs CSVParser with supplied separator and quote char.
		/// Allows setting the "strict quotes" flag
		/// </summary>
		/// <param name="separator">    the delimiter to use for separating entries </param>
		/// <param name="quotechar">    the character to use for quoted elements </param>
		/// <param name="escape">       the character to use for escaping a separator or quote </param>
		/// <param name="strictQuotes"> if true, characters outside the quotes are ignored </param>
		public CSVParser(char separator, char quotechar, char escape, bool strictQuotes) : this(separator, quotechar, escape, strictQuotes, DEFAULT_IGNORE_LEADING_WHITESPACE)
		{
		}

		/// <summary>
		/// Constructs CSVParser with supplied separator and quote char.
		/// Allows setting the "strict quotes" and "ignore leading whitespace" flags
		/// </summary>
		/// <param name="separator">               the delimiter to use for separating entries </param>
		/// <param name="quotechar">               the character to use for quoted elements </param>
		/// <param name="escape">                  the character to use for escaping a separator or quote </param>
		/// <param name="strictQuotes">            if true, characters outside the quotes are ignored </param>
		/// <param name="ignoreLeadingWhiteSpace"> if true, white space in front of a quote in a field is ignored </param>
		public CSVParser(char separator, char quotechar, char escape, bool strictQuotes, bool ignoreLeadingWhiteSpace) : this(separator, quotechar, escape, strictQuotes, ignoreLeadingWhiteSpace, DEFAULT_IGNORE_QUOTATIONS)
		{
		}

		/// <summary>
		/// Constructs CSVParser with supplied separator and quote char.
		/// Allows setting the "strict quotes" and "ignore leading whitespace" flags
		/// </summary>
		/// <param name="separator">               the delimiter to use for separating entries </param>
		/// <param name="quotechar">               the character to use for quoted elements </param>
		/// <param name="escape">                  the character to use for escaping a separator or quote </param>
		/// <param name="strictQuotes">            if true, characters outside the quotes are ignored </param>
		/// <param name="ignoreLeadingWhiteSpace"> if true, white space in front of a quote in a field is ignored </param>
		public CSVParser(char separator, char quotechar, char escape, bool strictQuotes, bool ignoreLeadingWhiteSpace, bool ignoreQuotations)
		{
			if (anyCharactersAreTheSame(separator, quotechar, escape))
			{
				throw new System.NotSupportedException("The separator, quote, and escape characters must be different!");
			}
			if (separator == NULL_CHARACTER)
			{
				throw new System.NotSupportedException("The separator character must be defined!");
			}
			this.separator = separator;
			this.quotechar = quotechar;
			this.escape = escape;
			this.strictQuotes = strictQuotes;
			this.ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
			this.ignoreQuotations = ignoreQuotations;
		}

		private bool anyCharactersAreTheSame(char separator, char quotechar, char escape)
		{
			return isSameCharacter(separator, quotechar) || isSameCharacter(separator, escape) || isSameCharacter(quotechar, escape);
		}

		private bool isSameCharacter(char c1, char c2)
		{
			return c1 != NULL_CHARACTER && c1 == c2;
		}

		/// <returns> true if something was left over from last call(s) </returns>
		public virtual bool Pending
		{
			get
			{
				return !string.ReferenceEquals(pending, null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String[] parseLineMulti(String nextLine) throws java.io.IOException
		public virtual string[] parseLineMulti(string nextLine)
		{
			return parseLine(nextLine, true);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String[] parseLine(String nextLine) throws java.io.IOException
		public virtual string[] parseLine(string nextLine)
		{
			return parseLine(nextLine, false);
		}

		/// <summary>
		/// Parses an incoming String and returns an array of elements.
		/// </summary>
		/// <param name="nextLine"> the string to parse </param>
		/// <param name="multi"> </param>
		/// <returns> the comma-tokenized list of elements, or null if nextLine is null </returns>
		/// <exception cref="IOException"> if bad things happen during the read </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private String[] parseLine(String nextLine, boolean multi) throws java.io.IOException
		private string[] parseLine(string nextLine, bool multi)
		{

			if (!multi && !string.ReferenceEquals(pending, null))
			{
				pending = null;
			}

			if (string.ReferenceEquals(nextLine, null))
			{
				if (!string.ReferenceEquals(pending, null))
				{
					string s = pending;
					pending = null;
					return new string[]{s};
				}
				else
				{
					return null;
				}
			}

			IList<string> tokensOnThisLine = new List<string>();
			StringBuilder sb = new StringBuilder(INITIAL_READ_SIZE);
			bool inQuotes = false;
			if (!string.ReferenceEquals(pending, null))
			{
				sb.Append(pending);
				pending = null;
				inQuotes = !this.ignoreQuotations; //true;
			}
			for (int i = 0; i < nextLine.Length; i++)
			{

				char c = nextLine[i];
				if (c == this.escape)
				{
					if (isNextCharacterEscapable(nextLine, (inQuotes && !ignoreQuotations) || inField, i))
					{
						sb.Append(nextLine[i + 1]);
						i++;
					}
				}
				else if (c == quotechar)
				{
					if (isNextCharacterEscapedQuote(nextLine, (inQuotes && !ignoreQuotations) || inField, i))
					{
						sb.Append(nextLine[i + 1]);
						i++;
					}
					else
					{
						inQuotes = !inQuotes;

						// the tricky case of an embedded quote in the middle: a,bc"d"ef,g
						if (!strictQuotes)
						{
							if (i > 2 && nextLine[i - 1] != this.separator && nextLine.Length > (i + 1) && nextLine[i + 1] != this.separator)
							{

								if (ignoreLeadingWhiteSpace && sb.Length > 0 && isAllWhiteSpace(sb))
								{
									sb = new StringBuilder(INITIAL_READ_SIZE); //discard white space leading up to quote
								}
								else
								{
									sb.Append(c);
								}

							}
						}
					}
					inField = !inField;
				}
				else if (c == separator && !(inQuotes && !ignoreQuotations))
				{
					tokensOnThisLine.Add(sb.ToString());
					sb = new StringBuilder(INITIAL_READ_SIZE); // start work on next token
					inField = false;
				}
				else
				{
					if (!strictQuotes || (inQuotes && !ignoreQuotations))
					{
						sb.Append(c);
						inField = true;
					}
				}
			}
			// line is done - check status
			if ((inQuotes && !ignoreQuotations))
			{
				if (multi)
				{
					// continuing a quoted section, re-append newline
					sb.Append("\n");
					pending = sb.ToString();
					sb = null; // this partial content is not to be added to field list yet
				}
				else
				{
					throw new IOException("Un-terminated quoted field at end of CSV line");
				}
			}
			if (sb != null)
			{
				tokensOnThisLine.Add(sb.ToString());
			}
			return ((List<string>)tokensOnThisLine).ToArray();

		}

		/// <summary>
		/// precondition: the current character is a quote or an escape
		/// </summary>
		/// <param name="nextLine"> the current line </param>
		/// <param name="inQuotes"> true if the current context is quoted </param>
		/// <param name="i">        current index in line </param>
		/// <returns> true if the following character is a quote </returns>
		private bool isNextCharacterEscapedQuote(string nextLine, bool inQuotes, int i)
		{
			return inQuotes && nextLine.Length > (i + 1) && nextLine[i + 1] == quotechar;
		}

		/// <summary>
		/// precondition: the current character is an escape
		/// </summary>
		/// <param name="nextLine"> the current line </param>
		/// <param name="inQuotes"> true if the current context is quoted </param>
		/// <param name="i">        current index in line </param>
		/// <returns> true if the following character is a quote </returns>
		protected internal virtual bool isNextCharacterEscapable(string nextLine, bool inQuotes, int i)
		{
			return inQuotes && nextLine.Length > (i + 1) && (nextLine[i + 1] == quotechar || nextLine[i + 1] == this.escape);
		}

		/// <summary>
		/// precondition: sb.length() > 0
		/// </summary>
		/// <param name="sb"> A sequence of characters to examine </param>
		/// <returns> true if every character in the sequence is whitespace </returns>
		protected internal virtual bool isAllWhiteSpace(CharSequence sb)
		{
			bool result = true;
			for (int i = 0; i < sb.length(); i++)
			{
				char c = sb.charAt(i);

				if (!char.IsWhiteSpace(c))
				{
					return false;
				}
			}
			return result;
		}
	}

}