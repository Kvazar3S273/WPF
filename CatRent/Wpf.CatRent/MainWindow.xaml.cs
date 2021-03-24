using CatRenta.Application;
using CatRenta.EFData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Wpf.CatRent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<CatVM> _cats = new ObservableCollection<CatVM>();
        private EFDataContext _context = new EFDataContext();
        public int _idChangedCat { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataSeed.SeedDataAsync(_context);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = _context.Cats
                .Select(x => new CatVM()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Birthday = x.Birthday,
                    Details = x.Details,
                    ImageUrl = x.Image
                }).ToList();
            _cats = new ObservableCollection<CatVM>(list);
            dgSimple.ItemsSource = _cats;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCat addCat = new AddCat();
            addCat.Show();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditCat editCat = new EditCat();
            if(editCat.ShowDialog()==true)
            {
                if (dgSimple.SelectedItem != null)
                {
                    if (dgSimple.SelectedItem is CatVM)
                    {
                        var userView = dgSimple.SelectedItem as CatVM;
                        userView.Details = editCat.ChangeDetails;
                        userView.ImageUrl = editCat.ChangeImage;
                        _idChangedCat = int.Parse(userView.Id);
                    }
                }
            }
            var cat = _context.Cats.SingleOrDefault(c => c.Id == _idChangedCat);
            if(editCat.IsChangeName)
            {
                cat.Name = editCat.ChangeName;
            }
            if(editCat.IsChangeDetails)
            {
                cat.Details = editCat.ChangeDetails;
            }
            if(editCat.IsChangeImage)
            {
                cat.Image = editCat.ChangeImage;
            }
            _context.SaveChanges();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
