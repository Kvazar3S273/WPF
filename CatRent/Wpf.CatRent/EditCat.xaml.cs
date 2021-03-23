using CatRenta.Application;
using CatRenta.Domain;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf.CatRent
{
    /// <summary>
    /// Логика взаимодействия для EditCat.xaml
    /// </summary>
    public partial class EditCat : Window
    {
        public string ChangeDetails { get; set; }
        public string ChangeImage { get; set; }
        public string FileName { get; set; }
        public EditCat()
        {
            InitializeComponent();
        }

        // Вибір нового фото кота
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName; // визначаємо шлях до файла
                // нова папка (нове місце призначення файла)
                string newPath = "D:\\ШАГ\\0 Repository\\WPF\\CatRent\\Wpf.CatRent\\bin\\Debug\\netcoreapp3.1\\images\\";
                char ch = '\\'; // будемо шукати символ \
                string file = FileName; // назва файла (без шляху)
                int indexOfChar = file.IndexOf(ch); // індекс символа \
                do // проходимо весь шлях до назви файла
                {
                    indexOfChar = file.IndexOf(ch);
                    file = file.Substring(indexOfChar + 1); // виділяємо файл окремо від шляху
                } while (indexOfChar > 0);
                newPath += file;    // створюємо новий шлях для файла

                if (!File.Exists(newPath))
                {
                    File.Copy(FileName, newPath); // копіюємо файл в папку проекта
                }
                FileName = newPath; // замінюємо шлях до файла
            }
        }

        // Збереження відредагованих даних про кота
        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            ChangeDetails = tbChangeDetails.Text;
            ChangeImage = FileName;
            //MessageBox.Show(ChangeDetails);
            //MessageBox.Show(ChangeImage);
            DialogResult = true;
            this.Close();
        }
    }
}
