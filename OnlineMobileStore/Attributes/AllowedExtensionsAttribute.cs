﻿using System.ComponentModel.DataAnnotations;

namespace OnlineMobileStore.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string _allowedExtensions;
        public AllowedExtensionsAttribute(string allowedExtensions) 
        {
              _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult? 
            IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                var isallowed = _allowedExtensions.Split(',').Contains(extension,StringComparer.OrdinalIgnoreCase);
                if(!isallowed)
                {
                    return new ValidationResult("Only .jpg .jpeg .png files are allowed");
                }
            }
            return ValidationResult.Success;
        }
    }
}