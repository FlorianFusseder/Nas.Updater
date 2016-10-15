using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Nas.Updater.GUI
{
    partial class MainWindowViewModel
    {
        private ICommand _start;

        public ICommand Start => _start ?? (_start = new RelayCommand(Calc, (obj) => IsAllowed && ButtonAn));

        private void Calc(object obj)
        {
            var t = new Task(
            delegate
            {
                var watch = new Stopwatch();
                watch.Start();
                for (var i = 0; i < 1000000; i++)
                {
                    Zahl += i + 1;
                }

                watch.Stop();
                Zeit = watch.Elapsed.ToString();
            }
            );

            t.Start();

        }
    }
}
