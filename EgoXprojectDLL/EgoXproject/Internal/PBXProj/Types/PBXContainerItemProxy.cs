// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXContainerItemProxy : PBXBaseObject
    {
        const string _containerPortalKey = "containerPortal";
        const string _proxyTypeKey = "proxyType";
        const string _remoteGlobalIDStringKey = "remoteGlobalIDString";
        const string _remoteInfoKey = "remoteInfo";

        public PBXContainerItemProxy(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXContainerItemProxy, uid, dict)
        {
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
        }

        #endregion

    }
}

