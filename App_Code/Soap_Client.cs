using System;
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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Security;
using System.Configuration;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using back;
using krypto;
using System.Xml;
using System.IO.Compression;
using System.Net.Security;
using System.ServiceModel;
using System.Runtime.Serialization.Formatters;


/// <summary>
/// Opis podsumowujący dla Soap_Client
/// </summary>
public class Soap_Client
{ 
    static public string stationCode = "";
    static string endpoint = "https://szds-migrated.northeurope.cloudapp.azure.com:8080/SzdsClientWs.svc";
    static string operacja = "http://www.imgw.pl/szds/webservice/v100/GetAllMeasurements";

    static string resOk = " Success";
    static string resError = " Error";

    public string Wyjatek { get; set; }

    public static string wynik;   

    public static async Task<String> GetAllMeasurements()
    {
        string action = operacja;
        
        XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
        WebResponse webResponse = null;      

        return await Task.Run(() =>
        {
            try
            {        
                HttpWebRequest webRequest = CreateWebRequest(endpoint, operacja); 

                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();

                string soapResult;   

                webResponse = webRequest.EndGetResponse(asyncResult);

                StreamReader rd = new StreamReader(webResponse.GetResponseStream());
                soapResult = rd.ReadToEnd();
                rd.Close();
                wynik = soapResult;
                if (soapResult.Contains("Success")) return resOk;
                else return resError;
            }
            catch (Exception e)
            {
                if (e is WebException && ((WebException)e).Status
                            == WebExceptionStatus.ProtocolError)
                {
                    WebResponse errResp = ((WebException)e).Response;
                    using (Stream respStream = errResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream);
                        string text = reader.ReadToEnd();
                        return text;
                    }
                }
                return "error";
            }
            finally
            {
                if (webResponse != null) webResponse.Close();
            }
        });
    }


    private static HttpWebRequest CreateWebRequest(string url, string action)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);      
        webRequest.ContentType = string.Format("application/soap+xml;charset=UTF-8;action=\"{0}\"", action);
        webRequest.Accept = "gzip,deflate";
        webRequest.Method = "POST";
        webRequest.Host = "szds-migrated.northeurope.cloudapp.azure.com:8080";     
        webRequest.Connection = "Keep-Alive";   
        //webRequest.Timeout = 30000;
        return webRequest;
    }

    private static XmlDocument CreateSoapEnvelope()
    {
        XmlDocument soapEnvelopeXml = new XmlDocument();
        string pomiar = "";

        pomiar = "<soap:Body>";
        pomiar += "<v100:getAllMeasurementsRequest>";
        pomiar += "<v100:login>mkowalsk</v100:login>";
        pomiar += "<v100:stationCode>12595</v100:stationCode>";
        pomiar += "<v100:dateTimeFrom>" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "T" + DateTime.UtcNow.AddMinutes(-1).ToString("HH:mm") + ":00Z" + "</v100:dateTimeFrom>";
        pomiar += "<v100:dateTimeTo>" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "T" + DateTime.UtcNow.AddMinutes(-1).ToString("HH:mm") + ":59Z" + "</v100:dateTimeTo>";
        pomiar += "</v100:getAllMeasurementsRequest>";
        pomiar += "</soap:Body>";
        pomiar += "</soap:Envelope>";

        soapEnvelopeXml.LoadXml(@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:v100=""http://www.imgw.pl/szds/xsd/v100"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                    <soap:Header>
                        <wsse:Security soap:mustUnderstand=""1"">
                        <wsse:UsernameToken wsu:Id=""UsernameToken-1"">
                        <wsse:Username>mkowalski</wsse:Username>
                        <wsse:Password>Mkowalski1234!</wsse:Password>
                        </wsse:UsernameToken>
                        </wsse:Security>
                    </soap:Header>" + @"" + pomiar);

        return soapEnvelopeXml;
    }

    private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
    {
        String wyjatek = "";

        try
        {
            Stream stream = webRequest.GetRequestStream();
            soapEnvelopeXml.Save(stream);           
            stream.Close();
        }
        catch (Exception ex)
        {
            wyjatek += ex.Message;
        }
    }
}
