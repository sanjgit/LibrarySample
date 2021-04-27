using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BookTextResult
    {
        public int id { get; set; }
        public Dictionary<string,int> DistinctWords { get; set; }
        public List<MostCommonWords> TopCommonWords { get; set; }
    }
}