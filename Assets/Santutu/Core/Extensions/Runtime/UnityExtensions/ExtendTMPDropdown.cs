using System;
using TMPro;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendTMPDropdown
    {
        public static TMP_Dropdown.OptionData GetOptionByIndex(this TMP_Dropdown dropdown, int index)
        {
            return dropdown.options[index];
        }

        public static int GetValueByOptionText(this TMP_Dropdown dropdown, string optionText)
        {
            int index = 0;
            foreach (var option in dropdown.options)
            {
                if (option.text == optionText)
                {
                    return index;
                }

                index++;
            }

            throw new Exception(optionText);
        }

        public static TMP_Dropdown.OptionData GetSelectedOption(this TMP_Dropdown dropdown)
        {
            return dropdown.options[dropdown.value];
        }

        public static string GetSelectedOptionText(this TMP_Dropdown dropdown)
        {
            return dropdown.options[dropdown.value].text;
        }
    }
}