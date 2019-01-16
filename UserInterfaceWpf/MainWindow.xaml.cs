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
        string savePath;        //путь сохранения
        public MainWindow()
        {
            InitializeComponent();
            rootTagTree.RootPath = tagTree;     //пустое дерево
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
                rootTagTree.RootPath = tagTree;
            }
        }
        /// <summary>
        /// Биндинг treeView деревом
        /// </summary>
        /// <param name="tagTreeTemp">дерево, которое надо отобразить</param>
        private void FillingTreeView(TagItem tagTreeTemp)
        {
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
        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is System.Windows.Controls.TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e) //удаление из контекстного меню
        {           //вывод окна подтверждения удаления
            if (System.Windows.MessageBox.Show("Вы действительно хотите удалить предков?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
            {
                if (treeViewItemToChange != null)
                {
                    treeViewItemToChange.Focus();
                    treeViewItemToChange.ItemsSource = null;
                }
            }
        }

        private void MenuItemChangeName_Click(object sender, RoutedEventArgs e)
        {
            //treeViewItemToChange
        }

        private void MenuItemAddTag_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewItemToChange != null)
            {
                TagItem tempInsertedTag = new TagItem();
                List <TagItem> insertTagList= new List<TagItem> { tempInsertedTag };
                tempInsertedTag.Name = "AddedTag";      //вот тут изменить чтобы можно было задавать имя
                treeViewItemToChange.Focus();
                treeViewItemToChange.ItemsSource = insertTagList;
                
            }
        }
        /// <summary>
        /// кнопка сохранения в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveToFileAsync();
        }
        /// <summary>
        /// Выполняет сохранение файла XML в отдельном потоке
        /// </summary>
        private async void SaveToFileAsync()
        {
            FolderBrowserDialog fldDialog = new FolderBrowserDialog();
            fldDialog.ShowDialog();
            savePath = $@"{fldDialog.SelectedPath}\saved.xml"; //выбираем только папку
            await Task.Factory.StartNew(() => SaveToFile(), TaskCreationOptions.LongRunning);
        }
        /// <summary>
        /// Сохранение XML-файла
        /// </summary>
        private void SaveToFile()
        {
            tagTree.SaveTree(tagTree, savePath);
        }
    }
}
