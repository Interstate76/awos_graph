using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Web.Security;
using back;
using krypto;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using ad.imgw.dsspws;
using wykresy;
using backend;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI.DataVisualization.Charting;

public partial class AWOS_Default : System.Web.UI.Page
{
    #region Properties
    public string wyjatek { get; set; }
    public string uzyt { get; set; }
    public string pass { get; set; }
    public string idkto { get; set; }
    public string im { get; set; }
    public string upr_lotniska { get; set; }
    public string wiele_okien { get; set; }
    public string admin_tool { get; set; }
    public string station { get; set; }
    public int idlot { get; set; }

    string label_czas = "";

    //do wykresu
    Color kolorWykresu;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (Session["userInfo"] != null)
        {
            System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user = new System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string>();
            user = Session["userInfo"] as System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string>;
            if (SessionTest(user[sub_bag.jakchcesz.session], user[sub_bag.jakchcesz.id_kto]))
            {
                uzyt = user[sub_bag.jakchcesz.user];
                pass = user[sub_bag.jakchcesz.haslo];
                idkto = user[sub_bag.jakchcesz.id_kto];
                im = user[sub_bag.jakchcesz.nazwisko];
                upr_lotniska = user[sub_bag.jakchcesz.idlotniska];
                wiele_okien = user[sub_bag.jakchcesz.wiele_okien];
                admin_tool = user[sub_bag.jakchcesz.admin_tool];
                if (string.IsNullOrEmpty(upr_lotniska))
                    Response.Redirect("login.aspx?mes=Nie masz uprawnień do wglądu na lotniska", false);
                string[] ulot = upr_lotniska.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (ulot.Length == 1)
                {
                    upr_lotniska = upr_lotniska.Replace(",", "");
                    idlot = Convert.ToInt32(upr_lotniska);
                }
                string in_lot = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(in_lot))
                {
                    if (!upr_lotniska.Contains(in_lot))
                        Response.Redirect("default.aspx", false);
                }
            }
        }
        else
            Response.Redirect("login.aspx?mes=Nie zalogowałeś się. Błąd Session-UserInfo.", true);

        //zmiana stylów
        string x = Session["tryb"] as string;
        HtmlLink css = new HtmlLink();
        if (x == "noc")
            css.Href = "Styles/awos_noc.css";
        else if (x == "dzien")
            css.Href = "Styles/awos.css";
        else
        {
            css.Href = "Styles/awos.css";
            Session["tryb"] = "dzien";
        }
        css.Attributes["rel"] = "stylesheet";
        css.Attributes["type"] = "text/css";
        css.Attributes["media"] = "all";
        Page.Header.Controls.Add(css);
        if (Session["zakres"] == null)
            Session["zakres"] = -3; //zakres danych pierwszej strony - start sesji       
    }

    protected bool SessionTest(string set, string id)
    {
        //try
        //{
        //    DataTable x = sub_bag.LoadAWOSSQL(" select f_session_test(" + id + ",'" + set + "')", uzyt, pass);
        //    if (x.Rows.Count > 0)
        //    {
        //        return (bool)x.Rows[0][0];
        //    }
        //    else
        //    {
        //        wyjatek += "Bład brak pól w f_session_test: ";
        //        return false;
        //    }
        //}
        //catch (Exception ex)
        //{
        //    wyjatek += "Bład SessionTEST CATCH: " + ex.Message;
        //    return false;
        //}
        return true;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            lb_user.Text = im;
            ViewState["tabLotnisko"] = null;
            if (admin_tool == "False")
            {
                lb_dane.Visible = false;
                lb_odswiezOff.Visible = false;
            }
        }
        string x = Session["tryb"] as string;
        if (x == "noc")
            lb_noc.Text = "tryb dzienny";
        else if (x == "dzien")
            lb_noc.Text = "tryb nocny";
        else
            lb_noc.Text = "tryb nocny";

        try
        {
            div_okno.Visible = false;
            div_wybor.Visible = false;
            div_nieaktualne.Visible = false;
            div_tab.Visible = false;
            div_haslo.Visible = false;
            if (idlot == 0)
                idlot = Convert.ToInt16(Request.QueryString["id"]);
            if (idlot > 0)
                RunLotnisko(idlot, Convert.ToInt32(Session["zakres"]));
            else
                WyborLotniska();
        }
        catch (Exception ex)
        {
            wyjatek += "\\r\\nBłąd" + ex.Message;
        }


        int sec = DateTime.Now.Second;
        if (sec > 10)
            Tik.Interval = (70 - DateTime.Now.Second) * 1000; //10 sek po pełnej minucie (+3 odpytanie EDI ok 1-2 sek przetwarzanie i zapis do bazy)
        else
            Tik.Interval = 10 * 1000;
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (div_nieaktualne.Visible == true)
            Tik.Interval = 15 * 1000;
        if (!string.IsNullOrEmpty(wyjatek))
        {
            wyjatek = sub_bag.oczysz_tekst_alertjs(wyjatek);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "Skryptalarmu", "<script type=\"text/javascript\">alert(\"" + wyjatek + "\");</script>", false);
        }
        Page.MaintainScrollPositionOnPostBack = true;
    }
    protected void RunLotnisko(int id_lotnisko, int time = -3)
    {
        div_okno.Visible = false;
        div_wybor.Visible = false;
        bool brak_danych = false;

        if (id_lotnisko > 0)
        {
            if (ViewState["tabLotnisko"] == null)
                ViewState["tabLotnisko"] = MojeLotnisko(id_lotnisko);
            DataTable tab = ViewState["tabLotnisko"] as DataTable;
            Page.Title = "IMGW AWOS " + tab.Rows[0]["nazwa"].ToString();
            //test daty
            if (tab.Rows[0]["serwer_1"].ToString() == "maws")
            {
                if (!TestWaznosciPomiarow(id_lotnisko, -95)) //określa ile opoznienia w pomiarze jest dopuszczalne (MAWS 1,5 min)
                {
                    div_okno.Visible = true;
                    //div_nieaktualne.Visible = true;
                    WyswietlAktualneDane(id_lotnisko, time);
                    hf_work.Value = "false";
                    if (id_lotnisko == 18)
                    {
                        brak_danych = WypelnijLabelDanych_Azure();
                        if (!brak_danych)
                            hf_work.Value = "True";
                        else
                        {
                            hf_work.Value = "false";
                            //div_nieaktualne.Visible = true;
                        }
                    }
                    else WyswietlAktualneDane(id_lotnisko, time);
                }
                else
                {
                    if (id_lotnisko == 18)
                    {
                        brak_danych = WypelnijLabelDanych_Azure();
                        if (!brak_danych)
                            hf_work.Value = "True";
                        else
                        {
                            hf_work.Value = "false";
                            //div_nieaktualne.Visible = true;
                        }
                    }
                    else WyswietlAktualneDane(id_lotnisko, time);
                    hf_work.Value = "true";
                }
            }
            else
            {
                if (!TestWaznosciPomiarow(id_lotnisko, -200))
                {
                    div_okno.Visible = true;
                    WyswietlAktualneDane(id_lotnisko, time);
                    //div_nieaktualne.Visible = true;
                    hf_work.Value = "false";
                }
                else
                {
                    if (id_lotnisko == 18)
                    {
                        brak_danych = WypelnijLabelDanych_Azure();
                        if (!brak_danych)
                            hf_work.Value = "True";
                        else
                        {
                            hf_work.Value = "false";
                            //div_nieaktualne.Visible = true;
                        }
                    }
                    else WyswietlAktualneDane(id_lotnisko, time);
                    hf_work.Value = "true";
                }
            }
        }
        else
            wyjatek += "\\r\\nNie wybrałeś lotniska!";
    }
    protected DataTable MojeLotnisko(int id_lotnisko)
    {
        DataTable tab = new DataTable();
        string sql = "select * from lotnisko where id_lotnisko = " + id_lotnisko;
        try
        {
            tab = sub_bag.LoadAWOSSQL(sql, uzyt, pass);
            if (tab.Rows.Count > 0)
            {
                lb_lotnisko_nazwa.Text = tab.Rows[0]["pelna_nazwa"].ToString() + " (" + tab.Rows[0]["skrot"].ToString() + " / " + tab.Rows[0]["ICAO"].ToString() + ")";
                int av = Convert.ToInt32(tab.Rows[0]["prog_1"]) * 10;
                for (int i = 1; i < 4; i++)
                {
                    HtmlControl div = (HtmlControl)Page.FindControl("pas" + i);
                    div.Attributes["style"] = "transform:rotate(" + av + "deg); -webkit-transform:rotate(" + av + "deg); -moz-transform:rotate(" + av + "deg); -o-transform:rotate(" + av + "deg);z-index:10";
                }
                if (tab.Rows[0]["serwer_1"].ToString() == "maws")
                {
                    tdz.Visible = false;
                    end.Visible = false;
                    metrep.Visible = false;
                    fset_cloud.Visible = false;
                    fset_press.Visible = false;
                    fset_rvr.Visible = false;
                    fset_vis.Visible = false;
                    fset_pa11.Visible = true;
                    fset_opad_maws.Visible = true;
                    fset_temp.Visible = true;
                    fset_widz.Visible = true;
                    lb_prog_mid.Text = "Ogródek METEO";
                    mid.Attributes["class"] = "glowne_mid";

                }
                else
                {
                    tdz.Visible = true;
                    end.Visible = true;
                    metrep.Visible = true;
                    fset_cloud.Visible = true;
                    fset_press.Visible = true;
                    fset_rvr.Visible = true;
                    fset_vis.Visible = true;
                    fset_pa11.Visible = false;
                    fset_opad_maws.Visible = false;
                    fset_temp.Visible = false;
                    fset_widz.Visible = false;
                    lb_prog_mid.Text = "";
                    lb_prog_1.Text = tab.Rows[0]["prog_1"].ToString();
                    lb_prog_2.Text = tab.Rows[0]["prog_2"].ToString();
                    lb_prog_mid.Text = "MID";
                }
                return tab;
            }
        }
        catch (Exception ex)
        {
            wyjatek += "\\r\\nBład pobrania MojeLotnisko: " + ex.Message;
        }
        return tab;
    }
    protected bool TestWaznosciPomiarow(int id_lotnisko, int opoznienie_sek)
    {
        bool wynik = false;
        try
        {
            DateTime now = DateTime.Now.AddSeconds(opoznienie_sek);
            DataTable tab = sub_bag.LoadAWOSSQL("select * from pomiary_data where id_lotnisko =" + id_lotnisko, uzyt, pass);
            if (tab.Rows.Count == 1)
            {
                DateTime data_pomiar = (DateTime)tab.Rows[0]["data"];
                if (data_pomiar.Date == now.Date && data_pomiar.Hour == now.Hour && data_pomiar.Minute >= now.Minute)
                    wynik = true;
                else
                    wynik = false;
            }
        }
        catch (Exception ex)
        {
            wynik = false;
            wyjatek += "\\r\\nBład TestWaznosciPomiarow: " + ex.Message;
        }
        return wynik;
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <param name="param4"></param>
    /// <param name="param5"></param>
    /// <returns></returns>
    protected DataTable MakeTab(string param1 = "", string param2 = "", string param3 = "", string param4 = "", string param5 = "", string param6 = "",
        string param7 = "", string param8 = "", string param9 = "", string param10 = "")
    {
        DataTable tab1 = new DataTable();
        tab1.Columns.Add(param1, typeof(string));
        tab1.Columns.Add(param2, typeof(int));
        if (param3 != "")
            tab1.Columns.Add(param3, typeof(int));
        if (param4 != "")
            tab1.Columns.Add(param4, typeof(int));
        if (param5 != "")
            tab1.Columns.Add(param5, typeof(int));
        if (param6 != "")
            tab1.Columns.Add(param6, typeof(int));
        if (param7 != "")
            tab1.Columns.Add(param7, typeof(int));
        if (param8 != "")
            tab1.Columns.Add(param8, typeof(int));
        if (param9 != "")
            tab1.Columns.Add(param9, typeof(int));
        if (param10 != "")
            tab1.Columns.Add(param10, typeof(int));
        return tab1;
    }

    protected bool PomiarOK(string pomiar)
    {
        if ((pomiar != "") && (pomiar != "?"))
            return true;
        else
            return false;
    }
    protected void GraphChart(Chart nazwaWykresu, int time, int id_lotnisko, int kanal, string chartTitle, string seriesTitle, string jednostka, Color kolor, bool param = false)
    {
        DataTable tabAll = MakeTab("data", "wartosc");

        DataTable tab = sub_bag.LoadAWOSSQL("SELECT wartosc,data FROM public.pomiary WHERE id_kanal='" + kanal + "' And data >'" + DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss") + "' And id_lotnisko='" + id_lotnisko + "'", uzyt, pass);

        if (param)
            foreach (DataRow row in tab.Rows)
            {
                if (PomiarOK(row["wartosc"].ToString()) == true)
                    if (Convert.ToInt32(row["wartosc"]) <= 10000)
                    tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), row["wartosc"].ToString());
            }
        else
            foreach (DataRow row in tab.Rows)
                if (PomiarOK(row["wartosc"].ToString()) == true)
                    tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), row["wartosc"].ToString());

        DataRow[] result = tabAll.Select("wartosc > 0");

        if (result.Count() > 2)
        {
            #region ClearChart
            nazwaWykresu.Series.Clear();
            nazwaWykresu.ChartAreas.Clear();
            nazwaWykresu.Titles.Clear();
            nazwaWykresu.Legends.Clear();
            #endregion

            #region InitProp
            Legend legend = new Legend();
            ChartArea chartArea = new ChartArea();
            Series series = new Series();
            Title title = new Title();
            DataPoint points = new DataPoint();
            #endregion

            #region GetData
            nazwaWykresu.Visible = true;
            nazwaWykresu.DataSource = tabAll;
            #endregion

            #region LegendInit
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Bottom;
            #endregion

            #region SeriesInit
            series.Name = seriesTitle;
            series.XValueMember = "data";
            series.YValueMembers = "wartosc";
            series.ChartType = SeriesChartType.Line;
            series.Color = kolor;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 4;
            series.BorderWidth = 2;
            series.ToolTip = "Godzina: #VALX, " + seriesTitle + ": #VALY " + jednostka;
            #endregion

            #region TitleInit
            title.Text = chartTitle;
            title.Font = new Font("Verdana", 10f, FontStyle.Bold);

            #endregion

            #region Colors
            if (Session["tryb"].ToString() == "noc")
            {
                nazwaWykresu.BackColor = Color.FromArgb(1, 29, 43);
                series.LabelBackColor = Color.FromArgb(1, 29, 43);
                legend.BackColor = Color.FromArgb(1, 29, 43);
                legend.ForeColor = Color.FromArgb(21, 88, 107);
                title.ForeColor = Color.FromArgb(21, 88, 107);
                chartArea.BackColor = Color.FromArgb(1, 29, 43);
                chartArea.AxisX.LineColor = Color.Gray;
                chartArea.AxisY.LineColor = Color.Gray;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Gray;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Gray;
            }
            else
            {
                nazwaWykresu.BackColor = Color.FromArgb(242, 242, 242);
                series.LabelBackColor = Color.FromArgb(242, 242, 242);
                legend.BackColor = Color.FromArgb(242, 242, 242);
                title.ForeColor = Color.Black;
                legend.ForeColor = Color.Black;
                chartArea.BackColor = Color.FromArgb(242, 242, 242);
                chartArea.AxisX.LineColor = Color.Black;
                chartArea.AxisY.LineColor = Color.Black;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Black;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Black;
            }
            #endregion

            #region AssignInitedParams
            nazwaWykresu.Width = 580;
            nazwaWykresu.Height = 200;
            nazwaWykresu.Legends.Add(legend);
            nazwaWykresu.ChartAreas.Add(chartArea);
            nazwaWykresu.Series.Add(series);
            nazwaWykresu.Titles.Add(title);
            nazwaWykresu.AntiAliasing = AntiAliasingStyles.All;
            #endregion
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nazwaWykresu"></param>
    /// <param name="time"></param>
    /// <param name="id_lotnisko"></param>
    /// <param name="kanalWys"></param>
    /// <param name="kanalCloud"></param>
    /// <param name="chartTitle"></param>
    /// <param name="jednostka"></param>
    protected void CloudChart(Chart nazwaWykresu, int time, int id_lotnisko, int kanalWys, int kanalCloud, string chartTitle, string jednostka)
    {
        DataTable tabAll = MakeTab("data", "wartoscFEW", "wartoscSCT", "wartoscBKN", "wartoscOVC");

        DataTable tabW = sub_bag.LoadAWOSSQL("SELECT wartosc,data FROM public.pomiary WHERE id_kanal='" + kanalWys + "' And data >'" + DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss") + "' And id_lotnisko='" + id_lotnisko + "'", uzyt, pass);
        DataTable tabC = sub_bag.LoadAWOSSQL("SELECT wartosc,data FROM public.pomiary WHERE id_kanal='" + kanalCloud + "' And data >'" + DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss") + "' And id_lotnisko='" + id_lotnisko + "'", uzyt, pass);


        foreach (DataRow rowW in tabW.Rows)
            foreach (DataRow rowC in tabC.Rows)
            {
                if (rowW["data"].ToString() == rowC["data"].ToString())
                {
                    switch (KodChmur(rowC["wartosc"].ToString()))
                    {
                        case "FEW":
                            tabAll.Rows.Add(Dane.ConvTime(rowW["data"].ToString()), rowW["wartosc"].ToString(), null, null, null);
                            break;
                        case "SCT":
                            tabAll.Rows.Add(Dane.ConvTime(rowW["data"].ToString()), null, rowW["wartosc"].ToString(), null, null);
                            break;
                        case "BKN":
                            tabAll.Rows.Add(Dane.ConvTime(rowW["data"].ToString()), null, null, rowW["wartosc"].ToString(), null);
                            break;
                        case "OVC":
                            tabAll.Rows.Add(Dane.ConvTime(rowW["data"].ToString()), null, null, null, rowW["wartosc"].ToString());
                            break;
                    }
                }
            }

        DataRow[] result = tabW.Select("wartosc > 0");
        if (result.Count() > 2)
        {
            #region ClearChart
            nazwaWykresu.Series.Clear();
            nazwaWykresu.ChartAreas.Clear();
            nazwaWykresu.Titles.Clear();
            nazwaWykresu.Legends.Clear();
            #endregion

            #region InitProp
            Legend legend = new Legend();
            ChartArea chartArea = new ChartArea();
            Title title = new Title();
            DataPoint points = new DataPoint();
            Series seriesFEW = new Series();
            Series seriesSCT = new Series();
            Series seriesBKN = new Series();
            Series seriesOVC = new Series();
            #endregion

            #region GetData
            nazwaWykresu.Visible = true;
            nazwaWykresu.DataSource = tabAll;
            #endregion

            #region LegendInit
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Bottom;
            seriesFEW.Name = "FEW";
            seriesFEW.Color = Color.LightBlue;
            seriesSCT.Name = "SCT";
            seriesSCT.Color = Color.SkyBlue;
            seriesBKN.Name = "BKN";
            seriesBKN.Color = Color.CornflowerBlue;
            seriesOVC.Name = "OVC";
            seriesOVC.Color = Color.Blue;
            seriesFEW.ChartType = SeriesChartType.Point;
            seriesFEW.MarkerStyle = MarkerStyle.Circle;
            seriesSCT.ChartType = SeriesChartType.Point;
            seriesSCT.MarkerStyle = MarkerStyle.Circle;
            seriesBKN.ChartType = SeriesChartType.Point;
            seriesBKN.MarkerStyle = MarkerStyle.Circle;
            seriesOVC.ChartType = SeriesChartType.Point;
            seriesOVC.MarkerStyle = MarkerStyle.Circle;
            seriesFEW.MarkerSize = 8;
            seriesSCT.MarkerSize = 8;
            seriesBKN.MarkerSize = 8;
            seriesOVC.MarkerSize = 8;
            #endregion

            #region InitSeriesFEW  
            DataRow[] resultFEW = tabAll.Select("wartoscFEW > 0");
            if (resultFEW.Count() > 0)
            {
                seriesFEW.XValueMember = "data";
                seriesFEW.YValueMembers = "wartoscFEW";
                seriesFEW.BorderWidth = 2;
                seriesFEW.ToolTip = "Godzina:  #AXISLABEL, " + "FEW / wysokość:" + ": #VALY " + jednostka;
            }
            #endregion

            #region InitSeriesSCT
            DataRow[] resultSCT = tabAll.Select("wartoscSCT > 0");
            if (resultSCT.Count() > 0)
            {
                seriesSCT.XValueMember = "data";
                seriesSCT.YValueMembers = "wartoscSCT";
                seriesSCT.BorderWidth = 2;
                seriesSCT.ToolTip = "Godzina: #AXISLABEL, " + "SCT / wysokość" + ": #VALY " + jednostka;
            }
            #endregion

            #region InitSeriesBKN
            DataRow[] resultBKN = tabAll.Select("wartoscBKN > 0");
            if (resultBKN.Count() > 0)
            {
                seriesBKN.XValueMember = "data";
                seriesBKN.YValueMembers = "wartoscBKN";
                seriesBKN.BorderWidth = 2;
                seriesBKN.ToolTip = "Godzina: #AXISLABEL, " + "BKN / wysokość" + ": #VALY " + jednostka;
            }
            #endregion

            #region InitSeriesOVC
            DataRow[] resultOVC = tabAll.Select("wartoscOVC > 0");
            if (resultOVC.Count() > 0)
            {
                seriesOVC.XValueMember = "data";
                seriesOVC.YValueMembers = "wartoscOVC";
                seriesOVC.BorderWidth = 2;
                seriesOVC.ToolTip = "Godzina: #AXISLABEL, " + "OVC / wysokość" + ": #VALY " + jednostka;
            }
            #endregion


            #region TitleInit
            title.Text = chartTitle;
            title.Font = new Font("Verdana", 10f, FontStyle.Bold);

            #endregion

            #region Colors
            if (Session["tryb"].ToString() == "noc")
            {
                nazwaWykresu.BackColor = Color.FromArgb(1, 29, 43);
                seriesFEW.LabelBackColor = Color.FromArgb(1, 29, 43);
                seriesSCT.LabelBackColor = Color.FromArgb(1, 29, 43);
                seriesBKN.LabelBackColor = Color.FromArgb(1, 29, 43);
                seriesOVC.LabelBackColor = Color.FromArgb(1, 29, 43);
                legend.BackColor = Color.FromArgb(1, 29, 43);
                legend.ForeColor = Color.FromArgb(21, 88, 107);
                title.ForeColor = Color.FromArgb(21, 88, 107);
                chartArea.BackColor = Color.FromArgb(1, 29, 43);
                chartArea.AxisX.LineColor = Color.Gray;
                chartArea.AxisY.LineColor = Color.Gray;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Gray;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Gray;
            }
            else
            {
                nazwaWykresu.BackColor = Color.FromArgb(242, 242, 242);
                seriesFEW.LabelBackColor = Color.FromArgb(242, 242, 242);
                seriesSCT.LabelBackColor = Color.FromArgb(242, 242, 242);
                seriesBKN.LabelBackColor = Color.FromArgb(242, 242, 242);
                seriesOVC.LabelBackColor = Color.FromArgb(242, 242, 242);
                legend.BackColor = Color.FromArgb(242, 242, 242);
                title.ForeColor = Color.Black;
                legend.ForeColor = Color.Black;
                chartArea.BackColor = Color.FromArgb(242, 242, 242);
                chartArea.AxisX.LineColor = Color.Black;
                chartArea.AxisY.LineColor = Color.Black;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Black;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Black;
            }
            #endregion

            #region AssignInitedParams
            nazwaWykresu.Width = 580;
            nazwaWykresu.Height = 200;
            nazwaWykresu.Legends.Add(legend);
            nazwaWykresu.ChartAreas.Add(chartArea);
            nazwaWykresu.Titles.Add(title);
            nazwaWykresu.Series.Add(seriesFEW);
            nazwaWykresu.Series.Add(seriesSCT);
            nazwaWykresu.Series.Add(seriesBKN);
            nazwaWykresu.Series.Add(seriesOVC);
            nazwaWykresu.AntiAliasing = AntiAliasingStyles.All;
            #endregion
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nazwaWykresu"></param>
    /// <param name="time"></param>
    /// <param name="id_lotnisko"></param>
    /// <param name="kanal1"></param>
    /// <param name="kanal2"></param>
    /// <param name="chartTitle"></param>
    /// <param name="jednostka"></param>
    /// <param name="series1Name"></param>
    /// <param name="series2Name"></param>
    protected void VISChart(Chart nazwaWykresu, int time, int id_lotnisko, int kanal1, int kanal2, string chartTitle, string jednostka, string series1Name, string series2Name)
    {
        DataTable tabAll = MakeTab("data", "wartosc1", "wartosc2");

        DataTable tab1 = sub_bag.LoadAWOSSQL("SELECT wartosc,data FROM public.pomiary WHERE id_kanal='" + kanal1 + "' And data >'" + DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss") + "' And id_lotnisko='" + id_lotnisko + "'", uzyt, pass);
        DataTable tab2 = sub_bag.LoadAWOSSQL("SELECT wartosc,data FROM public.pomiary WHERE id_kanal='" + kanal2 + "' And data >'" + DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss") + "' And id_lotnisko='" + id_lotnisko + "'", uzyt, pass);

        foreach (DataRow row1 in tab1.Rows)
            foreach (DataRow row2 in tab2.Rows)
            {
                string tmp1 = row1["wartosc"].ToString().Replace("P", null);
                string tmp2 = row2["wartosc"].ToString().Replace("P", null);

                tmp1 = (Int32.Parse(tmp1) >= 10000) ? "10000" : tmp1;
                tmp2 = (Int32.Parse(tmp2) >= 10000) ? "10000" : tmp2;

                if (row1["data"].ToString() == row2["data"].ToString())
                    tabAll.Rows.Add(Dane.ConvTime(row1["data"].ToString()), tmp1, tmp2);
            }

        DataRow[] result1 = tabAll.Select("wartosc1 > 0");
        DataRow[] result2 = tabAll.Select("wartosc2 > 0");
        if (result1.Count() > 2 || result2.Count() > 2)
        {
            #region ClearChart
            nazwaWykresu.Series.Clear();
            nazwaWykresu.ChartAreas.Clear();
            nazwaWykresu.Titles.Clear();
            nazwaWykresu.Legends.Clear();
            #endregion

            #region InitProp
            Legend legend = new Legend();
            ChartArea chartArea = new ChartArea();
            Title title = new Title();
            DataPoint points = new DataPoint();
            Series series1 = new Series();
            Series series2 = new Series();

            #endregion

            #region GetData
            nazwaWykresu.Visible = true;
            nazwaWykresu.DataSource = tabAll;
            #endregion

            #region LegendInit
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Bottom;
            series1.Name = series1Name;
            series1.Color = Color.Purple;
            series2.Name = series2Name;
            series2.Color = Color.MediumPurple;
            series1.ChartType = SeriesChartType.Line;
            series1.MarkerStyle = MarkerStyle.Circle;
            series2.ChartType = SeriesChartType.Line;
            series2.MarkerStyle = MarkerStyle.Circle;
            series1.MarkerSize = 4;
            series2.MarkerSize = 4;
            #endregion

            #region InitSeries1  
            DataRow[] result_1 = tabAll.Select("wartosc1 > 0");
            if (result_1.Count() > 0)
            {
                series1.XValueMember = "data";
                series1.YValueMembers = "wartosc1";
                series1.BorderWidth = 2;
                series1.ToolTip = "Godzina:  #AXISLABEL, " + " Widzialność:" + ": #VALY " + jednostka;
            }
            #endregion

            #region InitSeries2
            DataRow[] result_2 = tabAll.Select("wartosc2 > 0");
            if (result_2.Count() > 0)
            {
                series2.XValueMember = "data";
                series2.YValueMembers = "wartosc2";
                series2.BorderWidth = 2;
                series2.ToolTip = "Godzina: #AXISLABEL, " + " Widzialność:" + ": #VALY " + jednostka;
            }
            #endregion         


            #region TitleInit
            title.Text = chartTitle;
            title.Font = new Font("Verdana", 10f, FontStyle.Bold);

            #endregion

            #region Colors
            if (Session["tryb"].ToString() == "noc")
            {
                nazwaWykresu.BackColor = Color.FromArgb(1, 29, 43);
                series1.LabelBackColor = Color.FromArgb(1, 29, 43);
                series2.LabelBackColor = Color.FromArgb(1, 29, 43);
                legend.BackColor = Color.FromArgb(1, 29, 43);
                legend.ForeColor = Color.FromArgb(21, 88, 107);
                title.ForeColor = Color.FromArgb(21, 88, 107);
                chartArea.BackColor = Color.FromArgb(1, 29, 43);
                chartArea.AxisX.LineColor = Color.Gray;
                chartArea.AxisY.LineColor = Color.Gray;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Gray;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Gray;
            }
            else
            {
                nazwaWykresu.BackColor = Color.FromArgb(242, 242, 242);
                series1.LabelBackColor = Color.FromArgb(242, 242, 242);
                series2.LabelBackColor = Color.FromArgb(242, 242, 242);
                legend.BackColor = Color.FromArgb(242, 242, 242);
                title.ForeColor = Color.Black;
                legend.ForeColor = Color.Black;
                chartArea.BackColor = Color.FromArgb(242, 242, 242);
                chartArea.AxisX.LineColor = Color.Black;
                chartArea.AxisY.LineColor = Color.Black;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Black;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Black;
            }
            #endregion

            #region AssignInitedParams
            nazwaWykresu.Width = 580;
            nazwaWykresu.Height = 200;
            nazwaWykresu.Legends.Add(legend);
            nazwaWykresu.ChartAreas.Add(chartArea);
            nazwaWykresu.Titles.Add(title);
            nazwaWykresu.Series.Add(series1);
            nazwaWykresu.Series.Add(series2);
            nazwaWykresu.AntiAliasing = AntiAliasingStyles.All;
            #endregion
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seria"></param>
    /// <param name="nazwa"></param>
    /// <param name="kolor"></param>
    /// <param name="XValMemb"></param>
    /// <param name="YValMemb"></param>
    /// <param name="ToolTip"></param>
    /// <param name="typWykresu"></param>
    protected void AddSeries(Series seria, string nazwa, Color kolor, string XValMemb, string YValMemb, string ToolTip, SeriesChartType typWykresu, bool legendVisible, int markerSize)
    {
        seria.IsVisibleInLegend = legendVisible;
        seria.Name = nazwa;
        seria.Color = kolor;
        seria.ChartType = SeriesChartType.Line;
        seria.MarkerStyle = MarkerStyle.Circle;
        seria.MarkerSize = markerSize;
        seria.XValueMember = XValMemb;
        seria.YValueMembers = YValMemb;
        seria.BorderWidth = 5;
        seria.ToolTip = ToolTip;
    }

    protected void WindRoseChart(Chart nazwaWykresu, int time, int id_lotnisko, int kanal, string chartTitle, string jednostka)
    {
        string kierPasa1 = "";
        string kierPasa2 = "";
        string kierProstopadle = "";

        DataTable tabAll = new DataTable();
        tabAll.Columns.Add("data", typeof(string));
        for (int i = 1; i < 11; i++)
        {
            tabAll.Columns.Add("wartosc" + i, typeof(int));
        }

        DataTable tab = sub_bag.LoadAWOSSQL("SELECT data, wartosc FROM public.pomiary WHERE id_kanal='" + kanal + "' And data >'" + DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss") + "' And id_lotnisko='" + id_lotnisko + "'", uzyt, pass);
        //int indPoczTab = 0;
        //int indKoncTab = tab.Rows.Count - 1;

        switch (id_lotnisko)
        {
            case 1: //EPSC 130 - 310 id_lotnisko=1
                kierPasa1 = "130";
                kierPasa2 = "310";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 360->80, 180->260";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "360", "0", "80", "180", "259", "260");
                break;
            case 4: //EPGD 110 - 290 id_lotnisko=4
                kierPasa1 = "110";
                kierPasa2 = "290";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 340->60, 160->240";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "0", "60", "340", "360", "160", "240");

                break;
            case 5: //EPLL 70 - 250 id_lotnisko=5
                kierPasa1 = "70";
                kierPasa2 = "250";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 300->20, 120->200";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "0", "20", "300", "360", "120", "200");


                break;
            case 6: //EPWR 110 - 290 id_lotnisko=6
                kierPasa1 = "110";
                kierPasa2 = "290";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 340->60, 160->240";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "0", "60", "340", "360", "160", "240");

                break;
            case 7: //EPKT 80 - 260 id_lotnisko=7
                kierPasa1 = "80";
                kierPasa2 = "260";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 310->30, 130->210";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "0", "30", "310", "360", "130", "210");


                break;
            case 8: //EPKK 70 - 250 id_lotnisko=8
                kierPasa1 = "70";
                kierPasa2 = "250";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 300->20, 120->200";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "0", "20", "300", "360", "120", "200");

                break;
            case 9: //EPKK 70 - 250 id_lotnisko=8
                kierPasa1 = "90";
                kierPasa2 = "270";
                kierProstopadle = "Kierunki prostopadłe do osi pasa: 320->40, 140->220";

                foreach (DataRow row in tab.Rows)
                    if (PomiarOK(row["wartosc"].ToString()) == true)
                        tabAll.Rows.Add(Dane.ConvTime(row["data"].ToString()), kierPasa1, kierPasa2, row["wartosc"].ToString(), "0", "40", "320", "360", "140", "220");

                break;
        }


        DataRow[] result = tabAll.Select("wartosc3 > 0");
        if (result.Count() > 2)
        {
            #region ClearChart
            nazwaWykresu.Series.Clear();
            nazwaWykresu.ChartAreas.Clear();
            nazwaWykresu.Titles.Clear();
            nazwaWykresu.Legends.Clear();
            #endregion

            #region InitProp
            Legend legend = new Legend();
            ChartArea chartArea = new ChartArea();
            Title title = new Title();
            DataPoint points = new DataPoint();
            Series series1 = new Series();
            Series series2 = new Series();
            Series series3 = new Series();
            Series series4 = new Series();
            Series series5 = new Series();
            Series series6 = new Series();
            Series series7 = new Series();
            Series series8 = new Series();
            Series series9 = new Series();
            Series series10 = new Series();
            #endregion

            #region GetData
            nazwaWykresu.Visible = true;
            nazwaWykresu.DataSource = tabAll;
            #endregion

            #region LegendInit
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Bottom;
            #endregion

            #region AddSeries            
            AddSeries(series1, "Kierunek pasa: " + kierPasa1 + "-" + kierPasa2 + "st", Color.Black, "data", "wartosc1", "Kierunek pasa" + ": #VALY " + jednostka, SeriesChartType.Line, true, 4);
            AddSeries(series2, "Kierunek pasa " + kierPasa2, Color.Black, "data", "wartosc2", "Kierunek pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);
            AddSeries(series3, "Kierunek wiatru", Color.DarkBlue, "data", "wartosc3", "Godzina: #AXISLABEL, " + " Kierunek wiatru:" + ": #VALY " + jednostka, SeriesChartType.Point, true, 8);
            AddSeries(series4, kierProstopadle, Color.LightBlue, "data", "wartosc4", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, true, 4);
            AddSeries(series5, "Kierunek prostopadły do osi pasa 2", Color.LightBlue, "data", "wartosc5", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);
            AddSeries(series6, "Kierunek prostopadły do osi pasa 3", Color.LightBlue, "data", "wartosc6", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);
            AddSeries(series7, "Kierunek prostopadły do osi pasa 4", Color.LightBlue, "data", "wartosc7", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);
            AddSeries(series8, "Kierunek prostopadły do osi pasa 5", Color.LightBlue, "data", "wartosc8", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);
            AddSeries(series9, "Kierunek prostopadły do osi pasa 6", Color.LightBlue, "data", "wartosc9", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);
            AddSeries(series10, "Kierunek prostopadły do osi pasa 7", Color.LightBlue, "data", "wartosc10", "Kierunek prostopadły do osi pasa:" + ": #VALY " + jednostka, SeriesChartType.Line, false, 4);

            #endregion


            #region TitleInit
            title.Text = chartTitle;
            title.Font = new Font("Verdana", 10f, FontStyle.Bold);
            #endregion

            #region Colors
            if (Session["tryb"].ToString() == "noc")
            {
                nazwaWykresu.BackColor = Color.FromArgb(1, 29, 43);
                series3.LabelBackColor = Color.FromArgb(1, 29, 43);
                series4.LabelBackColor = Color.FromArgb(1, 29, 43);
                legend.BackColor = Color.FromArgb(1, 29, 43);
                legend.ForeColor = Color.FromArgb(21, 88, 107);
                title.ForeColor = Color.FromArgb(21, 88, 107);
                chartArea.BackColor = Color.FromArgb(1, 29, 43);
                chartArea.AxisX.LineColor = Color.Gray;
                chartArea.AxisY.LineColor = Color.Gray;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Gray;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Gray;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Gray;
            }
            else
            {
                nazwaWykresu.BackColor = Color.FromArgb(242, 242, 242);
                series3.LabelBackColor = Color.FromArgb(242, 242, 242);
                series4.LabelBackColor = Color.FromArgb(242, 242, 242);
                legend.BackColor = Color.FromArgb(242, 242, 242);
                title.ForeColor = Color.Black;
                legend.ForeColor = Color.Black;
                chartArea.BackColor = Color.FromArgb(242, 242, 242);
                chartArea.AxisX.LineColor = Color.Black;
                chartArea.AxisY.LineColor = Color.Black;
                chartArea.Axes[1].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[0].MajorGrid.LineColor = Color.Black;
                chartArea.Axes[1].LabelStyle.ForeColor = Color.Black;
                chartArea.Axes[0].LabelStyle.ForeColor = Color.Black;
            }
            #endregion

            #region AssignInitedParams

            chartArea.AxisY.Interval = 45;
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = 360;
            nazwaWykresu.Width = 580;
            nazwaWykresu.Height = 360;
            nazwaWykresu.Legends.Add(legend);
            nazwaWykresu.ChartAreas.Add(chartArea);
            nazwaWykresu.Titles.Add(title);
            nazwaWykresu.Series.Add(series4);
            nazwaWykresu.Series.Add(series5);
            nazwaWykresu.Series.Add(series6);
            nazwaWykresu.Series.Add(series7);
            nazwaWykresu.Series.Add(series8);
            nazwaWykresu.Series.Add(series9);
            nazwaWykresu.Series.Add(series10);
            nazwaWykresu.Series.Add(series1);
            nazwaWykresu.Series.Add(series2);
            nazwaWykresu.Series.Add(series3);
            nazwaWykresu.AntiAliasing = AntiAliasingStyles.All;
            #endregion
        }
    }

    protected void SaveChart(string sql, string param, string comm1, string comm2)
    {
        try
        {
            sub_bag.InputAWOSSQL(sql, "", "");
            wyjatek = comm1 + " ustawienia " + param;
        }
        catch (Exception ex)
        {
            wyjatek = "Problem z " + comm2 + "\\r\\n\\r\\n" + ex.ToString();
        }
    }

    #region TDZ GRAPH    
    protected void TDZ_Graph(int id_lotnisko, int time)
    {
        try
        {
            WindTdz();
            CloudTdz();
            VisTdz();
        }
        catch (Exception ex)
        {
            wyjatek += "Error " + ex.Message;
        }
    }

    protected void WindTdz()
    {
        //Kierunek wiatru
        WindRoseChart(RoseWind_tdz, Convert.ToInt32(Session["zakres"]), idlot, 5, "Wykres kierunku wiatru z zaznaczeniem kierunku/osi pasa", "[st]");
        //Wiatr
        GraphChart(ChartWind_tdz, Convert.ToInt32(Session["zakres"]), idlot, 6, "Wykres prędkości wiatru średnia 10min", "Prędkość", " [kt]", Color.DarkBlue);
        GraphChart(ChartPoryw_tdz, Convert.ToInt32(Session["zakres"]), idlot, 87, "Wykres porywów wiatru średnia 10min", "Poryw", " [kt]", Color.DarkBlue);
    }

    protected void WindBtnTdz_Click(object sender, EventArgs e)
    {
        WindTdz();
        CheckTdz("cloud");
        CheckTdz("vis");
        CheckMid("all");
        CheckEnd("all");

        if (WindButtonTdz.Text == "WIND OFF")
        {
            WindButtonTdz.Text = "WIND ON";
            WindButtonTdz.CssClass = "btn2";
        }
        else
        {
            WindButtonTdz.Text = "WIND OFF";
            WindButtonTdz.CssClass = "btn1";
            RoseWind_tdz.Visible = false;
            ChartWind_tdz.Visible = false;
            ChartPoryw_tdz.Visible = false;
        }
    }

    protected void VisTdz()
    {
        //widzialnosc            
        VISChart(ChartTDZ_rvr_2A, Convert.ToInt32(Session["zakres"]), idlot, 68, 8, "RVR / VIS 1min średnia", "[m]", "RVR", "VIS");
    }

    protected void VisBtnTdz_Click(object sender, EventArgs e)
    {
        VisTdz();
        CheckTdz("cloud");
        CheckTdz("wind");
        CheckMid("all");
        CheckEnd("all");

        if (VisButtonTdz.Text == "VIS OFF")
        {
            VisButtonTdz.Text = "VIS ON";
            VisButtonTdz.CssClass = "btn2";
        }
        else
        {
            VisButtonTdz.Text = "VIS OFF";
            VisButtonTdz.CssClass = "btn1";
            ChartTDZ_rvr_2A.Visible = false;
        }
    }

    protected void CloudTdz()
    {
        CloudChart(ChartCloud3_tdz, Convert.ToInt32(Session["zakres"]), idlot, 17, 14, "Wykres podstawy chmur – warstwa 3", " [ft]");
        CloudChart(ChartCloud2_tdz, Convert.ToInt32(Session["zakres"]), idlot, 18, 15, "Wykres podstawy chmur – warstwa 2", " [ft]");
        CloudChart(ChartCloud1_tdz, Convert.ToInt32(Session["zakres"]), idlot, 19, 16, "Wykres podstawy chmur – warstwa 1", " [ft]");
    }

    protected void CloudBtnTdz_Click(object sender, EventArgs e)
    {
        CloudTdz();
        CheckTdz("wind");
        CheckTdz("vis");
        CheckMid("all");
        CheckEnd("all");

        if (CloudButtonTdz.Text == "CLOUD OFF")
        {
            CloudButtonTdz.Text = "CLOUD ON";
            CloudButtonTdz.CssClass = "btn2";
        }
        else
        {
            CloudButtonTdz.Text = "CLOUD OFF";
            CloudButtonTdz.CssClass = "btn1";
            ChartCloud3_tdz.Visible = false;
            ChartCloud2_tdz.Visible = false;
            ChartCloud1_tdz.Visible = false;
        }
    }

    protected void SaveBtnTdz_Click(object sender, EventArgs e)
    {
        string update = SqlUpdateTdz();
        string insert = SqlInsertTdz();
        string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";

        DataTable tab = sub_bag.LoadAWOSSQL(select, uzyt, pass);
        if (tab.Rows.Count > 0)
            SaveChart(update, "TDZ", "Zaktualizowano", "aktualizacją");
        else
            SaveChart(insert, "TDZ", "Zapisano", "zapisem");
    }

    protected void ClearBtnTdz_Click(object sender, EventArgs e)
    {
        string update = SqlUpdateClearTdz();
        string insert = SqlInsertClearTdz();
        string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";

        DataTable tab = sub_bag.LoadAWOSSQL(select, uzyt, pass);
        if (tab.Rows.Count > 0)
            SaveChart(update, "TDZ", "Zaktualizowano", "aktualizacją");
        else
            SaveChart(insert, "TDZ", "Zapisano", "zapisem");
        ClearLabelButtonTdz();
    }

    protected void ClearLabelButtonTdz()
    {
        CloudButtonTdz.Text = "CLOUD OFF";
        CloudButtonTdz.CssClass = "btn1";
        WindButtonTdz.Text = "WIND OFF";
        WindButtonTdz.CssClass = "btn1";
        VisButtonTdz.Text = "VIS OFF";
        VisButtonTdz.CssClass = "btn1";
        RoseWind_tdz.Visible = false;
        ChartWind_tdz.Visible = false;
        ChartPoryw_tdz.Visible = false;
        ChartTDZ_rvr_2A.Visible = false;
        ChartCloud3_tdz.Visible = false;
        ChartCloud2_tdz.Visible = false;
        ChartCloud1_tdz.Visible = false;
    }

    protected string SqlInsertClearTdz()
    {
        string sql = "INSERT INTO public.user_wykres(id_user, id_lotnisko, tdz_kierunek_wiatru, tdz_predkosc_wiatru, tdz_poryw, tdz_vis_rvr, tdz_cloud3, tdz_cloud2, tdz_cloud1) VALUES (";
        sql += "'" + idkto + "', '" + idlot + "', ";
        sql += "'false', 'false', 'false', 'false', 'false', 'false', 'false');";
        return sql;
    }

    protected string SqlUpdateClearTdz()
    {
        string sql = "UPDATE public.user_wykres SET ";
        sql += "tdz_cloud3 = 'false', tdz_cloud2 = 'false', tdz_cloud1 = 'false', ";
        sql += "tdz_kierunek_wiatru = 'false', tdz_predkosc_wiatru='false', tdz_poryw='false', ";
        sql += "tdz_vis_rvr = 'false' ";
        sql += "WHERE id_user='" + idkto + "'And id_lotnisko='" + idlot + "'";
        return sql;
    }

    protected string SqlInsertTdz()
    {
        string sql = "INSERT INTO public.user_wykres(id_user, id_lotnisko, tdz_kierunek_wiatru, tdz_predkosc_wiatru, tdz_poryw, tdz_vis_rvr, tdz_cloud3, tdz_cloud2, tdz_cloud1) VALUES (";
        sql += "'" + idkto + "', '" + idlot + "', ";
        if (WindButtonTdz.Text == "WIND ON")
            sql += "'true', 'true', 'true', ";
        else
            sql += "'false', 'false', 'false', ";

        if (VisButtonTdz.Text == "VIS ON")
            sql += "'true', ";
        else
            sql += "'false', ";

        if (CloudButtonTdz.Text == "CLOUD ON")
            sql += "'true', 'true', 'true'";
        else
            sql += "'false', 'false', 'false'";

        sql += ");";
        return sql;
    }

    protected string SqlUpdateTdz()
    {
        string sql = "UPDATE public.user_wykres SET ";
        if (CloudButtonTdz.Text == "CLOUD ON")
            sql += "tdz_cloud3 = 'true', tdz_cloud2 = 'true', tdz_cloud1 = 'true', ";
        else
            sql += "tdz_cloud3 = 'false', tdz_cloud2 = 'false', tdz_cloud1 = 'false', ";

        if (WindButtonTdz.Text == "WIND ON")
            sql += "tdz_kierunek_wiatru = 'true', tdz_predkosc_wiatru='true', tdz_poryw='true', ";
        else
            sql += "tdz_kierunek_wiatru = 'false', tdz_predkosc_wiatru='false', tdz_poryw='false', ";

        if (VisButtonTdz.Text == "VIS ON")
            sql += "tdz_vis_rvr = 'true' ";
        else
            sql += "tdz_vis_rvr = 'false' ";

        sql += "WHERE id_user='" + idkto + "'And id_lotnisko='" + idlot + "'";
        return sql;
    }

    protected void CheckTdz(string param)
    {
        switch (param)
        {
            case "cloud":
                if (CloudButtonTdz.Text == "CLOUD ON")
                    CloudTdz();
                break;
            case "wind":
                if (WindButtonTdz.Text == "WIND ON")
                    WindTdz();
                break;
            case "vis":
                if (VisButtonTdz.Text == "VIS ON")
                    VisTdz();
                break;
            case "all":
                if (CloudButtonTdz.Text == "CLOUD ON")
                    CloudTdz();
                if (WindButtonTdz.Text == "WIND ON")
                    WindTdz();
                if (VisButtonTdz.Text == "VIS ON")
                    VisTdz();
                break;
        }
    }

    protected void LoadTdzChart()
    {
        try
        {
            string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";
            DataTable dt = sub_bag.LoadAWOSSQL(select, uzyt, pass);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    if (Convert.ToBoolean(item["tdz_cloud1"]) == true)
                    {
                        CloudTdz();
                        CloudButtonTdz.Text = "CLOUD ON";
                        CloudButtonTdz.CssClass = "btn2";
                    }
                    if (Convert.ToBoolean(item["tdz_kierunek_wiatru"]) == true)
                    {
                        WindTdz();
                        WindButtonTdz.Text = "WIND ON";
                        WindButtonTdz.CssClass = "btn2";
                    }
                    if (Convert.ToBoolean(item["tdz_vis_rvr"]) == true)
                    {
                        VisTdz();
                        VisButtonTdz.Text = "VIS ON";
                        VisButtonTdz.CssClass = "btn2";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            wyjatek += "Problem z dostępem do bazy: " + ex;
        }
    }
    #endregion

    #region MID GRAPH    
    protected void MID_Graph(int id_lotnisko, int time)
    {
        try
        {
            WindMid();
            CloudMid();
            VisMid();
        }
        catch (Exception ex)
        {
            wyjatek += "Error " + ex.Message;
        }
    }

    protected void WindMid()
    {
        //Kierunek wiatru
        WindRoseChart(RoseWind_mid, Convert.ToInt32(Session["zakres"]), idlot, 25, "Wykres kierunku wiatru z zaznaczeniem kierunku/osi pasa", "[st]");

        //wiatr
        GraphChart(ChartWind_Mid, Convert.ToInt32(Session["zakres"]), idlot, 26, "Wykres prędkości wiatru średnia 10min", "Prędkość", " [kt]", Color.DarkBlue);
        GraphChart(ChartPoryw_Mid, Convert.ToInt32(Session["zakres"]), idlot, 88, "Wykres porywów wiatru średnia 10min", "Poryw", " [kt]", Color.DarkBlue);
    }

    protected void WindBtnMid_Click(object sender, EventArgs e)
    {
        WindMid();
        CheckMid("cloud");
        CheckMid("vis");
        CheckTdz("all");
        CheckEnd("all");

        if (WindButtonMid.Text == "WIND OFF")
        {
            WindButtonMid.Text = "WIND ON";
            WindButtonMid.CssClass = "btn2";
        }
        else
        {
            WindButtonMid.Text = "WIND OFF";
            WindButtonMid.CssClass = "btn1";
            RoseWind_mid.Visible = false;
            ChartWind_Mid.Visible = false;
            ChartPoryw_Mid.Visible = false;
        }
    }

    protected void VisMid()
    {
        //widzialnosc dominujaca i minimalna  
        VISChart(ChartMID_widzDomMin, Convert.ToInt32(Session["zakres"]), idlot, 63, 64, "VIS dominująca i minimalna 1min", "[m]", "DOM", "MIN");
    }

    protected void VisBtnMid_Click(object sender, EventArgs e)
    {
        VisMid();
        CheckMid("cloud");
        CheckMid("wind");
        CheckTdz("all");
        CheckEnd("all");

        if (VisButtonMid.Text == "VIS OFF")
        {
            VisButtonMid.Text = "VIS ON";
            VisButtonMid.CssClass = "btn2";
        }
        else
        {
            VisButtonMid.Text = "VIS OFF";
            VisButtonMid.CssClass = "btn1";
            ChartMID_widzDomMin.Visible = false;
        }
    }

    protected void CloudMid()
    {
        CloudChart(ChartCloud3_mid, Convert.ToInt32(Session["zakres"]), idlot, 36, 33, "Wykres podstawy chmur – warstwa 3", " [ft]");
        CloudChart(ChartCloud2_mid, Convert.ToInt32(Session["zakres"]), idlot, 37, 34, "Wykres podstawy chmur – warstwa 2", " [ft]");
        CloudChart(ChartCloud1_mid, Convert.ToInt32(Session["zakres"]), idlot, 38, 35, "Wykres podstawy chmur – warstwa 1", " [ft]");
    }

    protected void CloudBtnMid_Click(object sender, EventArgs e)
    {
        CloudMid();
        CheckMid("wind");
        CheckMid("vis");
        CheckTdz("all");
        CheckEnd("all");

        if (CloudButtonMid.Text == "CLOUD OFF")
        {
            CloudButtonMid.Text = "CLOUD ON";
            CloudButtonMid.CssClass = "btn2";
        }
        else
        {
            CloudButtonMid.Text = "CLOUD OFF";
            CloudButtonMid.CssClass = "btn1";
            ChartCloud3_mid.Visible = false;
            ChartCloud2_mid.Visible = false;
            ChartCloud1_mid.Visible = false;
        }
    }

    protected void SaveBtnMid_Click(object sender, EventArgs e)
    {
        string update = SqlUpdateMid();
        string insert = SqlInsertMid();
        string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";

        DataTable tab = sub_bag.LoadAWOSSQL(select, uzyt, pass);
        if (tab.Rows.Count > 0)
            SaveChart(update, "MID", "Zaktualizowano", "aktualizacją");
        else
            SaveChart(insert, "MID", "Zapisano", "zapisem");
    }

    protected void ClearBtnMid_Click(object sender, EventArgs e)
    {
        string update = SqlUpdateClearMid();
        string insert = SqlInsertClearMid();
        string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";

        DataTable tab = sub_bag.LoadAWOSSQL(select, uzyt, pass);
        if (tab.Rows.Count > 0)
            SaveChart(update, "MID", "Zaktualizowano", "aktualizacją");
        else
            SaveChart(insert, "MID", "Zapisano", "zapisem");
        ClearLabelButtonMid();
    }

    protected void ClearLabelButtonMid()
    {
        CloudButtonMid.Text = "CLOUD OFF";
        CloudButtonMid.CssClass = "btn1";
        WindButtonMid.Text = "WIND OFF";
        WindButtonMid.CssClass = "btn1";
        VisButtonMid.Text = "VIS OFF";
        VisButtonMid.CssClass = "btn1";
        RoseWind_mid.Visible = false;
        ChartWind_Mid.Visible = false;
        ChartPoryw_Mid.Visible = false;
        ChartMID_widzDomMin.Visible = false;
        ChartCloud3_mid.Visible = false;
        ChartCloud2_mid.Visible = false;
        ChartCloud1_mid.Visible = false;
    }

    protected string SqlInsertClearMid()
    {
        string sql = "INSERT INTO public.user_wykres(id_user, id_lotnisko, mid_kierunek_wiatru, mid_predkosc_wiatru, mid_poryw_wiatru, mid_vis_dom_min, mid_cloud3, mid_cloud2, mid_cloud1) VALUES (";
        sql += "'" + idkto + "', '" + idlot + "', ";
        sql += "'false', 'false', 'false', 'false', 'false', 'false', 'false');";
        return sql;
    }

    protected string SqlUpdateClearMid()
    {
        string sql = "UPDATE public.user_wykres SET ";
        sql += "mid_cloud3 = 'false', mid_cloud2 = 'false', mid_cloud1 = 'false', ";
        sql += "mid_kierunek_wiatru = 'false', mid_predkosc_wiatru='false', mid_poryw_wiatru='false', ";
        sql += "mid_vis_dom_min = 'false' ";
        sql += "WHERE id_user='" + idkto + "'And id_lotnisko='" + idlot + "'";
        return sql;
    }

    protected string SqlInsertMid()
    {
        string sql = "INSERT INTO public.user_wykres(id_user, id_lotnisko, mid_kierunek_wiatru, mid_predkosc_wiatru, mid_poryw_wiatru, mid_vis_dom_min, mid_cloud3, mid_cloud2, mid_cloud1) VALUES (";
        sql += "'" + idkto + "', '" + idlot + "', ";
        if (WindButtonMid.Text == "WIND ON")
            sql += "'true', 'true', 'true', ";
        else
            sql += "'false', 'false', 'false', ";

        if (VisButtonMid.Text == "VIS ON")
            sql += "'true', ";
        else
            sql += "'false', ";

        if (CloudButtonMid.Text == "CLOUD ON")
            sql += "'true', 'true', 'true'";
        else
            sql += "'false', 'false', 'false'";

        sql += ");";
        return sql;
    }


    protected string SqlUpdateMid()
    {
        string sql = "UPDATE public.user_wykres SET ";
        if (CloudButtonMid.Text == "CLOUD ON")
            sql += "mid_cloud3 = 'true', mid_cloud2 = 'true', mid_cloud1 = 'true', ";
        else
            sql += "mid_cloud3 = 'false', mid_cloud2 = 'false', mid_cloud1 = 'false', ";

        if (WindButtonMid.Text == "WIND ON")
            sql += "mid_kierunek_wiatru = 'true', mid_predkosc_wiatru='true', mid_poryw_wiatru='true', ";
        else
            sql += "mid_kierunek_wiatru = 'false', mid_predkosc_wiatru='false', mid_poryw_wiatru='false', ";

        if (VisButtonMid.Text == "VIS ON")
            sql += "mid_vis_dom_min = 'true' ";
        else
            sql += "mid_vis_dom_min = 'false' ";

        sql += "WHERE id_user='" + idkto + "'And id_lotnisko='" + idlot + "'";
        return sql;
    }

    protected void CheckMid(string param)
    {
        switch (param)
        {
            case "cloud":
                if (CloudButtonMid.Text == "CLOUD ON")
                    CloudMid();
                break;
            case "wind":
                if (WindButtonMid.Text == "WIND ON")
                    WindMid();
                break;
            case "vis":
                if (VisButtonMid.Text == "VIS ON")
                    VisMid();
                break;
            case "all":
                if (CloudButtonMid.Text == "CLOUD ON")
                    CloudMid();
                if (WindButtonMid.Text == "WIND ON")
                    WindMid();
                if (VisButtonMid.Text == "VIS ON")
                    VisMid();
                break;
        }
    }

    protected void LoadMidChart()
    {
        try
        {
            string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";
            DataTable dt = sub_bag.LoadAWOSSQL(select, uzyt, pass);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    if (Convert.ToBoolean(item["mid_cloud1"]) == true)
                    {
                        CloudMid();
                        CloudButtonMid.Text = "CLOUD ON";
                        CloudButtonMid.CssClass = "btn2";
                    }
                    if (Convert.ToBoolean(item["mid_kierunek_wiatru"]) == true)
                    {
                        WindMid();
                        WindButtonMid.Text = "WIND ON";
                        WindButtonMid.CssClass = "btn2";
                    }
                    if (Convert.ToBoolean(item["mid_vis_dom_min"]) == true)
                    {
                        VisMid();
                        VisButtonMid.Text = "VIS ON";
                        VisButtonMid.CssClass = "btn2";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            wyjatek += "Problem z dostępem do bazy: " + ex;
        }
    }
    #endregion

    #region END GRAPH        
    protected void END_Graph(int id_lotnisko, int time)
    {
        try
        {
            WindEnd();
            CloudEnd();
            VisEnd();
        }
        catch (Exception ex)
        {
            wyjatek += "Error " + ex.Message;
        }
    }

    protected void WindEnd()
    {
        //Kierunek wiatru
        WindRoseChart(RoseWind_end, Convert.ToInt32(Session["zakres"]), idlot, 45, "Wykres kierunku wiatru z zaznaczeniem kierunku/osi pasa", "[st]");

        //wiatr
        GraphChart(ChartWind_end, Convert.ToInt32(Session["zakres"]), idlot, 46, "Wykres prędkości wiatru średnia 10min", "Prędkość", " [kt]", Color.DarkBlue);
        GraphChart(ChartPoryw_end, Convert.ToInt32(Session["zakres"]), idlot, 89, "Wykres porywów wiatru średnia 10min", "Poryw", " [kt]", Color.DarkBlue);
    }

    protected void WindBtnEnd_Click(object sender, EventArgs e)
    {
        WindEnd();
        CheckEnd("cloud");
        CheckEnd("vis");
        CheckMid("all");
        CheckTdz("all");

        if (WindButtonEnd.Text == "WIND OFF")
        {
            WindButtonEnd.Text = "WIND ON";
            WindButtonEnd.CssClass = "btn2";
        }
        else
        {
            WindButtonEnd.Text = "WIND OFF";
            WindButtonEnd.CssClass = "btn1";
            RoseWind_end.Visible = false;
            ChartWind_end.Visible = false;
            ChartPoryw_end.Visible = false;
        }
    }

    protected void VisEnd()
    {
        //widzialnosc       
        VISChart(ChartEND_RvrVis, Convert.ToInt32(Session["zakres"]), idlot, 69, 48, "RVR / VIS 1min średnia", "[m]", "RVR", "VIS");
    }

    protected void VisBtnEnd_Click(object sender, EventArgs e)
    {
        VisEnd();
        CheckEnd("cloud");
        CheckEnd("wind");
        CheckMid("all");
        CheckTdz("all");

        if (VisButtonEnd.Text == "VIS OFF")
        {
            VisButtonEnd.Text = "VIS ON";
            VisButtonEnd.CssClass = "btn2";
        }
        else
        {
            VisButtonEnd.Text = "VIS OFF";
            VisButtonEnd.CssClass = "btn1";
            ChartEND_RvrVis.Visible = false;
        }
    }

    protected void CloudEnd()
    {
        CloudChart(ChartCloud3_end, Convert.ToInt32(Session["zakres"]), idlot, 58, 55, "Wykres podstawy chmur – warstwa 3", " [ft]");
        CloudChart(ChartCloud2_end, Convert.ToInt32(Session["zakres"]), idlot, 60, 56, "Wykres podstawy chmur – warstwa 2", " [ft]");
        CloudChart(ChartCloud1_end, Convert.ToInt32(Session["zakres"]), idlot, 61, 57, "Wykres podstawy chmur – warstwa 1", " [ft]");
    }

    protected void CloudBtnEnd_Click(object sender, EventArgs e)
    {
        CloudEnd();
        CheckEnd("wind");
        CheckEnd("vis");
        CheckMid("all");
        CheckTdz("all");

        if (CloudButtonEnd.Text == "CLOUD OFF")
        {
            CloudButtonEnd.Text = "CLOUD ON";
            CloudButtonEnd.CssClass = "btn2";
        }
        else
        {
            CloudButtonEnd.Text = "CLOUD OFF";
            CloudButtonEnd.CssClass = "btn1";
            ChartCloud3_end.Visible = false;
            ChartCloud2_end.Visible = false;
            ChartCloud1_end.Visible = false;
        }
    }

    protected void SaveBtnEnd_Click(object sender, EventArgs e)
    {
        string update = SqlUpdateEnd();
        string insert = SqlInsertEnd();
        string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";

        DataTable tab = sub_bag.LoadAWOSSQL(select, uzyt, pass);
        if (tab.Rows.Count > 0)
            SaveChart(update, "END", "Zaktualizowano", "aktualizacją");
        else
            SaveChart(insert, "END", "Zapisano", "zapisem");
    }

    protected void ClearBtnEnd_Click(object sender, EventArgs e)
    {
        string update = SqlUpdateClearEnd();
        string insert = SqlInsertClearEnd();
        string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";

        DataTable tab = sub_bag.LoadAWOSSQL(select, uzyt, pass);
        if (tab.Rows.Count > 0)
            SaveChart(update, "END", "Zaktualizowano", "aktualizacją");
        else
            SaveChart(insert, "END", "Zapisano", "zapisem");
        ClearLabelButtonEnd();
    }

    protected void ClearLabelButtonEnd()
    {
        CloudButtonEnd.Text = "CLOUD OFF";
        CloudButtonEnd.CssClass = "btn1";
        WindButtonEnd.Text = "WIND OFF";
        WindButtonEnd.CssClass = "btn1";
        VisButtonEnd.Text = "VIS OFF";
        VisButtonEnd.CssClass = "btn1";
        RoseWind_end.Visible = false;
        ChartWind_end.Visible = false;
        ChartPoryw_end.Visible = false;
        ChartEND_RvrVis.Visible = false;
        ChartCloud3_end.Visible = false;
        ChartCloud2_end.Visible = false;
        ChartCloud1_end.Visible = false;
    }

    protected string SqlInsertClearEnd()
    {
        string sql = "INSERT INTO public.user_wykres(id_user, id_lotnisko, end_kierunek_wiatru, end_predkosc_wiatru, end_poryw, end_vis_rvr, end_cloud3, end_cloud2, end_cloud1) VALUES (";
        sql += "'" + idkto + "', '" + idlot + "', ";
        sql += "'false', 'false', 'false', 'false', 'false', 'false', 'false');";
        return sql;
    }

    protected string SqlUpdateClearEnd()
    {
        string sql = "UPDATE public.user_wykres SET ";
        sql += "end_cloud3 = 'false', end_cloud2 = 'false', end_cloud1 = 'false', ";
        sql += "end_kierunek_wiatru = 'false', end_predkosc_wiatru='false', end_poryw='false', ";
        sql += "end_vis_rvr = 'false' ";
        sql += "WHERE id_user='" + idkto + "'And id_lotnisko='" + idlot + "'";
        return sql;
    }

    protected string SqlInsertEnd()
    {
        string sql = "INSERT INTO public.user_wykres(id_user, id_lotnisko, end_kierunek_wiatru, end_predkosc_wiatru, end_poryw, end_vis_rvr, end_cloud3, end_cloud2, end_cloud1) VALUES (";
        sql += "'" + idkto + "', '" + idlot + "', ";
        if (WindButtonEnd.Text == "WIND ON")
            sql += "'true', 'true', 'true', ";
        else
            sql += "'false', 'false', 'false', ";

        if (VisButtonEnd.Text == "VIS ON")
            sql += "'true', ";
        else
            sql += "'false', ";

        if (CloudButtonEnd.Text == "CLOUD ON")
            sql += "'true', 'true', 'true'";
        else
            sql += "'false', 'false', 'false'";

        sql += ");";
        return sql;
    }

    protected string SqlUpdateEnd()
    {
        string sql = "UPDATE public.user_wykres SET ";
        if (CloudButtonEnd.Text == "CLOUD ON")
            sql += "end_cloud3 = 'true', end_cloud2 = 'true', end_cloud1 = 'true', ";
        else
            sql += "end_cloud3 = 'false', end_cloud2 = 'false', end_cloud1 = 'false', ";

        if (WindButtonEnd.Text == "WIND ON")
            sql += "end_kierunek_wiatru = 'true', end_predkosc_wiatru='true', end_poryw='true', ";
        else
            sql += "end_kierunek_wiatru = 'false', end_predkosc_wiatru='false', end_poryw='false', ";

        if (VisButtonEnd.Text == "VIS ON")
            sql += "end_vis_rvr = 'true' ";
        else
            sql += "end_vis_rvr = 'false' ";

        sql += "WHERE id_user='" + idkto + "'And id_lotnisko='" + idlot + "'";
        return sql;
    }

    protected void CheckEnd(string param)
    {
        switch (param)
        {
            case "cloud":
                if (CloudButtonEnd.Text == "CLOUD ON")
                    CloudEnd();
                break;
            case "wind":
                if (WindButtonEnd.Text == "WIND ON")
                    WindEnd();
                break;
            case "vis":
                if (VisButtonEnd.Text == "VIS ON")
                    VisEnd();
                break;
            case "all":
                if (CloudButtonEnd.Text == "CLOUD ON")
                    CloudEnd();
                if (WindButtonEnd.Text == "WIND ON")
                    WindEnd();
                if (VisButtonEnd.Text == "VIS ON")
                    VisEnd();
                break;
        }
    }

    protected void LoadEndChart()
    {
        try
        {
            string select = "SELECT * FROM public.user_wykres WHERE id_user='" + idkto + "' And id_lotnisko='" + idlot + "'";
            DataTable dt = sub_bag.LoadAWOSSQL(select, uzyt, pass);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    if (Convert.ToBoolean(item["end_cloud1"]) == true)
                    {
                        CloudEnd();
                        CloudButtonEnd.Text = "CLOUD ON";
                        CloudButtonEnd.CssClass = "btn2";
                    }
                    if (Convert.ToBoolean(item["end_kierunek_wiatru"]) == true)
                    {
                        WindEnd();
                        WindButtonEnd.Text = "WIND ON";
                        WindButtonEnd.CssClass = "btn2";
                    }
                    if (Convert.ToBoolean(item["end_vis_rvr"]) == true)
                    {
                        VisEnd();
                        VisButtonEnd.Text = "VIS ON";
                        VisButtonEnd.CssClass = "btn2";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            wyjatek += "Problem z dostępem do bazy: " + ex;
        }
    }
    #endregion

    protected void WyswietlAktualneDane(int id_lotnisko, int time = -3)
    {
        try
        {
            if (uzyt != null)
            {
                DataTable tab = sub_bag.LoadAWOSSQL("select * from view_pomiary_" + id_lotnisko, uzyt, pass);
                if (tab.Rows.Count > 0)
                {
                    WypelnijLabelDanych(tab);
                    CisnienieQNH(ViewState["tabLotnisko"] as DataTable);
                    TempPunktRosy();
                    LoadTdzChart();
                    LoadMidChart();
                    LoadEndChart();
                    CheckTdz("all");
                    CheckMid("all");
                    CheckEnd("all");
                }
                else
                    wyjatek += "\\r\\nBrak danych w bazie!";
            }
        }
        catch (Exception ex)
        {
            wyjatek += "\\r\\nBŁĄD WyswietlAktualneDane :" + ex.Message;
        }
    }
    protected DataTable WyswietlAktualneDane_Tab(int id_lotnisko)
    {
        try
        {
            DataTable tab = sub_bag.LoadAWOSSQL("select * from view_pomiary_" + id_lotnisko, uzyt, pass);
            return tab;
        }
        catch (Exception ex)
        {
            wyjatek += "\\r\\nBŁĄD WyswietlAktualneDane :" + ex.Message;
        }
        return null;
    }

    protected void CisnienieQNH(DataTable tab_lotnisko)
    {
        if (tab_lotnisko.Rows.Count > 0)
        {
            if (tab_lotnisko.Rows[0]["serwer_1"].ToString() == "maws")
            {
                //	Hpa - CISNIENIE Na BAROMETRZE [hPa]
                //	Hb  - WYSOKOSC BAROMETRU [m]     
                //	Hp  - WYSOKOSC PUNKTU ODNIESIENIA [m]        
                //	Ts  - TEMPERATURA POWIETRZA [C] 
                try
                {
                    double Hpa = Convert.ToDouble(press_pa11.Text.Replace('.', ','));
                    double Hb = Convert.ToDouble(tab_lotnisko.Rows[0]["barometr_wys"]);
                    double Hp = Convert.ToDouble(tab_lotnisko.Rows[0]["barometr_wys_podn"]);
                    double Ts = Convert.ToDouble(temp_maws.Text.Replace('.', ','));
                    double wynik = cisnienie.qnh.Qfe2Qnh(Hp, cisnienie.qnh.Hpa2Qfe(Hpa, Hb, Hp, Ts));
                    press_qnh.Text = wynik.ToString("F1");
                    press_qnh.ToolTip = "Opis: Obliczona wartość ciśnienia QNH\r\nDane do obliczeń: \r\nHpa - CISNIENIE Na BAROMETRZE [hPa]: " + Hpa.ToString() + "\r\nHb  - WYSOKOSC BAROMETRU [m]: " + Hb.ToString() +
                        "\r\nHp  - WYSOKOSC PUNKTU ODNIESIENIA [m]: " + Hp.ToString() + "\r\nTs  - TEMPERATURA POWIETRZA [C]: " + Ts.ToString();

                }
                catch (Exception ex)
                {
                    //wyjatek += "\r\n Błąd obliczania QNH: " + ex.Message;
                }
            }
        }


    }
    protected void TempPunktRosy()
    {
        try
        {
            double a = Convert.ToDouble(temp_maws.Text.Replace('.', ','));
            double b = Convert.ToDouble(rh_maws.Text.Replace('.', ','));
            double wynik = Math.Sqrt(Math.Sqrt(Math.Sqrt(b / 100))) * (112 + (0.9 * a)) + (0.1 * a) - 112;
            dew_point_maws.Text = wynik.ToString("F1").Replace(',', '.');
            dew_point_maws.ToolTip = "Opis: Obliczona wartość temperatury puntku rosy\r\n" +
                       "RH  - WILGOTNOŚĆ WZGLĘDNA [%]: " + b.ToString() + "\r\nTs  - TEMPERATURA POWIETRZA [C]: " + a.ToString();

        }
        catch (Exception ex)
        {
            dew_point_maws.Text = "err";
            dew_point_maws.ToolTip = "Błąd przy obliczeniach: " + ex.Message;


        }
    }

    //IMPLEMENTACJA WEB SOAP REQUEST         
    public static byte[] GZipDecompress(byte[] serializedPhenomenas)
    {
        using (MemoryStream to = new MemoryStream())
        {
            using (MemoryStream from = new MemoryStream(serializedPhenomenas))
            {
                using (GZipStream compress = new GZipStream(from, CompressionMode.Decompress))
                {
                    compress.CopyTo(to);
                }
            }
            return to.ToArray();
        }
    }


    protected bool WypelnijLabelDanych_Azure()
    {
        lb_data.ForeColor = System.Drawing.Color.White;
        DataTable tab = new DataTable();
        bool brak_danych = false;

        

        
        


        Task<string> zadanie;

        try
        {
            zadanie = Soap_Client.GetAllMeasurements();            
            zadanie.Wait();         

        }

        catch (Exception ex)
        {
            wyjatek += "Brak danych Azure: " + ex;
        }


        string wind_rose_2B_rose = ""; string wind_rose_3B_rose = ""; string wind_rose_4B_rose = "";
        string wind_rose_3B_rosedata = ""; string wind_rose_4B_rosedata = ""; string wind_rose_2B_rosedata = "";
            
        try
        {            

                Npgsql.NpgsqlConnection nConn = new Npgsql.NpgsqlConnection();
                XmlDocument raw_XML = new XmlDocument();

                //string test = "H4sIAAAAAAAEANVda1PiSBT9vlX7Hyg/I/TtJGAsdKoD0aGGYLEwOvoNlUEYAYvHBPz127xcdpeavu3SW0drUowkwOF4+z5Op2+XPs0Hz5mfnfGkNxqeHVFOHH06//23khqP24ur70mnPZmNO4POcNpavHQy+uLh5HQ+eTw7eppOX07z+TRNc6mXG427eSkE5b8ltebDU2fQPnq7uGe++Lg3nEzbw4fOkf7wTKb0j89dPamfnvYGnXMppH8sisfkt8g7FUL/uyvlV6c2l41eOuP2dDSujbq9YSa/ffpn+3nWOc+U8uv/bJ4d/PVR5ef2ZHIekRCxWOIr5f91coUuvw+eS8zCiLkQrjBfXMJgLmTJjFqIL0vUFRjUIsvn+mOhlmurVjhWbR6JG6aBRiJlAy7TZRzUZqYDPJ/HGYkb/4Hj9cxMS0EfkekALb54IhuaQzmtzVrAwA6yJ2yzbsKgthiMQFwvwTBwr21E4Zi29Fim7cUERbeXldzYGOF4PwqzRQbZARjZvESVwLyf52V9Rv1FYGTnjZg94YFRHZ4UGePRF2gBUhYZhSORBDMRn1OBrYajkjCgC2amN6DLOKA5EV2CGXVo9noboiMgohkhxhfFCyyqOb4aLSyaMRcIDbNH2QK3/oo+48DmWDURWgZi1hU8OF2BYdUBmqNmaJKbMheHZ37mgYOZiuaiaxsRcVAz5o4ITUeVDMFpW5Xj5B5mpn04xTpgqDZbaQ8nIFKRr6MqHNicNA8tuFDIyT0EGtU88ZfA5geI7/eAiltehCEov8dQEQJEmYmjRKLB9nyWWi3AyhermRigiXNtJQy6BVqRu9R+zfcp6AwKTLuRkjMoyQezbsFQQra333ykUCPxUmy2jgrkRBiawkawxsHMnPAHy/k47mObqeLcvae9nnmiLhAUCUlIXo9xfydJMNDlXmby0st8792b60cd0rHAS20pBc6c7gkYcLOleBSgYfZZoxLNwM1uOwwLYJi/MnhGMw9eJgIGmlv0osG2qB6RcJMgjvruwzmRKzNmQrNtzr0KaI6Pp5yBgeZMRaMRneMUM0vMQMVMeZZ5mg0YmRNatqfUV7/eiv2kq26TvlrUyyKtL8S8ftGYJ6+jNKnoo+zrx6itVCKTilJRsz57vJlPlIpJv75/P7ye3Ct1UZUB3V+mSr3epsnO71Grql+v0p1ry/XX3fd5ez5KUn2u1VCqVd1/vrHzvpWqV+s3dq67+KaPa6Uai6TnL5SqSn19nLQaM/399OOP5eMOrh9+feHr73A7Tyrxotbv7rxXktb6SuPQRz+e6++6Oe4qj81o9Hgj1uea/gprWdRfqp/rabsZDdo31e4+XnZ4m//6/OHOLTHavlbbQbctxZJfqvWT//DZmoa+2msjv3zPShzU+g+2n1tZ2ul+HLvfIU7Xx//Dv5u/aayifnVp41Lbc7Q6/na++g5bOzszua9mXax/Ihj3ZU4kPPLAXC5b8AfDzVgLRT4YZkYl4qHxbLYND842OHkmmipAHE0UzXmYk2O5HoTRNQxmClnqHFodcnlVuavWr27iXFwvX1Xi6/iPnNFk3mIkEP2MGIkmajCXM4ChLjCUL0+gxUjWQlAwzGY36FEIhtnMswcn1LHVXDDcHoNrtHHI8x5oFsJPSHBumFhq/Tmm1i98ILJlMcecE4KCTWGOmQBCweZOG0KB5k5TQIFmpiBQmMOTYo4p40DhJp81GF27PrIdjAhr9m1BY6zGsEWNsJ7LFjPI3ci2sEma13O5bzZgixqjXaQ11xiL52xhIyyeO7wHcb/21hYzSg8ea8/HGIzu1wwf3kLc9160ZhpjgZG9gSDcaH94A3HfBtWFq3bdGsZJiuq8ibILpl03eXPBtPvGz9ZZE0bLRWuyAfqs2GI+FoxQ7r6Jly1sj1hNANHyJow2udZkY7TEOnw0d9+qyd5A2Ik1kOQE0rX18AHdfVc9a6ox+jtYw4Zo/WxdxYB0pbDWysyg3TdceUfSh9D8yIHsBFczgrTmPDzV7ntc2mIucppMue6gbD3vBdGo7j1iKkJTGydFgfPtVRyWMkCoEVqtuJCt0SIMSJN+R0YNNhSJn1wDBRmETobWZu2zlBDXG++8YzQi9DK0t2uEvfTskyeEDbxc3hSCwzXGJkcOkhDn+1oefm7D/aYqTuUbh9u7OeDa+W6+1l6Pk4K4brzoQAB+xwYUpfz+zbHP/wQHEQ5VTHsAAA==";
                //byte[] data = Convert.FromBase64String(test);

                byte[] data = Convert.FromBase64String(Convert.ToString(Soap_Client.wynik).Substring(Soap_Client.wynik.IndexOf("<measurements>"), Soap_Client.wynik.IndexOf("</measurements>") - Soap_Client.wynik.IndexOf("<measurements>")).Replace("<measurements>", ""));


                byte[] bDecompresed = GZipDecompress(data);

                string decodedString = Encoding.UTF8.GetString(bDecompresed);

                XmlDocument xdoc = new XmlDocument();

                xdoc.LoadXml(decodedString);

                wind_rose_5B.Text = decodedString;


                XmlNodeList listaWartosci = xdoc.GetElementsByTagName("value");

                XmlNodeList listaKanalow = xdoc.GetElementsByTagName("measurementClass");

                XmlNodeList listaCzas = xdoc.GetElementsByTagName("time");


            for (int i = 0; i < listaKanalow.Count; i++) // parsowanie string XML
            {

                string kanal = Convert.ToString(listaKanalow[i].InnerXml);
                string pomiar = listaWartosci[i].InnerXml.Replace(",", ".");
                string czas = listaCzas[i].InnerXml.Replace("T", " ").Replace("Z", "");
                label_czas = czas;

                switch (kanal)
                {
                    case "B200E020C2":
                        wind_rose_1B.Text = MSnaKT(pomiar);
                        wind_rose_1B.ToolTip = "Ave: " + MSnaKT(pomiar) + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B200E020C2" + "\r\nOpis: prędkość wiatru/automat MAWS/pomiary minutowe automatyczne/średnia 2 min.";
                        break;
                    case "B201E020C1":
                        //wind_rose_5B.Text = pomiar;
                        wind_rose_5B.ToolTip = "Ave: " + pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B201E020C1" + "\r\nOpis: kierunek/automat MAWS/pomiary minutowe automatyczne/średnia 10 min.";
                        break;
                    case "B200E020C1":
                        wind_rose_6B.Text = MSnaKT(pomiar);
                        wind_rose_6B.ToolTip = "Ave: " + MSnaKT(pomiar) + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B200E020C1" + "\r\nOpis: prędkość wiatru/automat MAWS/pomiary minutowe automatyczne/średnia 10 min."; ;
                        break;
                    case "B200E020B1":
                        wind_rose_7B.Text = MSnaKT(pomiar);
                        wind_rose_7B.ToolTip = "Max: " + MSnaKT(pomiar) + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B200E020B1" + "\r\nOpis: prędkość wiatru/automat MAWS/pomiary minutowe automatyczne/max 10 min.";
                        break;
                    case "B610E020C1":
                        widz_maws.Text = pomiar;
                        widz_maws.ToolTip = "Ave: " + pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B610E020C1" + "\r\nOpis: widzialność pozioma/automat MAWS/minutowe automatyczne/średnia 10 min.";
                        break;
                    case "B400F02000":
                        press_pa11.Text = pomiar;
                        press_pa11.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B400F02000" + "\r\nOpis: ciśnienie obserwowane (na poziomie stacji)/automat PA11/pomiary min. automat./wartość bezpośrednia";
                        break;
                    case "B400E02000":
                        press_maws.Text = pomiar;
                        press_maws.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B400E02000" + "\r\nOpis: ciśnienie obserwowane (na poziomie stacji)/automat MAWS/pomiary min. automat./wartość bezpośrednia";
                        break;
                    case "B100E02000":
                        temp_maws.Text = pomiar;
                        temp_maws.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B100E02000" + "\r\nOpis: temperatura powietrza w klatce meteorologicznej/pomiary minutowe automatyczne/wartość bezpośrednia";
                        break;
                    case "B500E02000":
                        rh_maws.Text = pomiar;
                        rh_maws.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B500E02000" + "\r\nOpis: wilgotność względna/automat MAWS/pomiary minutowe automatyczne/wartość bezpośrednia";
                        break;
                    case "B600K02000":
                        opad_1m_maws.Text = pomiar;
                        opad_1m_maws.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B600K02000" + "\r\nOpis: opad wartość/SEBA podłączona do MAWS/pomiary minutowe automatyczne/wartość bezpośrednia";
                        break;
                    case "B600K020FG":
                        opad_1h_maws.Text = pomiar;
                        opad_1h_maws.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B600K020FG" + "\r\nOpis: opad wartość/SEBA podłączona do MAWS/pomiary minutowe automatyczne/suma godzinowa";
                        break;
                    case "B600K020FD":
                        opad_24h_maws.Text = pomiar;
                        opad_24h_maws.ToolTip = pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B600K020FD" + "\r\nOpis: opad wartość/SEBA podłączona do MAWS/pomiary minutowe automatyczne/suma dobowa";
                        break;
                    // DANE NIEZBĘDNE do Róży wiatrów
                    case "B201E020A2":
                        sub_bag.InputAWOSSQL("INSERT INTO pomiary (id_lotnisko,id_kanal,data,wartosc,data_pomiaru) values ('18','76','" + DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:00") + "','" + pomiar + "','" + czas + "')", uzyt, pass);
                        wind_rose_2B_rose = pomiar;
                        wind_rose_2B_rosedata = czas;
                        break;
                    case "B201E020B2":
                        sub_bag.InputAWOSSQL("INSERT INTO pomiary (id_lotnisko,id_kanal,data,wartosc,data_pomiaru) values ('18','77','" + DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:00") + "','" + pomiar + "','" + czas + "')", uzyt, pass);
                        wind_rose_4B_rose = pomiar;
                        wind_rose_4B_rosedata = czas;
                        break;
                    case "B201E020C2":
                        wind_rose_3B.Text = pomiar;
                        wind_rose_3B.ToolTip = "Ave: " + pomiar + "\r\nData pomiaru: " + czas + "\r\nParametr: " + "B201E020C2" + "\r\nOpis: kierunek/automat MAWS/pomiary minutowe automatyczne/średnia 2 min.";
                        wind_rose_3B_rose = pomiar;
                        wind_rose_3B_rosedata = czas;
                        break;
                }

                lb_data.Text = label_czas; // czas ostatniego pomiaru ze stringa XML
            }
            if (lb_data.Text == "")
                lb_data.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        }

        catch (Exception ex)
        {
            wyjatek += "Błąd: " + ex;
            div_nieaktualne.Visible = true;
            brak_danych = true;
        }

        if (!brak_danych)
        {
            try
            {
                double Hpa = Convert.ToDouble(press_pa11.Text.Replace('.', ','));
                double Hb = 222.5;
                double Hp = 222.5;
                double Ts = Convert.ToDouble(temp_maws.Text.Replace('.', ','));
                double wynik_qnh = cisnienie.qnh.Qfe2Qnh(Hp, cisnienie.qnh.Hpa2Qfe(Hpa, Hb, Hp, Ts));
                press_qnh.Text = wynik_qnh.ToString("F1").Replace(',', '.');
                press_qnh.ToolTip = "Opis: Obliczona wartość ciśnienia QNH\r\nDane do obliczeń: \r\nHpa - CISNIENIE Na BAROMETRZE [hPa]: " + Hpa.ToString() + "\r\nHb  - WYSOKOSC BAROMETRU [m]: " + Hb.ToString() +
                     "\r\nHp  - WYSOKOSC PUNKTU ODNIESIENIA [m]: " + Hp.ToString() + "\r\nTs  - TEMPERATURA POWIETRZA [C]: " + Ts.ToString();

            }

            catch (Exception ex)
            {
                //wyjatek += "Błąd w obliczeniach Ciśnienia QNH";
                press_qnh.Text = "/";
            }

            try
            {
                double a = Convert.ToDouble(temp_maws.Text.Replace('.', ','));
                double b = Convert.ToDouble(rh_maws.Text.Replace('.', ','));
                double wynik_rosa = Math.Sqrt(Math.Sqrt(Math.Sqrt(b / 100))) * (112 + (0.9 * a)) + (0.1 * a) - 112;

                dew_point_maws.Text = wynik_rosa.ToString("F1").Replace(',', '.');
                dew_point_maws.ToolTip = "Opis: Obliczona wartość temperatury puntku rosy\r\n" +
                           "RH  - WILGOTNOŚĆ WZGLĘDNA [%]: " + b.ToString() + "\r\nTs  - TEMPERATURA POWIETRZA [C]: " + a.ToString();
            }

            catch (Exception ex)
            {
                //wyjatek += "Błąd w obliczeniach temp. punktu rosy";
                dew_point_maws.Text = "/";
            }

            try
            {
                tab.Columns.Add("parametr", typeof(string));
                tab.Columns.Add("grupa", typeof(int));
                tab.Columns.Add("wartosc", typeof(string));
                tab.Columns.Add("tooltip", typeof(string));


                DataRow row2B = tab.NewRow();
                row2B["parametr"] = "WD_min";
                row2B["grupa"] = 2;
                row2B["wartosc"] = RozaZaokraglenie(wind_rose_2B_rose);
                row2B["tooltip"] = "Min: " + wind_rose_2B_rose + "\r\nData pomiaru: " + wind_rose_2B_rosedata + "\r\nParametr: " + "B201E020A2" + "\r\nOpis: " + "kierunek/automat MAWS/pomiary minutowe automatyczne/min 2 min.";
                tab.Rows.Add(row2B);

                DataRow row4B = tab.NewRow();
                row4B["parametr"] = "WD_max";
                row4B["grupa"] = 2;
                row4B["wartosc"] = RozaZaokraglenie(wind_rose_4B_rose);
                row4B["tooltip"] = "Max: " + wind_rose_4B_rose + "\r\nData pomiaru: " + wind_rose_4B_rosedata + "\r\nParametr: " + "B201E020B2" + "\r\nOpis: " + "kierunek/automat MAWS/pomiary minutowe automatyczne/max 2 min.";
                tab.Rows.Add(row4B);

                DataRow row3B = tab.NewRow();
                row3B["parametr"] = "WD_av";
                row3B["grupa"] = 2;
                row3B["wartosc"] = RozaZaokraglenie(wind_rose_3B_rose);
                row3B["tooltip"] = "Ave: " + wind_rose_3B_rose + "\r\nData pomiaru: " + wind_rose_3B_rosedata + "\r\nParametr: " + "B200E020C2" + "\r\nOpis: " + "prędkość wiatru/ automat MAWS/pomiary minutowe automatyczne/średnia 2 min.";
                tab.Rows.Add(row3B);

                RozaWiatrow(tab);

            }
            catch (Exception ex)
            {
                wyjatek += "Brak danych do obliczenia róży wiatru";
            }
        }

        return brak_danych;
    }

    protected void ColorFill(Label label, string kolor, Color borderColor)
    {
        label.BorderColor = borderColor;
        label.Style.Add("color", kolor);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="backColor"></param>
    /// <param name="foreColor"></param>
    /// <param name="tekst"></param>
    protected void AlertStyle(Label label, Color backColor, Color foreColor, string tekst)
    {
        label.BackColor = backColor;
        //label.Width = 610;
        label.Text = tekst;
        label.Visible = true;
        label.Font.Bold = true;
        label.ForeColor = foreColor;
        label.BorderWidth = 4;
    }

    /// <summary>
    /// Metoda koloryzująca label wilgotności
    /// </summary>
    /// <param name="dane"></param>
    /// <param name="label"></param>
    /// <param name="labelname"></param>
    protected void ColorFillHumidity(DataTable dane, string label, Label labelname)
    {
        foreach (DataRow item in dane.Rows)
        {
            if (item["parametr_label"].ToString() == label)
            {
                double x = Int32.Parse(item["wartosc"].ToString().Replace(".", ","));
                if (x >= 99) //99
                {
                    ColorFill(labelname, "brown", Color.Brown);
                    AlertStyle(infoLabelHumidity_tdz, Color.Brown, Color.White, "HUMIDITY");
                }
            }
        }
    }

    /// <summary>
    /// Metoda koloryzująca label Chmur podstawy najniższej
    /// </summary>
    /// <param name="dane"></param>
    /// <param name="label"></param>
    /// <param name="labelname"></param>
    protected void ColorFillCloud(DataTable dane, string label, Label labelname, Label labelinfo)
    {
        foreach (DataRow item in dane.Rows)
        {
            if (item["parametr_label"].ToString() == label)
            {
                double x = Int32.Parse(item["wartosc"].ToString().Replace(".", ","));
                if ((x != 0) && (x <= 300))
                {
                    ColorFill(labelname, "DeepSkyBlue", Color.DeepSkyBlue);
                    AlertStyle(labelinfo, Color.DeepSkyBlue, Color.White, "CLOUD");
                }
            }
        }
    }


    /// <summary>
    /// Metoda koloryzująca label widzialności przeważającej VIS DOM
    /// </summary>
    /// <param name="dane"></param>
    /// <param name="label"></param>
    /// <param name="labelname"></param>
    /// <param name="labelinfo"></param>
    protected void ColorFillVisDom(DataTable dane, string label, Label labelname)
    {
        foreach (DataRow item in dane.Rows)
        {
            if (item["parametr_label"].ToString() == label)
            {
                double x = Int32.Parse(item["wartosc"].ToString().Replace(".", ","));
                if (x <= 1500)
                {
                    ColorFill(labelname, "Gold", Color.Gold);
                    AlertStyle(infoLabelVisDom_mid, Color.Gold, Color.White, "VIS DOM");
                }
            }
        }
    }


    /// <summary>
    /// Metoda koloryzujaca label w zaleznosci od wartosci temp
    /// </summary>
    /// <param name="dane">Tabela</param>
    /// <param name="label">Label</param>
    protected void ColorFillTemp(DataTable dane, string label, Label labelname)
    {
        foreach (DataRow item in dane.Rows)
        {
            if (item["parametr_label"].ToString() == label)
            {
                if (!item["wartosc"].ToString().Contains("M"))
                {
                    //Regex regex =item["wartosc"].ToString() new Regex(".[0-9]");
                    //double x = Double.Parse(regex.Replace(item["wartosc"].ToString(), ""));                   
                    double x = Double.Parse(item["wartosc"].ToString().Replace(".", ","));
                    if (x >= 30.0)
                    {
                        ColorFill(labelname, "red", Color.Red);
                        AlertStyle(infoLabelTemp_tdz, Color.Red, Color.White, "TEMP");
                    }
                    else if (x >= 28.0 && x <= 29.9)
                    {
                        ColorFill(labelname, "orange", Color.Orange);
                        AlertStyle(infoLabelTemp_tdz, Color.Orange, Color.White, "TEMP");
                    }
                    else if (x >= 0 && x <= 2)
                    {
                        ColorFill(labelname, "blue", Color.Blue);
                        AlertStyle(infoLabelTemp_tdz, Color.Blue, Color.White, "TEMP");

                    }
                    else
                    {
                        ColorFill(labelname, "black", Color.Gray);
                        infoLabelTemp_tdz.Visible = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label1"></param>
    /// <param name="label2"></param>
    /// <param name="wartosc"></param>
    protected void CheckWind(Label label1, Label label2, Int32 wartosc)
    {
        if (wartosc > 27)
        {
            ColorFill(label1, "red", Color.Red);
            AlertStyle(label2, Color.Red, Color.White, "WIND");
        }
        else
        {
            label2.Visible = false;
            ColorFill(label1, "black", Color.Gray);
        }
    }

    /// <summary>
    /// Metoda koloryzujaca label w zaleznosci od wartosci predkosci wiatru
    /// </summary>
    /// <param name="dane"></param>
    /// <param name="label"></param>
    /// <param name="labelname"></param>
    protected void ColorFillLabel(DataTable dane, string label)
    {

        foreach (DataRow item in dane.Rows)
        {
            if (PomiarOK(item["wartosc"].ToString()) == true)
                switch (item["parametr_label"].ToString())
                {
                    case "wind_rose_6A":
                        CheckWind(wind_rose_6A, infoLabelWind_tdz, Convert.ToInt32(item["wartosc"]));
                        break;
                    case "wind_rose_7A":
                        CheckWind(wind_rose_7A, infoLabelWind_tdz, Convert.ToInt32(item["wartosc"]));
                        break;
                    case "wind_rose_6B":
                        CheckWind(wind_rose_6B, infoLabelWind_mid, Convert.ToInt32(item["wartosc"]));
                        break;
                    case "wind_rose_7B":
                        CheckWind(wind_rose_7B, infoLabelWind_mid, Convert.ToInt32(item["wartosc"]));
                        break;
                    case "wind_rose_6C":
                        CheckWind(wind_rose_6C, infoLabelWind_end, Convert.ToInt32(item["wartosc"]));
                        break;
                    case "wind_rose_7C":
                        CheckWind(wind_rose_7C, infoLabelWind_end, Convert.ToInt32(item["wartosc"]));
                        break;
                }
        }
    }

    protected void WypelnijLabelDanych(DataTable dane)
    {
        lb_data.Text = dane.Rows[0]["data"].ToString();
        //  lb_data.ForeColor = System.Drawing.Color.White;
        DataTable tab = new DataTable();
        tab.Columns.Add("parametr", typeof(string));
        tab.Columns.Add("grupa", typeof(int));
        tab.Columns.Add("wartosc", typeof(string));
        tab.Columns.Add("tooltip", typeof(string));
        foreach (DataRow item in dane.Rows)
        {
            switch (item["parametr_label"].ToString())
            {

                case "wind_rose_2A":
                    DataRow row2A = tab.NewRow();
                    row2A["parametr"] = "WD_min";
                    row2A["grupa"] = 1;
                    row2A["wartosc"] = item["wartosc"].ToString();
                    row2A["tooltip"] = "Min: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row2A);
                    break;
                case "wind_rose_4A":
                    DataRow row4A = tab.NewRow();
                    row4A["parametr"] = "WD_max";
                    row4A["grupa"] = 1;
                    row4A["wartosc"] = item["wartosc"].ToString();
                    row4A["tooltip"] = "Max: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row4A);
                    break;
                case "wind_rose_3A":
                    DataRow row3A = tab.NewRow();
                    row3A["parametr"] = "WD_av";
                    row3A["grupa"] = 1;
                    row3A["wartosc"] = item["wartosc"].ToString();
                    row3A["tooltip"] = "Ave: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row3A);
                    wind_rose_3A.Text = item["wartosc"].ToString();
                    wind_rose_3A.ToolTip = "Data pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    break;
                case "wind_rose_2B":
                    DataRow row2B = tab.NewRow();
                    row2B["parametr"] = "WD_min";
                    row2B["grupa"] = 2;
                    row2B["wartosc"] = RozaZaokraglenie(item["wartosc"].ToString());
                    row2B["tooltip"] = "Min: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row2B);
                    break;
                case "wind_rose_4B":
                    DataRow row4B = tab.NewRow();
                    row4B["parametr"] = "WD_max";
                    row4B["grupa"] = 2;
                    row4B["wartosc"] = RozaZaokraglenie(item["wartosc"].ToString());
                    row4B["tooltip"] = "Max: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row4B);
                    break;
                case "wind_rose_3B":
                    DataRow row3B = tab.NewRow();
                    row3B["parametr"] = "WD_av";
                    row3B["grupa"] = 2;
                    row3B["wartosc"] = RozaZaokraglenie(item["wartosc"].ToString());
                    row3B["tooltip"] = "Ave: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row3B);
                    wind_rose_3B.Text = RozaZaokraglenie(item["wartosc"].ToString());
                    wind_rose_3B.ToolTip = "Ave: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    break;
                case "wind_rose_5B":
                    wind_rose_5B.Text = RozaZaokraglenie(item["wartosc"].ToString());
                    wind_rose_5B.ToolTip = "Wartość: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    break;
                case "wind_rose_2C":
                    DataRow row2C = tab.NewRow();
                    row2C["parametr"] = "WD_min";
                    row2C["grupa"] = 3;
                    row2C["wartosc"] = item["wartosc"].ToString();
                    row2C["tooltip"] = "Min: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row2C);
                    break;
                case "wind_rose_4C":
                    DataRow row4C = tab.NewRow();
                    row4C["parametr"] = "WD_max";
                    row4C["grupa"] = 3;
                    row4C["wartosc"] = item["wartosc"].ToString();
                    row4C["tooltip"] = "Max: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row4C);
                    break;
                case "wind_rose_3C":
                    DataRow row3C = tab.NewRow();
                    row3C["parametr"] = "WD_av";
                    row3C["grupa"] = 3;
                    row3C["wartosc"] = item["wartosc"].ToString();
                    row3C["tooltip"] = "Ave: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    tab.Rows.Add(row3C);
                    wind_rose_3C.Text = item["wartosc"].ToString();
                    wind_rose_3C.ToolTip = "Data pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                    break;
                default:
                    //TDZ
                    ColorFillLabel(dane, item["parametr_label"].ToString());
                    ColorFillTemp(dane, "temp_all", temp_all);

                    //HUMIDITY
                    ColorFillHumidity(dane, "rh_all", rh_all);

                    //CLOUD
                    ColorFillCloud(dane, "cloud_1A", cloud_1A, infoLabelCloud_tdz);
                    ColorFillCloud(dane, "cloud_1", cloud_1, infoLabelCloud_mid);
                    ColorFillCloud(dane, "cloud_1C", cloud_1C, infoLabelCloud_end);

                    //VIS DOM
                    ColorFillVisDom(dane, "vis_dom", vis_dom);


                    Label lb = (Label)Page.FindControl(item["parametr_label"].ToString());
                    if (lb != null)
                    {
                        if (lb.ID.Contains("cloud_okt_"))
                        {
                            lb.Text = KodChmur(item["wartosc"].ToString());
                            lb.ToolTip = "Wartość: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                        }
                        else if (lb.ID.Contains("cloud_"))
                        {
                            if (item["wartosc"].ToString() == "0")
                            {
                                lb.Text = "-";
                                lb.ToolTip = "Wartość: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                            }
                            else
                            {
                                lb.Text = item["wartosc"].ToString();
                                lb.ToolTip = "Data pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                            }
                        }
                        else if (lb.ID.Contains("weather_"))
                        {
                            lb.Text = item["wartosc"].ToString();
                            lb.ToolTip = "DANE Z CZUJNIKA AUTOMATYCZNEGO\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                        }
                        else if (lb.ID.Contains("metreport"))
                        {
                            lb.Text = item["wartosc"].ToString();
                            lb.ToolTip = "Data pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                        }
                        else if (lb.ID.Contains("qnh"))
                        {
                            string[] tmp = item["wartosc"].ToString().Split(new string[] { ",", "." }, StringSplitOptions.RemoveEmptyEntries);
                            if (tmp.Length == 2)
                                lb.Text = tmp[0];
                            else
                                lb.Text = item["wartosc"].ToString();
                            lb.ToolTip = "Wartość: " + item["wartosc"].ToString() + "\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                        }
                        else
                        {
                            if (item["wartosc"].ToString().StartsWith("M"))
                                lb.Text = item["wartosc"].ToString().Replace('M', '-');
                            else
                                lb.Text = item["wartosc"].ToString();
                            lb.ToolTip = "Data pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                            if (item["parametr"].ToString() == "B200E002B1" || item["parametr"].ToString() == "B200E002C1" || item["parametr"].ToString() == "B200E002C2")
                            {
                                lb.Text = MSnaKT(item["wartosc"].ToString());
                                lb.ToolTip = "Wartość: " + item["wartosc"].ToString() + "m/s\r\nData pomiaru: " + item["data_pomiaru"].ToString() + "\r\nParametr: " + item["parametr"].ToString() + "\r\nOpis: " + item["opis"].ToString();
                            }
                        }
                    }
                    else
                        wyjatek += "\\r\\n Nie znaleziono kontrolki: " + item["parametr_label"].ToString();
                    break;
            }
        }
        if (cloud_1.Text != "-")
            vertvis.Text = "-";
        if (cloud_1A.Text != "-")
            vertvis_A.Text = "-";
        if (cloud_1C.Text != "-")
            vertvis_C.Text = "-";
        RozaWiatrow(tab);
    }
    protected string MSnaKT(string daneIn)
    {
        double kt = 1.9438461718;
        try
        {
            System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            double d = double.Parse(daneIn.Replace(",", "."), nfi);
            double x = d * kt;
            if (x > 0.5)
                return (Math.Round(x, 0)).ToString();
            else
                return "0";
        }
        catch (Exception ex)
        {
            wyjatek += "Bład Convert to Double - MSnaKT: " + ex.Message;
            return "x";
        }

    }
    protected string RozaZaokraglenie(string dana)
    {
        try
        {
            int i = Convert.ToInt16(dana);
            return (((int)Math.Round(i / 10.0)) * 10).ToString();
        }
        catch (Exception ex)
        {
            //wyjatek += "\\r\\n Zaokraglenie: " + ex.Message;
            return "?";
        }
    }
    protected void RozaWiatrow(DataTable tab)
    {
        string color = "#C0C0C0";
        string color_new = "#FF6600";
        string color_w = "#FF9d5d";
        if (Session["tryb"].ToString() == "noc")
        {
            color = "#04394b";
            color_new = "#15586b";
            color_w = "#0f4954";
        }
        if (tab.Rows.Count > 0)
        {
            for (int g = 1; g < 4; g++)
            {
                DataRow[] row = tab.Select(" grupa = " + g);
                if (row.Count() > 0)
                {
                    for (int j = 0; j < 360; j += 10)
                    {
                        HtmlControl div = (HtmlControl)Page.FindControl("stp" + j + g);
                        if (div != null)
                            div.Style["background-color"] = color;
                    }
                    string min = "0", max = "0";
                    foreach (DataRow rowIn in row)
                    {
                        switch (rowIn["parametr"].ToString())
                        {
                            case "WD_min":
                                min = rowIn["wartosc"].ToString();
                                if (min.Length == 3 && min.StartsWith("0"))
                                    min = min.Substring(1, 2);
                                if (min == "00" || min == "000" || min == "360")
                                    min = "0";
                                HtmlControl div = (HtmlControl)Page.FindControl("stp" + min + g);
                                if (div != null)
                                {
                                    div.Style["background-color"] = color_new;
                                    div.Attributes["title"] = (string)rowIn["tooltip"];
                                }
                                break;
                            case "WD_max":
                                max = rowIn["wartosc"].ToString();
                                if (max.Length == 3 && max.StartsWith("0"))
                                    max = max.Substring(1, 2);
                                if (max == "00" || max == "000" || max == "360")
                                    max = "0";
                                HtmlControl div2 = (HtmlControl)Page.FindControl("stp" + max + g);
                                if (div2 != null)
                                {
                                    div2.Style["background-color"] = color_new;
                                    div2.Attributes["title"] = (string)rowIn["tooltip"];
                                }
                                break;
                            case "WD_av":
                                string av = rowIn["wartosc"].ToString();
                                HtmlControl div3 = (HtmlControl)Page.FindControl("av" + g);
                                if (div3 != null)
                                {
                                    if (av != "?")
                                    {
                                        int[] tl = TopLeft(av);
                                        div3.Attributes["title"] = (string)rowIn["tooltip"];
                                        div3.Attributes["style"] = "transform:rotate(" + av + "deg); -webkit-transform:rotate(" + av + "deg); -moz-transform:rotate(" + av + "deg); -o-transform:rotate(" + av + "deg);position:absolute;background-color: " + color_w + ";"
                                            + "top:" + tl[0].ToString() + "px;left:" + tl[1].ToString() + "px;";
                                    }
                                }
                                break;
                        }
                    }
                    try
                    {
                        if (max != "?" && min != "?")
                        {
                            int Imin = Convert.ToInt32(min);
                            int Imax = Convert.ToInt32(max);
                            if (Imin == 360)
                                Imin = 0;
                            if (Imax == 360)
                                Imax = 0;
                            int w = Imin;
                            while (w != Imax)
                            {
                                HtmlControl div = (HtmlControl)Page.FindControl("stp" + w + g);
                                if (div != null)
                                    div.Style["background-color"] = color_new;
                                if (w == 360)
                                    w = 0;
                                else
                                    w += 10;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        wyjatek += "\\r\\nBład danych Wiatrowych!";
                    }
                }
            }

        }
        else
            wyjatek += "\\r\\n Nie znaleziono danych do rózy wiatrów!";
    }
    protected int[] TopLeft(string ang)
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
    protected string KodChmur(string dana)
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
                {"?","ERR"}
        };
        return map[dana];
    }
    protected void WyborLotniska()
    {
        Tik.Enabled = false;
        ViewState["tabLotnisko"] = null;
        if (ddl_wybierz_lotnisko.Items.Count <= 0)
        {
            try
            {
                if (idkto != null)
                {
                    string sql = "select vl.id_lotnisko, vl.nazwa from user_lotnisko ul inner join view_lotnisko vl on vl.id_lotnisko = ul.id_lotnisko and vl.aktywny=true where ul.id_user = " + idkto + " order by nazwa";
                    DataTable tab = sub_bag.LoadAWOSSQL(sql, uzyt, pass);
                    if (tab.Rows.Count > 0)
                    {
                        ddl_wybierz_lotnisko.DataSource = tab;
                        ddl_wybierz_lotnisko.DataTextField = tab.Columns["nazwa"].ToString();
                        ddl_wybierz_lotnisko.DataValueField = tab.Columns["id_lotnisko"].ToString();
                        ddl_wybierz_lotnisko.DataBind();
                        if (wiele_okien == "True")
                        {
                            cb_wiele_okien.Visible = true;
                            ddl_wybierz_lotnisko.SelectionMode = ListSelectionMode.Multiple;
                            ddl_wybierz_lotnisko.SelectedIndexChanged -= new EventHandler(btn_lotnisko_Click);
                            ddl_wybierz_lotnisko.AutoPostBack = false;
                        }
                        else
                        {
                            cb_wiele_okien.Visible = false;
                        }
                        div_okno.Visible = true;
                        div_wybor.Visible = true;
                    }
                }
                else wyjatek += "Aktywna może być tylko jedna sesja! Zaloguj się ponownie.";
            }
            catch (Exception ex)
            {
                wyjatek += "//r//nProblem z WyborLotniska: " + ex.Message;
            }
        }
    }
    protected void btn_lotnisko_Click(object sender, EventArgs e)
    {
        if (!cb_wiele_okien.Checked)
        {
            if (ddl_wybierz_lotnisko.SelectedValue != "")
            {
                ViewState["tabLotnisko"] = null;
                Response.Redirect("default.aspx?id=" + Convert.ToInt32(ddl_wybierz_lotnisko.SelectedValue));
            }
        }
        else
        {
            string js = "";
            ViewState["tabLotnisko"] = null;

            foreach (ListItem item in ddl_wybierz_lotnisko.Items)
            {
                if (item.Selected && item.Value != ddl_wybierz_lotnisko.SelectedValue)
                    js += "window.open('default.aspx?id=" + item.Value + "');";
            }
            if (ddl_wybierz_lotnisko.SelectedValue != "")
                js += "window.open('default.aspx?id=" + ddl_wybierz_lotnisko.SelectedValue + "','_self');";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "wind_open", js, true);

        }
    }
    protected void btn_OK_Click(object sender, EventArgs e)
    {
        div_okno.Visible = false;
        div_nieaktualne.Visible = false;
        Tik.Interval = 15 * 1000;
    }
    protected void btn_OK2_Click(object sender, EventArgs e)
    {
        div_okno.Visible = false;
        div_tab.Visible = false;
    }
    protected void lb_dane_Click(object sender, EventArgs e)
    {
        DataTable tab = WyswietlAktualneDane_Tab(idlot);
        if (tab != null)
        {
            tab.Columns.Remove("id_lotnisko");
            tab.Columns.Remove("id_kanal");
            tab.AcceptChanges();
            GridView_tab.DataSource = tab;
            GridView_tab.DataBind();
        }
        div_okno.Visible = true;
        div_tab.Visible = true;
    }
    protected void lb_odswiez_Click(object sender, EventArgs e)
    {

        if (((LinkButton)sender).ID == "lb_odswiez")
            WyswietlAktualneDane(idlot, Convert.ToInt32(Session["zakres"]));
        else if (((LinkButton)sender).ID == "lb_odswiezOff")
        {
            if (Tik.Enabled == false)
            {
                Tik.Enabled = true;
                lb_odswiezOff.Text = "wyłącz odświeżanie";
            }
            else
            {
                Tik.Enabled = false;
                lb_odswiezOff.Text = "włącz odświeżanie";
            }
        }
    }

    protected void lb_zakres_Click(object sender, EventArgs e)
    {
        if (((LinkButton)sender).ID == "lb_zakresDanych")
        {
            WyswietlAktualneDane(idlot, -6);
            if (lb_zakresDanych.Text == "Zakres danych 6h")
            {
                lb_zakresDanych.Text = "Zakres danych 3h";
                WyswietlAktualneDane(idlot, -6);
                Session["zakres"] = "-6";

            }
            else
            {
                lb_zakresDanych.Text = "Zakres danych 6h";
                WyswietlAktualneDane(idlot, -3);
                Session["zakres"] = "-3";
            }
        }

    }

    protected void lb_noc_Click(object sender, EventArgs e)
    {
        if (((LinkButton)sender).ID == "lb_noc")
        {
            foreach (Control c in Page.Header.Controls)
            {
                if (c.ToString() == "System.Web.UI.HtmlControls.HtmlLink")
                    Page.Header.Controls.Remove(c);
            }

            HtmlLink css = new HtmlLink();
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            css.Attributes["media"] = "all";
            if (lb_noc.Text == "tryb nocny")
            {
                lb_noc.Text = "tryb dzienny";
                css.Href = "Styles/awos_noc.css";
                Session["tryb"] = "noc";
            }
            else
            {
                css.Href = "Styles/awos.css";
                Session["tryb"] = "dzien";
                lb_noc.Text = "tryb nocny";
            }
            Page.Header.Controls.Add(css);
            WyswietlAktualneDane(idlot, Convert.ToInt32(Session["zakres"]));
        }
    }
    protected void lb_logout_Click(object sender, EventArgs e)
    {
        bool x = false;
        Session["userInfo"] = null;
        Session["zakres"] = null;
        x = sub_bag.SessjaLogOut(idkto);
        FormsAuthentication.RedirectToLoginPage();
        Response.Redirect("login.aspx?mes=Wylogowałeś się!", false);
    }
    protected void lb_pass_Click(object sender, EventArgs e)
    {
        div_okno.Visible = true;
        div_haslo.Visible = true;
    }


    protected void Button_Click(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "bt_anuluj")
        {
            tb_new.Text = "";
            tb_new2.Text = "";
            div_okno.Visible = false;
            div_haslo.Visible = false;
        }
        else
        {
            if (TestHasla(tb_new.Text))
            {
                string old_pass = pass;
                if (Crypto.Decrypt(old_pass) == tb_old.Text)
                {
                    string upd = "UPDATE public.user SET haslo='" + Crypto.Encrypt(tb_new.Text) + "' WHERE id_user='" + idkto + "'";
                    try
                    {
                        sub_bag.InputAWOSSQL(upd, "", "");
                        wyjatek = "Hasło zostało zmienione!";
                    }
                    catch (Exception ex)
                    {
                        wyjatek = "Problem ze zmianą hasła! \\r\\n\\r\\n" + ex.ToString();
                    }
                }
                else
                    wyjatek = "Stare hasło jest niepoprawne!";
            }
            else
            {
                tb_new.Text = "";
                tb_new2.Text = "";
            }
        }
    }

    bool TestHasla(string txt)
    {
        bool wynik = true;
        if (txt.Length < 3)
        {
            wyjatek = "hasło musi mieć co najmniej 3 znaki!! \\r\\nsłownie: TRZY";
            wynik = false;
        }
        foreach (char c in txt)
        {
            if (!char.IsLetterOrDigit(c))
            {
                wyjatek = "tylko znaki alfanumeryczne";
                wynik = false;
            }
        }
        if (txt.Contains(" "))
        {
            wyjatek = "nie używaj spacji!";
            wynik = false;
        }
        return wynik;
    }
    public string TelDyzurny()
    {
        try
        {
            //Service1 x = new Service1();
            //return x.telefonDyzurny();
            return "22 5694 372";
        }
        catch (Exception ex)
        {
            return "22 5694 372";
        }
    }
}

