﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IocConfig.UI
{
    public class HtmlAttribute : IHtmlString
    {
        private string _internalValue = String.Empty;
        private string _seperator;

        public string Name { get; set; }
        public string Value { get; set; }
        public bool Condition { get; set; }

        public HtmlAttribute(string name)
            : this(name, null)
        {
        }

        public HtmlAttribute(string name, string seperator)
        {
            Name = name;
            _seperator = seperator ?? " ";
        }

        public HtmlAttribute Add(string value)
        {
            return Add(value, true);
        }

        public HtmlAttribute Add(string value, bool condition)
        {
            if (!String.IsNullOrWhiteSpace(value) && condition)
                _internalValue += value + _seperator;

            return this;
        }

        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(_internalValue))
                _internalValue = String.Format("{0}=\"{1}\"", Name, _internalValue.Substring(0, _internalValue.Length - _seperator.Length));
            return _internalValue;
        }

        public string ToHtmlString()
        {
            return this.ToString();
        }
    }
}
