using CatRenta.Application;
using CatRenta.Application.Interfaces;
using CatRenta.EFData;
using CatRenta.Domain;
using CatRenta.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
        private readonly CatVM edit = new CatVM();
        private BackgroundWorker worker = null;
        readonly ManualResetEvent _inwork = new ManualResetEvent(false);
        public int _id { get; set; }
        public int moreCats { get; set; }

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
                    ImageUrl = x.Image,
                    Price = x.AppCatPrices
                    .OrderByDescending(x=>x.DateCreate)
                    .FirstOrDefault().Price
                }).ToList();
            _cats = new ObservableCollection<CatVM>(list);
            dgSimple.ItemsSource = _cats;
        }
        private void Cat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCatWindow addCat = new AddCatWindow(this._cats);
            addCat.Show();
        }

        private void btnAddRange_Click(object sender, RoutedEventArgs e)
        {
            pbStatus.Value = 0;
            worker = new BackgroundWorker();
            moreCats = int.Parse(tbHowCats.Text);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            StartWorker();
        }
        
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgSimple.SelectedItem!=null)
            {
                if(dgSimple.SelectedItem is CatVM)
                {
                    var catView = dgSimple.SelectedItem as CatVM;
                    int id = catView.Id;
                    _id = id;
                }
            }

            EditCat editCat = new EditCat(_id);
            editCat.ShowDialog();
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

        private void StartWorker()
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
            _inwork.Set();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                tbStatus.Text = "Відміна";
            }
            else
            {
                tbStatus.Text = "Котів додано в БД";
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(worker.IsBusy)
            {
                worker.CancelAsync();
                _inwork.Set();
                pbStatus.Value = 0;
            }
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ICatService catService = new CatService();

            for (int i = 1; i <= moreCats; i++)
            {
                _inwork.WaitOne();
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                catService.InsertCats(i);
                int progressPercentage = Convert.ToInt32((double)i * 100 / moreCats);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
                Thread.Sleep(50);
            }
        }
    }
}
