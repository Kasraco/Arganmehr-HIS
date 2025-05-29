using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources.ManageResource
{
    public class ResXResourceEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public ResXResourceEntry() { }
        public ResXResourceEntry(object key, object value)
        {
            Key = key.ToString();
            Value = value.ToString();
        }
        public ResXResourceEntry(DictionaryEntry dictionaryEntry)
        {
            Key = dictionaryEntry.Key.ToString();
            Value = dictionaryEntry.Value != null ? dictionaryEntry.Value.ToString() : string.Empty;
        }
        public DictionaryEntry ToDictionaryEntry()
        {
            return new DictionaryEntry(Key, Value);
        }
    }
}
