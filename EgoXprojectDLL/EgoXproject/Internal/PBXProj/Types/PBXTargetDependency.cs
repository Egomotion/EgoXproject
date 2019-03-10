// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXTargetDependency : PBXBaseObject
    {
        const string TARGET_KEY = "target";
        const string TARGET_PROXY_KEY = "targetProxy";

        public PBXTargetDependency(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXTargetDependency, uid, dict)
        {
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            Target = PopulateObject<PBXNativeTarget>(TargetID, allObjects);
            TargetProxy = PopulateObject<PBXContainerItemProxy>(TargetProxyID, allObjects);
        }

        #endregion


        public string TargetID
        {
            get
            {
                return Dict.StringValue(TARGET_KEY);
            }
        }

        public string TargetProxyID
        {
            get
            {
                return Dict.StringValue(TARGET_PROXY_KEY);
            }
        }

        public PBXNativeTarget Target
        {
            get;
            private set;
        }

        public PBXContainerItemProxy TargetProxy
        {
            get;
            private set;
        }
    }
}

