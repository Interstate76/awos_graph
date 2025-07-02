using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// Summary description for qnh
/// </summary>

namespace cisnienie
{
    public class qnh
    {
        //zaokrąglanie liczby "d"  do "i" miejsca po przecinku

        private static double Zaokraglaj(double d, int i)
        {
            d *= (i * 10);
            d = Math.Round(d);
            d /= (i * 10);
            return d;
        }

        //Hpa na Hg

        public static double HPa2Hg(double Hpa)
        {
            Hpa = 0.750062 * Hpa;
            return Zaokraglaj(Hpa, 1);
        }

        //Hpa na inch

        public static double HPa2Inch(double Hpa)
        {
            Hpa = 0.0393700787402 * 0.750062 * Hpa;
            return Zaokraglaj(Hpa, 1);
        }

        //Hpa na QFE

        public static double Hpa2Qfe(double Hpa, double Hb, double Hp, double Ts)
        {
            double G = 9.80665;
            double B = 0.0065;
            double R = 287.05287;

            //	Hpa - CISNIENIE Na BAROMETRZE [hPa]
            //	Hb  - WYSOKOSC BAROMETRU [m]     
            //	Hp  - WYSOKOSC PUNKTU ODNIESIENIA [m]        
            //	Ts  - TEMPERATURA POWIETRZA [C] 

            double c = G / (B * R);
            double h = Hb - Hp;
            double Tk = 273.15 + Ts;

            double a = 1 + (B * h) / Tk;
            Hpa = Hpa * Math.Pow(a, c);
            return Zaokraglaj(Hpa, 1);
        }

        //	Qfe na Qnh
        public static double Qfe2Qnh(double Hp, double Qfe)
        {
            //	Hp  - WYSOKOSC PUNKTU ODNIESIENIA [m] 

            double G = 9.80665;
            double B = 0.0065;
            double R = 287.05287;
            double c = G / (B * R);

            double a = 1 + (B * Hp) / (288.15 - B * Hp);
            double QNH = Qfe * Math.Pow(a, c);
            return Zaokraglaj(QNH, 1);

        }

    }
}
