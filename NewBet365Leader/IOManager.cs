using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirefoxBet365Placer.Json;

namespace FirefoxBet365Placer
{
    class IOManager
    {
        private static string accListFile = "accList.bin";
        private static string betDataFile = "betData.bin";


        public static void saveBetData(List<BetItem> infoList)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + betDataFile;
            //serialize
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, infoList);
            }
        }

        public static List<BetItem> readBetData()
        {
            List<BetItem> ret = new List<BetItem>();
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\" + betDataFile;
                //deserialize
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    ret = (List<BetItem>)bformatter.Deserialize(stream);
                    DateTime today = DateTime.Now;
                    ret = (List<BetItem>)ret.Where(o => 
                        (
                            o.placedDate.Year == today.Year && 
                            o.placedDate.Month == today.Month && 
                            o.placedDate.Day == today.Day
                        )
                    );
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }

        public static void removeBetData()
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\" + betDataFile;
                File.Delete(path);
            }
            catch (Exception)
            {

            }
        }

        public static void saveAccountList(List<Account> accList)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + accListFile;
            //serialize
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, accList);
            }
        }

        public static void writeHtmlContent(string content, string filePrefix)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\Temp\\" + filePrefix + "-betContent.html";
                //serialize
                using (StreamWriter stream = new StreamWriter(path))
                {
                    stream.Write(content);
                }
            }
            catch (Exception)
            {

            }
        }

        public static List<Account> readAccountList()
        {
            List<Account> ret = new List<Account>();
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\" + accListFile;
                //deserialize
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    ret = (List<Account>)bformatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {

            }
            return ret;
        }
        public static List<Account> readAccountList1()
        {
            List<Account> ret = new List<Account>();
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\" + accListFile;
                //deserialize
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    ret = (List<Account>)bformatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {

            }
            return ret;
        }
    }
}
