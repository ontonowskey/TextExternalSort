using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.InteropServices;

namespace TxtSort
{
    class Sort
    {
        public static void Split(string file)
        {
            ProgressBar progressBar1 = new ProgressBar();
            progressBar1.Name = "progressBar1";
            progressBar1.Width = 200;
            progressBar1.Height = 30;
            progressBar1.Location = new System.Drawing.Point(350, 70);
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            Form.ActiveForm.Controls.Add(progressBar1);
            int split_num = 1;
            StreamWriter writer = new StreamWriter(
              string.Format("c:\\TestAntonov\\split{0:d5}.dat", split_num));
            long read_line = 0;
            using (StreamReader reader = new StreamReader(file))
            {
                while (reader.Peek() >= 0)
                {
                    // Сообщаем прогресс
                    if (++read_line % 5000 == 0)
                        progressBar1.Value = (int)(100 * reader.BaseStream.Position / reader.BaseStream.Length);

                    // Копируем строку
                    writer.WriteLine(reader.ReadLine());

                    // Если файл все еще большой, делаем еще сплит.
                    if (writer.BaseStream.Length > 300000000 && reader.Peek() >= 0)
                    {
                        writer.Close();
                        split_num++;
                        writer = new StreamWriter(
                          string.Format("c:\\TestAntonov\\split{0:d5}.dat", split_num));
                    }
                }
            }
            writer.Close();
        }

        public static void SortParts()
        {

            foreach (string path in Directory.GetFiles("C:\\TestAntonov\\", "split*.dat"))
            {
                // Читаем все строки в список
                // Выходной массив строк
                string[] lines = File.ReadAllLines(path);
                var str = new StringBuilder();
                for (int i = 0; i<lines.Length;i++)
                {
                    var builder = new StringBuilder();
                    char[] lineChar = lines[i].ToCharArray();
                    Array.Sort(lineChar);
                    foreach (var c in lineChar)
                    {
                        builder.Append(c);
                    }
                    lines[i] = builder.ToString();

                    //int index = lines[i].LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                    //if (index > 0)
                    //{
                    //    string sym = lines[i].Substring(index);
                    //    string num = lines[i].Remove(index);
                    //    str.Append(sym + num);
                    //    lines[i] = str.ToString();
                    //}
                }


                // Создаем файлы с отсортированными элементами
                string newpath = path.Replace("split", "sorted");
                // Записываем
                File.WriteAllLines(newpath, lines);
                // Удаляем неотсортированный парт
                File.Delete(path);
                // Удаляем содержимое массива из памяти
                lines = null;
                // Производим сборку мусора на всякий случай
                GC.Collect();
            }

        }

        public static void MergeParts()
        {
            string[] paths = Directory.GetFiles("C:\\TestAntonov\\", "sorted*.dat");
            int chunks = paths.Length; // Количество чанков
            int recordsize = 100; // estimated record size
            long maxusage = 36000000000; // max memory usage
            long buffersize = maxusage / chunks; // bytes of each queue
            double recordoverhead = 7.5; // The overhead of using Queue<>
            int bufferlen = (int)(buffersize / recordsize /
              recordoverhead); // number of records in each queue

            // Open the files
            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i]);

            // Make the queues
            Queue<string>[] queues = new Queue<string>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<string>(bufferlen);

            // Load the queues
            for (int i = 0; i < chunks; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Merge!
            StreamWriter sw = new StreamWriter("C:\\TestAntonov\\BigFileSorted.txt");
            bool done = false;
            long lowest_index;
            int j = 0;
            string lowest_value;
            while (!done)
            {

                // Находим минимальное значение чанка
                lowest_index = -1;
                lowest_value = "";
                for (j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowest_index < 0 ||
                          String.CompareOrdinal(
                            queues[j].Peek(), lowest_value) < 0)
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }

                // Ничего не найдено в очереди? Закончили.
                if (lowest_index == -1) { done = true; break; }

                // Output it
                sw.WriteLine(lowest_value);

                // Удалить из очереди
                queues[lowest_index].Dequeue();
                // Мы обнулили очередь? Поднять
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index],
                      readers[lowest_index], bufferlen);
                    // Ничего не осталось для чтения?
                    if (queues[lowest_index].Count == 0)
                    {
                        queues[lowest_index] = null;
                    }
                }
            }
            sw.Close();

            // Закрываем и удаляем
            for (int i = 0; i < chunks; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }
        }

        static void LoadQueue(Queue<string> queue,
          StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }


    }
}

