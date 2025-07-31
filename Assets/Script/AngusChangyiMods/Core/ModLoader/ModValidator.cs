using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AngusChangyiMods.Core.ModLoader
{
    public static class ModValidator
    {
        public const string nameEmptyError = "Mod Name cannot be empty";
        public const string packageIdEmptyError = "Mod Package ID cannot be empty";

        public static readonly string packageIdFormatError =
            $"Invalid Package ID format, required format is {Mod.PackageIdRegex}";

        public const string authorEmptyError = "Mod Author cannot be empty";
        public const string descriptionEmptyError = "Mod Description cannot be empty";
        public const string rootDirectoryEmptyError = "Mod Root Directory cannot be empty";


        public static void Validate(ModMetaData meta)
        {
            if (meta == null)
                throw new ArgumentNullException(nameof(meta));

            if (meta.Name == null)
            {
                meta.SetError(nameEmptyError);
            }

            if (meta.PackageId == null)
            {
                meta.SetError(packageIdEmptyError);
            }
            else if (!Mod.PackageIdRegex.IsMatch(meta.PackageId))
            {
                meta.SetError(packageIdFormatError);
            }

            if (meta.Author == null)
            {
                meta.SetError(authorEmptyError);
            }

            if (meta.Description == null)
            {
                meta.SetError(descriptionEmptyError);
            }

            if (meta.RootDirectory == null)
            {
                meta.SetError(rootDirectoryEmptyError);
            }
        }
    }
}