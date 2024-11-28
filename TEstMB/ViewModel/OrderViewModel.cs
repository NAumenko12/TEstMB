using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TEstMB.Model;
using TEstMB.View;

namespace TEstMB.ViewModel
{
    internal class OrderViewModel
    {
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Testt; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";
        private ObservableCollection<Заявки> _заявки;

        public OrderViewModel()
        {
            LoadЗаявки();
            AddOrderCommand = new RelayCommand(AddOrderNavigate);
            GoHomeNavigateCommand = new RelayCommand(GoHomeNavigate);
            StatisticNavigateCommand = new RelayCommand(StatisticNavigate);
            EditOrderCommand = new RelayCommand(OpenEditProductWindow);
        }

        public ObservableCollection<Заявки> Заявки
        {
            get => _заявки;
            set
            {
                _заявки = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddOrderCommand { get; }
        public ICommand GoHomeNavigateCommand { get; }
        public ICommand StatisticNavigateCommand { get; }
        public ICommand EditOrderCommand { get; }

        private void LoadЗаявки()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    z.ID_Заявки, 
                    z.Дата_добавления, 
                    z.Вид_оргтехники, 
                    z.Модель, 
                    z.Описание_проблемы, 
                    z.Статус_заявки, 
                    z.Дата_окончания, 
                    z.FK_Запчасти, 
                    z.FK_Мастера, 
                    z.FK_Заказчика,
                    p.Название AS Название_запчасти,
                    m.ФИО AS ФИО_мастера,
                    c.ФИО AS ФИО_заказчика
                FROM 
                    Заявки z
                INNER JOIN 
                    Запчасти p ON z.FK_Запчасти = p.ID_Запчасти
                INNER JOIN 
                    Пользователи m ON z.FK_Мастера = m.ID_Пользователя
                INNER JOIN 
                    Пользователи c ON z.FK_Заказчика = c.ID_Пользователя";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var заявки = new ObservableCollection<Заявки>();
                        while (reader.Read())
                        {
                            var заявка = new Заявки
                            {
                                ID_Заявки = reader.GetInt32(0),
                                Дата_добавления = reader.GetDateTime(1),
                                Вид_оргтехники = reader.IsDBNull(2) ? "В работе" : reader.GetString(2),
                                Модель = reader.IsDBNull(3) ? "В работе" : reader.GetString(3),
                                Описание_проблемы = reader.IsDBNull(4) ? "В работе" : reader.GetString(4),
                                Статус_заявки = reader.IsDBNull(5) ? "В работе" : reader.GetString(5),
                                Дата_окончания = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                FK_Запчасти = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                                FK_Мастера = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                FK_Заказчика = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                                Запчасти = new Запчасти { Название = reader.IsDBNull(10) ? "В работе" : reader.GetString(10) },
                                Пользователи = new Пользователи { ФИО = reader.IsDBNull(11) ? "В работе" : reader.GetString(11) },
                                Пользователи1 = new Пользователи { ФИО = reader.IsDBNull(12) ? "В работе" : reader.GetString(12) }
                            };
                            заявки.Add(заявка);
                        }
                        Заявки = заявки;
                    }
                }
            }
        }

        private void AddOrderNavigate(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var addOrderWindow = new AddOrderWindow();
                mainWindow.MainContent.Content = addOrderWindow;
            }
        }

        private void GoHomeNavigate(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var loginWindow = new LoginWindow();
                mainWindow.MainContent.Content = loginWindow;
            }
        }

        private void StatisticNavigate(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var statisticWindow = new StatisticWindow();
                mainWindow.MainContent.Content = statisticWindow;
            }
        }

        private void OpenEditProductWindow(object parameter)
        {
            var product = parameter as Заявки;
            if (product != null)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    var editWindow = new EditOrderWindow(product);
                    mainWindow.MainContent.Content = editWindow;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
