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
using Wpf.CatRent.Views;

namespace Wpf.CatRent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<CatVM> _cats = new ObservableCollection<CatVM>();
        private EFDataContext _context = new EFDataContext();
        public int _idCat { get; set; }
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
                    Id = x.Id,
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
            AddCatWindow addCat = new AddCatWindow(this._cats);
            addCat.Show();
        }
        private void btnValidation_Click(object sender, RoutedEventArgs e)
        {
            UserView window = new UserView();
            window.Show();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Запускаємо форму редагування
            EditCat editCat = new EditCat();
            // Якщо результат роботи форми позитивний, проводимо заміну полів
            if(editCat.ShowDialog()==true)
            {
                if (dgSimple.SelectedItem != null)
                {
                    if (dgSimple.SelectedItem is CatVM)
                    {
                        var userView = dgSimple.SelectedItem as CatVM;
                        userView.Details = editCat.ChangeDetails;
                        userView.ImageUrl = editCat.ChangeImage;
                        _idCat = userView.Id;
                    }
                }
            }
            // Зберігаємо всі зміни в БД
            var cat = _context.Cats.SingleOrDefault(c => c.Id == _idCat);
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
        // Оновляємо датагрід
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }

        // Видалення кота з поля, на якому стоїть курсор
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgSimple.SelectedItem != null)
            {
                if (dgSimple.SelectedItem is CatVM)
                {
                    var userView = dgSimple.SelectedItem as CatVM;
                    int id = userView.Id;
                    _idCat = id;
                    var cat = _context.Cats.SingleOrDefault(c => c.Id == id);
                    _context.Cats.Remove(cat);
                    _context.SaveChanges();
                }
            }
        }
    }
}
