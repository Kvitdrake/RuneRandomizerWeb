// StringExtensions.cs
using System.Text;
using System.Security.Cryptography;

namespace webb_tst_site3.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
        public static string GetSHA256Hash(string input)
        {
            // Создаем объект SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Преобразуем строку в массив байтов (UTF-8 по умолчанию)
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Вычисляем хеш
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Преобразуем массив байтов в строку в шестнадцатеричном формате
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // "x2" означает 2 символа в нижнем регистре
                }

                return sb.ToString();
            }
        }
    }
}