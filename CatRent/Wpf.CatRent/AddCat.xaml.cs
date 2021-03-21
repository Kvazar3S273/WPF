using CatRenta.Application;
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
        public string FileName { get; set; }

        public AddCat()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog()==true)
            {
                FileName = openFileDialog.FileName;
                string file = FileName.Substring(2);
                File.Copy(FileName, $"D:\\ШАГ\\0 Repository\\WPF\\CatRent\\Wpf.CatRent\\bin\\Debug\\netcoreapp3.1\\images\\{file}");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _cats.Add(new CatVM
            {
                Name = tbName.Text,
                Birthday = (DateTime)dpDate.SelectedDate,
                Details = tbDetails.Text,
                ImageUrl = FileName.Substring(2)
            });
            _context.Add(_cats);
            _context.SaveChanges();
        }
    }
}
