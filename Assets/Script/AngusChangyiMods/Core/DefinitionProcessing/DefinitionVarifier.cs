using System;
using System.Xml.Linq;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionVarifier
    {
        bool VerifyDefinitions(XElement element, out string errorMessage);
    }

    public class DefinitionVarifier : IDefinitionVarifier
    {
        public const string error_lostDefName = "Definition element is missing the required attribute 'defName'.";
        public const string error_illegalDefName = "defName '{0}' is in illegal format, must be in the format OOO.OOO, and can only contain alphanumeric characters, with the first character being an alphabet letter.";
        
        public bool VerifyDefinitions(XElement element, out string errorMessage)
        {
            XElement defNameElement = element.Element(Def.DefName);

            if (defNameElement == null)
            {
                errorMessage = error_lostDefName;
                return false; // 沒有 defName 元素，驗證失敗
            }

            string defname = defNameElement.Value;
            if (!System.Text.RegularExpressions.Regex.IsMatch(defname, Def.DefNamePattern))
            {
                errorMessage = string.Format(error_illegalDefName, defname);
                return false; // defName 格式不合法，驗證失敗
            }

            errorMessage = string.Empty; // 清空錯誤信息
            return true;
        }
    }
}