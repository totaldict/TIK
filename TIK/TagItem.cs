using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace TIK
{
    [Serializable]
    public class TagItem
    {
        string name;//имя тэга
        public object data;//значение
        //public TagItem left, right;//ветви
        string fullPath;
        int level;
        static Random rnd = new Random();   //для заполнения дерева не вручную
        static int count = 0;
        //TagItem find;
        public static bool findOk = false;
        //TagItem parent;     //родитель
        List<TagItem> childrens;  //список детей



        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        public string Fullpath
        {
            get { return fullPath; }
            set { fullPath = value; }
        }
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        public string LevelString
        {
            get { return $"L{level}"; }
        }

        /// <summary>
        /// Свойство возвращает List потомков
        /// </summary>
        public List<TagItem> ChildrensList
        {
            get
            {
                return this.childrens;
            }
            set
            {
                try
                {
                    childrens = value;
                }
                catch { }
            }
        }
        /// <summary>
        /// Вся информация о теге в строчку
        /// </summary>
        public string TagToString
        {
            get { return $"{Name}, {data}, L{Level}, {ShowType(this)}, {Fullpath}"; }
        }
        /// <summary>
        /// Возвращает иконку, которая соответствует типу данных
        /// </summary>
        public BitmapSource IconSource
        {
            get
            {
                if (this.data is int) return ConvertBitmap(Properties.Resources._1int);
                if (this.data is bool) return ConvertBitmap(Properties.Resources._2bool);
                if (this.data is double) return ConvertBitmap(Properties.Resources._3double);
                if (this.data == null) return ConvertBitmap(Properties.Resources._4none);
                return ConvertBitmap(Properties.Resources._4none);
            }
        }
        /// <summary>
        /// Свойство возвращает тип значения
        /// </summary>
        public string ShowTagType
        {
            get
            {
                string type = "none";//тип значения
                if (this.data is double) type = "double";
                if (this.data is int) type = "int";
                if (this.data is bool) type = "bool";
                if (this.data == null) type = "none";
                return type;
            }
        }
        public TagItem()
        {
            name = "tag" + count;
            data = null;
            fullPath = "";
            count++;
            childrens = new List<TagItem>();
        }
        public TagItem(string n, object d)
        {
            name = n + count;
            data = d;
            fullPath = "";
            count++;
            childrens = new List<TagItem>();
        }
        /// <summary>
        /// Возвращает тип значения тэга
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string ShowType(TagItem tag)
        {
            string type = "none";//тип значения
            if (tag.data is double) type = "double";
            if (tag.data is int) type = "int";
            if (tag.data is bool) type = "bool";
            if (tag.data == null) type = "none";
            return type;
        }
        /// <summary>
        /// Возвращает значение тэга
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public object ReadTagData(TagItem tag)
        {
            return tag.data;
        }
        /// <summary>
        /// Метод записи значения тега
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public TagItem WriteTagData(TagItem tag)
        {
            Console.WriteLine($"Введите значение тэга {tag.name}:");
            int i;
            double d;
            bool b;
            if (int.TryParse(Console.ReadLine(), out i))
            {
                tag.data = i;
                return tag;
            }
            if (double.TryParse(Console.ReadLine(), out d))
            {
                tag.data = d;
                return tag;
            }
            if (bool.TryParse(Console.ReadLine(), out b))
            {
                tag.data = b;
                return tag;
            }
            tag.data = null;
            return tag;
        }
        /// <summary>
        /// Метод изменения имени тега
        /// </summary>
        /// <param name="tag">тег, имя которого надо изменить</param>
        /// <returns></returns>
        public void ChangeTagName(TagItem tag)
        {
            bool ok = false;
            string newName;
            Console.WriteLine($"Введите новое имя тэга {tag.name}:");
            do
            {
                newName = Console.ReadLine();               //проверяем на пустую строку или пробел - не очень названия для тега, думаю
                if (newName.CompareTo("") == 0 || newName.CompareTo(" ") == 0 || newName.Length < 2)    //ну и длинна должна быть больше 2х символов
                    ok = false;
                else ok = true;
            } while (!ok);
            tag.name = newName;
        }
        /// <summary>
        /// Возвращает рандомное значение int/double/bool
        /// </summary>
        /// <returns></returns>
        static object ReturnData()
        {
            int i;
            double d;
            bool b;
            int n = rnd.Next(2, 99);
            if (n % 2 == 0) return i = rnd.Next(0, 99);
            if (n % 3 == 0) return d = Math.Round(rnd.NextDouble() * 100, 2);   //представляем в удобном виде число double
            if (n > 50) b = true;
            else return b = false;
            return b;

        }

        /// <summary>
        /// Создание дерева, количество элементов - size.
        /// </summary>
        /// <param name="size">количество элементов</param>
        /// <returns></returns>
        public static TagItem AddElement(int size, string path = "first", int setLevel=1)
        {
            TagItem newTag;
            int numberOfElements, remainOfElements;
            object number;
            if (size == 0) { newTag = null; return newTag; }
            numberOfElements = size / 2;
            remainOfElements = size - numberOfElements - 1;
            number = TagItem.ReturnData();
            newTag = new TagItem("tag", number);
            newTag.Level = setLevel;
            if (path.CompareTo("first") == 0)
                newTag.Fullpath = newTag.Name; //путь для первого элемента
            else
                newTag.Fullpath = $"{path}.{newTag.Name}";

            newTag.childrens.Add(AddElement(numberOfElements, newTag.Fullpath, newTag.Level+1));
            newTag.childrens.Add(AddElement(remainOfElements, newTag.Fullpath, newTag.Level + 1));
            DelNullChilds(newTag);

            return newTag;
        }
        /// <summary>
        /// Печатает структуру дерева
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="l">отступ от левого края</param>
        //public static void ShowTree(TagItem tag, int l)
        //{
        //    if (tag != null)
        //    {
        //        ShowTree(tag.left, l + 10);//переход к левому поддереву
        //        for (int i = 0; i < l; i++) Console.Write(" ");
        //        Console.WriteLine($"{tag.name}: {tag.data} L{tag.Level}");  //сюда можно добавить поля, которые необходимо видеть при печати
        //        ShowTree(tag.right, l + 10);//переход к правому поддереву
        //    }
        //}
        /// <summary>
        /// Вывод построчного списка тэгов – полный путь, уровень вложенности, тип, значение;
        /// </summary>
        /// <param name="tag"></param>
        public static void ShowTreeInLine(TagItem tag)
        {
            if (tag.childrens != null)
            {
                foreach (TagItem item in tag.childrens)
                {
                    if (item == null) break;
                    Console.WriteLine($"{item.Fullpath}, L{item.Level}, {item.ShowType(item)}, {item.data} ");
                    ShowTreeInLine(item);
                }
            }
        }
        //удаляет нулевых детей =)
        public static void DelNullChilds(TagItem tag)
        {
            for (int i = 0; i < tag.childrens.Count; i++)
            {
                if (tag.childrens[i] == null) tag.childrens.RemoveAt(i);
            }
        }
        /// <summary>
        /// Поиск по имени тега
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        //public TagItem FindTag(TagItem tag, string str)
        //{
        //    TagItem help = tag;
        //    if (help.name.CompareTo(str) == 0)
        //    {
        //        findOk = true;                  //##криво.
        //        find = help;
        //        return help;
        //    }
        //    if (help.left != null && !findOk) FindTag(help.left, str);
        //    if (help.right != null && !findOk) FindTag(help.right, str);
        //    if (findOk) return find;
        //    else return null;
        //}

        /// <summary>
        /// добавление тега
        /// </summary>
        //public void AddTagItem(TagItem tag)
        //{
        //    TagItem addItem = new TagItem();
        //    addItem = WriteTagData(addItem);
        //    if (tag.left == null)
        //    {
        //        tag.left = addItem;
        //        return;
        //    }
        //    if (tag.right == null)
        //    {
        //        tag.right = addItem;
        //        return;
        //    }
        //    addItem.left = tag.left;
        //    tag.left = addItem;
        //    Console.WriteLine("Тег добавлен!");
        //    return;
        //}
        /// <summary>
        /// Метод удаления тега.
        /// </summary>
        /// <param name="tag">откуда его удаляем</param>
        /// <param name="delTag">какой тэг удаляем</param>
        //public TagItem DelTagItem(TagItem tag, TagItem delTag)
        //{
        //    string[] arrNames = delTag.Fullpath.Split('.'); //из свойства Путь нашего тэга находим его родителя
        //    string parentName = arrNames[arrNames.Length - 2];
        //    findOk = false;
        //    TagItem parentTag = FindTag(tag, parentName);
        //    if (delTag.left == null && delTag.right == null)        //если у удаляемого нет наследников - всё зачищаем
        //    {
        //        if (parentTag.left == delTag) parentTag.left = null;
        //        if (parentTag.right == delTag) parentTag.right = null;
        //    }
        //    if (delTag.left == null && delTag.right != null)        //если у удаляемого есть наследник справа - добавляем его к родителю
        //    {
        //        if (parentTag.left == delTag) parentTag.left = delTag.right;
        //        if (parentTag.right == delTag) parentTag.right = delTag.right;
        //    }
        //    if (delTag.left != null && delTag.right == null)        //если у удаляемого есть наследник слева - добавляем его к родителю
        //    {
        //        if (parentTag.left == delTag) parentTag.left = delTag.left;
        //        if (parentTag.right == delTag) parentTag.right = delTag.left;
        //    }
        //    if (delTag.left != null && delTag.right != null)        //если у удаляемого есть ОБА наследника - добавляем одного к родителю, второго куда ни будь в конец
        //    {
        //        if (parentTag.left == delTag)
        //        {
        //            parentTag.left = delTag.left;
        //            TagItem end = FindEndOfBranch(delTag);
        //            end.left = delTag.right;
        //        }
        //        if (parentTag.right == delTag)
        //        {
        //            parentTag.right = delTag.right;
        //            TagItem end = FindEndOfBranch(delTag);
        //            end.left = delTag.left;
        //        }
        //    }
        //    Console.WriteLine("Тег удалён!");
        //    return tag;
        //}
        /// <summary>
        /// Метод для нахождения конца ветки
        /// </summary>
        /// <param name="tag">откуда будем искать</param>
        /// <returns></returns>
        //TagItem FindEndOfBranch(TagItem tag)
        //{
        //    TagItem end = tag;
        //    do
        //    {
        //        end = end.left;
        //    } while (end.left != null);
        //    return end;
        //}

        /// <summary>
        /// Прописываем пути и Level у каждого тега
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="prev"></param>
        //public static void SetPaths(TagItem tag, string prev, int l = 1)
        //{
        //    if (tag != null)
        //    {
        //        if (prev.CompareTo("first") == 0)
        //        {
        //            tag.Fullpath = tag.name;
        //            tag.Level = 1;
        //        }
        //        else
        //        {
        //            tag.Fullpath = $"{prev}.{tag.name}";
        //            tag.Level = ++l;
        //        }
        //        SetPaths(tag.left, tag.Fullpath, tag.Level);
        //        SetPaths(tag.right, tag.Fullpath, tag.Level);
        //    }
        //}
        /// <summary>
        /// сохраняем в файл
        /// </summary>
        /// <param name="tag"></param>
        public void SaveTree(TagItem tag, string savePath)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(TagItem));
            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, tag);
                //Console.WriteLine($@"Дерево сохранено в {savePath}.");
            }
        }
        /// <summary>
        /// читаем из файла
        /// </summary>
        /// <returns></returns>
        public TagItem LoadTree(string loadPath)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(TagItem));
            using (FileStream fs = new FileStream(loadPath, FileMode.OpenOrCreate))
            {
                TagItem newTagItems = (TagItem)formatter.Deserialize(fs);
                Console.WriteLine($@"Дерево прочитано из {loadPath}.");
                return newTagItems;
            }
        }
        /// <summary>
        /// Переводит Bitmap в BitmapSource
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

    }

}

