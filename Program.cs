using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LFT
{
    class Program
    {
        static void Main(string[] args)
        {          
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "teste\\6.txt");
            string text = System.IO.File.ReadAllText(path);
            var parser = new Parser(text);
            try
            {             
                parser.Parseaza();
            }
            catch(Exception e)
            {
                
            }
            finally
            {
                if (parser.Erori.Count() > 0)
                {
                    Console.WriteLine("Codul scris nu a putut fi evaluat. Au aparut urmatoarele erori:");
                    parser.AfiseazaErori();
                }
                else
                {
                    //in variabile avem si rezultatul expresiilor;
                    //am considerat ca daca vrem sa evaluam o expresie, o vom asigna unei variabile de tipul respectiv
                    parser.AfiseazaVariabile();
                }
            }           
            Console.ReadKey();
        }
    }
}
