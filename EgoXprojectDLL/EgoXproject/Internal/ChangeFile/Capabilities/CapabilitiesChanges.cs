// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class CapabilitiesChanges : BaseChangeGroup
    {
        Dictionary<SystemCapability, BaseCapability> _capabilities = new Dictionary<SystemCapability, BaseCapability>();

        public CapabilitiesChanges()
        {
        }

        public CapabilitiesChanges(PListDictionary dic)
        {
            if (dic == null)
            {
                return;
            }

            foreach (var kvp in dic)
            {
                SystemCapability key;

                try
                {
                    key = (SystemCapability)System.Enum.Parse(typeof(SystemCapability), kvp.Key);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("EgoXproject: Invalid entry in Capabilities section. Skipping. " + e.Message);
                    continue;
                }

                var capDic = kvp.Value as PListDictionary;

                if (capDic == null)
                {
                    Debug.LogError("EgoXproject: Invalid entry in Capabilities section. Skipping. ");
                    continue;
                }

                _capabilities[key] = SystemCapabilityHelper.Create(key, capDic);
            }
        }

        #region implemented abstract members of BaseChangeGroup

        public override IPListElement Serialize()
        {
            var dic = new PListDictionary();

            foreach (var kvp in _capabilities)
            {
                dic[kvp.Key.ToString()] = kvp.Value.Serialize();
            }

            return dic;
        }

        public override bool HasChanges()
        {
            return _capabilities.Count > 0;
        }

        public override void Clear()
        {
            _capabilities.Clear();
        }

        #endregion

        public void Merge(CapabilitiesChanges other)
        {
            foreach (var kvp in other._capabilities)
            {
                _capabilities[kvp.Key] = kvp.Value;
            }
        }

        public BaseCapability Capability(SystemCapability systemCapability)
        {
            BaseCapability capability;

            if (_capabilities.TryGetValue(systemCapability, out capability))
            {
                return capability;
            }

            return null;
        }

        public SystemCapability[] ActiveCapabilities()
        {
            return _capabilities.Keys.ToArray();
        }

        public bool IsCapabilityEnabled(SystemCapability capability)
        {
            return _capabilities.ContainsKey(capability);
        }

        public void EnableCapability(SystemCapability capability, bool enabled)
        {
            if (enabled)
            {
                if (!IsCapabilityEnabled(capability))
                {
                    _capabilities[capability] = SystemCapabilityHelper.Create(capability);
                }
            }
            else
            {
                _capabilities.Remove(capability);
            }
        }
    }
}
