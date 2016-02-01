using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace EppParser.Classes
{
    public class EppContent : IList<EppItem>, IEppElement
    {
        #region IList implemented

        private List<EppItem> list = new List<EppItem>();
        public EppItem this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public int Count
        { get { return list.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public void Add(EppItem item)
        { list.Add(item); }

        public void Clear()
        { list.Clear(); }

        public bool Contains(EppItem item)
        { return list.Contains(item); }

        public void CopyTo(EppItem[] array, int arrayIndex)
        { list.CopyTo(array, arrayIndex); }

        public IEnumerator<EppItem> GetEnumerator()
        { return list.GetEnumerator(); }

        public int IndexOf(EppItem item)
        { return list.IndexOf(item); }

        public void Insert(int index, EppItem item)
        { list.Insert(index, item); }

        public bool Remove(EppItem item)
        { return list.Remove(item); }

        public void RemoveAt(int index)
        { list.RemoveAt(index); }

        IEnumerator IEnumerable.GetEnumerator()
        { return list.GetEnumerator(); }

        #endregion

        public string[] GetStringArray()
        {
            List<string> a = new List<string>();
            foreach (EppItem f in this)
                a.Add(f.GetString());
            return a.ToArray();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", (Count > 0) ? this[0].ToString() : "pusty", Count);
        }

        public void AddString(string value)
        { Add(new EppItem() { StringValue = value }); }

        public void AddInt(int value)
        { Add(new EppItem() { IntValue = value }); }

        public void AddDecimal(decimal value)
        { Add(new EppItem() { DecimalValue = value }); }

        public void AddDateTime(DateTime value)
        { Add(new EppItem() { DateTimeValue = value }); }

        public void AddRangeObjects(object[] values)
        {
            foreach (object o in values)
            {
                Type type = o.GetType();
                EppItem i = new EppItem();
                switch (type.Name)
                {
                    case "String":
                        i.StringValue = (string)o;
                        break;
                    case "Int32":
                        i.IntValue = (int)o;
                        break;
                    case "Decimal":
                        decimal v1 = (decimal)o;
                        i.DecimalValue = Convert.ToDecimal(v1);
                        break;
                    case "Double":
                        double v2 = (double)o;
                        i.DecimalValue = Convert.ToDecimal(v2);
                        break;
                    case "DateTime":
                        i.DateTimeValue = (DateTime)o;
                        break;
                    default:
                        throw new Exception(string.Format("Nieznany typ '{0}'", type.Name));
                }
                Add(i);
                type.ToString();
            }

        }

        public void AddRange(EppItem[] items)
        {
            foreach (EppItem i in items)
                Add(i);
        }


        public void Parse(StringReader reader)
        {
            int iChar = 0;
            while (true)
            {
                int peek = reader.Peek();
                bool fieldString = (peek == 34);

                EppItem f = new EppItem();
                if (peek < 0 | peek == 10 || peek == 13)
                {
                    if (iChar == 44)
                        this.Add(f);
                    break;
                }
                else if (fieldString)
                {
                    while (true)
                    {
                        iChar = reader.Read();

                        if (iChar < 0 || (iChar == 44 && f.StringValue.EndsWith("\"") && !f.StringValue.EndsWith("\\\"")))
                            break;

                        if (iChar == 10 && f.StringValue.EndsWith("\"") && !f.StringValue.EndsWith("\\\""))
                            break;

                        if (iChar > 0)
                            f.StringValue += Char.ConvertFromUtf32(iChar);

                    }
                    f.StringValue = f.StringValue.Substring(1, f.StringValue.Length - 2);
                }
                else
                {
                    while (true)
                    {
                        iChar = reader.Read();

                        if (iChar == 46)
                            f.IsDecimal = true;

                        if (iChar == 44 || iChar < 0)
                            break;

                        if (iChar > 0 & iChar != 44)
                            f.StringValue += Char.ConvertFromUtf32(iChar);

                        if (reader.Peek() == 13 || reader.Peek() == 10)
                            break;
                    }

                    if (f.StringValue.Length == 0)
                    {
                        f.IsInt = false;
                        f.IsDecimal = false;
                    }
                    else if (f.IsDecimal)
                    {
                        f.IsInt = false;
                        f.DecimalPlaces = f.StringValue.Substring(f.StringValue.IndexOf(".") + 1).Length;
                    }
                    else if (f.StringValue.Length == 14)
                    {
                        f.IsDateTime = true;
                    }
                    else
                        f.IsInt = true;
                }

                this.Add(f);
            }

        }

        public string GetString()
        {
            string[] a = GetStringArray();
            return string.Join(",", a);
        }
    }
}
