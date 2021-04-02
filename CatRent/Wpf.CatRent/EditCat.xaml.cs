using CatRenta.Application;
using CatRenta.Domain;
using CatRenta.EFData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Wpf.CatRent
{
    /// <summary>
    /// Логика взаимодействия для EditCat.xaml
    /// </summary>
    public partial class EditCat : Window
    {
        public string File_Name { get; set; }
        private string selectedFile = string.Empty;
        private EFDataContext _context = new EFDataContext();
        private readonly CatVM cat = new CatVM();
        public int _editedIndex { get; set; }
        public EditCat(int editedIndex)
        {
            InitializeComponent();
            cat.EnableValidation = true;
            _editedIndex = editedIndex;
            DataContext = cat;
        }

        // Вибір нового фото кота
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    selectedFile = openFileDialog.FileName;
                    //MessageBox.Show(selectedFile);
                }
                catch 
                {
                    MessageBox.Show("Файл не відкривається, можливо не підходить формат");
                }
            }
        }

        // Збереження відредагованих даних про кота
        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            var edit = _context.Cats.SingleOrDefault(e => e.Id == _editedIndex);

            if(!string.IsNullOrEmpty(selectedFile))
            {
                string extension = Path.GetExtension(selectedFile);
                string imageNameplusext = Path.GetRandomFileName();
                string imageName = Path.GetFileNameWithoutExtension(imageNameplusext) + extension;
                string dir = Directory.GetCurrentDirectory();
                string saveDir = Path.Combine(dir, "images");
                if (!Directory.Exists(saveDir))
                   Directory.CreateDirectory(saveDir);
                string fileSave = Path.Combine(saveDir, imageName);

                File.Copy(selectedFile, fileSave);

                edit.Image = fileSave;
                //_context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(tbChangeName.Text))
            {
                if (!cat.Error.Equals("Введене неправильне ім\'я"))
                {
                    //MessageBox.Show("Its OK name");
                    edit.Name = tbChangeName.Text;
                    //_context.SaveChanges();
                }
            }

            if (!string.IsNullOrEmpty(tbChangePrice.Text))
            {
                if (!cat.Error.Equals("Помилка при введенні ціни"))
                {
                    //MessageBox.Show("Its OK price");
                    edit.AppCatPrices = new List<AppCatPrice>
                    {
                        new AppCatPrice
                        {
                            CatId=edit.Id,
                            DateCreate=DateTime.Now,
                            Price=decimal.Parse(tbChangePrice.Text)
                        }
                    };
                    //_context.SaveChanges();
                }
            }

            if (dpDate.SelectedDate != null)
            {
                if (!cat.Error.Equals("Некоректна дата!"))
                {
                    edit.Birthday = (DateTime)dpDate.SelectedDate;
                    //_context.SaveChanges();
                }
            }

            if (!string.IsNullOrEmpty(tbChangeDetails.Text))
            {
                edit.Details = tbChangeDetails.Text;
                //_context.SaveChanges();
            }
            _context.SaveChanges();

            Close();
        }
    }
}
