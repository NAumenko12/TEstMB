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
using System.Windows.Navigation;
using TEstMB.Model;
using TEstMB.View;

namespace TEstMB.ViewModel
{
    internal class EditOrderViewModel
    {
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Testt; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";
        private Заявки _selectedЗаявка;
        private ObservableCollection<Пользователи> _мастера;
        private string _комментарий;

        public ICommand GoNavigateToOrderCommand { get; }
        public EditOrderViewModel(Заявки selectedЗаявка)
        {
            SelectedЗаявка = selectedЗаявка;
            LoadMaster();
            LoadComent();
            SaveCommand = new RelayCommand(Save);
            GoNavigateToOrderCommand = new RelayCommand(NavigateToOrderBack);
        }

        public Заявки SelectedЗаявка
        {
            get => _selectedЗаявка;
            set
            {
                _selectedЗаявка = value;
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

        public string Комментарий
        {
            get => _комментарий;
            set
            {
                _комментарий = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private void LoadMaster()
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

        private void LoadComent()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    Комментарий
                FROM 
                    Комментарии
                WHERE 
                    FK_Заявки = @ID_Заявки";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Заявки", SelectedЗаявка.ID_Заявки);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Комментарий = reader.GetString(0);
                        }
                    }
                }
            }
        }

        private void Save(object parameter)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string updateQuery = @"
                UPDATE 
                    Заявки
                SET 
                    Описание_проблемы = @Описание_проблемы, 
                    Статус_заявки = @Статус_заявки, 
                    FK_Мастера = @FK_Мастера
                WHERE 
                    ID_Заявки = @ID_Заявки";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Описание_проблемы", SelectedЗаявка.Описание_проблемы);
                    updateCommand.Parameters.AddWithValue("@Статус_заявки", SelectedЗаявка.Статус_заявки);
                    updateCommand.Parameters.AddWithValue("@FK_Мастера", SelectedЗаявка.FK_Мастера);
                    updateCommand.Parameters.AddWithValue("@ID_Заявки", SelectedЗаявка.ID_Заявки);
                    updateCommand.ExecuteNonQuery();
                }

                string updateCommentQuery = @"
                UPDATE 
                    Комментарии
                SET 
                    Комментарий = @Комментарий
                WHERE 
                    FK_Заявки = @ID_Заявки";
                using (SqlCommand updateCommentCommand = new SqlCommand(updateCommentQuery, connection))
                {
                    updateCommentCommand.Parameters.AddWithValue("@Комментарий", Комментарий);
                    updateCommentCommand.Parameters.AddWithValue("@ID_Заявки", SelectedЗаявка.ID_Заявки);
                    updateCommentCommand.ExecuteNonQuery();
                }
            }
            NavigateToOrderBeforeSave();
        }
        private void NavigateToOrderBack(object parametr)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var orderWindow = new OrderWindow();
                mainWindow.MainContent.Content = orderWindow;
            }
        }
        private void NavigateToOrderBeforeSave()
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
