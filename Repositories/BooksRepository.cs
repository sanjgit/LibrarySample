using Library.DataLayer;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Library.Repositories
{
    public class BooksRepository
    {
        MemoryCache memoryCache = MemoryCache.Default;
        
        public List<BookDTO> GetBooks()
        {
            try
            {
                if (HttpContext.Current.Session["ListOfBooks"] == null)
                {
                    ServiceRepository.SeedData();
                }
                List<BookDTO> lstTextBooks = (List<BookDTO>)HttpContext.Current.Session["ListOfBooks"];

                return lstTextBooks;
            }
            catch (Exception ex)
            {

                throw ex;
            }   
           
        }
        public BookTextResult GetTopCommonWords(int fileId)
        {
            try
            {
                List<BookDTO> lstTextBooks = GetBooks();
                string path = "";
                foreach (BookDTO BT in lstTextBooks)
                {
                    if (BT.Id == fileId)
                    {
                        path = BT.Path;
                    }
                }
                List<BookTextResult> lstBookTextResult = new List<BookTextResult>();
                //check if file text count in cache            
                if (HttpContext.Current.Session["BookTextCounts"] != null)
                {
                    lstBookTextResult = (List<BookTextResult>)HttpContext.Current.Session["BookTextCounts"];
                    foreach (BookTextResult BTR in lstBookTextResult)
                    {
                        if (BTR.id == fileId)
                        {
                            return BTR;
                        }
                    }
                }
                string txts = File.ReadAllText(path, Encoding.UTF8);
                var distinctWords = DistictWords(txts);
                var topCommonWords = MostCommonWords(distinctWords, 10);

                //memoryCache.Remove("BookTextCounts");           

                BookTextResult bookCountResult = new BookTextResult();
                bookCountResult.id = fileId;
                bookCountResult.TopCommonWords = topCommonWords;
                bookCountResult.DistinctWords = distinctWords;
                lstBookTextResult.Add(bookCountResult);

                var cacheItemPolicy = new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddDays(1) };
                HttpContext.Current.Session["BookTextCounts"] = lstBookTextResult;

                return bookCountResult;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public List<MostCommonWords> searchByString(int fileId, string searchQuery)
        {
            try
            {
                List<BookTextResult> lstBookTextResult;
                BookTextResult bookTestRes = new BookTextResult();
                if (HttpContext.Current.Session["BookTextCounts"] != null)
                {
                    lstBookTextResult = (List<BookTextResult>)HttpContext.Current.Session["BookTextCounts"];
                    foreach (BookTextResult BTR in lstBookTextResult)
                    {
                        if (BTR.id == fileId)
                        {
                            bookTestRes = BTR;
                        }
                    }
                }
                else
                {
                    bookTestRes = GetTopCommonWords(fileId);
                }
                return Search(bookTestRes.DistinctWords, searchQuery);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }
        public List<MostCommonWords> Search(Dictionary<string,int> unqiueWords, string query)
        {
            try
            {
                var resultList = new List<MostCommonWords>();
                if (query.Length > 2)
                {
                    resultList = unqiueWords.Where(r => r.Key.StartsWith(query.ToLower())).OrderByDescending(a => a.Value).Select(x => new MostCommonWords() { Word_Name = Capitalize(x.Key), Count_Of_Occurance = x.Value }).ToList();
                }
                return resultList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
         
        }
        public Dictionary<string, int> DistictWords(string text)
        {
            try
            {
                var delimiterChars = new char[] { ' ', ',', ';', '"', ':', '\t', '\"', '\r','”', '{', '}', '[', ']', '=', '/', '-', '.', '?', '!' };

                var match = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, int> freq = new Dictionary<string, int>();
                foreach (var word in match)
                {
                    if (freq.ContainsKey(word))
                    {
                        freq[word]++;
                    }
                    else
                    {
                        freq.Add(word, 1);
                    }
                }
                return freq;
          
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public List<MostCommonWords> MostCommonWords(Dictionary<string,int> allWordsFreq, int numWords = int.MaxValue)
        {
            //var delimiterChars = new char[] { ' ', ',', ':', '\t', '\"', '\r', '{', '}', '[', ']', '=', '/', '-', '.' };
            try
            {
                List<MostCommonWords> topWords = new List<MostCommonWords>();
                //string query = "que";
                //var searcheList = allWordsFreq.Where(r => r.Key.StartsWith(query.ToLower()));
                foreach (var elem in allWordsFreq.Where(r => r.Key.Length >= 5).OrderByDescending(a => a.Value).Take(numWords))
                {
                    MostCommonWords mcw = new MostCommonWords();
                    mcw.Word_Name = Capitalize(elem.Key);
                    mcw.Count_Of_Occurance = elem.Value;
                    topWords.Add(mcw);
                }
                return topWords;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}