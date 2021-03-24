using CatRenta.Application;
using CatRenta.Domain;
using CatRenta.EFData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для AddCat.xaml
    /// </summary>
    public partial class AddCat : Window
    {
        private ObservableCollection<CatVM> _cats = new ObservableCollection<CatVM>();
        private EFDataContext _context = new EFDataContext();
        /// <summary>
        /// Повний шлях до файла
        /// </summary>
        public string FileName { get; set; }
        private bool _gender { get; set; }
        public AddCat()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            
        }

        // Вибір фото кота
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog()==true)
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

        // Збереження нового кота
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var cats = new List<AppCat>
                {
                    new AppCat
                        {
                        Name = tbName.Text,
                        Gender = _gender,
                        Birthday = (DateTime)dpDate.SelectedDate,
                        Details = tbDetails.Text,
                        Image = FileName
                        }
                };
            foreach (var cat in cats)
            {
                _context.Add(cat);
                _context.SaveChanges();
            }
            this.Close();
        }

        // Виставлення статі кота за радіобаттонами
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = (RadioButton)sender;
            _gender = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = (RadioButton)sender;
            _gender = false;
        }
    }
}
