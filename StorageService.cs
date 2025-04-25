using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AdminPanel.Services
{
    public interface IStorageService
    {
        string Get(string key);
        void Set(string key, string value);
        void Remove(string key);
    }

    public class StorageService : IStorageService
    {
        private readonly string XMLFilePath = Path.Combine(Environment.CurrentDirectory, "ApplicationStorage.xml"); 

        public string Get(string key)
        {
            throw new NotImplementedException();
        }
        public void Set(string key, string value)
        {
            var xmlFile = new XmlDocument();
            xmlFile.Load(XMLFilePath);
            if (xmlFile == null) throw new ArgumentException("Файл не найден!");

            // Получение корневого элемента
            var root = xmlFile.DocumentElement;
            if (root == null) throw new ArgumentException("Не найден корневой элемент в файле!");

            // Создание нового элемента
            var newElement = xmlFile.CreateElement(key);
            newElement.InnerText = value;

            // Добавление элемента в корень
            root.AppendChild(newElement);

            // Сохранение изменений
            xmlFile.Save(XMLFilePath);
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

      
    }
}
