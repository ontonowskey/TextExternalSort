using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TxtSort
{
    class TxtFile
    {
        public static void Start(string path)
        {
            ulong size = 10737418240 / 20;
            Random rnd = new Random();
            char[] letters = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
            //ulong size = 100000;
            if (!Directory.Exists(@"C:\\TestAntonov\"))
                Directory.CreateDirectory(@"C:\\TestAntonov\");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            {
                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter file = new StreamWriter(fileStream);
                file.BaseStream.Seek(0, SeekOrigin.End);
                // записываем в файл до тех пор пока он не будет весить 10 гб
                for (ulong bytes = 0; bytes < size; bytes++)
                {
                    //запись в строку рандомного размера 10-30 символов
                    uint num_letters = (uint)rnd.Next(10, 30);
                    for (int j = 0; j < num_letters; j++)
                    {
                        int someChar = rnd.Next(0, letters.Length - 1);
                        file.Write(letters[someChar]);
                        bytes++;
                        if (bytes == size) // если счётчик достиг 10гб то заканчиваем запись
                        {
                            goto End;
                        }
                    }
                    file.WriteLine(); // переход на новую строку
                    bytes++;
                    if (bytes == size)
                    {
                        goto End;
                    }
                    bytes++;
                    if (bytes == size)
                    {
                        goto End;
                    }
                }
            End:
                file.Flush();
                file.Close();
                fileStream.Close();
            }
        }

        public static void Delete()
        {
            foreach (string path in Directory.GetFiles("C:\\TestAntonov\\", "split*.dat"))
            {
                File.Delete(path);
            }
            if (File.Exists("C:\\TestAntonov\\Text.txt"))
                File.Delete("C:\\TestAntonov\\Text.txt");
            foreach (string path in Directory.GetFiles("C:\\TestAntonov\\", "sorted*.dat"))
            {
                File.Delete(path);
            }
        }
    }
}
