using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Timer
{
    public partial class MainViewModel : IDataErrorInfo
    {
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(SaatBaşlangıç):
                        if (!DateTime.TryParseExact(SaatBaşlangıç, "H:m", new CultureInfo("tr-TR"), DateTimeStyles.None, out _))
                        {
                            error = "Başlangıç Saati Doğru Girilmedi";
                        }

                        break;

                    case nameof(SaatBitiş):
                        if (!DateTime.TryParseExact(SaatBitiş, "H:m", new CultureInfo("tr-TR"), DateTimeStyles.None, out _))
                        {
                            error = "Bitiş Saati Doğru Girilmedi";
                        }

                        break;
                }

                return error;
            }
        }
    }

    public partial class MainViewModel : InpcBase
    {
        private string saatBaşlangıç;

        private string saatBitiş;

        private DateTime fark;

        public string SaatBaşlangıç
        {
            get { return saatBaşlangıç; }

            set
            {
                if (saatBaşlangıç != value)
                {
                    saatBaşlangıç = value;
                    OnPropertyChanged(nameof(SaatBaşlangıç));
                }
            }
        }

        public string SaatBitiş
        {
            get { return saatBitiş; }
            set
            {
                if (saatBitiş != value)
                {
                    saatBitiş = value;
                    OnPropertyChanged(nameof(SaatBitiş));
                }
            }
        }

        public DateTime Fark
        {
            get { return fark; }

            set
            {
                if (fark != value)
                {
                    fark = value;
                    OnPropertyChanged(nameof(Fark));
                }
            }
        }

        public string Sonuç
        {
            get { return sonuç; }

            set
            {
                if (sonuç != value)
                {
                    sonuç = value;
                    OnPropertyChanged(nameof(Sonuç));
                }
            }
        }

        private DispatcherTimer timer;

        private DispatcherTimer timer2;

        private string sonuç;

        private static DateTime birinciaralık;

        private static DateTime ikinciaralık;

        public MainViewModel()
        {
            ZamanlamaBaşlat = new RelayCommand(parameter =>
            {
                try
                {
                    var başlangıç = DateTime.ParseExact(SaatBaşlangıç, "H:m", null);
                    var bitiş = DateTime.ParseExact(SaatBitiş, "H:m", null);

                    if (başlangıç > bitiş)
                    {
                        MessageBox.Show("Başlangıç Saati Bitiş Saatinden Küçük Olmalıdır.");
                        return;
                    }
                    Fark = bitiş.AddTicks(-başlangıç.Ticks);
                    birinciaralık = Fark;
                    ikinciaralık = Fark.AddDays(1).AddTicks(-Fark.Ticks * 2);
                    timer = new DispatcherTimer
                    {
                        Interval = new TimeSpan(0, 0, 1),
                    };
                    timer2 = new DispatcherTimer
                    {
                        Interval = new TimeSpan(0, 0, 1),
                    };
                    timer.Tick += Timer_Tick;
                    timer2.Tick += Timer2_Tick;
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }, parameter => timer?.IsEnabled != true);
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            ikinciaralık = ikinciaralık.AddSeconds(-1);
            Sonuç = ikinciaralık.ToString("H:mm:ss");
            if (ikinciaralık.Ticks == 0)
            {
                ikinciaralık = Fark.AddMinutes(2).AddTicks(-Fark.Ticks * 2);
                timer2.Stop();
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            birinciaralık = birinciaralık.AddSeconds(-1);
            Sonuç = birinciaralık.ToString("H:mm:ss");
            if (birinciaralık.Ticks == 0)
            {
                birinciaralık = Fark;
                timer.Stop();
                timer2.Start();
            }
        }

        public ICommand ZamanlamaBaşlat { get; }
    }
}