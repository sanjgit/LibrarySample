using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Library.DataLayer
{
    public class DataReader
    {
        static int LOOP_COUNT = 1;
        public TrieNode m_root;
        private string m_path;
        public int m_pathId;
        public DataReader(string path,int pathId, ref TrieNode root)
        {
            m_root = root;
            m_path = path;
            m_pathId = pathId;
        }

        public void ThreadRun()
        {
            for (int i = 0; i < LOOP_COUNT; i++) // fake large data set buy parsing smaller file multiple times
            {
                using (FileStream fstream = new FileStream(m_path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sreader = new StreamReader(fstream))
                    {
                        string line;
                        while ((line = sreader.ReadLine()) != null)
                        {
                            var delimiterChars = new char[] { ' ', ',', ':', '\t', '\"', '\r', '{', '}', '[', ']', '=', '/', '-', '.' };
                            string[] chunks = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string chunk in chunks)
                            {
                                if (chunk.Length > 0)
                                {
                                    m_root.AddWord(chunk.Trim().ToLower());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}