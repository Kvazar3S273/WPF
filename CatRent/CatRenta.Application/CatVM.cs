using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatRenta.Application.Validators;
using FluentValidation;

namespace CatRenta.Application
{
    public class CatVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private int _id;
        private string _name;
        private DateTime _birthday;
        private string _details;
        private string _imageUrl;
        private decimal _price;
        public bool EnableValidation { get; set; }
        private readonly UserValidator _userValidator;

        public CatVM()
        {
            _userValidator = new UserValidator();
        }

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }


        public DateTime Birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                OnPropertyChanged("Birthday");
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged("Details");
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged("ImageUrl");
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }
        public string this[string columnName]
        {
            get
            {
                if (EnableValidation)
                {
                    var firstOrDefault = _userValidator.Validate(this)
                        .Errors.FirstOrDefault(lol => lol.PropertyName == columnName);
                    if (firstOrDefault != null)
                        return _userValidator != null ? firstOrDefault.ErrorMessage : "";
                }
                return "";
            }
        }

        public string Error
        {
            get
            {
                if (_userValidator != null)
                {
                    if (EnableValidation)
                    {
                        var results = _userValidator.Validate(this);
                        if (results != null && results.Errors.Any())
                        {
                            var errors = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());
                            return errors;
                        }
                    }
                }
                return string.Empty;
            }
        }

        //public void NotifyPropertyChanged(string propName)
        //{
        //    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //}
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
