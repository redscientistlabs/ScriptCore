﻿namespace LunarBind
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using MoonSharp.Interpreter;

    internal class BindEnum : BindItem
    {
        internal Type EnumType { get; private set; }
        private List<KeyValuePair<string, int>> enumVals = new List<KeyValuePair<string, int>>();
        public BindEnum(string name, Type e)
        {
            EnumType = e;
            Name = name;
            var fields = e.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                var hidden = (LunarBindHideAttribute)Attribute.GetCustomAttribute(field, typeof(LunarBindHideAttribute)) != null ||
                    (MoonSharpHiddenAttribute)Attribute.GetCustomAttribute(field, typeof(MoonSharpHiddenAttribute)) != null;
                
                if(!hidden)
                {
                    enumVals.Add(new KeyValuePair<string, int>(field.Name, (int)field.GetValue(null)));
                }
            }
        }

        public string[] GetAllEnumPaths(string prefix = "")
        {
            string[] ret = new string[enumVals.Count];
            prefix += Name + ".";
            for (int i = 0; i < enumVals.Count; i++)
            {
                ret[i] = prefix + enumVals[i].Key;
            }
            return ret;
        }

        internal Table CreateEnumTable(Script script)
        {
            Table t = new Table(script);
            foreach (var item in enumVals)
            {
                t[item.Key] = item.Value;
            }
            return t;
        }

        internal override void AddToScript(Script script)
        {
            Table t = new Table(script);

            foreach (var item in enumVals)
            {
                t[item.Key] = item.Value;
            }

            script.Globals[Name] = t;
        }
    }

}
