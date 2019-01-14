using System;
using System.Collections.Generic;
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

namespace UserInterfaceWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string openPath;    //путь к открытому файлу
        TagItem tagTree = new TagItem();
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Открытие XML-файла и загрузка его в TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog myDialog = new Microsoft.Win32.OpenFileDialog();
            myDialog.Filter = "XML-документ(*.XML)|*.XML" + "|Все файлы (*.*)|*.*";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                openPath = myDialog.FileName;   //назначаем путь к файлу
                tagTree = tagTree.LoadTree(openPath);       //загружаем из файла наш XML
                TagStorage rootTagTree = new TagStorage();
                rootTagTree.RootPath = tagTree;
                FillingTreeView(rootTagTree.Root);
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
    }
}
