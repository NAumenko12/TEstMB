using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TEstMB.Model;

namespace TEstMB.ViewModel
{
    internal class StatisticViewModel
    {
        private readonly string _connectionString = @"Data Source=EUGENE; DataBase=Testt; Integrated Security=True; Trusted_Connection=true; MultipleActiveResultSets=true; TrustServerCertificate=true; encrypt=false;";
        private ObservableCollection<Заявки> _выполненныеЗаявки;
        private int _количествоВыполненныхЗаявок;
        private TimeSpan _среднееВремяВыполнения;

        public StatisticViewModel()
        {
            LoadСтатистика();
        }

        public ObservableCollection<Заявки> ВыполненныеЗаявки
        {
            get => _выполненныеЗаявки;
            set
            {
                _выполненныеЗаявки = value;
                OnPropertyChanged();
            }
        }

        public int КоличествоВыполненныхЗаявок
        {
            get => _количествоВыполненныхЗаявок;
            set
            {
                _количествоВыполненныхЗаявок = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan СреднееВремяВыполнения
        {
            get => _среднееВремяВыполнения;
            set
            {
                _среднееВремяВыполнения = value;
                OnPropertyChanged();
            }
        }

        private void LoadСтатистика()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    z.ID_Заявки, 
                    z.Дата_добавления, 
                    z.Дата_окончания
                FROM 
                    Заявки z
                WHERE 
                    z.Статус_заявки IN ('Выполнено', 'Готова к выдаче','Готов к выдаче' )";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var выполненныеЗаявки = new ObservableCollection<Заявки>();
                        long общаяРазницаTicks = 0;
                        int количествоЗаявок = 0;

                        while (reader.Read())
                        {
                            var заявка = new Заявки
                            {
                                ID_Заявки = reader.GetInt32(0),
                                Дата_добавления = reader.GetDateTime(1),
                                Дата_окончания = reader.GetDateTime(2)
                            };
                            выполненныеЗаявки.Add(заявка);

                            TimeSpan разница = (TimeSpan)(заявка.Дата_окончания - заявка.Дата_добавления);
                            общаяРазницаTicks += разница.Ticks;
                            количествоЗаявок++;
                        }

                        ВыполненныеЗаявки = выполненныеЗаявки;
                        КоличествоВыполненныхЗаявок = количествоЗаявок;

                        if (количествоЗаявок > 0)
                        {
                            СреднееВремяВыполнения = TimeSpan.FromTicks(общаяРазницаTicks / количествоЗаявок);
                        }
                    }
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

