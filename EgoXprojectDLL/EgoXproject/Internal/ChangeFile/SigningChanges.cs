// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class SigningChanges : BaseChangeGroup
    {
        const string TEAM_ID_KEY = "TeamId";
        const string AUTO_PROVISIONING_KEY = "AutomaticProvisioning";

        public SigningChanges()
        {
            AutomaticProvisioning = true;
        }

        public SigningChanges(PListDictionary dic)
        {
            if (dic == null)
            {
                return;
            }

            TeamId = dic.StringValue(TEAM_ID_KEY);

            if (dic.ContainsKey(AUTO_PROVISIONING_KEY))
            {
                AutomaticProvisioning = dic.BoolValue(AUTO_PROVISIONING_KEY);
            }
            else
            {
                AutomaticProvisioning = true;
            }
        }

        public string TeamId
        {
            get;
            set;
        }

        public bool AutomaticProvisioning
        {
            get;
            set;
        }

        #region implemented abstract members of BaseChangeGroup

        public override IPListElement Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfNotEmpty(TEAM_ID_KEY, TeamId);

            if (!AutomaticProvisioning)
            {
                dic.Add(AUTO_PROVISIONING_KEY, AutomaticProvisioning);
            }

            return dic;
        }

        public override bool HasChanges()
        {
            return !string.IsNullOrEmpty(TeamId) || !AutomaticProvisioning;
        }

        public override void Clear()
        {
            TeamId = "";
            AutomaticProvisioning = true;
        }

        #endregion

        public void Merge(SigningChanges other)
        {
            if (!string.IsNullOrEmpty(other.TeamId))
            {
                TeamId = other.TeamId;
            }

            AutomaticProvisioning = other.AutomaticProvisioning;
        }
    }
}
