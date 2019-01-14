using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TIK
{
    [Serializable]
    public class TagStorage
    {
        TagItem root;

        /// <summary>
        /// Путь к дереву
        /// </summary>
        public TagItem RootPath
        {
            get { return root.left; }
            set { root.left = value; }
        }
        /// <summary>
        /// Позвращает наследников корневого элемента
        /// </summary>
        public List<TagItem> ChildrensList
        {
            get
            {
                List<TagItem> childrens = new List<TagItem>() { this.root.left};
                return childrens;
            }
        }
        public TagItem Root
        {
            get { return root; }
            set { root = value; }    
        }
        public TagStorage()
        {
            Root=new TagItem();
        }

        //public void Save(TagItem tag)
        //{
        //    var element = new XElement(tag.Fullpath, new XAttribute("Name",tag.name), new XAttribute("Data", tag.data), 
        //         new XElement("Left", tag.left), new XElement("Right", tag.right));
        //    element.Save(@"D:\Tags.xml");
        //}




    }
}
