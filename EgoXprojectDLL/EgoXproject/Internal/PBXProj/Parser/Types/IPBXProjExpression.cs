//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.Collections;

namespace Egomotion.EgoXproject.Internal
{
    internal interface IPBXProjExpression
    {
        string Comment
        {
            get;
            set;
        }

        string ToStringWithComment();
    }
}
