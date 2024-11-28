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
using TEstMB;
using TEstMB.Model;
using TEstMB.View;

namespace TEstMB.ViewModel
{
    internal class AddOrderViewModel
    {
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Testt; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";
        private Заявки _newЗаявка;
        private string _комментарий;
        private ObservableCollection<Пользователи> _мастера;
        private ObservableCollection<Пользователи> _клиенты;
        private ObservableCollection<Запчасти> _запчасти;

        public AddOrderViewModel()
        {
            NewЗаявка = new Заявки();
            LoadМастера();
            LoadКлиенты();
            LoadЗапчасти();
            SaveCommand = new RelayCommand(Save);
        }

        public Заявки NewЗаявка
        {
            get => _newЗаявка;
            set
            {
                _newЗаявка = value;
                OnPropertyChanged();
            }
        }

        public string Комментарий
        {
            get => _комментарий;
            set
            {
                _комментарий = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Пользователи> Мастера
        {
            get => _мастера;
            set
            {
                _мастера = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Пользователи> Клиенты
        {
            get => _клиенты;
            set
            {
                _клиенты = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Запчасти> Запчасти
        {
            get => _запчасти;
            set
            {
                _запчасти = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private void LoadМастера()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    ID_Пользователя, 
                    ФИО
                FROM 
                    Пользователи
                WHERE 
                    FK_Роли = 2";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var мастера = new ObservableCollection<Пользователи>();
                        while (reader.Read())
                        {
                            var мастер = new Пользователи
                            {
                                ID_Пользователя = reader.GetInt32(0),
                                ФИО = reader.GetString(1)
                            };
                            мастера.Add(мастер);
                        }
                        Мастера = мастера;
                    }
                }
            }
        }

        private void LoadКлиенты()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    ID_Пользователя, 
                    ФИО
                FROM 
                    Пользователи
                WHERE 
                    FK_Роли = 3";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var клиенты = new ObservableCollection<Пользователи>();
                        while (reader.Read())
                        {
                            var клиент = new Пользователи
                            {
                                ID_Пользователя = reader.GetInt32(0),
                                ФИО = reader.GetString(1)
                            };
                            клиенты.Add(клиент);
                        }
                        Клиенты = клиенты;
                    }
                }
            }
        }

        private void LoadЗапчасти()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    ID_Запчасти, 
                    Название
                FROM 
                    Запчасти";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var запчасти = new ObservableCollection<Запчасти>();
                        while (reader.Read())
                        {
                            var запчасть = new Запчасти
                            {
                                ID_Запчасти = reader.GetInt32(0),
                                Название = reader.GetString(1)
                            };
                            запчасти.Add(запчасть);
                        }
                        Запчасти = запчасти;
                    }
                }
            }
        }

        private void Save(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NewЗаявка.Вид_оргтехники) ||
                string.IsNullOrWhiteSpace(NewЗаявка.Модель) ||
                string.IsNullOrWhiteSpace(NewЗаявка.Описание_проблемы) ||
                string.IsNullOrWhiteSpace(NewЗаявка.Статус_заявки) ||
                NewЗаявка.FK_Запчасти == null ||
                NewЗаявка.FK_Мастера == null ||
                NewЗаявка.FK_Заказчика == null ||
                string.IsNullOrWhiteSpace(Комментарий) ||
                NewЗаявка.Дата_окончания == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO 
                    Заявки (Дата_добавления, Вид_оргтехники, Модель, Описание_проблемы, Статус_заявки, Дата_окончания, FK_Запчасти, FK_Мастера, FK_Заказчика)
                VALUES 
                    (@Дата_добавления, @Вид_оргтехники, @Модель, @Описание_проблемы, @Статус_заявки, @Дата_окончания, @FK_Запчасти, @FK_Мастера, @FK_Заказчика);
                SELECT SCOPE_IDENTITY();";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Дата_добавления", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@Вид_оргтехники", NewЗаявка.Вид_оргтехники);
                    insertCommand.Parameters.AddWithValue("@Модель", NewЗаявка.Модель);
                    insertCommand.Parameters.AddWithValue("@Описание_проблемы", NewЗаявка.Описание_проблемы);
                    insertCommand.Parameters.AddWithValue("@Статус_заявки", NewЗаявка.Статус_заявки);
                    insertCommand.Parameters.AddWithValue("@Дата_окончания", NewЗаявка.Дата_окончания);
                    insertCommand.Parameters.AddWithValue("@FK_Запчасти", NewЗаявка.FK_Запчасти);
                    insertCommand.Parameters.AddWithValue("@FK_Мастера", NewЗаявка.FK_Мастера);
                    insertCommand.Parameters.AddWithValue("@FK_Заказчика", NewЗаявка.FK_Заказчика);

                    int newOrderId = Convert.ToInt32(insertCommand.ExecuteScalar());

                    string insertCommentQuery = @"
                    INSERT INTO 
                        Комментарии (Комментарий, FK_Мастера, FK_Заявки)
                    VALUES 
                        (@Комментарий, @FK_Мастера, @FK_Заявки)";
                    using (SqlCommand insertCommentCommand = new SqlCommand(insertCommentQuery, connection))
                    {
                        insertCommentCommand.Parameters.AddWithValue("@Комментарий", Комментарий);
                        insertCommentCommand.Parameters.AddWithValue("@FK_Мастера", NewЗаявка.FK_Мастера);
                        insertCommentCommand.Parameters.AddWithValue("@FK_Заявки", newOrderId);
                        insertCommentCommand.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Заявка успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            GoHomeNavigate();
        }

        private void GoHomeNavigate()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var orderWindow = new OrderWindow();
                mainWindow.MainContent.Content = orderWindow;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}