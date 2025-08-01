﻿using System.Text.RegularExpressions;

namespace AngusChangyiMods.Core
{
    public class Mod
    {
        public const string Root = "ModMetaData";
        public const string Name = "name";
        public const string PackageId = "packageId";
        public const string Author = "author";
        public const string Description = "description";
        public const string SupportedVersions = "supportedVersions";
        public const string Li = "li";

        public static readonly Regex PackageIdRegex = new(@"^[a-zA-Z][a-zA-Z0-9]*\.[a-zA-Z0-9]+$", RegexOptions.Compiled);
    }
}