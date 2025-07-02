using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Opis podsumowujący dla clsAWOS
/// </summary>
public class clsAWOS
{
    //event dla log w głównym oknie
    public delegate void meInfoEventHandler(string info);
    public event meInfoEventHandler meInfo;
    //---------------------------------------------------------------

    public string sql_string, wyjatek;
    public readonly DateTime DataTest;
    public string dtAWOS(DataTable tab_parametry)
    {
        if (tab_parametry.Rows.Count > 0)
        {
            foreach (DataRow RowEDI in tab_parametry.Rows)
            {
                string ParametryDoEDI = RowEDI["parametry"].ToString();
                if (!string.IsNullOrEmpty(ParametryDoEDI))
                {
                    // ParametryDoEDI = ParametryDoEDI.Remove(ParametryDoEDI.Length - 1, 1);
                    if (meInfo != null)
                    {
                        meInfo("id lotniska: " + RowEDI["id_lotnisko"].ToString());
                    }

                    //jesli maws
                    if ((string)tab_parametry.Rows[0]["serwer_1"] == "maws")
                    {
                        //string exit = GetMeasurmentFromCBDO((string)RowEDI["serwer_2"], ParametryDoEDI, (int)RowEDI["id_lotnisko"]);
                        //if (exit == "ERR")
                        //    return "";
                        //else
                        //    return exit;
                    }
                    else
                    {
                        string IP;
                        string DaneZEDI = GetFromLotnisko(ParametryDoEDI,
                                         (string)RowEDI["serwer_1"],
                                         (string)RowEDI["serwer_2"],
                                         (int)RowEDI["id_lotnisko"]);
                        IP = (string)RowEDI["serwer_1"];

                        if (DaneZEDI == "ERR")
                        {
                            DaneZEDI = GetFromLotnisko(ParametryDoEDI,
                                        (string)RowEDI["serwer_2"],
                                        (string)RowEDI["serwer_1"],
                                        (int)RowEDI["id_lotnisko"]);
                            IP = (string)RowEDI["serwer_2"];
                        }

                        if (DaneZEDI == "ERR")
                        {
                            return "";
                        }
                        else
                            return string.Format("{0}SELECT f_lotnisko_ip({1},'{2}');",
                                                DaneZEDI,
                                                (int)RowEDI["id_lotnisko"],
                                                IP);

                        // return DaneZEDI + "select f_lotnisko_ip(" + (int)tab_parametry.Rows[0]["id_lotnisko"] + ",'" + IP + "')";
                    }
                }
            }
        }
        else
        {
            wyjatek += "\\r\\nBrak ParametrowDoEDI!";
        }
        return "";
    }

    string GetFromLotnisko(string DoEDI, string ip, string ip2, int id_lotnisko)
    {
        string DaneZEDI = SocetConn(DoEDI, ip, ip2, id_lotnisko);
        if (string.IsNullOrEmpty(DaneZEDI) || DaneZEDI == "ERR")
        {
            wyjatek += "\\r\\nBrak danych z EDI!";
            return "ERR";
        }

        else
            return DekodujIZapiszWartosci(DaneZEDI, id_lotnisko);

    }
    string DekodujIZapiszWartosci(string inDane, int id_lotnisko)
    {
        string sql_insert = "";
        string[] line = inDane.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //data pomiarow
        int j = 0;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i].Contains("Error") || string.IsNullOrEmpty(line[i]))
                line[i] = null;
            else
                j = i;
        }
        string dx = line[j].Replace("\"", "");
        string[] del = dx.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        DateTime data_pom = Convert.ToDateTime(del[0]);
        sql_insert += "delete from pomiary_data where id_lotnisko =" + id_lotnisko +
            "; INSERT INTO pomiary_data (id_lotnisko, data) values (" + id_lotnisko + ",'" + data_pom.ToString("yyyy-MM-dd HH:mm:00") + "'); " +
            "insert into pomiary (id_lotnisko,id_kanal,data,wartosc,data_pomiaru) values ";
        foreach (string str in line)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string x = str.Replace("\"", "");
                string[] element = x.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (element.Length == 5)
                    sql_insert += "(" + id_lotnisko + ",(select k.id_kanal from kanaly k where k.parametr = '," +
                        element[2] + "," + element[3] + "'), '" + data_pom.ToString("yyyy-MM-dd HH:mm:00") + "', '" + element[4] + "', '" + element[0] + "'), ";
            }
        }
        if (!string.IsNullOrEmpty(sql_insert))
        {
            sql_insert = sql_insert.Remove(sql_insert.Length - 2, 2);
            sql_insert += ";   REFRESH MATERIALIZED VIEW view_pomiary_" + id_lotnisko + "; ";
        }
        return sql_insert;
    }
    string SocetConn(string DoEDI, string ip, string ip2, int id_lotnisko)
    {
        string wyj = "";
        // Data buffer for incoming data.
        byte[] bytes = new byte[4096];
        try
        {
            IPAddress ipAddress = IPAddress.Parse(ip); //adres pierwszy
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 125);
            if (meInfo != null)
            {
                meInfo("  IP: " + ipAddress);
            }
            Socket sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(remoteEP);
                byte[] msg = Encoding.ASCII.GetBytes("+TIME" + Environment.NewLine);
                int bytesSent = sender.Send(msg);
                byte[] msg2 = Encoding.ASCII.GetBytes(DoEDI + Environment.NewLine);
                int bytesSent2 = sender.Send(msg2);
                int bytesRec = sender.Receive(bytes);
                wyj = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                wyjatek += "\\r\\nArgumentNullException : {0}" + ane.ToString();
            }
            catch (SocketException se)
            {
                return "ERR";
            }
            catch (Exception e)
            {
                wyjatek += "\\r\\nUnexpected exception : {0}" + e.ToString();
            }

        }
        catch (Exception e)
        {
            wyjatek += e.ToString();
        }

        return wyj;
    }
    
}