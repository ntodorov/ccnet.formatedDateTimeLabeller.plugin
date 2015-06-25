using Exortech.NetReflector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Globalization;

namespace ccnet.formatedDateTimeLabeller.plugin
{
    /// <summary>
    /// The formatedDateTimeLabeller is used to generate labels in the format "yy.mm.dd.revision". Using the formatedDateTimeLabeller makes it easy for the user to identify and communicate the date that a particular build occurred.
    /// Other format could be y.M.d.revision or yyyy.MM.dd
    /// </summary>
    /// <title>Date Labeller</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para>
    /// The revision is increased on every build done at the same day, so if you do 2 builds on 2015/01/20, the first will be have label 15.01.20.001,  and the second will be 15.01.20.002
    /// If you use y.M.d for 2009/01/03 will be 9.1.3.001
    /// YearFormat, MothFormat and DayFormat are used in DateTime.ToString() as formaters
    /// https://msdn.microsoft.com/en-us/library/8kb3ddd4%28v=vs.110%29.aspx
    /// </para>
    /// <para>
    /// This labeller has been contributed by Nikolay Todorov
    /// </para>
    /// </remarks>
    /// <example>
    /// <code title="Minimal Example">
    /// &lt;labeller type="formatedDateTimeLabeller" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("formatedDateTimeLabeller")]
    public class formatedDateTimeLabeller : ITask, ILabeller
	{
		private readonly DateTimeProvider dateTimeProvider;

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="formatedDateTimeLabeller"/> class.
        /// </summary>
        /// <param name="dateTimeProvider">The date time provider.</param>
        public formatedDateTimeLabeller(DateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.YearFormat = "yy";
            this.MonthFormat = "MM";
            this.DayFormat = "dd";
            RevisionFormat = "000";
            IncrementOnFailed = false;
            ResetRevision = true;
        }

        public formatedDateTimeLabeller()
            : this(new DateTimeProvider())
        {
        }
        
        #endregion

        /// <summary>
        /// The format for the year part as in C# function DateTime.ToString("yy");
        /// </summary>
        /// <version>1.0</version>
        /// <default>yy</default>
        /// 
        private string yearFormat = "yy";

        [ReflectorProperty("yearFormat", Required = false)]
        public string YearFormat {
            get { return yearFormat; }

            set 
            { 
                yearFormat = value;
                if (yearFormat.Trim().Length == 1)
                    yearFormat = "%" + yearFormat;
            }
        }

        /// <summary>
        /// The format for the month part as in C# function DateTime.ToString("MM");
        /// </summary>
        /// <version>1.0</version>
        /// <default>MM</default>
        private string monthFormat = "MM";

        [ReflectorProperty("monthFormat", Required = false)]
        public string MonthFormat 
        {
            get { return monthFormat; }

            set 
            {
                monthFormat = value;
                if (monthFormat.Trim().Length == 1)
                    monthFormat = "%" + monthFormat;
            }
        }

        /// <summary>
        /// The format for the day part as in C# function DateTime.ToString("dd");
        /// </summary>
        /// <version>1.0</version>
        /// <default>dd</default>
        private string dayFormat = "dd";
         
        [ReflectorProperty("dayFormat", Required = false)]
        public string DayFormat 
        {
            get { return dayFormat; }

            set 
            {
                dayFormat = value;
                if (dayFormat.Trim().Length == 1)
                    dayFormat = "%" + dayFormat;
            }
        }
        /// <summary>
        /// The format for the revision part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>000</default>
        [ReflectorProperty("revisionFormat", Required = false)]
        public string RevisionFormat { get; set; }

        /// <summary>
        /// Determines if the build should be labeled even if it fails.
        /// So you can examine previous failed builds to determine why
        /// it failed.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
		[ReflectorProperty("incrementOnFailure", Required = false)]
		public bool IncrementOnFailed { get; set; }

        /// <summary>
        /// Determines if the RevisionNumber should be reset to 1 every day.
        /// 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("resetRevision", Required = false)]
        public bool ResetRevision { get; set; }


        public void Run(IIntegrationResult result)
        {
            result.Label = this.Generate(result);
        }

        /// <summary>
        /// Generates the specified integration result.	
        /// </summary>
        /// <param name="integrationResult">The integration result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string Generate(IIntegrationResult integrationResult)
		{
			DateTime now = dateTimeProvider.Now;
            
            Version newVersion = GenNewVersion(now);
                      
			Version version = ParseVersion(now, integrationResult.LastIntegration);


			int revision = version.Revision;

            if (ResetRevision)
            {
                if ((newVersion.Major == version.Major && newVersion.Minor == version.Minor && newVersion.Build == version.Build))
                {
                    revision += 1;
                }
                else
                {
                    revision = 1;
                }
            }
            else revision += 1;

            return string.Format(CultureInfo.CurrentCulture,"{0}.{1}.{2}.{3}",
                   now.ToString(YearFormat, CultureInfo.CurrentCulture), 
                   now.ToString(MonthFormat, CultureInfo.CurrentCulture), 
                   now.ToString(DayFormat, CultureInfo.CurrentCulture), 
                   revision.ToString(RevisionFormat, CultureInfo.CurrentCulture));
		}

        
		private Version ParseVersion(DateTime date, IntegrationSummary lastIntegrationSummary)
		{
			try
			{
				if (IncrementOnFailed)
					return new Version(lastIntegrationSummary.Label);
				else
					return new Version(lastIntegrationSummary.LastSuccessfulIntegrationLabel);
			}
			catch (SystemException)
			{
				return new Version(date.Year, date.Month, date.Day, 0);
			}
		}

        private Version GenNewVersion(DateTime now)
        {
            try
            {

                int major = Convert.ToInt32(now.ToString(YearFormat, CultureInfo.CurrentCulture));
                int minor = Convert.ToInt32(now.ToString(MonthFormat, CultureInfo.CurrentCulture));
                int build = Convert.ToInt32(now.ToString(DayFormat, CultureInfo.CurrentCulture));

                return new Version(major, minor, build, 0);
            }
            catch (SystemException)
            {
                return new Version(now.Year, now.Month, now.Day, 0);
            }
        }
	}
}
