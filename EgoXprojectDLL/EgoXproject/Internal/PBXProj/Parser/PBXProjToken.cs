//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.Collections;
using System;

namespace Egomotion.EgoXproject.Internal
{
    internal enum PBXProjTokenType
    {
        None,
        String,
        Symbol,
        Comment
    }

    ;

    internal class PBXProjToken
    {
        readonly PBXProjTokenType _type;
        readonly string _value;

        /// <summary>Initializes a new instance of the <see cref="PBXProjTokenType"/> class.</summary>
        /// <param name="type">The type of the token.</param>
        /// <param name="value">The value of the token.</param>
        public PBXProjToken(PBXProjTokenType type, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value), "Value must not be null or empty");
            }

            _type = type;
            _value = value;
        }

        /// <summary>Gets the type of this token.</summary>
        /// <value>The type of this token.</value>
        public PBXProjTokenType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>Gets the value of this token.</summary>
        /// <value>The value of this token.</value>
        public string Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>Determines whether this token has the specified type and value.</summary>
        /// <param name="type">The type to check against.</param>
        /// <param name="value">The value to check against.</param>
        /// <returns>If this token has the specified type and value, <c>true</c>; otherwise, <c>false</c>.</returns>
        public bool Equals(PBXProjTokenType type, string value)
        {
            return _type == type && _value == value;
        }
    }
}