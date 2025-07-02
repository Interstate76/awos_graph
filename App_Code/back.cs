using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Npgsql;
using System.Web.UI.WebControls;
using System.Configuration;
using krypto;
using System.Data.SqlClient;


namespace back
{
    public class sub_bag
    {

        public enum jakchcesz
        {
            user,
            status,
            nazwisko,
            imie,
            nazwisko_solo,
            haslo,
            id_kto,
            session,
            idlotniska,
            wiele_okien,
            admin_tool
        }

        public static Dictionary<jakchcesz, string> getLogInfo(string user, string password)
        {
            Dictionary<jakchcesz, string> tReturn = new System.Collections.Generic.Dictionary<jakchcesz, string>();
            NpgsqlConnection objConn_db = new NpgsqlConnection();
            string strsql = "SELECT * FROM public.user u INNER JOIN public.user_lotnisko l on l.id_user = u.id_user WHERE u.login = '" + user + "'";
            
            DataTable dtTbl = new DataTable();

            //tworzenie połączenia w oparciu o usera i hasło z bazy
            objConn_db.ConnectionString = ConnectionString("", "");
            objConn_db.CreateCommand();
            try
            {
                NpgsqlDataAdapter objDtAdapter_Typ = new NpgsqlDataAdapter(strsql, objConn_db);
                objConn_db.Open();
                objDtAdapter_Typ.Fill(dtTbl);
            }
            catch (Exception ex)
            {
                System.ArgumentException err = new System.ArgumentException("błąd odczytu z bazy danych#" + Environment.NewLine + ex.ToString());
                throw err;
            }
            finally
            {
                objConn_db.Close();
            }






            string y = Crypto.Encrypt(password);

            if (dtTbl.Rows.Count <= 0 || dtTbl.Rows[0]["haslo"].ToString() != Crypto.Encrypt(password))
            {
                System.ArgumentException err = new System.ArgumentException("nieprawidłowe dane logowania");
                throw err;
            }
            string id_elementy = "";
            tReturn.Add(jakchcesz.status, "LogOk");
            tReturn.Add(jakchcesz.user, user);
            tReturn.Add(jakchcesz.nazwisko, dtTbl.Rows[0]["imie"].ToString() + " " + dtTbl.Rows[0]["nazwisko"].ToString());
            tReturn.Add(jakchcesz.nazwisko_solo, dtTbl.Rows[0]["nazwisko"].ToString());
            tReturn.Add(jakchcesz.imie, dtTbl.Rows[0]["imie"].ToString());
            tReturn.Add(jakchcesz.haslo, dtTbl.Rows[0]["haslo"].ToString());
            tReturn.Add(jakchcesz.id_kto, dtTbl.Rows[0]["id_user"].ToString());
            tReturn.Add(jakchcesz.wiele_okien, dtTbl.Rows[0]["wiele_okien"].ToString());
            tReturn.Add(jakchcesz.admin_tool, dtTbl.Rows[0]["admin_tool"].ToString());
            foreach (DataRow row in dtTbl.Rows)
            {
                id_elementy += row["id_lotnisko"].ToString() + ",";
            }
            tReturn.Add(jakchcesz.idlotniska, id_elementy);
            tReturn.Add(jakchcesz.session, dtTbl.Rows[0]["session"].ToString());
            return tReturn;
        }
        public static Dictionary<jakchcesz, string> SessjaUpdate(string user, Dictionary<jakchcesz, string> tReturn)
        {
            string sesja = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string inpSQL = "UPDATE public.user set session='" + sesja + "' where id_user = '" + user + "'";
                sub_bag.InputAWOSSQL(inpSQL, "", "");
            }
            catch (Exception ex)
            {
                System.ArgumentException err = new System.ArgumentException("błąd odczytu z bazy danych#" + Environment.NewLine + ex.ToString());
                throw err;
            }
            tReturn[jakchcesz.session] = sesja;
            return tReturn; 
        }
        public static bool SessjaLogOut(string user)
        {
            try
            {
                string inpSQL = "UPDATE public.user set session=NULL where id_user = '" + user + "'";
                //sub_bag.InputAWOSSQL(inpSQL, "", "");
            }
            catch (Exception ex)
            {
                System.ArgumentException err = new System.ArgumentException("błąd odczytu z bazy danych#" + Environment.NewLine + ex.ToString());
                throw err;
            }
            return true;
        }
        public static string ConnectionString(string user, string password)
        {
            NpgsqlConnectionStringBuilder cn = new NpgsqlConnectionStringBuilder();
            cn.Database = "db_awos";
            //cn.UserName = "awos";
            //cn.Password = cn.Password = Crypto.Decrypt("2UxF1X/HyQB9DSpBc1G/Jg==");
            //cn.Port = 5432;
            cn.UserName = "awos_pomiar";
			cn.Password = "@WoS4PoMiAr";
            cn.Port = 9999; //9999;  
            //cn.Host = ConfigurationManager.AppSettings["dbIP"];
            cn.Host = "172.31.74.70"; //"172.31.74.70";
            //cn.Protocol = ProtocolVersion.Version3;
            cn.SSL = false;
            cn.SslMode = SslMode.Disable;
            cn.Pooling = true;
            cn.MinPoolSize = 0;
            cn.MaxPoolSize = 100;
            cn.Timeout = 10;
            cn.CommandTimeout = 60;
            cn.ConnectionLifeTime = 60;
            return cn.ToString();
        }

        public static DataTable LoadAWOSSQL(string strSQL, string user, string password)
        {
            DataTable dtTbl = new DataTable();
            NpgsqlConnection objConn_db = new NpgsqlConnection();

            //tworzenie połączenia w oparciu o usera i hasło z bazy
            objConn_db.ConnectionString = ConnectionString("", "");
            objConn_db.CreateCommand();

            try
            {
                NpgsqlDataAdapter objDtAdapter_Typ = new NpgsqlDataAdapter(strSQL, objConn_db);
                objConn_db.Open();
                objDtAdapter_Typ.Fill(dtTbl);
                return dtTbl;
            }
            catch (Exception ex)
            {
                System.ArgumentException err = new System.ArgumentException("błąd odczytu z bazy danych#" + Environment.NewLine + ex.ToString());
                throw err;
            }
            finally
            {
                objConn_db.Close();
            }
        }
        public static void InputAWOSSQL(string strSQL, string user, string password)
        {
            NpgsqlConnection Conn = new NpgsqlConnection();
            //tworzenie połączenia w oparciu o usera i hasło z bazy
            Conn.ConnectionString = ConnectionString("", "");
            Conn.CreateCommand();
            try
            {
                NpgsqlCommand sqC = new NpgsqlCommand(strSQL, Conn);
                Conn.Open();
                sqC.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.ArgumentException err = new System.ArgumentException("błąd zapisu do bazy danych#" + Environment.NewLine + ex.ToString());
                throw err;
            }
            finally
            {
                Conn.Close();
            }
        }

        public static string Wiatr(string Kierunek)
        {
            string gwiazda = "";
            if ((Kierunek == "///"))
                return "///";
            if(Kierunek.Contains("*"))
                gwiazda = "*";
            Int16 K = 800;
            try { K = Convert.ToInt16(Kierunek); }
            catch {
              string newString = System.Text.RegularExpressions.Regex.Replace(Kierunek, "[^.0-9]", "");
              double tmp = Math.Round(Convert.ToDouble(newString), 0);
              K = Convert.ToInt16(tmp);
            }
            string wyjscie = K.ToString();
            
            if (K < 11)
                wyjscie = "N";
            else if (K >= 11 && K < 33)
                wyjscie = "NNE";
            else if (K >= 33 && K < 56)
                wyjscie = "NE";
            else if (K >= 33 && K < 78)
                wyjscie = "ENE";
            else if (K >= 78 && K < 101)
                wyjscie = "E";
            else if (K >= 101 && K < 123)
                wyjscie = "ESE";
            else if (K >= 123 && K < 146)
                wyjscie = "SE";
            else if (K >= 146 && K < 168)
                wyjscie = "SSE";
            else if (K >= 168 && K < 191)
                wyjscie = "S";
            else if (K >= 191 && K < 213)
                wyjscie = "SSW";
            else if (K >= 213 && K < 236)
                wyjscie = "SW";
            else if (K >= 236 && K < 258)
                wyjscie = "WSW";
            else if (K >= 258 && K < 281)
                wyjscie = "W";
            else if (K >= 281 && K < 303)
                wyjscie = "WNW";
            else if (K >= 303 && K < 326)
                wyjscie = "NW";
            else if (K >= 326 && K < 348)
                wyjscie = "NNW";
            else if (K >= 348 && K < 365)
                wyjscie = "N";
            else
                wyjscie = "ERROR";
            return wyjscie + gwiazda;
        }
        public static int IsWolne(System.DateTime data)
        {
            int functionReturnValue = 0;
            if (data.DayOfWeek == DayOfWeek.Saturday)
                return 1;
            if (data.DayOfWeek == DayOfWeek.Sunday)
                return 2;

            //wielkanoc i bc
            int rok = data.Year;
            int a = (rok % 19);
            int b = (rok % 4);
            int c = (rok % 7);
            int m = 0;
            int n = 0;
            if (rok >= 1583 && rok < 1700)
            {
                m = 22;
                n = 2;
            }
            else if (rok >= 1700 && rok < 1800)
            {
                m = 23;
                n = 3;
            }
            else if (rok >= 1800 && rok < 1900)
            {
                m = 23;
                n = 4;
            }
            else if (rok >= 1900 && rok < 2100)
            {
                m = 24;
                n = 5;
            }
            else if (rok >= 2100)
            {
                m = 24;
                n = 6;
            }
            int d = ((19 * a + m) % 30);
            int e = ((2 * b + 4 * c + 6 * d + n) % 7);
            string tmp = null;

            if ((22 + d + e) <= 31)
            {
                tmp = rok + ".03." + (22 + d + e);
            }
            else if ((d + e - 9) == 25 | ((d + e - 9) == 26 & a <= 11))
            {
                tmp = rok + ".04.19";
            }
            else
            {
                tmp = rok + ".04." + (d + e - 9);
            }

            ArrayList tabWolne = new ArrayList();
            System.DateTime dta = System.DateTime.Parse(tmp);
            tabWolne.Add(dta.ToString("MM.dd"));
            //wielkanoc
            tabWolne.Add(dta.AddDays(1).ToString("MM.dd"));
            //drugi dzień wielkanoc
            tabWolne.Add(dta.AddDays(49).ToString("MM.dd"));
            //zielone świątki
            tabWolne.Add(dta.AddDays(60).ToString("MM.dd"));
            //boże ciało

            //stałe wolne
            string[] stale_wolne = ("01.01;01.06;05.01;05.03;08.15;11.01;11.11;12.25;12.26".Split(';'));
            tabWolne.AddRange(stale_wolne);
            tabWolne.Sort();
            foreach (string it in tabWolne)
            {
                if (data.ToString("MM.dd") == it)
                {
                    return 2;
                }
            }
            return functionReturnValue;
        }
        #region "zamiana znakow"
        public static string Zamien(string tekst)
        {
            string tekst_zamieniony = null;
            tekst_zamieniony = tekst.ToString().Replace("'", "#z#");
            return tekst_zamieniony;
        }
        public static string Odmien(string tekst)
        {
            string tekst_zamieniony = null;
            tekst_zamieniony = tekst.ToString().Replace("#z#", "'");
            return tekst_zamieniony;
        }
        public static string oczysz_tekst_alertjs(string wiadomosc)
        {
            if (!string.IsNullOrEmpty(wiadomosc))
            {
                wiadomosc = wiadomosc.Replace("\r\n", "\\r\\n");
                wiadomosc = wiadomosc.Replace("(", "\\(");
                wiadomosc = wiadomosc.Replace(")", "\\)");
                wiadomosc = wiadomosc.Replace("\"", "\\\"");
            }
            return wiadomosc;
        }
        #endregion


        public static DataTable DistingtZTabeli(DataTable tab, string kolumna1)
        {
            DataView view = new DataView(tab);
            DataTable tab_wynik = view.ToTable(true, kolumna1);
            return tab_wynik;
        }
        public static DataTable DistingtZTabeli(DataTable tab, string kolumna1, string kolumna2)
        {
            DataView view = new DataView(tab);
            DataTable tab_wynik = view.ToTable(true, kolumna1, kolumna2);
            return tab_wynik;
        }
        public static DataTable DistingtZTabeli(DataTable tab, string[] kolumny)
        {
            DataView view = new DataView(tab);
            DataTable tab_wynik = view.ToTable(true, kolumny);
            return tab_wynik;
        }
    }
}

