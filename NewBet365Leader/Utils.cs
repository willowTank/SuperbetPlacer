using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FirefoxBet365Placer.Json;

namespace FirefoxBet365Placer
{
    public class Utils
    {
        private static NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint;
        private static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

        public static string ToFractions(double number, int precision = 100)
        {
            int w, n, d;
            number = number - 1;
            Utils.RoundToMixedFraction(number, precision, out w, out n, out d);
            var ret = $"{w * d + n}/{d}";
            return ret;
        }

        public static void RoundToMixedFraction(double input, int accuracy, out int whole, out int numerator, out int denominator)
        {
            double dblAccuracy = (double)accuracy;
            whole = (int)(Math.Truncate(input));
            var fraction = Math.Abs(input - whole);
            if (fraction == 0)
            {
                numerator = 0;
                denominator = 1;
                return;
            }
            var n = Enumerable.Range(0, accuracy + 1).SkipWhile(e => (e / dblAccuracy) < fraction).First();
            var hi = n / dblAccuracy;
            var lo = (n - 1) / dblAccuracy;
            if ((fraction - lo) < (hi - fraction)) n--;
            if (n == accuracy)
            {
                whole++;
                numerator = 0;
                denominator = 1;
                return;
            }
            var gcd = Utils.GCD(n, accuracy);
            numerator = n / gcd;
            denominator = accuracy / gcd;
        }


        public static int GCD(int a, int b)
        {
            if (b == 0) return a;
            else return Utils.GCD(b, a % b);
        }

        public static double calculateStake(EuBetItem betitem, EuAccount account)
        {
            double stake = -1;
            try
            {
                string[] stakeParts = betitem.stake.Split('/');
                // stake format: 5/10.  isFlatStake = true => unitStake, isFlatStake = false => unitStake * 5
                /*
                 *  If stake is 5, take 5 
                 */
                if (stakeParts.Length == 1)
                {
                    stake = Utils.ParseToDouble(stakeParts[0]) * Utils.ParseToDouble(account.unitStake);
                }
                else
                {
                    stake = Utils.ParseToDouble(account.unitStake) * Utils.ParseToDouble(stakeParts[0]);
                }

                if (account.isFlatStake)
                {
                    stake = Utils.ParseToDouble(account.unitStake);
                }
            }
            catch (Exception)
            {

            }
            return stake;
        }

        public static double ParseToDouble(string str)
        {
            double value = 0;
            str = str.Replace(",", ".");
            double.TryParse(str, style, culture, out value);
            return value;
        }
        public static long getTick()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            long timestamp = (long)t.TotalMilliseconds;
            return timestamp;
        }

        public static string DecryptMessage(string AuthorizationCode, string keyString, string ivString)
        {
            byte[] key = Encoding.ASCII.GetBytes(keyString);
            byte[] iv = Encoding.ASCII.GetBytes(ivString);

            using (var rijndaelManaged =
                    new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
            {
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.KeySize = 256;
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(AuthorizationCode)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
        }

        public static List<List<BetItem>> SplitList(List<BetItem> locations, int nSize = 2)
        {
            var list = new List<List<BetItem>>();
            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }
            return list;
        }

        public static bool IsContainsLeague(List<string> leagueList, string league)
        {
            foreach (string item in leagueList)
            {
                if (league.StartsWith(item))
                    return true;
            }
            return false;
        }

        public static int GetRandValue(int maxValue)
        {
            Random random = new Random();
            return random.Next(0, maxValue);
        }
        /*
         * @param pon random positive or negative
         */
        public static int GetRandValue(int minValue, int maxValue, bool pon = false)
        {
            int c = maxValue - minValue + 1;
            Random random = new Random();
            return (int)Math.Floor(random.NextDouble() * c + minValue) * (pon ? _pon() : 1);
        }

        /**
         * positive or negative
         */
        public static int _pon(){
            return GetRandValue(10) >= 5 ? 1 : -1;
        }

        public static Point ThreeBezier(decimal t, Point p1, Point cp1, Point cp2, Point p2)
        {
            decimal x1 = p1.x, y1 = p1.y;
            decimal x2 = p2.x, y2 = p2.y;
            decimal cx1 = cp1.x, cy1 = cp1.y;
            decimal cx2 = cp2.x, cy2 = cp2.y;

            decimal x =
                x1 * (1 - t) * (1 - t) * (1 - t) +
                3 * cx1 * t * (1 - t) * (1 - t) +
                3 * cx2 * t * t * (1 - t) +
                x2 * t * t * t;
            decimal y =
                y1 * (1 - t) * (1 - t) * (1 - t) +
                3 * cy1 * t * (1 - t) * (1 - t) +
                3 * cy2 * t * t * (1 - t) +
                y2 * t * t * t;
            return new Point(x, y);
        }

        public static string GenerateRandomNumberString(int length)
        {
            StringBuilder strBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                strBuilder.Append(random.Next(0, 10));
            }

            return strBuilder.ToString();
        }

        public static bool isAUSorUK(BetItem betitem)
        {
            if (betitem.league.Contains("(") && betitem.league.Contains(")"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ClearFolder(string FolderName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(FolderName);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch (Exception) { } // Ignore all exceptions
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    ClearFolder(di.FullName);
                    try
                    {
                        di.Delete();
                    }
                    catch (Exception) { } // Ignore all exceptions
                }
            }
            catch
            {
            }
        }

        public static double FractionToDouble(string fraction)
        {
            double result;

            if (double.TryParse(fraction, out result))
            {
                return result;
            }

            string[] split = fraction.Split(new char[] { ' ', '/' });

            if (split.Length == 2 || split.Length == 3)
            {
                int a, b;

                if (int.TryParse(split[0], out a) && int.TryParse(split[1], out b))
                {
                    if (split.Length == 2)
                    {
                        return 1 + Math.Floor((double)100 * a / b) / 100;
                    }

                    int c;

                    if (int.TryParse(split[2], out c))
                    {
                        return a + (double)b / c;
                    }
                }
            }

            throw new FormatException("Not a valid fraction. => " + fraction);
        }

        public static string ReplaceStr(string STR, string ReplaceSTR, string FirstString, string LastString)
        {
            string FinalString;
            if (!STR.Contains(FirstString)) return STR;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            if (LastString != null)
            {
                string preSTR = STR.Substring(0, Pos1);
                int Pos2 = STR.IndexOf(LastString, Pos1);
                FinalString = preSTR + ReplaceSTR + STR.Substring(Pos2);
            }
            else
            {
                string preSTR = STR.Substring(0, Pos1);
                FinalString = preSTR + ReplaceSTR;
            }

            return FinalString;
        }

        public static double GetComboOdds(List<BetItem> betItems)
        {
            double comboOdds = 1;
            try
            {
                foreach(BetItem betitem in betItems)
                {
                    comboOdds *= betitem.odds;
                }
            }
            catch (Exception ex)
            {
            }
            return comboOdds;
        }

        public static string GetUserAgent(string username)
        {
            try
            {
                int uniqueIndex = Math.Abs(username.GetHashCode());
                string[] lines = System.IO.File.ReadAllLines("user-agent.txt");
                int totalCount = lines.Length - 1;
                return lines[Math.Abs(uniqueIndex % totalCount)];
            }
            catch (Exception ex)
            {
            }
            return "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:91.0) Gecko/20100101 Firefox/91.0";
        }

        public static int GetRunningInstanceCount()
        {
            int currentCount = 0;
            try
            {
                Process[] liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    return 0;

                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName.Contains("Bet365"))
                        currentCount++;
                }
            }
            catch (Exception)
            {
            }
            return currentCount;
        }

        public static string Between(string STR, string FirstString, string LastString = null)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            if (LastString != null)
            {
                STR = STR.Substring(Pos1);
                int Pos2 = STR.IndexOf(LastString);
                if (Pos2 > 0)
                    FinalString = STR.Substring(0, Pos2);
                else
                    FinalString = STR;
            }
            else
            {
                FinalString = STR.Substring(Pos1);
            }

            return FinalString;
        }
        public static decimal ParseToDecimal(string str)
        {
            decimal value = 0;
            decimal.TryParse(str, style, culture, out value);
            return value;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
