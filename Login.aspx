<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" Debug="true"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>AWOS Monitor</title>
    <style type="text/css">
        html, body {
	        height: 100%;
	        background-color: #FFFFFF; 
	        overflow: hidden;
        }
        body {
	          display: table;
                margin: 0 auto;
                width: 639px;
        }
        #srodek 
        {
            position: absolute;
	        top: 10%;
	        display: table-cell;
	        vertical-align: middle;
        }
        p { padding: 0; }
        #Zaloguj
        {
            padding: 3px;
            border: solid 1px #6083b9;
            text-align: center;
            font-size: 10px;
            color: #FFFFFF;
            background-color:  #f39f08;
            font-weight: bold;
            font-family: Verdana;    
            float: right;
        }
        #login
        {	
                  background-color: #f3f3f3;
    background-image: url("img/imgw-min.svg");
    background-position: left center;
    background-repeat: no-repeat;
    border: medium solid;
    color: #6083b9;
    display: table-cell;
    font-family: Tahoma;
    font-size: 26px;
    font-weight: bold;
    height: 305px;
    vertical-align: top;
    width: 633px;
        } 
        input[type="text"], input[type="password"]
        {
            border: solid 1px #6083b9;
            height: 30px;
            width:300px;
            padding-left : 5px;
            font-size: 24px;
            font-family: Verdana;
            font-size: bold;
            
        }
        .txt_login
        {
            margin: 0;
            padding: 0px 0px 0px 2px;
            font-size: 10px;
            font-family: Verdana;
            font-size: bold;
            text-align:left;
        }
        #tresc
        {
    font-size: 10px;
    margin-bottom: 10px;
    padding: 55px 20px 20px 305px;
        }
        #stopka {
             clear: both;
             text-align: right;
             padding: 30px 50px 15px 491px;
             vertical-align: bottom;
             font-size: 10px;
             font-family: Tahoma;
        }
        #sas
        {
            margin-top: 5px;
            margin-right: 25px;
             text-align: right;
             vertical-align: bottom;
             font-size: 14px;
             font-family: Tahoma;
             color: white;
             font-weight: bold;
            }
        #admin
        {
            margin-top: 5px;
            margin-right: 5px;
             text-align: center;
             vertical-align: bottom;
             font-size: 14px;
             font-family: Tahoma;
             color: white;
             font-weight: bold;
            }
         a 
         {
              color: White;
               font-weight: bold;
             }
             a:hover
             {
                  color: red;
               font-weight: bold;
                 }
                 
         #box
         {
    margin: 0 auto;
    width: 50%;
    z-index: 20;
    background-color: #ffffff;
    border: 2px solid #6083b9;;     
    vertical-align: bottom;
     text-align: center;
   
    }

#box .div
{
    width:100%;
    text-align:center;
    color:#ffffff;
        font-size: 18px;
    font-weight: bold;
    background-color:#6083b9;;  
       
         padding: 5px 0;
}



#box .btn
{
    
    background-color: #6083b9;;
    border: medium none;
    color: #ffffff;
    font-size: 18px;
    font-weight: bold;
    padding: 5px 10px 5px 10px;
    text-align: center;
    margin: 0 auto 25px;
}

#box .btn:hover
{
    background-color: #000000;
    border: medium none;
    background-color: #f2f2f2;
    border: medium none;
    color: #000000;
    font-size: 18px;
    font-weight: bold;
    padding: 5px 10px 5px 10px;
    text-align: center;
    cursor:pointer;
    
}
    
             
             
    </style>
    <script type="text/javascript">
        if (areCookiesEnabled() == false) { 
            alert("Nie ma możliwości zapisać plików cookies (ciasteczek) na twoim komputerze!\r\nZmień ustawienia stwoje przeglądarki\r\nStrona nie będzie działać!")
        }


        function createCookie(name, value, days) {
            var expires;
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            }
            else expires = "";
            document.cookie = name + "=" + value + expires + "; path=/";
        }

        function readCookie(name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        }

        function eraseCookie(name) {
            createCookie(name, "", -1);
        }

        function areCookiesEnabled() {
            var r = false;
            createCookie("testing", "Hello", 1);
            if (readCookie("testing") != null) {
                r = true;
                eraseCookie("testing");
            }
            return r;
        }

//        //test przegladarki i rozdzielczości
//        if(screen.height >= 768 && screen.width >= 1024)
//        {
//            var useragent = navigator.userAgent;
//            var bName = (useragent.indexOf('Opera') > -1) ? 'Opera' : navigator.appName;
//            var pos = useragent.indexOf('MSIE');
//            if (pos > -1) {
//                bVer = useragent.substring(pos + 5);
//                var pos = bVer.indexOf(';');
//                var bVer = bVer.substring(0,pos);
//            }
//            var pos = useragent.indexOf('Opera');
//            if (pos > -1)	{
//                bVer = useragent.substring(pos + 6);
//                var pos = bVer.indexOf(' ');
//                var bVer = bVer.substring(0, pos);
//            }
//            if (bName == "Netscape") {
//                var bVer = useragent.substring(8);
//                var pos = bVer.indexOf(' ');
//                var bVer = bVer.substring(0, pos);
//            }
//            if (bName == "Netscape" && parseInt(navigator.appVersion) >= 5) {
//                var pos = useragent.lastIndexOf('/');
//                var bVer = useragent.substring(pos + 1);
//            }
//            var test_poprawny = 0;
//            var test_string = bVer.split(".");
//            switch(bName) //sprawdza czy jest poprawna wersja
//            {
//                case "Netscape":
//                if(test_string[0] > 3 || (test_string[0] = 3&& test_string[1] >= 5))
//                {   test_poprawny = 1;  }
//                break;
//                case "Opera":
//                if(test_string[0] >= 9 && test_string[1] >= 8)
//                {   test_poprawny = 1; }
//                break;
//                case "Microsoft Internet Explorer":
//                if(test_string[0] >= 7 && test_string[1] >= 0)
//                {   test_poprawny = 1; }
//                break;
//            }    
//            if(test_poprawny == 0)
//            {
//                alert("Wersja twojej przeglądarki '"+bName+"' ("+bVer+") \r\nnie spełnia wymogów tej strony!! \r\nProszę zainstalować najnowszą wersję przeglądarki. \r\nStrona może błędnie działać lub nieprawidłowo wyglądać! ");
//            }
//        }
//        else
//        {
//            alert("Rozdzialczość twojego monitora jest zbyt mała by poprawnie wyświetlić tę aplikacje!!");
//        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="srodek">	
	        <div id="login">
	            <div id="tresc">
                <p>
                    <span style="display: block;font-size: 26px;margin: 0 auto;padding: 0 50px 20px;">AWOS Monitor</span>
                    <span class="txt_login">Login:</span>
                    <br />
                    <asp:TextBox  ID="txtUser"  runat="server" ></asp:TextBox>
                    <span style="display: block; font-size: 10px;">&nbsp;</span>
                    <span class="txt_login">Hasło:</span>
                    <br />
                    <asp:TextBox ID="txtPassword"  runat="server" TextMode="Password"></asp:TextBox>
                    <span style="display: block; font-size: 10px;">&nbsp;</span>
                    <asp:Button ID="Zaloguj" runat="server" Text="ZALOGUJ"  onclick="LoginUser" PostBackUrl="~/login.aspx"   />
                </p>
                </div>
            </div> 
               <asp:Panel ID="div_okno" runat="server" >
                   <div style="height: 300px;position: absolute;top: 70px;width: 100%;z-index: 200;">
             
                        <asp:Panel ID="box" runat="server" style="">                         
                                <br />
                            <span style="color:Red; font-weight:bold;">

                                <asp:Label ID="lb_box" runat="server" Text="Ostatnie logowanie: "></asp:Label>                              
                            </span>
                            <br /><br />
                            <asp:Button ID="bt_yes" runat="server" CssClass="btn" onclick="btn_OK_Click"  Text="ZALOGUJ" />
                        </asp:Panel>
                   </div>
               </asp:Panel>


          
        </div>
    </form>
</body>
</html>
