using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _0
{
    class Program
    {

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();

            List<string> filesname = Directory.GetFiles(path, "*.txt").ToList<string>();
            List<string> allID = new List<string>() { "" };
            List<int> allSum = new List<int>() { };
            
            if(filesname.Count == 0) { Console.WriteLine("Файл не был найден!!!"); Console.ReadLine(); return;}

            int j = 0;
            for (int indexFile = 0; indexFile < filesname.Count; indexFile++)
            {
                var sr = new StreamReader(filesname[indexFile]);
                String buffer = sr.ReadToEnd();
                sr.Close();
                String[] data = buffer.Split(';');
                if((data.Length < 15) || (data[0] != "mail") || (data[1] != "region"))
                { 
                    Console.WriteLine("\n!!!Был встречен неправильный файл: " + filesname[indexFile]); 
                    continue; 
                }
                for (int i = 15; i < data.Length; i += 9)
                {
                    allID.Add(data[i]);
                }
                int tmpSum = 0;
                for (int i = 14; i < data.Length; i += 9)
                {
                    tmpSum += (int.Parse(data[i]) / 100);
                }

                allID.Add("|");
                allSum.Add(tmpSum);

                String[] nameFile = filesname[indexFile].Split('\\');
                int tmp = nameFile.Length - 1;
                Console.WriteLine(nameFile[tmp] + " - "+ allSum[j]);
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - -");
                j++;
            }

            Console.WriteLine("1 - Продолжить\n0 - Выйти из программы");
            string input = Convert.ToString(Console.ReadLine());
            int check = 0;
            while (check == 0)
            {
                switch (input)
                {
                    case "1":
                        check = -1;
                        break;
                    case "0":
                        check = -1;
                        return;  
                    default:
                        input = Convert.ToString(Console.ReadLine());
                        break;
                }
            }   

            if (!Directory.Exists(path + "/Resault")) //если папки нет - создаем
            {
                Directory.CreateDirectory(path + "/Resault");
            }
            FileStream fs = File.Create(path + "/Resault/" + "res.txt");
            StreamWriter writer = new StreamWriter(fs);
            writer.Write("SELECT\nfrom_unixtime(o.created, '%d.%m.%Y %H:%i:%s') as Создана,\ns.number AS Карта,\ns.sector AS Сектор,\no.oid AS Операция\nFROM operations o\ninner join sectors s on s.sid = o.purpose\nleft join actions as a on o.action = a.aid\nWHERE o.status = 2     /* 1 - Выполнена      2 - В обработке*/\nAND s.number IN(\n");
            for(int i = 1; i < allID.Count - 1; i++)
            {
                if (i == allID.Count - 2) { writer.Write("'" + allID[i] + "')\n"); continue; }
                if (allID[i] == "|") { continue; }

                writer.Write("'" + allID[i] + "',\n"); 
            }
            writer.Write("order by o.oid desc ");
            writer.Close();
            System.Diagnostics.Process.Start(path + "/Resault/" + "res.txt");
        }
    }
}
