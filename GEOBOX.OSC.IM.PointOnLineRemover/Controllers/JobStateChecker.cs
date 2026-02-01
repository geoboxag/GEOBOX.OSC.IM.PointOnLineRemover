using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Map.IM.Forms;
using Autodesk.Map.IM.Data.Jobs3;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Controllers
{
    /// <summary>
    /// Class for Check is database job enabled an is actual job state
    /// </summary>
    internal sealed class JobStateChecker
    {
        /// <summary>
        /// Check Job: is document not job enabled or seleced job is in open state
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static bool IsReadyForRun(Document document)
        {
            // Document is not job enabled = ready for run
            if (document.Connection.IsJobEnabled == false) return true;

            // Check actual job
            Job job = document.Connection.Jobs.Selected;
            if (job == null) return false;

            // Job is Live or ...
            if (job.ID == 1 || job.ID == 2) return false;

            // select job is not in open state
            if (IsJobStateInOpen(job)) return false;

            return true;
        }

        /// <summary>
        /// Check Job: is topic Ownership in Job and state in open
        /// </summary>
        /// <param name="job">Job for Check</param>
        /// <returns>true = Ownership and open, false = not Ownership or not open</returns>
        public static bool IsJobStateInOpen(Job job)
        {
            foreach (JobTopic topic in job.JobTopics)
            {
                if (topic.CurrentJobStateID == 3)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
