// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class AssociatedDomainsCapability : BaseCapability
    {
        const string ASSOCIATED_DOMAINS_KEY = "AssociatedDomains";

        public List<string> AssociatedDomains
        {
            get;
            private set;
        }

        public AssociatedDomainsCapability()
        {
            AssociatedDomains = new List<string>();
        }

        public AssociatedDomainsCapability(PListDictionary dic)
        {
            var groups = dic.ArrayValue(ASSOCIATED_DOMAINS_KEY);

            if (groups != null && groups.Count > 0)
            {
                AssociatedDomains = new List<string>(groups.ToStringArray());
            }
            else
            {
                AssociatedDomains = new List<string>();
            }
        }

        public AssociatedDomainsCapability(AssociatedDomainsCapability other)
        : base (other)
        {
            AssociatedDomains = new List<string>(other.AssociatedDomains);
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();

            if (AssociatedDomains.Count > 0)
            {
                dic.Add(ASSOCIATED_DOMAINS_KEY, new PListArray(AssociatedDomains));
            }

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new AssociatedDomainsCapability(this);
        }

        #endregion
    }
}

