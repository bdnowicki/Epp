using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EppParser.Classes
{
    public class EppFile : IList<EppHeaderInfo>, IEppElement
    {
        /// <summary>
        /// Info header with definition of file
        /// </summary>
        public EppHeaderInfo Info { get; private set; }

        public EppHeaderInfo[] this[string OpisName]
        {
            get
            {
                List<EppHeaderInfo> l = new List<EppHeaderInfo>();
                foreach (var a in this)
                    if (a.Opis[0].StringValue == OpisName)
                        l.Add(a);

                return l.ToArray();
            }
        }

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
                            EppHeaderContent hContent = new EppHeaderContent()
                            {
                                Name = "ZAWARTOSC"
                            };
                            header = hContent;
                            ((EppHeaderInfo)prev).Zawartosc = hContent;
                        }
                        else
                        {
                            string name = l.Substring(1, l.Length - 2);
                            header = new EppHeaderInfo();
                            header.Name = name;
                            Add(header as EppHeaderInfo);
                        }

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

                        if (header is EppHeaderInfo && Contains(header as EppHeaderInfo))
                        {
                            EppHeaderInfo ehiOld = header as EppHeaderInfo;
                            EppHeaderInfo ehiNew = null;

                            int i = IndexOf(ehiOld);


                            switch (ehiOld.Opis[0].StringValue)
                            {
                                case "FS":
                                case "FZ":
                                case "RZ":
                                case "RS":
                                case "PZ":
                                case "WZ":
                                case "PW":
                                case "RW":
                                case "ZD":
                                case "ZK":
                                case "PA":
                                    ehiNew = new DokumentHeaderInfo();
                                    break;

                                case "TOWARY":
                                    ehiNew = new TowaryHeaderInfo();
                                    break;

                                default:
                                    ehiNew = new EppHeaderInfo();
                                    break;
                            }
                            ehiNew.Name = ehiOld.Name;
                            ehiNew.Opis = ehiOld.Opis;
                            ehiNew.Zawartosc = ehiOld.Zawartosc;
                            this[i] = ehiNew;
                            header = ehiNew;
                        }

                        prev = header;
                    }
                    else
                        reader.Read();
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

        private List<EppHeaderInfo> list = new List<EppHeaderInfo>();
        public EppHeaderInfo this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public int Count { get { return list.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(EppHeaderInfo item) { list.Add(item); }

        public void Clear() { list.Clear(); }

        public bool Contains(EppHeaderInfo item)
        {
            return list.Contains(item);
        }

        public void CopyTo(EppHeaderInfo[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<EppHeaderInfo> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(EppHeaderInfo item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, EppHeaderInfo item)
        {
            list.Insert(index, item);
        }

        public bool Remove(EppHeaderInfo item)
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

            foreach (EppHeader e in this)
                sb.Append(e.GetString());

            return sb.ToString();
        }

        public Encoding Encoding
        {
            get
            {
                if (Info.Opis.Count == 24 && Info.Opis[2].IsInt)
                    if (Info.Opis[2].IntValue == 852)
                        return Encoding.GetEncoding(852);

                return Encoding.GetEncoding(1250);
            }
        }

        public void Init(DateTime from, DateTime to)
        {
            Clear();
            Info = new EppHeaderInfo() { Name = "INFO" };
            Info.Opis.AddRangeObjects(new object[]
            {
                "1.05",
                3,
                1250,
                "EppParser",
                "VEGAMARKET",
                "VEGAMARKET",
                "VEGAMARKET Piotr Spyrczak",
                "Wrocław",
                "52-013",
                "ul. Opolska 143a",
                "8991385553",
                "MAG",
                "Główny",
                "Magazyn główny",
                "",
                1,
                from,
                to,
                "Spyrczak Piotr",
                DateTime.Now,
                "Polska",
                "PL",
                "PL8991385553",
                1
            });
        }
    }
}
