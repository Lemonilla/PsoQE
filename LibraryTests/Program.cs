using Library;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            Quest qst = FileManipulation.LoadQstFromFile("test.qst");
            return;
        }
    }
}
