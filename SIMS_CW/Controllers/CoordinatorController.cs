using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
namespace SIMS_CW.Controllers
{
    public class CoordinatorController : Controller
    {

        DbModel dbData = new DbModel();


        // GET: Coordinator
        public ActionResult PercentageOfIdeasByDepartment(string type = "pie")
        {
            List<idea> idea = dbData.ideas.ToList();

            string xvalues = "";
            string yvalues = "";

            foreach (department dept in dbData.departments.ToList())
            {
                decimal percentage = (decimal.Divide(idea.Where(i => i.user.department.department_id == dept.department_id).Count(), idea.Count())) * 100;
                xvalues += dept.department_name +  " \n\n" + string.Format("{0:.#}", percentage) + "% ,";
                yvalues += percentage + ",";
            }

            xvalues = xvalues.Remove(xvalues.Length - 1, 1);
            yvalues = yvalues.Remove(yvalues.Length - 1, 1);

            var myChart = new Chart(width: 700, height: 500, theme: ChartTheme.Vanilla3D)
            .AddTitle("Percentage of Claims By Academic Year")
            .AddLegend("Percentage of Claims By Academic Year", "Percentage of Claims By Academic Year")
            .AddSeries(

                name: "Percentage of Claims By Academic Year",
                legend: "Percentage of Claims By Academic Year",
                chartType: type,
                xValue: xvalues.Split(','),
                yValues: yvalues.Split(',')).Write("png");

            return null;

        }


        public ActionResult NumberOfIdeascharts(string type = "bar")
        {
            List<idea> ideaa = dbData.ideas.ToList(); 
            string xvalues = "";
            string yvalues = "";

            foreach (department dept in dbData.departments.ToList())
            {
                xvalues += dept.department_name + ",";
                yvalues += ideaa.Where(i => i.user.department.department_id == dept.department_id).Count() + ",";
            }

            xvalues = xvalues.Remove(xvalues.Length - 1, 1);
            yvalues = yvalues.Remove(yvalues.Length - 1, 1);

            var myChart = new Chart(width: 600, height: 400, theme: ChartTheme.Blue)
            .AddTitle("Number of Ideas By Each Department Bar Chart")
            .AddSeries(
                name: "Number of Ideas",
                xField: "Department Names",
                yFields: "Number of Ideas",
                chartType: type,
                xValue: xvalues.Split(','),
                yValues: yvalues.Split(',')).Write("png");

            
            

            return null;

        }

        public ActionResult NumberOfContributorsPerDepartment(string type = "line")
        {
            List<idea> ideaa = dbData.ideas.ToList();
            List<comment> cmt = dbData.comments.ToList();

            string xvalues = "";
            string yvalues = "";
            string yvalues2 = "";

            foreach (department dept in dbData.departments.ToList())
            {
                xvalues += dept.department_name + ",";
                yvalues += ideaa.Where(i => i.user.user_id == dept.department_id).Distinct().Count() + ",";
                yvalues2 += cmt.Where(c => c.user.user_id == dept.department_id).Distinct().Count() + ",";

            }

            xvalues = xvalues.Remove(xvalues.Length - 1, 1);
            yvalues = yvalues.Remove(yvalues.Length - 1, 1);
            yvalues2 = yvalues2.Remove(yvalues2.Length - 1, 1);

            var myChart = new Chart(width: 600, height: 400, theme: ChartTheme.Yellow)
            .AddTitle("Number of Contributors within Each Department Bar Chart")
              .AddSeries(
                chartType: type,
                xValue: xvalues.Split(','),
                yValues: yvalues.Split(','),
                name: "Ideas")
            .AddSeries(
                chartType: "Column",
                xValue: xvalues.Split(','),
                yValues: yvalues2.Split(','),
                name: "Comments")
            .AddLegend()
            .Write("png");

            return null;
        }

        public ActionResult Statistics()
        {
            return View();
        }
    }
}