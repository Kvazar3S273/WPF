using CatRenta.Application;
using CatRenta.Application.Interfaces;
using CatRenta.EFData;
using CatRenta.Infrastructure.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        // Створюємо подію синхронізації потоку, якою будемо управляти вручну
        ManualResetEvent _mrse = new ManualResetEvent(false);
        
        readonly ManualResetEvent _inwork = new ManualResetEvent(false);
        
        private readonly CatVM edit = new CatVM();

        // Лічильник кількості натискань кнопки
        private int countPush { get; set; } = 0;
        public int _id { get; set; }

        // Кількість котів, що будемо додавати за один раз
        public int moreCats { get; set; }
        public int _idCat { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            _catService.EventInsertItem += UpdateUIAsync;
        }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblInfoStatus.Text = "Підключення до БД......";
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Запускаємо асинхронний потік підрахунку котів в БД
            await Task.Run(() =>
            {
                _context.Cats.Count();
            });
            stopWatch.Stop();

            // Обраховуємо затрачений на це час
            TimeSpan ts = stopWatch.Elapsed;
            
            // Форматуємо відображення затраченого часу
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            lblCursorPosition.Text = elapsedTime;
            lblInfoStatus.Text = "Підключення до БД пройшло успішно";
            
            // Підраховуємо скільки в базі є котів на даний момент
            int catCount = _context.Cats.Count();
            lblDBCount.Text = $"Котів у базі: {catCount}";
            
            // Запускаємо асинхронний потік заповнення БД котів
            await DataSeed.SeedDataAsync(_context);

            // Обраховуємо затрачений час на отримання котів з БД
            stopWatch = new Stopwatch();
            stopWatch.Start();

            // Отримуємо котів з бази
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

            // Форматуємо знайдений час і виводимо його
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            lblCursorPosition.Text = elapsedTime;
            lblInfoStatus.Text = "Читання даних із БД успішно";

            // Виводимо отриману множину котів у вигляді колекції в датагрід
            _cats = new ObservableCollection<CatVM>(list);
            dgSimple.ItemsSource = _cats;
        }
        
        // ручні методи управління подіями 
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
            btnAddRange.IsEnabled = false;
            // Запускаємо додавання
            Resume();
            // Задаємо кількість котів, яку хочемо додати
            moreCats = 1000;
            // Задаємо параметри для прогрес бара
            pbCats.Maximum = moreCats;
            // Запускаємо асинхронний потік додавання котів
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
            btnAddRange.IsEnabled = true;
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
            }));
        }
    }
}
