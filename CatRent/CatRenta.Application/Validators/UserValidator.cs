using CatRenta.Application.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CatRenta.Application.Validators
{
    public class UserValidator : AbstractValidator<UserViewModel>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name)
                .Must(BeAValidName)
                .WithMessage("Введене неправильне ім\'я");

            RuleFor(user => user.Email)
                .EmailAddress()
                .WithMessage("Please Specify a Valid E-Mail Address");

            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Please Specify a Email.");

            RuleFor(user => user.Zip)
                .Must(BeAValidZip)
                .WithMessage("Please Enter a Valid Zip Code");


        }

        private static bool BeAValidZip(string zip)
        {
            if (!string.IsNullOrEmpty(zip))
            {
                var regex = new Regex(@"\d{5}");
                return regex.IsMatch(zip);
            }
            return false;
        }

        private static bool BeAValidName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var regex = new Regex(@"\D");
                return regex.IsMatch(name);
            }
            return false;
        }

    }
}