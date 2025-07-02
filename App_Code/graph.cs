using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts;
using System.Web.UI.WebControls;
using backend;

/// <summary>
/// Summary description for graph
/// </summary>
/// 
namespace wykresy
{
    public class graph
    {

        public virtual void RysujWykres(Dane[] pomiary, string title, string opisX, string opisY, Label labelWykres, string nazwaWykresu, System.Drawing.Color kolor)
    {
        try
        {
            Series Serie = new Series();

            Serie.Name = opisX;
            Serie.Type = ChartTypes.Line;
            Series series = new Series { Name = opisX };


            XAxis xaxis = new XAxis();
            List<string> categories = new List<string>();
            List<object> data = new List<object>();

            foreach (var item in pomiary)
            {
                if (item.WartPom != "/")
                {
                    categories.Add(item.CzasPom);
                    if (nazwaWykresu.Contains("Cloud"))
                        data.Add(item.WartPom);
                    else data.Add(item.WartPom);

                }
            }


            if (data.Count > 1)
            {
                Highcharts wykres = new Highcharts(nazwaWykresu);

                wykres.InitChart(new Chart()
                {
                    DefaultSeriesType = ChartTypes.Line,
                    Height = 250,
                    BackgroundColor = new DotNet.Highcharts.Helpers.BackColorOrGradient(kolor),
                    ClassName = "chart",
                    ZoomType = ZoomTypes.X
                })
                .SetTitle(new Title { Text = title })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = opisY, Style = "font: 'normal 16px Arial'" } });



                wykres.SetSeries(series);


                xaxis.Categories = categories.ToArray();
                wykres.SetXAxis(xaxis);

                series.Data = new Data(data.ToArray());











                labelWykres.Text = wykres.ToHtmlString();
            }

        }
        catch (Exception)
        {
            labelWykres.Text = "";
        }
    }

}
}