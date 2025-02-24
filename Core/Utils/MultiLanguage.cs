﻿namespace LeagueSharp.SDK.Core.Utils
{
    using Enumerations;
    using Properties;
    using SDK.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Resources;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides multi-lingual strings.
    /// </summary>
    public static class MultiLanguage
    {
        /// <summary>
        /// The translations
        /// </summary>
        private static Dictionary<string, string> Translations = new Dictionary<string, string>();

        /// <summary>
        /// Translates the text into the loaded language.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <returns>System.String.</returns>
        public static string Translation(string textToTranslate)
        {
            var textToTranslateToLower = textToTranslate.ToLower();
            if (Translations.ContainsKey(textToTranslateToLower))
            {
                return Translations[textToTranslateToLower];
            }
            else if (Translations.ContainsKey(textToTranslate))
            {
                return Translations[textToTranslate];
            }
            else
            {
                return textToTranslate;
            }
        }

        /// <summary>
        /// judge the select language
        /// </summary>
        public static void LoadTranslation()
        {
            try
            {
                var SelectLanguage = Sandbox.SandboxConfig.SelectedLanguage;

                if (SelectLanguage == "Chinese")
                {
                    LoadLanguage("Chinese");
                }
                else
                {
                    // ignore
                }
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, $"[MultiLanguage] Load Translation Catch Exception：{ex.Message}");
            }
        }

        /// <summary>
        /// Loads the translation.
        /// </summary>
        /// <param name="languageName">Name of the language.</param>
        /// <returns><c>true</c> if the operation succeeded, <c>false</c> otherwise false.</returns>
        public static bool LoadLanguage(string languageName)
        {
            try
            {
                var languageStrings = new ResourceManager("LeagueSharp.SDK.Properties.Resources", typeof(Resources).Assembly).GetString(languageName + "Json");
                if (string.IsNullOrEmpty(languageStrings))
                {
                    return false;
                }
                Translations = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(languageStrings);
                return true;
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, $"[MultiLanguage] Load Language Catch Exception：{ex.Message}");
                return false;
            }
        }

        private static string DesDecrypt(string decryptString, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
    }
}
