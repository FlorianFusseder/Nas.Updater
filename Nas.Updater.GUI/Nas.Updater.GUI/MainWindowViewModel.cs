using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nas.Updater.GUI
{
    partial class MainWindowViewModel : BaseViewModel
    {
        private bool _isAllowed;

        public bool IsAllowed
        {
            get { return _isAllowed; }
            set
            {
                _isAllowed = value;
                OnPropertyChanged();
            }
        }

        private bool _buttonAn;

        public bool ButtonAn
        {
            get { return _buttonAn; }
            set
            {
                _buttonAn = value;
                OnPropertyChanged();
            }
        }
        private long _zahl;

        public long Zahl
        {
            get { return _zahl; }
            set
            {
                _zahl = value;
                OnPropertyChanged();
            }
        }

        private string _zeit;

        public string Zeit
        {
            get { return _zeit; }
            set
            {
                _zeit = value;
                OnPropertyChanged();
            }
        }


        public MainWindowViewModel()
        {
            _isAllowed = true;
            ButtonAn = false;
            Zeit = "N/A";
        }
    }
}
