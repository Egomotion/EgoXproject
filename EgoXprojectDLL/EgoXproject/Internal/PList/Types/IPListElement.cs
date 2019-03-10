//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal interface IPListElement
    {
        XElement Xml();

        IPListElement Copy();
    }

}
