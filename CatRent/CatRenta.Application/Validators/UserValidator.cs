using CatRenta.Application.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CatRenta.Application.Validators
{
    public class UserValidator : AbstractValidator<CatVM>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name)
                .Must(BeAValidName)
                .WithMessage("Введене неправильне ім\'я");

            RuleFor(user => user.Price)
                .Must(BeAValidPrice)
                .WithMessage("Помилка при введенні ціни");
            
            RuleFor(cat => cat.Birthday)
                .Must(BeValidDate)
                .WithMessage("Неправильна дата");
        }

        
        private static bool BeAValidName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var regex = new Regex(@"^(?:([А-Яа-я])(?!\1))*$");
                return regex.IsMatch(name);
            }
            return false;
        }

        private static bool BeAValidPrice(decimal price)
        {
            if (price>0)
            {
                var regex = new Regex(@"\d");
                return regex.IsMatch(price.ToString());
            }
            return false;
        }
        private static bool BeValidDate(DateTime date)
        {
            int current = DateTime.Now.Year;
            int testyear = date.Year;
            if (testyear <= current)
            {
                return true;
            }
            return false;

        }

    }
}