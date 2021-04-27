using Library.Controllers;
using Library.Models;
using Library.Repositories;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Tests
{
    [TestFixture]
    public class LibraryTests
    {
        private const string SAMPLE_TEXT = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. 
                Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate 
                velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        public LibraryTests()
        {
        }

        [Test]
        public void MostCommonWordsTest()
        {
            var bookRepo = new BooksRepository();

            var result = bookRepo.DistictWords(SAMPLE_TEXT);
            var topCommonWords = bookRepo.MostCommonWords(result, 10);
            
            Assert.AreEqual(10, topCommonWords.Count);
          
        }

        [Test]
        public void SearchTest()
        {
            var bookRepo = new BooksRepository();
            var result = bookRepo.DistictWords(SAMPLE_TEXT);
            var searchedList = bookRepo.Search(result, "min");

            Assert.AreEqual(1, searchedList.Count);            
        }
        [Test]
        public void SearchTestWithLessThan2Chars()
        {
            var bookRepo = new BooksRepository();
            var result = bookRepo.DistictWords(SAMPLE_TEXT);
            var searchedList = bookRepo.Search(result, "mi");

            Assert.AreEqual(0, searchedList.Count);
        }
    }
}