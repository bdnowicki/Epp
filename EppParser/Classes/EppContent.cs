using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EppParser.Classes
{
    public class EppContent : IList<EppField>, IEppElement
    {
        #region IList implemented

        private List<EppField> list = new List<EppField>();
        public EppField this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public int Count
        { get { return list.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        public void Add(EppField item)
        { list.Add(item); }

        public void Clear()
        { list.Clear(); }

        public bool Contains(EppField item)
        { return list.Contains(item); }

        public void CopyTo(EppField[] array, int arrayIndex)
        { list.CopyTo(array, arrayIndex); }

        public IEnumerator<EppField> GetEnumerator()
        { return list.GetEnumerator(); }

        public int IndexOf(EppField item)
        { return list.IndexOf(item); }

        public void Insert(int index, EppField item)
        { list.Insert(index, item); }

        public bool Remove(EppField item)
        { return list.Remove(item); }

        public void RemoveAt(int index)
        { list.RemoveAt(index); }

        IEnumerator IEnumerable.GetEnumerator()
        { return list.GetEnumerator(); }

        #endregion

        public string[] GetStringArray()
        {
            List<string> a = new List<string>();
            foreach (EppField f in this)
                a.Add(f.GetString());
            return a.ToArray();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", (Count > 0) ? this[0].ToString() : "pusty", Count);
        }


        public void Parse(StringReader reader)
        {
            int iChar = 0;
            while (true)
            {
                int peek = reader.Peek();
                bool fieldString = (peek == 34);

                EppField f = new EppField();
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
