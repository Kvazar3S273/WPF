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
using System.Diagnostics;

namespace Wpf.CatRent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<CatVM> _cats = new ObservableCollection<CatVM>();
        private EFDataContext _context = new EFDataContext();
        private ICatService _catService = new CatService();
        ManualResetEvent _mrse = new ManualResetEvent(false);
        //bool abort = false;
        private readonly CatVM edit = new CatVM();
        //private BackgroundWorker worker = null;
        readonly ManualResetEvent _inwork = new ManualResetEvent(false);
        private int countPush { get; set; } = 0;

        public int _id { get; set; }
        public int moreCats { get; set; }

        public int _idCat { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            //DataSeed.SeedDataAsync(_context);
            _catService.EventInsertItem += UpdateUIAsync;
        }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblInfoStatus.Text = "Підключення до БД......";
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            await Task.Run(() =>
            {
                _context.Cats.Count();
            });
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            lblCursorPosition.Text = elapsedTime;
            lblInfoStatus.Text = "Підключення до БД пройшло успішно";
            int catCount = _context.Cats.Count();
            lblDBCount.Text = $"Котів у базі: {catCount}";
            await DataSeed.SeedDataAsync(_context);

            stopWatch = new Stopwatch();
            stopWatch.Start();

            var list = _context.Cats.AsQueryable()
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
                })
                //.OrderBy(x=>x.Name)
                .ToList();
            stopWatch.Stop();
            ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            //Debug.WriteLine("Сідер 1 закінчив свою роботу: " + elapsedTime);
            lblCursorPosition.Text = elapsedTime;
            lblInfoStatus.Text = "Читання даних із БД успішно";

            _cats = new ObservableCollection<CatVM>(list);
            dgSimple.ItemsSource = _cats;
        }
        //private void Cat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        public void Resume() => _mrse.Set();
        public void Pause() => _mrse.Reset();
        /// <summary>
        /// Кнопка "пауза"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPauseAddRange_Click(object sender, RoutedEventArgs e)
        {
            countPush++;
            
            if (countPush %2 == 0)
            {
                btnPauseAddRange.Content = "Пауза";
                Resume();
            }
            else
            {
                btnPauseAddRange.Content = "Продовжити";
                Pause();
            }
        }
        /// <summary>
        /// Кнопка "додати одного кота"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCatWindow addCat = new AddCatWindow(this._cats);
            addCat.Show();
        }
        /// <summary>
        /// Кнопка "додати багато котів"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAddRange_Click(object sender, RoutedEventArgs e)
        {
            ///pbStatus.Value = 0;
            ///worker = new BackgroundWorker();
            ///moreCats = int.Parse(tbHowCats.Text);
            ///worker.WorkerReportsProgress = true;
            ///worker.WorkerSupportsCancellation = true;
            ///worker.DoWork += worker_DoWork;
            ///worker.ProgressChanged += worker_ProgressChanged;
            ///worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            ///StartWorker();
            ///
            btnAddRange.IsEnabled = false;
            Resume();
            moreCats = 500;
            pbCats.Maximum = moreCats;
            await _catService.InsertCatsAsync(moreCats, _mrse);
            btnAddRange.IsEnabled = true;
        }
        
        /// <summary>
        /// Кнопка "редагувати кота"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Кнопка "оновити грід"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }

        // Видалення кота з поля, на якому стоїть курсор
        /// <summary>
        /// Кнопка "видалити кота"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Кнопка "відміна додавання багатьох котів"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ///if(worker.IsBusy)
            ///{
            ///    worker.CancelAsync();
            ///    _inwork.Set();
            ///    pbStatus.Value = 0;
            ///}

            btnAddRange.Content = "Додати одним махом";
            pbCats.Value = 0;
            _catService.CanselAsyncMethod = true;
        }

        void UpdateUIAsync(int i)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                btnAddRange.Content = $"{i}";
                pbCats.Value = i;
                //Debug.WriteLine("Thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            }));

        }

        ///private void worker_DoWork(object sender, DoWorkEventArgs e)
        ///{
        ///    ICatService catService = new CatService();
        ///    for (int i = 1; i <= moreCats; i++)
        ///    {
        ///        _inwork.WaitOne();
        ///        if (worker.CancellationPending)
        ///        {
        ///            e.Cancel = true;
        ///            break;
        ///        }
        ///        catService.InsertCats(i);
        ///        int progressPercentage = Convert.ToInt32((double)i * 100 / moreCats);
        ///        (sender as BackgroundWorker).ReportProgress(progressPercentage);
        ///        Thread.Sleep(50);
        ///    }
        ///}
        ///

        ///private void StartWorker()
        ///{
        ///    if (!worker.IsBusy)
        ///    {
        ///        worker.RunWorkerAsync();
        ///    }
        ///    _inwork.Set();
        ///}

        ///private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        ///{
        ///    if (e.Cancelled)
        ///    {
        ///        tbStatus.Text = "Відміна";
        ///    }
        ///    else
        ///    {
        ///        tbStatus.Text = "Котів додано в БД";
        ///    }
        ///}

        ///private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        ///{
        ///    pbStatus.Value = e.ProgressPercentage;
        ///}

    }
}
