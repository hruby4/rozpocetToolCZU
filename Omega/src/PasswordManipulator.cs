﻿using System;
using System.Linq;
using System.Security.Cryptography;


namespace Omega
{
    public class PasswordManipulator
    {
        /// <summary>
        /// Method <c>buildHash</c> hashes the given string
        /// </summary>
        /// <returns>
        /// hashed string
        /// </returns>
        public static string buildHash(string password)
        {
            byte[] salt = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] saltedPasswordBytes = new byte[passwordBytes.Length + salt.Length];
            passwordBytes.CopyTo(saltedPasswordBytes, 0);
            salt.CopyTo(saltedPasswordBytes, passwordBytes.Length);
            byte[] hashBytes = new SHA256Managed().ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Method <c>verifyPassword</c> compares if string and hashed string is equal
        /// </summary>
        /// <returns>
        /// if its equal true then false
        /// </returns>
        public static bool verifyPassword(string userInput, string hashedPassword)
        {
            string[] saltAndHash = hashedPassword.Split('.');
            byte[] salt = Convert.FromBase64String(saltAndHash[0]);
            byte[] hash = Convert.FromBase64String(saltAndHash[1]);
            byte[] userInputBytes = System.Text.Encoding.UTF8.GetBytes(userInput);
            byte[] saltedUserInputBytes = new byte[userInputBytes.Length + salt.Length];
            userInputBytes.CopyTo(saltedUserInputBytes, 0);
            salt.CopyTo(saltedUserInputBytes, userInputBytes.Length);
            byte[] computedHash = new SHA256Managed().ComputeHash(saltedUserInputBytes);
            return hash.SequenceEqual(computedHash);
        }

    }
}
