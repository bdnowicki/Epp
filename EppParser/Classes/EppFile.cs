using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EppParser.Classes
{
    public class EppFile : IList<EppHeader>, IEppElement
    {
        /// <summary>
        /// Info header with definition of file
        /// </summary>
        public EppHeaderInfo Info { get; private set; }


        public EppFile()
        {
            Info = new EppHeaderInfo() { Name = "INFO" };
        }

        public void LoadEpp(Stream file)
        {
            string cnt = "";
            file.Position = 0;
            int enc = 0;

            using (StreamReader reader = new StreamReader(file, Encoding.GetEncoding(852)))
            {
                string line = reader.ReadLine();
                if (line != "[INFO]")
                    throw new Exception("Błędny format pliku");

                line = reader.ReadLine();
                string[] fields = line.Split(new string[] { "," }, StringSplitOptions.None);
                if (fields.Length != 24)
                    throw new Exception("Błędny format pliku");

                if (!int.TryParse(fields[2], out enc))
                    throw new Exception("Błędny format pliku");


                file.Position = 0;
                using (StreamReader reader2 = new StreamReader(file, Encoding.GetEncoding(enc)))
                {
                    cnt = reader2.ReadToEnd();
                }
            }

            LoadEpp(cnt);
        }

        public void LoadEpp(string file)
        {
            using (StringReader reader = new StringReader(file))
            {
                int peek = 0;
                EppHeader prev = null;

                while (peek >= 0)
                {
                    peek = reader.Peek();
                    if (peek == 91)
                    {
                        EppHeader header = null;
                        string l = reader.ReadLine();
                        if (l == "[INFO]")
                            header = Info;
                        else if (l == "[ZAWARTOSC]" && prev != null && prev is EppHeaderInfo)
                        {
                            header = new EppHeaderContent() { Name = "ZAWARTOSC" };
                            ((EppHeaderInfo)prev).Zawartosc = header as EppHeaderContent;
                        }
                        else
                        { header = new EppHeaderInfo() { Name = l.Substring(1, l.Length - 2) }; Add(header); }

                        prev = header;

                        string cnt = "";

                        peek = reader.Peek();
                        while (peek != 91 && peek > 0)
                        {
                            cnt += reader.ReadLine();
                            peek = reader.Peek();
                            cnt += "\n";
                        }

                        using (StringReader cReader = new StringReader(cnt))
                        {
                            int cPeek = cReader.Peek();
                            while (cPeek >= 0)
                            {
                                cPeek = cReader.Peek();
                                if (cPeek == 10)
                                {
                                    cReader.Read();
                                    continue;
                                }

                                EppContent eppContent = new EppContent();
                                eppContent.Parse(cReader);
                                if (eppContent.Count > 0)
                                {
                                    if (header is EppHeaderInfo)
                                    {
                                        ((EppHeaderInfo)header).Opis = eppContent;
                                    }
                                    else if (header is EppHeaderContent)
                                    {
                                        ((EppHeaderContent)header).Add(eppContent);
                                    }
                                }
                            }
                        }
                    }
                }

            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Info.ToString());

            return sb.ToString();
        }

        #region IList implemented

        private List<EppHeader> list = new List<EppHeader>();
        public EppHeader this[int index]
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
        public void Add(EppHeader item)
        {
            list.Add(item);
        }
        public void Clear()
        {
            list.Clear();
        }
        public bool Contains(EppHeader item)
        {
            return list.Contains(item);
        }
        public void CopyTo(EppHeader[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }
        public IEnumerator<EppHeader> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        public int IndexOf(EppHeader item)
        {
            return list.IndexOf(item);
        }
        public void Insert(int index, EppHeader item)
        {
            list.Insert(index, item);
        }
        public bool Remove(EppHeader item)
        {
            return list.Remove(item);
        }
        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        public string GetString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Info.GetString());

            foreach(EppHeader e in this)
                sb.Append(e.GetString());

            return sb.ToString();
        }

    }
}
