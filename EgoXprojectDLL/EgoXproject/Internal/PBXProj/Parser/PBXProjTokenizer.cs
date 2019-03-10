//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.Collections;
using System.IO;
using System.Text;
using System;

namespace Egomotion.EgoXproject.Internal
{
    /// <summary>Converts a sequence of characters to a sequence of tokens.</summary>
    internal class PBXProjTokenizer
    {
        static readonly string _validWordChars = "./-_";

        TextReader _source;
        // the source to read characters from
        char _currentChar;
        // the current character
        char _previousChar;
        StringBuilder _tokenValueBuffer;
        // a buffer for building the value of a token

        /// <summary>Initializes a new instance of the <see cref="PBXProjTokenizer"/> class.</summary>
        /// <param name="source">The source <see cref="TextReader"/> to read the characters from.</param>
        public PBXProjTokenizer(TextReader source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Must have a TextReader");
            }

            _source = source;
            _tokenValueBuffer = new StringBuilder();
            // read the first character
            ReadNextChar();
        }

        /// <summary>Reads the next character.</summary>
        /// <remarks>After calling this method, _currentChar will contain the read character.</remarks>
        void ReadNextChar()
        {
            int nChar = _source.Read();
            _previousChar = _currentChar;

            if (nChar > 0)
            {
                _currentChar = (char) nChar;
            }
            else
            {
                _currentChar = '\0';
            }
        }

        /// <summary>Skips the white-space characters.</summary>
        void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_currentChar))
            {
                ReadNextChar();
            }
        }

        /// <summary>Gets a value indicating whether the tokenizer is at the end of the source.</summary>
        /// <value><c>true</c> if the tokenizer is at the end of the source; otherwise, <c>false</c>.</value>
        bool AtEndOfSource
        {
            get
            {
                return _currentChar == '\0';
            }
        }

        /// <summary>Appends the current character to the token value buffer and reads the next character.</summary>
        void StoreCurrentCharAndReadNext()
        {
            _tokenValueBuffer.Append(_currentChar);
            ReadNextChar();
        }

        /// <summary>Extracts the value from the token value buffer and clears the buffer.</summary>
        /// <returns>The value from the token value buffer.</returns>
        string ExtractStoredChars()
        {
            string value = _tokenValueBuffer.ToString();
            _tokenValueBuffer.Length = 0;
            return value;
        }

        /// <summary>Checks for an unexpected end of the source.</summary>
        /// <exception cref="PBXProjParserException">The end of the source has been reached unexpectedly.</exception>
        void CheckForUnexpectedEndOfSource()
        {
            if (AtEndOfSource)
            {
                throw new PBXProjParserException("Unexpected end of source.");
            }
        }

        /// <summary>Throws a <see cref="PBXProjParserException"/> because the current character is invalid.</summary>
        /// <exception cref="PBXProjParserException">The current character is invalid.</exception>
        void CheckForInvalidCharacter()
        {
            if (_tokenValueBuffer.Length == 0)
            {
                throw new PBXProjParserException("Invalid character '" + _currentChar.ToString() + "'.");
            }
            else
            {
                throw new PBXProjParserException("Invalid character '"
                                                 + _currentChar.ToString() + "' after '"
                                                 + _tokenValueBuffer + "'.");
            }
        }

        /// <summary>Reads the next <see cref="PBXProjToken"/>.</summary>
        /// <returns>The next <see cref="PBXProjToken"/>, or <c>null</c> if the end of the source has been reached.</returns>
        /// <exception cref="PBXProjParserException">The source contains invalid characters.</exception>
        public PBXProjToken ReadNextToken()
        {
            SkipWhitespace();

            if (AtEndOfSource)
            {
                return null;
            }

            // if the first character is a letter, the token is a word
            if (char.IsLetterOrDigit(_currentChar))
            {
                return ReadWord();
            }

            // if the first character is a quote, the token is a quoted string
            if (_currentChar == '"')
            {
                return ReadQuotedString();
            }

            // in all other cases, the token should be a symbol
            return ReadSymbol();
        }

        bool IsVaildWordChar(char c)
        {
            return (char.IsLetterOrDigit(c) ||
                    _validWordChars.Contains(c.ToString()));
        }

        PBXProjToken ReadWord()
        {
            do
            {
                StoreCurrentCharAndReadNext();
            }
            while (IsVaildWordChar(_currentChar));

            string chars = ExtractStoredChars();
            return new PBXProjToken(PBXProjTokenType.String, chars);
        }

        PBXProjToken ReadQuotedString()
        {
            do
            {
                StoreCurrentCharAndReadNext();
            }
            while (!AtEndOfSource && !(_currentChar == '"' && _previousChar != '\\'));

            StoreCurrentCharAndReadNext(); //store the final "
            CheckForUnexpectedEndOfSource();
            return new PBXProjToken(PBXProjTokenType.String, ExtractStoredChars());
        }

        PBXProjToken ReadSymbol()
        {
            //          UnityEngine.Debug.Log (_currentChar);
            switch (_currentChar)
            {
            case '{':
            case '}':
            case ';':
            case '=':
            case '(':
            case ')':
            case ',':
                StoreCurrentCharAndReadNext();
                return new PBXProjToken(PBXProjTokenType.Symbol, ExtractStoredChars());

            // the symbols // /* (and */)
            case '/':
                StoreCurrentCharAndReadNext();

                if (_currentChar == '/')
                {
                    return ReadCPPComment();
                }
                else if (_currentChar == '*')
                {
                    return ReadCComment();
                }
                else if (IsVaildWordChar(_currentChar))
                {
                    return ReadWord();
                }

                CheckForUnexpectedEndOfSource();
                CheckForInvalidCharacter();
                break;

            case '.':
            case '-':
            case '_':
                return ReadWord();

            default:
                CheckForUnexpectedEndOfSource();
                CheckForInvalidCharacter();
                break;
            }

            return null;
        }

        PBXProjToken ReadCPPComment()
        {
            while (!AtEndOfSource && !(_currentChar == '\n' || _currentChar == '\r'))
            {
                StoreCurrentCharAndReadNext();
            }

            CheckForUnexpectedEndOfSource();
            return new PBXProjToken(PBXProjTokenType.Comment, ExtractStoredChars());
        }

        PBXProjToken ReadCComment()
        {
            while (!AtEndOfSource && !(_previousChar == '*' && _currentChar == '/'))
            {
                StoreCurrentCharAndReadNext();
            }

            StoreCurrentCharAndReadNext();
            CheckForUnexpectedEndOfSource();
            return new PBXProjToken(PBXProjTokenType.Comment, ExtractStoredChars());
        }

    }
}