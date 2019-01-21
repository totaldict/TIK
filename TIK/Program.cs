using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TIK
{
    class Program
    {

        static void PrintMenu()
        {
            Console.WriteLine(@"Меню:
1. Сформировать дерево тегов;
2. Загрузка/сохранение дерева тэгов;
3. Печать тегов;
4. Удаление тэга по имени;
5. Добавление нового тэга;
6. Переименование тэга;
0. Выход.");
        }
        /// <summary>
        /// проверка ввода числа для меню
        /// </summary>
        /// <param name="k">Параметр ограничения макс. числа.</param>
        /// <returns></returns>
        static public int Vvod(int k)         
        {
            int n = 0;
            bool ok1 = false, ok2 = false;
            do
            {
                ok1 = int.TryParse(Console.ReadLine(), out n);
                if (((n < k) && (n >= 0)) || (k == 0)) ok2 = true;      //проверяем входит ли в диапазон, если параметр k=0, то диапазон не учитываем 

            } while ((!ok1) || (!ok2));
            return n;
        }
        static void Main(string[] args)
        {
            int n;
            TagItem tagTree=new TagItem();
            do
            {
                PrintMenu();
                n = Vvod(7);
                switch (n)
                {
                    case 1: //1. Сформировать дерево.
                        {
                            Console.WriteLine("Сколько элементов будет в дереве?");
                            int k = Vvod(40);   //здесь ограничение на макс. 40 элементов
                            tagTree = TagItem.AddElement(k, "first");
                            //TagItem.SetPaths(tagTree, "first"); //прописываем пути с первого
                            break;
                        }
                    case 2: //2. Загрузка / выгрузка дерева тэгов(D:\Tags.xml);
                        {
                            Console.WriteLine("Загрузка/сохранение дерева тэгов:\n1.Загрузить.\n2.Сохранить.\n0.Выход.");
                            int k = Vvod(3);
                            if (k == 1)
                            {
                                tagTree = tagTree.LoadTree(@"D:\Tags.xml");   //Загрузка из XML
                                //TagItem.ShowTree(tagTree, 10);
                            }
                            if (k == 2)
                            {
                                tagTree.SaveTree(tagTree, @"D:\Tags.xml");      //Сохранение в XML
                            }
                            break;
                        }
                    case 3: //3. Вывод построчного списка тэгов – полный путь, уровень вложенности, тип, значение;
                        {
                            Console.WriteLine("Печать тегов:\n1.В виде дерева.\n2.Построчно.\n0.Выход.");
                            int k = Vvod(3);
                            if (k == 1)
                            {
                               // TagItem.ShowTree(tagTree, 10);  //вывод деревом
                            }
                            if (k == 2)
                            {
                                TagItem.ShowTreeInLine(tagTree);    //вывод построчно
                            }
                            break;
                        }
                    case 4: //4. Удаление тэга по имени;
                        {
                            //Console.WriteLine("Введите имя тега, который удаляем:");
                            //string name = Console.ReadLine();
                            //TagItem.findOk = false;
                            //TagItem find = tagTree.FindTag(tagTree, name);  //ищем этот тег
                            //if (find != null)
                            //    Console.WriteLine($"Найден тег {name}:{find.ReadTagData(find)}");
                            //else
                            //    Console.WriteLine($"Тег {name} не найден!");
                            //tagTree = tagTree.DelTagItem(tagTree, find);    
                            //TagItem.SetPaths(tagTree, "first"); //перепрописываем пути с первого после удаления
                            ////TagItem.ShowTree(tagTree, 10);     
                            break;
                        }
                    case 5: //5. Добавление нового тэга.
                        {
                            //Console.WriteLine("Куда добавить тег?");
                            //string name = Console.ReadLine();
                            //TagItem.findOk = false;
                            //TagItem find = tagTree.FindTag(tagTree, name);  //ищем этот тег
                            //if (find != null)
                            //    Console.WriteLine($"Найден тег {name}:{find.ReadTagData(find)}");
                            //else
                            //    Console.WriteLine($"Тег {name} не найден!");
                            //find.AddTagItem(find);
                            ////TagItem.ShowTree(tagTree, 10);

                            break;
                        }
                    case 6: //6. Переименование тэга.
                        {
                            //Console.WriteLine("Введите имя тега, который переименовываем?");
                            //string name = Console.ReadLine();
                            //TagItem.findOk = false;
                            //TagItem find = tagTree.FindTag(tagTree, name);  //ищем этот тег
                            //if (find != null)
                            //    Console.WriteLine($"Найден тег {name}:{find.ReadTagData(find)}");
                            //else
                            //    Console.WriteLine($"Тег {name} не найден!");
                            //find.ChangeTagName(find);
                            //TagItem.SetPaths(tagTree, "first"); //перепрописываем пути с первого после переименования тега
                            //TagItem.ShowTree(tagTree, 10);
                            break;
                        }
                }
            } while (n != 0);



            
            //TagStorage Root = new TagStorage();
            //Root.Root = tagTree;    //пихаем в корень наше дерево.
            //
            
        }
    }
}
