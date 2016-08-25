using System;
using System.Collections;
using System.Collections.Generic;

namespace ThermoChart_Control
{
    public class DoubleKeyDictionary<TK, T, TV> :
        IEnumerable<DoubleKeyPairValue<TK, T, TV>>,
        IEquatable<DoubleKeyDictionary<TK, T, TV>>
    {
        private Dictionary<T, TV> _mInnerDictionary;

        public DoubleKeyDictionary()
        {
            OuterDictionary = new Dictionary<TK, Dictionary<T, TV>>();
        }

        private Dictionary<TK, Dictionary<T, TV>> OuterDictionary { get; }

        public TV this[TK index1, T index2]
        {
            get { return OuterDictionary[index1][index2]; }
            set { Add(index1, index2, value); }
        }

        #region IEnumerable<DoubleKeyPairValue<K,T,V>> Members

        public IEnumerator<DoubleKeyPairValue<TK, T, TV>> GetEnumerator()
        {
            foreach (var outer in OuterDictionary)
            {
                foreach (var inner in outer.Value)
                {
                    yield return new DoubleKeyPairValue<TK, T, TV>(outer.Key, inner.Key, inner.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<DoubleKeyDictionary<K,T,V>> Members

        public bool Equals(DoubleKeyDictionary<TK, T, TV> other)
        {
            if (OuterDictionary.Keys.Count != other.OuterDictionary.Keys.Count)
                return false;

            bool isEqual = true;

            foreach (var innerItems in OuterDictionary)
            {
                if (!other.OuterDictionary.ContainsKey(innerItems.Key))
                    isEqual = false;

                if (!isEqual)
                    break;

                // here we can be sure that the key is in both lists, 
                // but we need to check the contents of the inner dictionary
                Dictionary<T, TV> otherInnerDictionary = other.OuterDictionary[innerItems.Key];
                foreach (var innerValue in innerItems.Value)
                {
                    if (!otherInnerDictionary.ContainsValue(innerValue.Value))
                        isEqual = false;
                    if (!otherInnerDictionary.ContainsKey(innerValue.Key))
                        isEqual = false;
                }

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        #endregion

        public void Add(TK key1, T key2, TV value)
        {
            if (OuterDictionary.ContainsKey(key1))
            {
                if (_mInnerDictionary.ContainsKey(key2))
                    OuterDictionary[key1][key2] = value;
                else
                {
                    _mInnerDictionary = OuterDictionary[key1];
                    _mInnerDictionary.Add(key2, value);
                    OuterDictionary[key1] = _mInnerDictionary;
                }
            }
            else
            {
                _mInnerDictionary = new Dictionary<T, TV> {[key2] = value};
                OuterDictionary.Add(key1, _mInnerDictionary);
            }
        }
    }

    public class DoubleKeyPairValue<TK, T, TV>
    {
        public DoubleKeyPairValue(TK key1, T key2, TV value)
        {
            Key1 = key1;
            Key2 = key2;
            Value = value;
        }

        public TK Key1 { get; set; }

        public T Key2 { get; set; }

        public TV Value { get; set; }

        public override string ToString()
        {
            return Key1 + " - " + Key2 + " - " + Value;
        }
    }
}