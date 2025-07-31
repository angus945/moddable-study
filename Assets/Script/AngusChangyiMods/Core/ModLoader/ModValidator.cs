using System;
using System.Text.RegularExpressions;

namespace AngusChangyiMods.Core.ModLoader
{
    public static class ModValidator
    {
        public static void Validate(ModMetaData meta)
        {
            if (meta == null)
                throw new ArgumentNullException(nameof(meta));
            
            if (meta.Name == null)
                throw new ArgumentNullException(Mod.Name);

            if (meta.PackageId == null)
                throw new ArgumentNullException(Mod.PackageId);

            if (!Mod.PackageIdRegex.IsMatch(meta.PackageId))
                throw new FormatException($"Invalid packageId format: {meta.PackageId}");

            if (meta.Author == null)
                throw new ArgumentNullException(Mod.Author);
            
            if (meta.Description == null)
                throw new ArgumentNullException(Mod.Description);
            
            if (meta.RootDirectory == null)
                throw new ArgumentNullException(nameof(meta.RootDirectory));
        }
    }
}