using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Based on
// https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Collections/SerializableDictionaryBase.cs

namespace Serialisable
{
    namespace Drawable
    {
        // Unity Property Drawer needs a non Generic class to target.
        // So the Serializable Dictionary inherits this class with nothing in it.
        public abstract class DrawableDictionaryHelper
        {

        }
    }

    [System.Serializable]
    public class SerialisedDictionary<TKey, TValue> : Drawable.DrawableDictionaryHelper, ISerializationCallbackReceiver
    {
        [System.NonSerialized] Dictionary<TKey, TValue> m_dict;

        [SerializeField] List<KeyValue> m_keyValues = new List<KeyValue>();

        [System.Serializable]
        public struct KeyValue
        {
            public TKey key;
            public TValue value;
        }

        public Dictionary<TKey, TValue> GetDictionary()
        {
            return m_dict;
        }

        public void SetDictionary(Dictionary<TKey, TValue> dict)
        {
            m_dict = dict;
        }

        public void Copy(SerialisedDictionary<TKey, TValue> other)
        {
            m_dict = new Dictionary<TKey, TValue>(other.m_dict);
            m_keyValues = new List<KeyValue>(other.m_keyValues);
        }

        public void CopyListToDictionary()
        {
            for (int i = 0; i < m_keyValues.Count; i++)
            {
                m_dict[m_keyValues[i].key] = m_keyValues[i].value;
            }
        }

        public void CopyDictionaryToList()
        {
            KeyValue keyValue = default;
            while (m_keyValues.Count < m_dict.Count)
            {
                m_keyValues.Add(keyValue);
            }

            int i = 0;
            var e = m_dict.GetEnumerator();
            while (e.MoveNext())
            {
                keyValue.key = e.Current.Key;
                keyValue.value = e.Current.Value;
                m_keyValues[i] = keyValue;
                i++;
            }
        }

        #region ISerializationCallbackReceiver
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_dict == null)
            {
                m_dict = new Dictionary<TKey, TValue>(m_keyValues.Count);
            }
            else
            {
                m_dict.Clear();
            }

            CopyListToDictionary();

            m_keyValues.Clear();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_dict == null || m_dict.Count == 0)
            {
                m_keyValues.Clear();
            }
            else
            {
                CopyDictionaryToList();
            }
        }
        #endregion
    }
}
