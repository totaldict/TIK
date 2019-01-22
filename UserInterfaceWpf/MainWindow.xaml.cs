using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TIK;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace UserInterfaceWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string openPath;    //путь к открытому файлу
        TagItem tagTree = new TagItem();
        TagStorage rootTagTree = new TagStorage();
        TreeViewItem treeViewItemToChange;  //элемент дерева, который изменяем
        TagItem tagItemToChange;        //элемент TagItem, который будем изменять
        string savePath;        //путь сохранения
        bool treeChanged = false;       //триггер, определяющий, изменено ли дерево
        public MainWindow()
        {
            InitializeComponent();
            rootTagTree.RootPath.Add(tagTree);     //пустое дерево
            FillingTreeView(rootTagTree.Root);  //пустое дерево загружается при старте
        }
        /// <summary>
        /// По нажатию на эту кнопку загружаются данные в TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadToTreeViewFromFileAsync();
        }
        /// <summary>
        /// Выполняет открытие файла XML в отдельном потоке и заполнение TreeView
        /// </summary>
        private async void LoadToTreeViewFromFileAsync()
        {
            await Task.Factory.StartNew(() => LoadToTreeViewFromFile(), TaskCreationOptions.LongRunning);
            FillingTreeView(rootTagTree.Root);
        }
        /// <summary>
        /// Открытие XML-файла
        /// </summary>
        private void LoadToTreeViewFromFile()
        {
            Microsoft.Win32.OpenFileDialog myDialog = new Microsoft.Win32.OpenFileDialog();
            myDialog.Filter = "XML-документ(*.XML)|*.XML" + "|Все файлы (*.*)|*.*";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                openPath = myDialog.FileName;   //назначаем путь к файлу
                tagTree = tagTree.LoadTree(openPath);       //загружаем из файла наш XML
                rootTagTree = new TagStorage();
                rootTagTree.RootPath.Add(tagTree);
            }
        }
        /// <summary>
        /// Биндинг treeView деревом
        /// </summary>
        /// <param name="tagTreeTemp">дерево, которое надо отобразить</param>
        private void FillingTreeView(TagItem tagTreeTemp)
        {
            treeView.ItemsSource = null;
            treeView.ItemsSource = tagTreeTemp.ChildrensList;
        }


        /// <summary>
        /// Выделение объекта на ПКМ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            treeViewItemToChange = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItemToChange != null)
            {
                treeViewItemToChange.Focus();       //выделение элемента по ПКМ
            }
        }
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);
            return source;
        }
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e) //удаление из контекстного меню
        {           //вывод окна подтверждения удаления
            if (System.Windows.MessageBox.Show("Вы действительно хотите удалить предков?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
            {
                tagItemToChange.ChildrensList = null;
                TreeUpdateAfterChanges();
            }
        }

        private void MenuItemChangeName_Click(object sender, RoutedEventArgs e)
        {
            InputBox inputBox = new InputBox("Введите новое имя:");
            tagItemToChange.Name = inputBox.getString();
            TreeUpdateAfterChanges(); //обновление дерева
        }

        private void MenuItemAddTag_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewItemToChange != null)
            {
                TagItem newTag = MakeNewInsertedTag();
                treeViewItemToChange.Focus();
                tagItemToChange.ChildrensList.Add(newTag);     //добавляем вставляемый тег в потомки родительского
                //((List<TagItem>)treeViewItemToChange.ItemsSource).Add(tempInsertedTag);  
                TreeUpdateAfterChanges();
            }
        }
        /// <summary>
        /// Создаёт новый вставляемый тег
        /// </summary>
        /// <returns></returns>
        private TagItem MakeNewInsertedTag()
        {
            string resultString;
            double resultDouble;
            int resultInt;
            bool resultBool;
            TagItem tempInsertedTag = new TagItem();
            InputBox inputBox = new InputBox("Имя тега:");   //Добавил окно ввода параметров добавляемого тега
            tempInsertedTag.Name = inputBox.getString();            //Оттуда берём имя тэга
            inputBox = new InputBox("Значение:");
            resultString = inputBox.getString();
            if (int.TryParse(resultString, out resultInt))
                tempInsertedTag.Data = resultInt;
            else
            {
                if (double.TryParse(resultString, out resultDouble))
                    tempInsertedTag.Data = resultDouble;
                else
                {
                    if (bool.TryParse(resultString, out resultBool))
                        tempInsertedTag.Data = resultBool;
                    else
                        tempInsertedTag.Data = resultString;
                }
            }
            tempInsertedTag.Fullpath = $"{tagItemToChange.Fullpath}.{tempInsertedTag.Name}";
            tempInsertedTag.Level = tagItemToChange.Level + 1;
            return tempInsertedTag;
        }
        /// <summary>
        /// Обновление дерева после изменения
        /// </summary>
        private void TreeUpdateAfterChanges()
        {
            TagItem.UpdatePaths(tagTree);  //обновляем пути fullpath
            FillingTreeView(rootTagTree.Root);
            treeChanged = true; //дерево было изменено - меняем триггер на true
        }
        /// <summary>
        /// кнопка сохранения в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveToFileAsync();
            treeChanged = false; //после сохранения меняем триггер "изменённости дерева" обратно на false
        }
        /// <summary>
        /// Выполняет сохранение файла XML в отдельном потоке
        /// </summary>
        private async void SaveToFileAsync()
        {
            ChangeSavePath();
            await Task.Factory.StartNew(() => SaveToFile(), TaskCreationOptions.LongRunning);
        }
        /// <summary>
        /// Выбор папки сохранения файла
        /// </summary>
        private void ChangeSavePath()
        {
            FolderBrowserDialog fldDialog = new FolderBrowserDialog();
            fldDialog.ShowDialog();         //выбираем только папку сохранения и сохраняем в файл с таймштампом
            savePath = $@"{fldDialog.SelectedPath}\saved{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}.xml";
        }
        /// <summary>
        /// Сохранение XML-файла
        /// </summary>
        private void SaveToFile()
        {
            tagTree.SaveTree(tagTree, savePath);
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as System.Windows.Controls.TreeView;
            // определяем тип выбранного элемента.
            if (tree.SelectedItem is TagItem)
            {
                tagItemToChange = (TagItem)tree.SelectedItem;   //выбираем тег для изменений

            }
        }

        /// <summary>
        /// Действие при закрытии окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (treeChanged)
            {
                string msg = "Данные были изменены. Сохранить данные перед выходом?";
                MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Закрытие окна", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel)
                {
                    // Если пользователь не хочет закрывать - не закрываем окно
                    e.Cancel = true;
                }
                if (result == MessageBoxResult.Yes)
                {
                    // Если пользователь хочет сохранить перед выходом
                    ChangeSavePath();
                    SaveToFile();
                }
                if (result == MessageBoxResult.No)
                {
                    // Если пользователь не хочет сохранять - закрываем окно
                }
            }
        }
    }
}
