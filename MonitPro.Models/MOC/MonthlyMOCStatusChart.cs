using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.MOC;
namespace MonitPro.Models.MOC
{
    public class MonthlyMOCStatusChart : MonitPro.Models.BaseEntity
    {
        public List<MOCMonthlyChart> monthlychart = new List<MOCMonthlyChart>();

        public List<PlantWise> plantwisechart = new List<PlantWise>();

        public List<MocCategoryCount> categorychart = new List<MocCategoryCount>();

        public List<MOCpriorityCount> mocpriority = new List<MOCpriorityCount>();

        public List<MOCOverallStatus> mocstatus = new List<MOCOverallStatus>();

        public List<MOCRecommandStatus> mocrecommand = new List<MOCRecommandStatus>();
    }
}