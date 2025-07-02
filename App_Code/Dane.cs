using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for azure
/// </summary>

namespace backend
{
    public class Dane
    {
        public string WartPom { get; set; }
        public string CzasPom { get; set; }


        public Dane(string wartPom, string czasPom)
        {
            WartPom = wartPom;
            CzasPom = czasPom;
        }
       
        public static string Stopy(string wartosc)
        {
            return (Convert.ToInt32((Convert.ToDouble(wartosc) * 3.28083989501))).ToString();
        }

        public static string MSnaKT(string daneIn)
        {
            double kt = 1.9438461718;
            try
            {
                double x = Convert.ToDouble(daneIn.Replace(".", ",")) * kt;
                if (x > 0.5)
                    return (Math.Round(x, 0)).ToString();
                else
                    return "0";
            }
            catch (Exception)
            {
                return "x";
            }
        }
        public static string RozaZaokraglenie(string dana)
        {
            try
            {
                int i = Convert.ToInt16(dana);
                return (((int)Math.Round(i / 10.0)) * 10).ToString();
            }
            catch (Exception)
            {
                return "?";
            }
        }

        public static string minPom(int min)
        {
            return DateTime.UtcNow.AddMinutes(Convert.ToInt32(min)).ToString("HH:mm");
        }

        public static int[] TopLeft(string ang)
        {
            Dictionary<string, int[]> map = new Dictionary<string, int[]> {
                {"0",new int[]{0,100}},
                {"00",new int[]{0,100}},
                {"000",new int[]{0,100}},
                {"10",new int[]{2,100}},
                {"20",new int[]{4,100}},
                {"30",new int[]{4,100}},
                {"40",new int[]{3,100}},
                {"50",new int[]{3,100}},
                {"60",new int[]{3,100}},
                {"70",new int[]{2,100}},
                {"80",new int[]{1,100}},
                {"90",new int[]{0,100}},
                {"010",new int[]{2,100}},
                {"020",new int[]{4,100}},
                {"030",new int[]{4,100}},
                {"040",new int[]{3,100}},
                {"050",new int[]{3,100}},
                {"060",new int[]{3,100}},
                {"070",new int[]{2,100}},
                {"080",new int[]{1,100}},
                {"090",new int[]{0,100}},
                {"100",new int[]{-1,102}},
                {"110",new int[]{-1,104}},
                {"120",new int[]{-2,104}},
                {"130",new int[]{-1,106}},
                {"140",new int[]{-1,106}},
                {"150",new int[]{-1,105}},
                {"160",new int[]{-1,107}},
                {"170",new int[]{0,109}},
                {"180",new int[]{0,110}},
                {"190",new int[]{2,111}},
                {"200",new int[]{3,112}},
                {"210",new int[]{4,112}},
                {"220",new int[]{6,111}},
                {"230",new int[]{6,111}},
                {"240",new int[]{7,111}},
                {"250",new int[]{8,111}},
                {"260",new int[]{8,111}},
                {"270",new int[]{10,100}},
                {"280",new int[]{10,100}},
                {"290",new int[]{9,100}},
                {"300",new int[]{8,100}},
                {"310",new int[]{8,100}},
                {"320",new int[]{6,100}},
                {"330",new int[]{6,100}},
                {"340",new int[]{6,100}},
                {"350",new int[]{6,100}},
                {"360",new int[]{0,100}},
                {"?",new int[]{0,0}}
        };
            return map[ang];
        }


        public static string KodChmur(string dana)
        {
            Dictionary<string, string> map = new Dictionary<string, string> {
                {"0","-"},
                {"1","FEW"},
                {"2","FEW"},
                {"3","SCT"},
                {"4","SCT"},
                {"5","BKN"},
                {"6","BKN"},
                {"7","BKN"},
                {"8","OVC"},
                {"?","ERR"},
                {"/","/"}
        };
            return map[dana];
        }

        public static string ConvTime(string czas)
        {
            return Convert.ToDateTime(czas).ToString("HH:mm");
        }


    }

}
