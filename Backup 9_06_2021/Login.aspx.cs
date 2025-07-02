using System;
using System.Collections;
using System.Configuration;
using System.Data;
//using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
//using System.Xml.Linq;
using back;
using krypto;

public partial class login : System.Web.UI.Page
{

    string wyjatek = "";
    string idkto = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
            Session["userInfo"] = null;
        div_okno.Visible = false;
        box.Visible = false;
       
        string message = Request.QueryString["mes"];
        if (!string.IsNullOrEmpty(message))
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Skryptalarmu", "<script type=\"text/javascript\">alert(\"Zostałeś przekierowany na tę stronę z powodu: \\r\\n" + message + "\");</script>", false);
       
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
       
        if (!string.IsNullOrEmpty(wyjatek))
        {
            wyjatek = sub_bag.oczysz_tekst_alertjs(wyjatek);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "Skryptalarmu", "<script type=\"text/javascript\">alert(\"" + wyjatek + "\");</script>", false);
        }
        wyjatek = "";
    }
    protected void LoginUser(object sender, EventArgs e)
    {
        if ((string.IsNullOrEmpty(txtUser.Text) | string.IsNullOrEmpty(txtPassword.Text)) )
        {
            wyjatek = "Nie podałeś LOGINU lub HASŁA!";
            Session["userInfo"] = null;
            return;
        }
        try
        {
            //userInfo initial = new userInfo();
            System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user = new System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string>();          
            user = sub_bag.getLogInfo(txtUser.Text, txtPassword.Text);
            if (user[sub_bag.jakchcesz.status] == "LogOk")
            {
                using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(ConfigurationManager.AppSettings["Katalog"].ToString() + "log.txt"))
                {
                    streamWriter.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "; Zalogonany: " + user[sub_bag.jakchcesz.user] + "; IP:" + Request.ServerVariables["REMOTE_ADDR"] + "; Browser:" + Request.ServerVariables["HTTP_USER_AGENT"] + ";");
                }
                Session["userInfo"] = user;
                ViewState["userInfoChrom"] = user;
                if (string.IsNullOrEmpty(user[sub_bag.jakchcesz.session].ToString()))
                {
                    System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user2 = sub_bag.SessjaUpdate(user[sub_bag.jakchcesz.id_kto], user);
                    Session["userInfo"] = user2;
                    FormsAuthentication.RedirectFromLoginPage(user2[sub_bag.jakchcesz.user], true);
                }
                else
                {
                    div_okno.Visible = true;
                    box.Visible = true;
                    lb_box.Text = "Ostatnie logowanie: " + user[sub_bag.jakchcesz.session].ToString();
                }
            }
            else
            {
                using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(ConfigurationManager.AppSettings["Katalog"].ToString() + "log.txt"))
                {
                    streamWriter.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": Nieudana próba logowania: " + txtUser.Text + " IP:" + Request.ServerVariables["REMOTE_ADDR"] + " Browser:" + Request.ServerVariables["HTTP_USER_AGENT"]);
                }
                Session["userInfo"] = null;
                FormsAuthentication.RedirectToLoginPage();
            }

        }
        catch (Exception ex)
        {
            switch (ex.Message)
            {
                case "brak id":
                    wyjatek += "Brak ID_KTO! Błąd skróconego logowania. Skontaktuj się z administatorem!";
                    break;
                case "brak osoby":
                    wyjatek += "Osoba niezarejestrowana w systemie DSSP - SAS. Skontaktuj się z administatorem!";
                    break;
                case "nieprawidłowe dane logowania":
                    wyjatek += "Wpisałeś zły LOGIN lub HASŁO! \\r\\nPopraw dane i ponownie się zaloguj!";
                    break;
                default:
                    wyjatek += "Błąd!\\r\\n\r\n" + ex.Message.ToUpper();
                    break;
            }
            Session["userInfo"] = null;
        }
    }

    protected void btn_OK_Click(object sender, EventArgs e)
    {
        if( ((Button)sender).ID == "bt_no")
        {
            div_okno.Visible = false;
            box.Visible = false;
            txtPassword.Text = "";
            txtUser.Text = "";
            Session["userInfo"] = null;
        }
        else if (((Button)sender).ID == "bt_yes")
        {
           div_okno.Visible = false;
           box.Visible = false;
           if (Session["userInfo"] != null)
           {
               System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user_logout = Session["userInfo"] as System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string>;
               System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user2 = sub_bag.SessjaUpdate(user_logout[sub_bag.jakchcesz.id_kto], user_logout);
               Session["userInfo"] = user2;
               FormsAuthentication.RedirectFromLoginPage(user2[sub_bag.jakchcesz.user], true);
           }
           else
           {
               System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user_logout = ViewState["userInfoChrom"] as System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string>;
               System.Collections.Generic.Dictionary<sub_bag.jakchcesz, string> user2 = sub_bag.SessjaUpdate(user_logout[sub_bag.jakchcesz.id_kto], user_logout);
               Session["userInfo"] = user2;
               FormsAuthentication.RedirectFromLoginPage(user2[sub_bag.jakchcesz.user], true);
           }
        }
    }
  
}
