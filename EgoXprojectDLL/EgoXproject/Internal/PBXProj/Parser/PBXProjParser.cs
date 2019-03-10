//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProjParser
    {
        public static string header = "// !$*UTF8*$!";

        PBXProjTokenizer _tokenizer;
        PBXProjToken _currentToken;

        public PBXProjDictionary Parse(TextReader source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof (source), "Must provide a valid TextReader source");
            }

            _currentToken = null;
            _tokenizer = new PBXProjTokenizer(source);
            // read the first token
            ReadNextToken();

            //A poor file type check
            if (AtEndOfSource || _currentToken.Type != PBXProjTokenType.Comment && _currentToken.Value != header)
            {
                throw new PBXProjParserException("Text source is not a valid pbxproj file");
            }

            ReadNextToken();
            return ParseDictionary();
        }


        public PBXProjDictionary Parse(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return null;
            }

            using (TextReader reader = File.OpenText(fileName))
            {
                return Parse(reader);
            }
        }

        void ReadNextToken()
        {
            _currentToken = _tokenizer.ReadNextToken();
        }

        bool AtEndOfSource
        {
            get
            {
                return _currentToken == null;
            }
        }

        void CheckForUnexpectedEndOfSource()
        {
            if (AtEndOfSource)
            {
                throw new PBXProjParserException("Unexpected end of source.");
            }
        }

        /// <summary>Skips the current token if it is the specified token, or throws a <see cref="PBXProjParserException"/>.</summary>
        /// <param name="type">The type of token to expect.</param>
        /// <param name="value">The value of the token to expect.</param>
        void SkipExpected(PBXProjTokenType type, string value)
        {
            CheckForUnexpectedEndOfSource();

            if (!_currentToken.Equals(type, value))
            {
                throw new PBXProjParserException("Expected '" + value + "'. Not " + _currentToken.Value);
            }

            ReadNextToken();
        }

        void SkipComments()
        {
            while (_currentToken != null && _currentToken.Type == PBXProjTokenType.Comment)
            {
                ReadNextToken();
            }
        }

        IPBXProjExpression ParseExpression()
        {
            // int, string, bool, quoted string, dictionary or array
            IPBXProjExpression expression = null;

            if (_currentToken.Type == PBXProjTokenType.Symbol)
            {
                if (_currentToken.Value == "{")
                {
                    expression = ParseDictionary();
                }
                else if (_currentToken.Value == "(")
                {
                    expression = ParseArray();
                }
            }
            else
            {
                expression = ParseBaseExpression();
            }

            if (_currentToken != null && _currentToken.Type == PBXProjTokenType.Comment)
            {
                expression.Comment = _currentToken.Value;
                ReadNextToken();
            }

            return expression;
        }

        PBXProjDictionary ParseDictionary()
        {
            // {assignment assignment assignment};
            // { variable = expression; variable = expression; variable = expression;};
            // _current = {
            PBXProjDictionary dic = new PBXProjDictionary();
            ReadNextToken();

            while (!(_currentToken.Type == PBXProjTokenType.Symbol && _currentToken.Value == "}"))      //doing !symbol && !} means that other symbols break the loop.
            {
                // assignment:
                // variable = expression;
                // comment variable comment = expresssion comment; comment
                string preComment = "", keyComment = "", postComment = "";

                if (_currentToken != null && _currentToken.Type == PBXProjTokenType.Comment)
                {
                    preComment = _currentToken.Value;
                    ReadNextToken();
                }

                if (_currentToken.Type != PBXProjTokenType.String)
                {
                    throw new PBXProjParserException("Expected a variable name, but got " + _currentToken.Value);
                }

                string key = _currentToken.Value;
                //              Debug.Log(key);
                ReadNextToken();

                if (_currentToken != null && _currentToken.Type == PBXProjTokenType.Comment)
                {
                    keyComment = _currentToken.Value;
                    ReadNextToken();
                }

                CheckForUnexpectedEndOfSource();
                SkipExpected(PBXProjTokenType.Symbol, "=");
                IPBXProjExpression exp = ParseExpression();

                if (exp == null)
                {
                    throw new PBXProjParserException("Expected an expression to be assigned to " + key);
                }

                SkipExpected(PBXProjTokenType.Symbol, ";");

                if (_currentToken != null && _currentToken.Type == PBXProjTokenType.Comment)
                {
                    postComment = _currentToken.Value;
                    ReadNextToken();
                }

                CheckForUnexpectedEndOfSource();
                dic.Add(key, exp);
                dic.SetPreCommentForKey(key, preComment);
                dic.SetCommentForKey(key, keyComment);
                dic.SetPostCommentForKey(key, postComment);
            }

            ReadNextToken();
            return dic;
        }

        PBXProjArray ParseArray()
        {
            // (expression,expression)
            // _current = (
            PBXProjArray array = new PBXProjArray();
            ReadNextToken(); // skip '('

            while (!(_currentToken.Type == PBXProjTokenType.Symbol && _currentToken.Value == ")"))
            {
                IPBXProjExpression expression = ParseExpression();
                SkipExpected(PBXProjTokenType.Symbol, ",");
                array.Add(expression);
            }

            ReadNextToken();
            return array;
        }


        IPBXProjExpression ParseBaseExpression()
        {
            CheckForUnexpectedEndOfSource();

            if (_currentToken.Type != PBXProjTokenType.String)
            {
                throw new PBXProjParserException("Expected a string value, not " + _currentToken.Value);
            }

            string value = _currentToken.Value;
            //is it a bool
            PBXProjBoolean boolExpression = ParseBoolean(value);

            if (boolExpression != null)
            {
                ReadNextToken();
                return boolExpression;
            }

            PBXProjString stringExpression = ParseString(value);

            if (stringExpression != null)
            {
                ReadNextToken();
                return stringExpression;
            }

            return null;
        }

        PBXProjString ParseString(string value)
        {
            return new PBXProjString(value);
        }

        PBXProjBoolean ParseBoolean(string value)
        {
            if (value == XcodeBool.YES || value == XcodeBool.NO)
            {
                return new PBXProjBoolean(value == XcodeBool.YES);
            }

            return null;
        }
    }
}