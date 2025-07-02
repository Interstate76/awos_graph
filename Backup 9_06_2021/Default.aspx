<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AWOS_Default" Debug="True" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<head runat="server">
    <title>IMGW AWOS Monitor</title>
    <script type="text/javascript">
        setInterval(function () {
            var x = document.getElementById("div_work").style.backgroundColor;
            var hf = document.getElementById("hf_work").value;
            if (hf == "false") {
                if (x != "rgb(0, 102, 204)") {
                    document.getElementById("div_work").style.backgroundColor = "#0066cc";
                }
                else {
                    document.getElementById("div_work").style.backgroundColor = "#ff0000";
                }
            }
            else {
                if (x != "rgb(0, 102, 204)") {
                    document.getElementById("div_work").style.backgroundColor = "#0066cc";
                }
                else {
                    document.getElementById("div_work").style.backgroundColor = "#009933";
                }
            }
        }, 2000);
    </script>
    <script src="Scripts/jquery-1.7.1.min.js"></script>
   <%-- <script src="Scripts/Highcharts-4.0.1/js/highcharts.js"></script>--%>

  <%--  <script type="text/javascript">
        $(function () {
            if (document.getElementById("wind_rose_6A").innerText > 27) {
                blinkeffect('#wind_rose_6A');
            }
            if (document.getElementById("wind_rose_7A").innerText > 27) {
                blinkeffect('#wind_rose_7A');
            }
            if (document.getElementById("wind_rose_6B").innerText > 27) {
                blinkeffect('#wind_rose_6B');
            }
            if (document.getElementById("wind_rose_7B").innerText > 27) {
                blinkeffect('#wind_rose_7B');
            }
            if (document.getElementById("wind_rose_6C").innerText > 27) {
                blinkeffect('#wind_rose_6C');
            }
            if (document.getElementById("wind_rose_7C").innerText > 27) {
                blinkeffect('#wind_rose_7C');
            }

            if ((document.getElementById("temp_all").innerText >= 0 && document.getElementById("temp_all").innerText <= 2) || document.getElementById("temp_all").innerText >= 28) {
                blinkeffect('#temp_all');
            }

            if (document.getElementById("rh_all").innerText > 99) {
                blinkeffect('#rh_all');
            }

            if (document.getElementById("cloud_1A").innerText <= 300) {
                blinkeffect('#cloud_1A');
            }

            if (document.getElementById("cloud_1").innerText <= 300) {
                blinkeffect('#cloud_1');
            }

            if (document.getElementById("cloud_1C").innerText <= 300) {
                blinkeffect('#cloud_1C');
            }

            if (document.getElementById("vis_dom").innerText <= 1500) {
                blinkeffect('#vis_dom');
            }

            blinkeffect('#infoLabelTemp_tdz');
            blinkeffect('#infoLabelWind_tdz');
            blinkeffect('#infoLabelWind_mid');
            blinkeffect('#infoLabelWind_end');
            blinkeffect('#infoLabelHumidity_tdz');
            blinkeffect('#infoLabelCloud_tdz');
            blinkeffect('#infoLabelCloud_mid');
            blinkeffect('#infoLabelCloud_end');
            blinkeffect('#infoLabelVisDom_mid');
        })
        function blinkeffect(selector) {
            $(selector).fadeOut('slow', function () {
                $(this).fadeIn('slow', function () {
                    blinkeffect(this);
                });
            });
        }
    </script>--%>


</head>

<body style="padding-top: 30px;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
        <asp:Timer runat="server" ID="Tik">
        </asp:Timer>


        <div class="wrap ">
            <div class="subMenu smint fxd" style="position: fixed; top: 0px; left: 0px;">
                <div>
                    <div style="display: inline-block; left: 0; padding-left: 10px; position: absolute;">
                        <ol id="menu" style="display: inline-block;">
                            <li><a class="subNavBtnMin fl">
                                <img src="img/menu_nale.gif" /></a>
                                <ul>
                                    <li><a id="lb_zmian_lot" runat="server" href="default.aspx">zmień lotnisko</a></li>
                                    <li>
                                        <asp:LinkButton ID="lb_dane" runat="server" OnClick="lb_dane_Click">podglad danych</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lb_odswiez" runat="server" OnClick="lb_odswiez_Click">odśwież</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lb_odswiezOff" runat="server" OnClick="lb_odswiez_Click">wyłącz odświeżanie</asp:LinkButton></li>
                                    <li>
                                    <li>
                                        <asp:LinkButton ID="lb_zakresDanych" runat="server" OnClick="lb_zakres_Click">Zakres danych 6h</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lb_noc" runat="server" OnClick="lb_noc_Click">tryb nocny</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lb_pass" runat="server" OnClick="lb_pass_Click">zmiana hasła</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lb_logout" runat="server" OnClick="lb_logout_Click">W Y L O G U J</asp:LinkButton></li>
                                </ul>
                            </li>
                        </ol>
                        <span id="x" class="subNav" style="display: inline-block; top: 0; left: 60px; min-width: 400px; text-align: left; position: absolute;">
                            <asp:Label ID="lb_user" runat="server" Text="XXXXXXX XXXXXXXXX"></asp:Label>
                        </span>
                    </div>
                    <div id="div_data">
                        <div id="div_work" title="Znacznik pracy systemu!&#xA;Czerwony punkt: brak danych.&#xA;Zielony punkt: poprawna praca systemu." style="background: none repeat scroll 0 0 #ff0000; display: inline-block; height: 10px; margin: 0; top: 10px; width: 10px;"></div>
                        <span id="data" class="subNav">Dane UTC:
                        <asp:Label ID="lb_data" runat="server" Text="9999-99-99 00:00:00"></asp:Label>
                        </span>
                        <asp:HiddenField ID="hf_work" runat="server" Value="false" />
                    </div>
                    <div style="position: absolute; top: 0; right: 0; padding-right: 10px;">
                        <span id="lot" class="subNav">
                            <asp:Label ID="lb_lotnisko_nazwa" runat="server" Text="XXXX (XXX / XXXX)"></asp:Label>
                        </span>

                    </div>
                </div>
            </div>

            <div class="section s1">
                <div class="inner">
                    <fieldset class="glowne" runat="server" id="tdz">
                        <legend class="lglowne">TDZ&nbsp;<asp:Label ID="lb_prog_1" runat="server" Text="00"></asp:Label>
                        </legend>
                        <asp:Label ID="infoLabelTemp_tdz" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <asp:Label ID="infoLabelWind_tdz" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <asp:Label ID="infoLabelHumidity_tdz" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <asp:Label ID="infoLabelCloud_tdz" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <fieldset class="fset">
                            <legend class="lset">WIATR</legend>
                            <div class="div_roza">
                                <div class="wind" style="top: 15px; left: 70px; background-color: transparent;">
                                    <asp:Label ID="wind_rose_3A" runat="server" Text="x" Style="font-size: 40px;"></asp:Label>
                                    <p>&deg;</p>
                                </div>
                                <div runat="server" id="pas1">
                                    <img src="img/pas.png" />
                                </div>
                                <div class="wind" style="top: 150px; left: 65px; background-color: transparent;">
                                    <asp:Label ID="wind_rose_1A" runat="server" Text="x" Style="font-size: 40px;"></asp:Label>
                                    <p>kt</p>
                                </div>
                                <div runat='server' title='2m kier sr' id='av1' style='background-color: transparent;'></div>
                                <div runat='server' title='0 stp' id='stp01' style='transform: rotate(0deg); -webkit-transform: rotate(0deg); -moz-transform: rotate(0deg); -o-transform: rotate(0deg); position: absolute; top: 0px; left: 100px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='10 stp' id='stp101' style='transform: rotate(10deg); -webkit-transform: rotate(10deg); -moz-transform: rotate(10deg); -o-transform: rotate(10deg); position: absolute; top: 2px; left: 117px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='20 stp' id='stp201' style='transform: rotate(20deg); -webkit-transform: rotate(20deg); -moz-transform: rotate(20deg); -o-transform: rotate(20deg); position: absolute; top: 6px; left: 134px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='30 stp' id='stp301' style='transform: rotate(30deg); -webkit-transform: rotate(30deg); -moz-transform: rotate(30deg); -o-transform: rotate(30deg); position: absolute; top: 13px; left: 150px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='40 stp' id='stp401' style='transform: rotate(40deg); -webkit-transform: rotate(40deg); -moz-transform: rotate(40deg); -o-transform: rotate(40deg); position: absolute; top: 23px; left: 164px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='50 stp' id='stp501' style='transform: rotate(50deg); -webkit-transform: rotate(50deg); -moz-transform: rotate(50deg); -o-transform: rotate(50deg); position: absolute; top: 36px; left: 177px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='60 stp' id='stp601' style='transform: rotate(60deg); -webkit-transform: rotate(60deg); -moz-transform: rotate(60deg); -o-transform: rotate(60deg); position: absolute; top: 50px; left: 187px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='70 stp' id='stp701' style='transform: rotate(70deg); -webkit-transform: rotate(70deg); -moz-transform: rotate(70deg); -o-transform: rotate(70deg); position: absolute; top: 66px; left: 194px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='80 stp' id='stp801' style='transform: rotate(80deg); -webkit-transform: rotate(80deg); -moz-transform: rotate(80deg); -o-transform: rotate(80deg); position: absolute; top: 83px; left: 198px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='90 stp' id='stp901' style='transform: rotate(90deg); -webkit-transform: rotate(90deg); -moz-transform: rotate(90deg); -o-transform: rotate(90deg); position: absolute; top: 100px; left: 200px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='100 stp' id='stp1001' style='transform: rotate(100deg); -webkit-transform: rotate(100deg); -moz-transform: rotate(100deg); -o-transform: rotate(100deg); position: absolute; top: 117px; left: 198px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='110 stp' id='stp1101' style='transform: rotate(110deg); -webkit-transform: rotate(110deg); -moz-transform: rotate(110deg); -o-transform: rotate(110deg); position: absolute; top: 134px; left: 194px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='120 stp' id='stp1201' style='transform: rotate(120deg); -webkit-transform: rotate(120deg); -moz-transform: rotate(120deg); -o-transform: rotate(120deg); position: absolute; top: 150px; left: 187px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='130 stp' id='stp1301' style='transform: rotate(130deg); -webkit-transform: rotate(130deg); -moz-transform: rotate(130deg); -o-transform: rotate(130deg); position: absolute; top: 164px; left: 177px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='140 stp' id='stp1401' style='transform: rotate(140deg); -webkit-transform: rotate(140deg); -moz-transform: rotate(140deg); -o-transform: rotate(140deg); position: absolute; top: 177px; left: 164px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='150 stp' id='stp1501' style='transform: rotate(150deg); -webkit-transform: rotate(150deg); -moz-transform: rotate(150deg); -o-transform: rotate(150deg); position: absolute; top: 187px; left: 150px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='160 stp' id='stp1601' style='transform: rotate(160deg); -webkit-transform: rotate(160deg); -moz-transform: rotate(160deg); -o-transform: rotate(160deg); position: absolute; top: 194px; left: 134px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='170 stp' id='stp1701' style='transform: rotate(170deg); -webkit-transform: rotate(170deg); -moz-transform: rotate(170deg); -o-transform: rotate(170deg); position: absolute; top: 198px; left: 117px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='180 stp' id='stp1801' style='transform: rotate(180deg); -webkit-transform: rotate(180deg); -moz-transform: rotate(180deg); -o-transform: rotate(180deg); position: absolute; top: 200px; left: 100px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='190 stp' id='stp1901' style='transform: rotate(190deg); -webkit-transform: rotate(190deg); -moz-transform: rotate(190deg); -o-transform: rotate(190deg); position: absolute; top: 198px; left: 83px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='200 stp' id='stp2001' style='transform: rotate(200deg); -webkit-transform: rotate(200deg); -moz-transform: rotate(200deg); -o-transform: rotate(200deg); position: absolute; top: 194px; left: 66px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='210 stp' id='stp2101' style='transform: rotate(210deg); -webkit-transform: rotate(210deg); -moz-transform: rotate(210deg); -o-transform: rotate(210deg); position: absolute; top: 187px; left: 50px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='220 stp' id='stp2201' style='transform: rotate(220deg); -webkit-transform: rotate(220deg); -moz-transform: rotate(220deg); -o-transform: rotate(220deg); position: absolute; top: 177px; left: 36px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='230 stp' id='stp2301' style='transform: rotate(230deg); -webkit-transform: rotate(230deg); -moz-transform: rotate(230deg); -o-transform: rotate(230deg); position: absolute; top: 164px; left: 23px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='240 stp' id='stp2401' style='transform: rotate(240deg); -webkit-transform: rotate(240deg); -moz-transform: rotate(240deg); -o-transform: rotate(240deg); position: absolute; top: 150px; left: 13px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='250 stp' id='stp2501' style='transform: rotate(250deg); -webkit-transform: rotate(250deg); -moz-transform: rotate(250deg); -o-transform: rotate(250deg); position: absolute; top: 134px; left: 6px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='260 stp' id='stp2601' style='transform: rotate(260deg); -webkit-transform: rotate(260deg); -moz-transform: rotate(260deg); -o-transform: rotate(260deg); position: absolute; top: 117px; left: 2px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='270 stp' id='stp2701' style='transform: rotate(270deg); -webkit-transform: rotate(270deg); -moz-transform: rotate(270deg); -o-transform: rotate(270deg); position: absolute; top: 100px; left: 0px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='280 stp' id='stp2801' style='transform: rotate(280deg); -webkit-transform: rotate(280deg); -moz-transform: rotate(280deg); -o-transform: rotate(280deg); position: absolute; top: 83px; left: 2px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='290 stp' id='stp2901' style='transform: rotate(290deg); -webkit-transform: rotate(290deg); -moz-transform: rotate(290deg); -o-transform: rotate(290deg); position: absolute; top: 66px; left: 6px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='300 stp' id='stp3001' style='transform: rotate(300deg); -webkit-transform: rotate(300deg); -moz-transform: rotate(300deg); -o-transform: rotate(300deg); position: absolute; top: 50px; left: 13px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='310 stp' id='stp3101' style='transform: rotate(310deg); -webkit-transform: rotate(310deg); -moz-transform: rotate(310deg); -o-transform: rotate(310deg); position: absolute; top: 36px; left: 23px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='320 stp' id='stp3201' style='transform: rotate(320deg); -webkit-transform: rotate(320deg); -moz-transform: rotate(320deg); -o-transform: rotate(320deg); position: absolute; top: 23px; left: 36px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='330 stp' id='stp3301' style='transform: rotate(330deg); -webkit-transform: rotate(330deg); -moz-transform: rotate(330deg); -o-transform: rotate(330deg); position: absolute; top: 13px; left: 50px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='340 stp' id='stp3401' style='transform: rotate(340deg); -webkit-transform: rotate(340deg); -moz-transform: rotate(340deg); -o-transform: rotate(340deg); position: absolute; top: 6px; left: 66px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='350 stp' id='stp3501' style='transform: rotate(350deg); -webkit-transform: rotate(350deg); -moz-transform: rotate(350deg); -o-transform: rotate(350deg); position: absolute; top: 2px; left: 83px; width: 10px; height: 10px;'></div>
                            </div>
                            <div class="val">
                                <p>Wiatr 10min śr.</p>
                                <asp:Label ID="wind_rose_5A" runat="server" Text="x"></asp:Label>
                                <p>&deg;</p>
                                <asp:Label ID="wind_rose_6A" runat="server" Text="x"></asp:Label>
                                <p>kt</p>
                            </div>
                            <br />
                            <div class="val">
                                <%-- <p>Maks. prędkość wiatru</p>--%>
                                <p>Poryw 10 min</p>
                                <asp:Label ID="wind_rose_7A" runat="server" Text="x"></asp:Label>
                                <p>kt</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset fset_2">
                            <legend class="lset">RVR</legend>
                            <div class="val">
                                <p>
                                    RVR<br />
                                    1 Min.
                                </p>
                                <asp:Label ID="rvr_1A" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset fset_2">
                            <legend class="lset">Widzialność</legend>
                            <div class="val">
                                <p>
                                    Widzialność<br />
                                    Śr z Min.
                                </p>
                                <asp:Label ID="rvr_2A" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset">
                            <legend class="lset">ZACHMURZENIE dla progu</legend>
                            <div class="val">
                                <p>Warstwa 3</p>
                                <asp:Label ID="cloud_okt_3A" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_3A" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <br />
                            <br />
                            <div class="val">
                                <p>Warstwa 2</p>
                                <asp:Label ID="cloud_okt_2A" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_2A" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <br />

                            <br />
                            <div class="val">
                                <p>Warstwa 1</p>
                                <asp:Label ID="cloud_okt_1A" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_1A" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <div class="val">
                                <p>VV</p>
                                <asp:Label ID="vertvis_A" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>

                            <br />
                        </fieldset>
                        <fieldset class="all">
                            <legend class="lset">TEMP/WILG dla lotniska</legend>
                            <div class="val">
                                <p>T/DP</p>
                                <asp:Label ID="temp_all" runat="server" Text="x"></asp:Label>
                                <p>&deg;C</p>
                                <asp:Label ID="dew_point_all" runat="server" Text="x"></asp:Label>
                                <p>&deg;C</p>
                            </div>
                            <div class="val">
                                <p>Wilg.</p>
                                <asp:Label ID="rh_all" runat="server" Text="x"></asp:Label>
                                <p>%</p>
                            </div>
                        </fieldset>

                        <div style="float: left;">
                            <asp:Button ID="WindButtonTdz" CssClass="btn1" Text="WIND OFF" OnClick="WindBtnTdz_Click" runat="server" />
                        </div>
                        <div style="float: left;">
                            <asp:Button ID="VisButtonTdz" CssClass="btn1" Text="VIS OFF" OnClick="VisBtnTdz_Click" runat="server" />
                        </div>
                        <div style="float: left;">
                            <asp:Button ID="CloudButtonTdz" CssClass="btn1" Text="CLOUD OFF" OnClick="CloudBtnTdz_Click" runat="server" />
                        </div>
                        <div style="float: right;">
                            <asp:Button ID="SaveButtonTdz" CssClass="btn1" Text="SAVE" OnClick="SaveBtnTdz_Click" runat="server" />
                        </div>
                        <div style="float: right;">
                            <asp:Button ID="ClearButtonTdz" CssClass="btn1" Text="CLEAR" OnClick="ClearBtnTdz_Click" runat="server" />
                        </div>
                        <br />
                        <asp:Chart ID="RoseWind_tdz" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartWind_tdz" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartPoryw_tdz" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud3_tdz" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud2_tdz" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud1_tdz" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartTDZ_rvr_2A" runat="server" Visible="false"></asp:Chart>
                    </fieldset>
                    <fieldset class="glowne" id="mid" runat="server">
                        <legend class="lglowne">
                            <asp:Label ID="lb_prog_mid" runat="server" Text="MID" /></legend>
                        <asp:Label ID="infoLabelWind_mid" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <asp:Label ID="infoLabelCloud_mid" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <asp:Label ID="infoLabelVisDom_mid" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <fieldset class="fset">
                            <legend class="lset">WIATR</legend>
                            <div class="div_roza">
                                <div class="wind" style="top: 15px; left: 70px; background-color: transparent;">
                                    <asp:Label ID="wind_rose_3B" runat="server" Text="x" Style="font-size: 40px;"></asp:Label>
                                    <p>&deg;</p>
                                </div>
                                <div runat="server" id="pas2">
                                    <img src="img/pas.png" />
                                </div>
                                <div class="wind" style="top: 150px; left: 65px; background-color: transparent;">
                                    <asp:Label ID="wind_rose_1B" runat="server" Text="x" Style="font-size: 40px;"></asp:Label>
                                    <p>kt</p>
                                </div>
                                <div runat='server' title='2m kier sr' id='av2' style='background-color: transparent;'></div>
                                <div runat='server' title='0 stp' id='stp02' style='transform: rotate(0deg); -webkit-transform: rotate(0deg); -moz-transform: rotate(0deg); -o-transform: rotate(0deg); position: absolute; top: 0px; left: 100px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='10 stp' id='stp102' style='transform: rotate(10deg); -webkit-transform: rotate(10deg); -moz-transform: rotate(10deg); -o-transform: rotate(10deg); position: absolute; top: 2px; left: 117px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='20 stp' id='stp202' style='transform: rotate(20deg); -webkit-transform: rotate(20deg); -moz-transform: rotate(20deg); -o-transform: rotate(20deg); position: absolute; top: 6px; left: 134px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='30 stp' id='stp302' style='transform: rotate(30deg); -webkit-transform: rotate(30deg); -moz-transform: rotate(30deg); -o-transform: rotate(30deg); position: absolute; top: 13px; left: 150px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='40 stp' id='stp402' style='transform: rotate(40deg); -webkit-transform: rotate(40deg); -moz-transform: rotate(40deg); -o-transform: rotate(40deg); position: absolute; top: 23px; left: 164px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='50 stp' id='stp502' style='transform: rotate(50deg); -webkit-transform: rotate(50deg); -moz-transform: rotate(50deg); -o-transform: rotate(50deg); position: absolute; top: 36px; left: 177px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='60 stp' id='stp602' style='transform: rotate(60deg); -webkit-transform: rotate(60deg); -moz-transform: rotate(60deg); -o-transform: rotate(60deg); position: absolute; top: 50px; left: 187px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='70 stp' id='stp702' style='transform: rotate(70deg); -webkit-transform: rotate(70deg); -moz-transform: rotate(70deg); -o-transform: rotate(70deg); position: absolute; top: 66px; left: 194px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='80 stp' id='stp802' style='transform: rotate(80deg); -webkit-transform: rotate(80deg); -moz-transform: rotate(80deg); -o-transform: rotate(80deg); position: absolute; top: 83px; left: 198px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='90 stp' id='stp902' style='transform: rotate(90deg); -webkit-transform: rotate(90deg); -moz-transform: rotate(90deg); -o-transform: rotate(90deg); position: absolute; top: 100px; left: 200px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='100 stp' id='stp1002' style='transform: rotate(100deg); -webkit-transform: rotate(100deg); -moz-transform: rotate(100deg); -o-transform: rotate(100deg); position: absolute; top: 117px; left: 198px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='110 stp' id='stp1102' style='transform: rotate(110deg); -webkit-transform: rotate(110deg); -moz-transform: rotate(110deg); -o-transform: rotate(110deg); position: absolute; top: 134px; left: 194px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='120 stp' id='stp1202' style='transform: rotate(120deg); -webkit-transform: rotate(120deg); -moz-transform: rotate(120deg); -o-transform: rotate(120deg); position: absolute; top: 150px; left: 187px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='130 stp' id='stp1302' style='transform: rotate(130deg); -webkit-transform: rotate(130deg); -moz-transform: rotate(130deg); -o-transform: rotate(130deg); position: absolute; top: 164px; left: 177px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='140 stp' id='stp1402' style='transform: rotate(140deg); -webkit-transform: rotate(140deg); -moz-transform: rotate(140deg); -o-transform: rotate(140deg); position: absolute; top: 177px; left: 164px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='150 stp' id='stp1502' style='transform: rotate(150deg); -webkit-transform: rotate(150deg); -moz-transform: rotate(150deg); -o-transform: rotate(150deg); position: absolute; top: 187px; left: 150px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='160 stp' id='stp1602' style='transform: rotate(160deg); -webkit-transform: rotate(160deg); -moz-transform: rotate(160deg); -o-transform: rotate(160deg); position: absolute; top: 194px; left: 134px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='170 stp' id='stp1702' style='transform: rotate(170deg); -webkit-transform: rotate(170deg); -moz-transform: rotate(170deg); -o-transform: rotate(170deg); position: absolute; top: 198px; left: 117px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='180 stp' id='stp1802' style='transform: rotate(180deg); -webkit-transform: rotate(180deg); -moz-transform: rotate(180deg); -o-transform: rotate(180deg); position: absolute; top: 200px; left: 100px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='190 stp' id='stp1902' style='transform: rotate(190deg); -webkit-transform: rotate(190deg); -moz-transform: rotate(190deg); -o-transform: rotate(190deg); position: absolute; top: 198px; left: 83px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='200 stp' id='stp2002' style='transform: rotate(200deg); -webkit-transform: rotate(200deg); -moz-transform: rotate(200deg); -o-transform: rotate(200deg); position: absolute; top: 194px; left: 66px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='210 stp' id='stp2102' style='transform: rotate(210deg); -webkit-transform: rotate(210deg); -moz-transform: rotate(210deg); -o-transform: rotate(210deg); position: absolute; top: 187px; left: 50px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='220 stp' id='stp2202' style='transform: rotate(220deg); -webkit-transform: rotate(220deg); -moz-transform: rotate(220deg); -o-transform: rotate(220deg); position: absolute; top: 177px; left: 36px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='230 stp' id='stp2302' style='transform: rotate(230deg); -webkit-transform: rotate(230deg); -moz-transform: rotate(230deg); -o-transform: rotate(230deg); position: absolute; top: 164px; left: 23px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='240 stp' id='stp2402' style='transform: rotate(240deg); -webkit-transform: rotate(240deg); -moz-transform: rotate(240deg); -o-transform: rotate(240deg); position: absolute; top: 150px; left: 13px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='250 stp' id='stp2502' style='transform: rotate(250deg); -webkit-transform: rotate(250deg); -moz-transform: rotate(250deg); -o-transform: rotate(250deg); position: absolute; top: 134px; left: 6px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='260 stp' id='stp2602' style='transform: rotate(260deg); -webkit-transform: rotate(260deg); -moz-transform: rotate(260deg); -o-transform: rotate(260deg); position: absolute; top: 117px; left: 2px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='270 stp' id='stp2702' style='transform: rotate(270deg); -webkit-transform: rotate(270deg); -moz-transform: rotate(270deg); -o-transform: rotate(270deg); position: absolute; top: 100px; left: 0px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='280 stp' id='stp2802' style='transform: rotate(280deg); -webkit-transform: rotate(280deg); -moz-transform: rotate(280deg); -o-transform: rotate(280deg); position: absolute; top: 83px; left: 2px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='290 stp' id='stp2902' style='transform: rotate(290deg); -webkit-transform: rotate(290deg); -moz-transform: rotate(290deg); -o-transform: rotate(290deg); position: absolute; top: 66px; left: 6px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='300 stp' id='stp3002' style='transform: rotate(300deg); -webkit-transform: rotate(300deg); -moz-transform: rotate(300deg); -o-transform: rotate(300deg); position: absolute; top: 50px; left: 13px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='310 stp' id='stp3102' style='transform: rotate(310deg); -webkit-transform: rotate(310deg); -moz-transform: rotate(310deg); -o-transform: rotate(310deg); position: absolute; top: 36px; left: 23px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='320 stp' id='stp3202' style='transform: rotate(320deg); -webkit-transform: rotate(320deg); -moz-transform: rotate(320deg); -o-transform: rotate(320deg); position: absolute; top: 23px; left: 36px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='330 stp' id='stp3302' style='transform: rotate(330deg); -webkit-transform: rotate(330deg); -moz-transform: rotate(330deg); -o-transform: rotate(330deg); position: absolute; top: 13px; left: 50px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='340 stp' id='stp3402' style='transform: rotate(340deg); -webkit-transform: rotate(340deg); -moz-transform: rotate(340deg); -o-transform: rotate(340deg); position: absolute; top: 6px; left: 66px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='350 stp' id='stp3502' style='transform: rotate(350deg); -webkit-transform: rotate(350deg); -moz-transform: rotate(350deg); -o-transform: rotate(350deg); position: absolute; top: 2px; left: 83px; width: 10px; height: 10px;'></div>
                            </div>
                            <div class="val">
                                <p>Wiatr 10min śr.</p>
                                <asp:Label ID="wind_rose_5B" runat="server" Text="x"></asp:Label>
                                <p>&deg;</p>
                                <asp:Label ID="wind_rose_6B" runat="server" Text="x"></asp:Label>
                                <p>kt</p>
                            </div>
                            <br />
                            <div class="val">
                                <%--   <p>Maks. prędkość wiatru</p>--%>
                                <p>Poryw 10 min</p>
                                <asp:Label ID="wind_rose_7B" runat="server" Text="x"></asp:Label>
                                <p>kt</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset" runat="server" id="fset_rvr">
                            <legend class="lset">RVR</legend>
                            <div class="val">
                                <p>
                                    RVR1<br />
                                    Min.
                                </p>
                                <asp:Label ID="rvr_1B" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                            <div class="val" title="Dane z czujnika automatycznego">
                                <p>
                                    Bieżąca<br />
                                    pogoda
                                </p>
                                <asp:Label ID="weather_B" runat="server" Text="x"></asp:Label>
                            </div>
                        </fieldset>
                        <fieldset class="all" runat="server" id="fset_vis">
                            <legend class="lset">Widzialność dominująca na lotnisku</legend>
                            <div class="val">
                                <p>Widzialność</p>
                                <asp:Label ID="vis_dom" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="all" runat="server" id="fset_vis1">
                            <legend class="lset">Widzialność minimalna na lotnisku</legend>
                            <div class="val">
                                <p>Widzialność</p>
                                <asp:Label ID="vis_min" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="all" runat="server" id="fset_widz">
                            <legend class="lset">Widzialność</legend>
                            <div class="val">
                                <p>Widzialność pozioma</p>
                                <asp:Label ID="widz_maws" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="all" runat="server" id="fset_press">
                            <legend class="lset">Ciśnienie</legend>
                            <div class="val">
                                <p>QNH</p>
                                <asp:Label ID="qnh" runat="server" Text="x"></asp:Label>
                                <p>hPa</p>
                            </div>
                            <div class="val">
                                <p>
                                    Aktualne<br />
                                    ciśnienie
                                </p>
                                <asp:Label ID="press_now" runat="server" Text="x"></asp:Label>
                                <p>hPa</p>
                            </div>
                        </fieldset>
                        <fieldset class="all" style="text-align: left;" runat="server" id="fset_cloud">
                            <legend class="lset">Zachmurzenie nad lotniskiem</legend>
                            <div class="val">
                                <p>Warstwa 3</p>
                                <asp:Label ID="cloud_okt_3" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_3" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <br />
                            <br />
                            <div class="val">
                                <p>Warstwa 2</p>
                                <asp:Label ID="cloud_okt_2" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_2" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <br />
                            <br />
                            <div class="val">
                                <p>Warstwa 1</p>
                                <asp:Label ID="cloud_okt_1" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_1" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <div class="val">
                                <p>VV</p>
                                <asp:Label ID="vertvis" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                        </fieldset>
                        <fieldset class="all" runat="server" id="fset_pa11">
                            <legend class="lset">Ciśnienie</legend>
                            <div class="val">
                                <p>
                                    PA11<br />
                                    Meteo
                                </p>
                                <asp:Label ID="press_pa11" runat="server" Text="x"></asp:Label>
                                <p>hPa</p>
                            </div>
                            <div class="val">
                                <p>
                                    Aktualne<br />
                                    ciśnienie MAWS
                                </p>
                                <asp:Label ID="press_maws" runat="server" Text="x"></asp:Label>
                                <p>hPa</p>
                            </div>
                            <div class="val">
                                <p>
                                    Obliczone<br />
                                    ciśnienie QNH
                                </p>
                                <asp:Label ID="press_qnh" runat="server" Text="x"></asp:Label>
                                <p>hPa</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset" runat="server" id="fset_temp">
                            <legend class="lset">TEMP/WILG</legend>
                            <div class="val">
                                <p>Temp.</p>
                                <asp:Label ID="temp_maws" runat="server" Text="x"></asp:Label>
                                <p>&deg;C</p>
                                <asp:Label ID="dew_point_maws" runat="server" Text="x"></asp:Label>
                                <p>&deg;C</p>
                            </div>
                            <div class="val">
                                <p>Wilg.</p>
                                <asp:Label ID="rh_maws" runat="server" Text="x"></asp:Label>
                                <p>%</p>
                            </div>
                        </fieldset>
                        <fieldset class="all" runat="server" id="fset_opad_maws">
                            <legend class="lset">OPAD</legend>
                            <div class="val">
                                <p>
                                    opad<br />
                                    1min.
                                </p>
                                <asp:Label ID="opad_1m_maws" runat="server" Text="x"></asp:Label>
                                <p>mm</p>
                            </div>
                            <div class="val">
                                <p>
                                    opad<br />
                                    1h
                                </p>
                                <asp:Label ID="opad_1h_maws" runat="server" Text="x"></asp:Label>
                                <p>mm</p>
                            </div>
                            <div class="val">
                                <p>
                                    opad<br />
                                    24h
                                </p>
                                <asp:Label ID="opad_24h_maws" runat="server" Text="x"></asp:Label>
                                <p>mm</p>
                            </div>
                        </fieldset>

                        <div style="float: left;">
                            <asp:Button ID="WindButtonMid" CssClass="btn1" Text="WIND OFF" OnClick="WindBtnMid_Click" runat="server" />
                        </div>
                        <div style="float: left;">
                            <asp:Button ID="VisButtonMid" CssClass="btn1" Text="VIS OFF" OnClick="VisBtnMid_Click" runat="server" />
                        </div>
                        <div style="float: left;">
                            <asp:Button ID="CloudButtonMid" CssClass="btn1" Text="CLOUD OFF" OnClick="CloudBtnMid_Click" runat="server" />
                        </div>
                        <div style="float: right;">
                            <asp:Button ID="SaveButtonMid" CssClass="btn1" Text="SAVE" OnClick="SaveBtnMid_Click" runat="server" />
                        </div>
                        <div style="float: right;">
                            <asp:Button ID="ClearButtonMid" CssClass="btn1" Text="CLEAR" OnClick="ClearBtnMid_Click" runat="server" />
                        </div>

                        <asp:Chart ID="RoseWind_mid" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartWind_Mid" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartPoryw_Mid" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud3_mid" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud2_mid" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud1_mid" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartMID_widzDomMin" runat="server" Visible="false"></asp:Chart>
                    </fieldset>
                    <fieldset class="glowne" runat="server" id="end">
                        <legend class="lglowne">END&nbsp;<asp:Label ID="lb_prog_2" runat="server" Text="00"></asp:Label></legend>
                        <asp:Label ID="infoLabelWind_end" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <asp:Label ID="infoLabelCloud_end" runat="server" Visible="false" CssClass="lalert"></asp:Label>
                        <fieldset class="fset">
                            <legend class="lset">WIATR</legend>
                            <div class="div_roza">
                                <div class="wind" style="top: 15px; left: 70px; background-color: transparent;">
                                    <asp:Label ID="wind_rose_3C" runat="server" Text="x" Style="font-size: 40px;"></asp:Label>
                                    <p>&deg;</p>
                                </div>
                                <div runat="server" id="pas3">
                                    <img src="img/pas.png" />
                                </div>
                                <div class="wind" style="top: 150px; left: 65px; background-color: transparent;">
                                    <asp:Label ID="wind_rose_1C" runat="server" Text="x" Style="font-size: 40px;"></asp:Label>
                                    <p>kt</p>
                                </div>
                                <div runat='server' title='2m kier sr' id='av3' style='background-color: transparent;'></div>
                                <div runat='server' title='0 stp' id='stp03' style='transform: rotate(0deg); -webkit-transform: rotate(0deg); -moz-transform: rotate(0deg); -o-transform: rotate(0deg); position: absolute; top: 0px; left: 100px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='10 stp' id='stp103' style='transform: rotate(10deg); -webkit-transform: rotate(10deg); -moz-transform: rotate(10deg); -o-transform: rotate(10deg); position: absolute; top: 2px; left: 117px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='20 stp' id='stp203' style='transform: rotate(20deg); -webkit-transform: rotate(20deg); -moz-transform: rotate(20deg); -o-transform: rotate(20deg); position: absolute; top: 6px; left: 134px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='30 stp' id='stp303' style='transform: rotate(30deg); -webkit-transform: rotate(30deg); -moz-transform: rotate(30deg); -o-transform: rotate(30deg); position: absolute; top: 13px; left: 150px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='40 stp' id='stp403' style='transform: rotate(40deg); -webkit-transform: rotate(40deg); -moz-transform: rotate(40deg); -o-transform: rotate(40deg); position: absolute; top: 23px; left: 164px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='50 stp' id='stp503' style='transform: rotate(50deg); -webkit-transform: rotate(50deg); -moz-transform: rotate(50deg); -o-transform: rotate(50deg); position: absolute; top: 36px; left: 177px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='60 stp' id='stp603' style='transform: rotate(60deg); -webkit-transform: rotate(60deg); -moz-transform: rotate(60deg); -o-transform: rotate(60deg); position: absolute; top: 50px; left: 187px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='70 stp' id='stp703' style='transform: rotate(70deg); -webkit-transform: rotate(70deg); -moz-transform: rotate(70deg); -o-transform: rotate(70deg); position: absolute; top: 66px; left: 194px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='80 stp' id='stp803' style='transform: rotate(80deg); -webkit-transform: rotate(80deg); -moz-transform: rotate(80deg); -o-transform: rotate(80deg); position: absolute; top: 83px; left: 198px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='90 stp' id='stp903' style='transform: rotate(90deg); -webkit-transform: rotate(90deg); -moz-transform: rotate(90deg); -o-transform: rotate(90deg); position: absolute; top: 100px; left: 200px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='100 stp' id='stp1003' style='transform: rotate(100deg); -webkit-transform: rotate(100deg); -moz-transform: rotate(100deg); -o-transform: rotate(100deg); position: absolute; top: 117px; left: 198px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='110 stp' id='stp1103' style='transform: rotate(110deg); -webkit-transform: rotate(110deg); -moz-transform: rotate(110deg); -o-transform: rotate(110deg); position: absolute; top: 134px; left: 194px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='120 stp' id='stp1203' style='transform: rotate(120deg); -webkit-transform: rotate(120deg); -moz-transform: rotate(120deg); -o-transform: rotate(120deg); position: absolute; top: 150px; left: 187px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='130 stp' id='stp1303' style='transform: rotate(130deg); -webkit-transform: rotate(130deg); -moz-transform: rotate(130deg); -o-transform: rotate(130deg); position: absolute; top: 164px; left: 177px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='140 stp' id='stp1403' style='transform: rotate(140deg); -webkit-transform: rotate(140deg); -moz-transform: rotate(140deg); -o-transform: rotate(140deg); position: absolute; top: 177px; left: 164px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='150 stp' id='stp1503' style='transform: rotate(150deg); -webkit-transform: rotate(150deg); -moz-transform: rotate(150deg); -o-transform: rotate(150deg); position: absolute; top: 187px; left: 150px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='160 stp' id='stp1603' style='transform: rotate(160deg); -webkit-transform: rotate(160deg); -moz-transform: rotate(160deg); -o-transform: rotate(160deg); position: absolute; top: 194px; left: 134px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='170 stp' id='stp1703' style='transform: rotate(170deg); -webkit-transform: rotate(170deg); -moz-transform: rotate(170deg); -o-transform: rotate(170deg); position: absolute; top: 198px; left: 117px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='180 stp' id='stp1803' style='transform: rotate(180deg); -webkit-transform: rotate(180deg); -moz-transform: rotate(180deg); -o-transform: rotate(180deg); position: absolute; top: 200px; left: 100px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='190 stp' id='stp1903' style='transform: rotate(190deg); -webkit-transform: rotate(190deg); -moz-transform: rotate(190deg); -o-transform: rotate(190deg); position: absolute; top: 198px; left: 83px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='200 stp' id='stp2003' style='transform: rotate(200deg); -webkit-transform: rotate(200deg); -moz-transform: rotate(200deg); -o-transform: rotate(200deg); position: absolute; top: 194px; left: 66px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='210 stp' id='stp2103' style='transform: rotate(210deg); -webkit-transform: rotate(210deg); -moz-transform: rotate(210deg); -o-transform: rotate(210deg); position: absolute; top: 187px; left: 50px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='220 stp' id='stp2203' style='transform: rotate(220deg); -webkit-transform: rotate(220deg); -moz-transform: rotate(220deg); -o-transform: rotate(220deg); position: absolute; top: 177px; left: 36px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='230 stp' id='stp2303' style='transform: rotate(230deg); -webkit-transform: rotate(230deg); -moz-transform: rotate(230deg); -o-transform: rotate(230deg); position: absolute; top: 164px; left: 23px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='240 stp' id='stp2403' style='transform: rotate(240deg); -webkit-transform: rotate(240deg); -moz-transform: rotate(240deg); -o-transform: rotate(240deg); position: absolute; top: 150px; left: 13px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='250 stp' id='stp2503' style='transform: rotate(250deg); -webkit-transform: rotate(250deg); -moz-transform: rotate(250deg); -o-transform: rotate(250deg); position: absolute; top: 134px; left: 6px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='260 stp' id='stp2603' style='transform: rotate(260deg); -webkit-transform: rotate(260deg); -moz-transform: rotate(260deg); -o-transform: rotate(260deg); position: absolute; top: 117px; left: 2px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='270 stp' id='stp2703' style='transform: rotate(270deg); -webkit-transform: rotate(270deg); -moz-transform: rotate(270deg); -o-transform: rotate(270deg); position: absolute; top: 100px; left: 0px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='280 stp' id='stp2803' style='transform: rotate(280deg); -webkit-transform: rotate(280deg); -moz-transform: rotate(280deg); -o-transform: rotate(280deg); position: absolute; top: 83px; left: 2px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='290 stp' id='stp2903' style='transform: rotate(290deg); -webkit-transform: rotate(290deg); -moz-transform: rotate(290deg); -o-transform: rotate(290deg); position: absolute; top: 66px; left: 6px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='300 stp' id='stp3003' style='transform: rotate(300deg); -webkit-transform: rotate(300deg); -moz-transform: rotate(300deg); -o-transform: rotate(300deg); position: absolute; top: 50px; left: 13px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='310 stp' id='stp3103' style='transform: rotate(310deg); -webkit-transform: rotate(310deg); -moz-transform: rotate(310deg); -o-transform: rotate(310deg); position: absolute; top: 36px; left: 23px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='320 stp' id='stp3203' style='transform: rotate(320deg); -webkit-transform: rotate(320deg); -moz-transform: rotate(320deg); -o-transform: rotate(320deg); position: absolute; top: 23px; left: 36px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='330 stp' id='stp3303' style='transform: rotate(330deg); -webkit-transform: rotate(330deg); -moz-transform: rotate(330deg); -o-transform: rotate(330deg); position: absolute; top: 13px; left: 50px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='340 stp' id='stp3403' style='transform: rotate(340deg); -webkit-transform: rotate(340deg); -moz-transform: rotate(340deg); -o-transform: rotate(340deg); position: absolute; top: 6px; left: 66px; width: 10px; height: 10px;'></div>
                                <div runat='server' title='350 stp' id='stp3503' style='transform: rotate(350deg); -webkit-transform: rotate(350deg); -moz-transform: rotate(350deg); -o-transform: rotate(350deg); position: absolute; top: 2px; left: 83px; width: 10px; height: 10px;'></div>
                            </div>
                            <div class="val">
                                <p>Wiatr 10min śr.</p>
                                <asp:Label ID="wind_rose_5C" runat="server" Text="x"></asp:Label>
                                <p>&deg;</p>
                                <asp:Label ID="wind_rose_6C" runat="server" Text="x"></asp:Label>
                                <p>kt</p>
                            </div>
                            <br />
                            <div class="val">
                                <%-- <p>Maks. prędkość wiatru</p>--%>
                                <p>Poryw 10 min</p>
                                <asp:Label ID="wind_rose_7C" runat="server" Text="x"></asp:Label>
                                <p>kt</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset fset_2">
                            <legend class="lset">RVR</legend>
                            <div class="val">
                                <p>
                                    RVR<br />
                                    1 Min.
                                </p>
                                <asp:Label ID="rvr_1C" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset fset_2">
                            <legend class="lset">Widzialność</legend>
                            <div class="val">
                                <p>
                                    Widzialność<br />
                                    Śr z Min.
                                </p>
                                <asp:Label ID="rvr_2C" runat="server" Text="x"></asp:Label>
                                <p>m</p>
                            </div>
                        </fieldset>
                        <fieldset class="fset">
                            <legend class="lset">ZACHMURZENIE dla progu</legend>
                            <div class="val">
                                <p>Warstwa 3</p>
                                <asp:Label ID="cloud_okt_3C" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_3C" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <br />
                            <br />
                            <div class="val">
                                <p>Warstwa 2</p>
                                <asp:Label ID="cloud_okt_2C" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_2C" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <br />
                            <br />
                            <div class="val">
                                <p>Warstwa 1</p>
                                <asp:Label ID="cloud_okt_1C" runat="server" Text="x"></asp:Label>
                                <asp:Label ID="cloud_1C" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                            <div class="val">
                                <p>VV</p>
                                <asp:Label ID="vertvis_C" runat="server" Text="x"></asp:Label>
                                <p>ft</p>
                            </div>
                        </fieldset>
                        <fieldset class="all">
                            <legend class="lset">OPAD dla lotniska</legend>
                            <div class="val">
                                <p>
                                    opad<br />
                                    1min.
                                </p>
                                <asp:Label ID="opad_1m" runat="server" Text="x"></asp:Label>
                                <p>mm</p>
                            </div>
                            <div class="val">
                                <p>
                                    opad<br />
                                    24h
                                </p>
                                <asp:Label ID="opad_24h" runat="server" Text="x"></asp:Label>
                                <p>mm</p>
                            </div>
                        </fieldset>
                        <div style="float: left;">
                            <asp:Button ID="WindButtonEnd" CssClass="btn1" Text="WIND OFF" OnClick="WindBtnEnd_Click" runat="server" />
                        </div>
                        <div style="float: left;">
                            <asp:Button ID="VisButtonEnd" CssClass="btn1" Text="VIS OFF" OnClick="VisBtnEnd_Click" runat="server" />
                        </div>
                        <div style="float: left;">
                            <asp:Button ID="CloudButtonEnd" CssClass="btn1" Text="CLOUD OFF" OnClick="CloudBtnEnd_Click" runat="server" />
                        </div>
                        <div style="float: right;">
                            <asp:Button ID="SaveButtonEnd" CssClass="btn1" Text="SAVE" OnClick="SaveBtnEnd_Click" runat="server" />
                        </div>
                        <div style="float: right;">
                            <asp:Button ID="ClearButtonEnd" CssClass="btn1" Text="CLEAR" OnClick="ClearBtnEnd_Click" runat="server" />
                        </div>

                        <asp:Chart ID="RoseWind_end" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartWind_end" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartPoryw_end" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud3_end" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud2_end" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartCloud1_end" runat="server" Visible="false"></asp:Chart>
                        <asp:Chart ID="ChartEND_RvrVis" runat="server" Visible="false"></asp:Chart>
                    </fieldset>
                    <fieldset class="metrep" style="" runat="server" id="metrep">
                        <legend class="lset">MetReport</legend>
                        <asp:Label ID="metreport" runat="server" Text="x"></asp:Label>
                        <br />
                        <%-- <span class="info_metreport">Komunikat generowany automatycznie, bez udziału obserwatora. Zawartości komunikatów nie należy wykorzystywać do pracy operacyjnej.</span> --%>
                    </fieldset>
                    <asp:Panel ID="div_okno" runat="server" Visible="false">
                        <div style="height: 300px; position: absolute; top: 100px; width: 100%; z-index: 200;">
                            <div style="display: inline-block; vertical-align: middle; width: 32%;"></div>
                            <asp:Panel ID="div_wybor" runat="server" Visible="false" Style="">
                                <div class="div">WYBIERZ LOTNISKO</div>
                                <%--                                <asp:DropDownList ID="ddl_wybierz_lotnisko" runat="server"  >
                                </asp:DropDownList>--%>
                                <asp:ListBox ID="ddl_wybierz_lotnisko" runat="server" OnSelectedIndexChanged="btn_lotnisko_Click" AutoPostBack="true"></asp:ListBox>
                                <br />
                                <p style="display: block; font-size: 12px; font-weight: bold; margin: 0;">
                                    <asp:CheckBox ID="cb_wiele_okien" runat="server" Text="otwórz wiele okien" Visible="false" />
                                </p>
                                <br />
                                <asp:Button ID="btn_lotnisko" CssClass="btn" runat="server" Text="Wybierz"
                                    OnClick="btn_lotnisko_Click" />
                                <br />
                            </asp:Panel>
                            <asp:Panel ID="div_nieaktualne" runat="server" Visible="false" Style="">
                                <div class="div">NIEAKTUALNE DANE</div>
                                <br />
                                <br />
                                <span style="color: Red; font-weight: bold;">Dane zapisane w bazie są nieaktualne!<br />
                                    <br />
                                    Jeśli problem będzie się powatarzał proszę o kontakt z administratorem systemu
                               <br />
                                    <br />
                                    Telefon dyżurny: <%= TelDyzurny() %>
                                </span>
                                <br />
                                <br />
                                <asp:Button ID="btn_ok" CssClass="btn" runat="server" Text="OK" OnClick="btn_OK_Click" />
                                <br />
                            </asp:Panel>
                            <asp:Panel ID="div_tab" runat="server" Visible="false">
                                <div class="div">Dane zapisane w bazie</div>
                                <br />
                                <asp:GridView ID="GridView_tab" runat="server" CssClass="grid" Font-Size="10" CellPadding="1"
                                    ForeColor="#333333" GridLines="Both">
                                    <AlternatingRowStyle BackColor="White" />
                                    <EditRowStyle BackColor="#2461BF" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <SelectedRowStyle BackColor="#D1DDF1" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                </asp:GridView>
                                <br />
                                <asp:Button ID="btn_ok2" CssClass="btn" runat="server" Text="OK" OnClick="btn_OK2_Click" />
                            </asp:Panel>
                            <asp:Panel ID="div_haslo" runat="server" Visible="false">
                                <div class="div">ZMIANA HASŁA UŻYTKOWNIKA</div>
                                <br />
                                <table style="margin: 0 auto; width: 95%;">
                                    <tr>
                                        <td>Podaj stare hasło:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tb_old" runat="server" Width="200" TextMode="Password"></asp:TextBox>
                                            <br />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="Static"
                                                runat="server" CssClass="err" ErrorMessage="Pole wymaga wypełnienia" ControlToValidate="tb_old"></asp:RequiredFieldValidator>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Podaj nowe hasło:</td>
                                        <td>
                                            <asp:TextBox ID="tb_new" runat="server" Width="200" TextMode="Password"></asp:TextBox>
                                            <br />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Display="Static"
                                                runat="server" CssClass="err" ErrorMessage="Pole wymaga wypełnienia" ControlToValidate="tb_new"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Podaj ponownie<br />
                                            nowe hasło:</td>
                                        <td>
                                            <asp:TextBox ID="tb_new2" runat="server" Width="200" TextMode="Password"></asp:TextBox>
                                            <br />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="Static"
                                                runat="server" CssClass="err" ErrorMessage="Pole wymaga wypełnienia" ControlToValidate="tb_new2"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:CompareValidator ID="CompareValidator1" runat="server"
                                                ErrorMessage="Pola nowego hasła muszą być identyczne" CssClass="err" ControlToCompare="tb_new" ControlToValidate="tb_new2"></asp:CompareValidator>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Button ID="bt_pass" runat="server" CssClass="btn" Text="Zmień hasło!" OnClick="Button_Click" />&nbsp;&nbsp;&nbsp;<asp:Button ID="bt_anuluj" CausesValidation="false" runat="server" CssClass="btn" Text="Anuluj" OnClick="Button_Click" />

                                <br />
                            </asp:Panel>
                            <div style="display: inline-block; vertical-align: middle; width: 32%;"></div>
                        </div>
                    </asp:Panel>
                </div>

            </div>
        </div>
        <div id="footer">
            Copyrights IMGW-PIB &copy; 2015-20&nbsp;|&nbsp;Wspierany przez Centrum Informatyki&nbsp;|&nbsp;Min. 1280x768&nbsp;|&nbsp;
            IE10, Chrom, Mozilla&nbsp;|&nbsp;AWOS Monitor&nbsp;|&nbsp;<a href="mailto:serwisavis@imgw.pl?subject=AWOS%20Monitor%20zgłoszenie">zgłoś błąd systemu administratorom</a>&nbsp;|&nbsp;tel:&nbsp;<%= TelDyzurny() %>
        </div>
    </form>
</body>
</html>
