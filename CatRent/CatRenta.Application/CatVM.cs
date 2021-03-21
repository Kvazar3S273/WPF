using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatRenta.Application
{
    public class CatVM : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private DateTime _birthday;
        private string _details;
        private string _imageUrl;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }


        public DateTime Birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                NotifyPropertyChanged("Birthday");
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                NotifyPropertyChanged("Details");
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                NotifyPropertyChanged("ImageUrl");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
