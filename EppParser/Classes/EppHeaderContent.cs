using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EppParser.Classes
{
   public class EppHeaderContent : EppHeader, IList<EppContent>
    {

        public EppHeaderContent() : base()
        {
            Name = "ZAWARTOSC";
        }

        private List<EppContent> list = new List<EppContent>();
        public EppContent this[int index]
        {
            get
            {
                return list[index];
            }

            set
            {
                list[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(EppContent item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(EppContent item)
        {
            return list.Contains(item);
        }

        public void CopyTo(EppContent[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<EppContent> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(EppContent item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, EppContent item)
        {
            list.Insert(index, item);
        }

        public bool Remove(EppContent item)
        {
            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public override string GetString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("[{0}]", Name));
            foreach (EppContent e in this)
                sb.AppendLine(e.GetString());

            return sb.ToString();
        }
    }
}
