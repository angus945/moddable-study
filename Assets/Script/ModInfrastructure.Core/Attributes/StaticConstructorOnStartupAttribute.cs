using System;

namespace ModArchitecture
{
    /// <summary>
    /// 標記靜態類別在模組程序集載入後應該被實例化的 Attribute
    /// 用於自動觸發靜態建構子，通常用於 Harmony patch 初始化
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StaticConstructorOnStartupAttribute : Attribute
    {
    }
}
