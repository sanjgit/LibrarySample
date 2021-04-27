using Library.DataLayer;
using Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Web;

namespace Library.Repositories
{
    public static class ServiceRepository
    {
        
        public static int Num_Of_Common_Words = 10;
        public static void SeedData()
        {
            try
            {
                var session = HttpContext.Current.Session;
                if (session["ListOfBooks"] == null)
                {


                    string[] fileArray = Directory.GetFiles(HttpContext.Current.Server.MapPath(("~/Resources")));
                    List<BookDTO> lstDto = new List<BookDTO>();

                    CacheItemPolicy policy = new CacheItemPolicy();
                    int bookId = 0;
                    foreach (var txtFile in fileArray)
                    {
                        BookDTO bookObj = new BookDTO();
                        bookObj.Title = Path.GetFileNameWithoutExtension(txtFile) + Path.GetExtension(txtFile);
                        bookObj.Id = ++bookId;
                        bookObj.Path = txtFile;

                        lstDto.Add(bookObj);
                    }
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(fileArray));
                    session["ListOfBooks"] = lstDto;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
    }
}