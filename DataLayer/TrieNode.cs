using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.DataLayer
{
    #region TRIE implementation

    public class TrieNode : IComparable<TrieNode>
    {
        private char m_char;
        public int m_word_count;
        private TrieNode m_parent = null;
        private ConcurrentDictionary<char, TrieNode> m_children = null;
        public TrieNode(TrieNode parent, char c)
        {
            m_char = c;
            m_word_count = 0;
            m_parent = parent;
            m_children = new ConcurrentDictionary<char, TrieNode>();
        }

        public void AddWord(string word, int index = 0)
        {
            if (index < word.Length)
            {
                char key = word[index];
                if (char.IsLetter(key))
                {
                    if (!m_children.ContainsKey(key))
                    {
                        m_children.TryAdd(key, new TrieNode(this, key));
                    }
                    m_children[key].AddWord(word, index + 1);
                }
                else
                {
                    // not a letter! retry with next char
                    AddWord(word, index + 1);
                }
            }
            else
            {
                if (m_parent != null) // empty words should never be counted
                {
                    lock (this)
                    {
                        m_word_count++;
                    }
                }
            }
        }

        public int GetCount(string word, int index = 0)
        {
            if (index < word.Length)
            {
                char key = word[index];
                if (!m_children.ContainsKey(key))
                {
                    return -1;
                }
                return m_children[key].GetCount(word, index + 1);
            }
            else
            {
                return m_word_count;
            }
        }

        public void FindPhrases(TrieNode root, String textBody, ref List<string> searchedList)
        {
            // a pointer to traverse the trie without damaging
            // the original reference
            TrieNode node = root;

            // a list of found ids
            List<string> foundPhrases = new List<string>();

            // starting traversal at trie root and first
            // word in text body
            for (int i = 0; i < textBody.Length;)
            {
                // if current node has current word as a child
                // move both node and words pointer forward
                if (node != null && node.m_children.ContainsKey(textBody[i]))
                {
                    // move trie pointer forward
                    node = node.m_children[textBody[i]];

                    // move words pointer forward
                    ++i;
                }
                else
                {
                    // current node does not have current
                    // word in its children

                    // if there is a phrase Id, then the previous
                    // sequence of words matched a phrase, add Id to
                    // found list
                    if (node.m_children.Count > 0)
                        foundPhrases.Add(node.ToString());


                    if (node == root)
                    {
                        // if trie pointer is already at root, increment
                        // words pointer
                        ++i;
                    }
                    else
                    {
                        // if not, leave words pointer at current word
                        // and return trie pointer to root
                        node = root;
                    }

                }
            }
            GetSearchCompleteList(node, node.ToString(), foundPhrases);
            searchedList = foundPhrases;
        }

        public void GetSearchCompleteList(TrieNode matchedNode, string completeWord, List<string> wordList)
        {
            foreach (var item in matchedNode.m_children)
            {
                string tmpWord = item.ToString();
                if (item.Value.m_children.Count == 0)
                {

                    wordList.Add(item.Value.ToString());
                }
                GetSearchCompleteList(item.Value, tmpWord, wordList);
            }
        }

        public void GetTopCounts(ref List<TrieNode> most_counted, ref int distinct_word_count, ref int total_word_count, ref List<string> lst_distinct_words)
        {
            if (m_word_count > 0)
            {
                distinct_word_count++;
                lst_distinct_words.Add(this.ToString());
                total_word_count += m_word_count;
            }
            if (m_word_count > most_counted[0].m_word_count && this.ToString().Length >= 5)
            {
                most_counted[0] = this;
                most_counted.Sort();
            }
            foreach (char key in m_children.Keys)
            {
                m_children[key].GetTopCounts(ref most_counted, ref distinct_word_count, ref total_word_count, ref lst_distinct_words);
            }
        }

        public override string ToString()
        {
            if (m_parent == null) return "";
            else return m_parent.ToString() + m_char;
        }

        public int CompareTo(TrieNode other)
        {
            return this.m_word_count.CompareTo(other.m_word_count);
        }
       
    }

    #endregion

}