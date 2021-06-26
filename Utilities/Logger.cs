using System;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;

namespace AssignmentD
{
    public class Logger
    {
        #region constants
        
        private const string BSET_TOOL_NAME = @"\ShapeMaker\";
        private const string DIRECTORY_NAME_EXT = "Logs";
        private const string TOOL_NAME = "ShapeMaker";
        private const string EXCEPTION = "Exception";
        private const string FILE_NAME_EXT = "{0}_Log.bl";
        private const string DATE_FORMAT = "ddMMMyyyy";
        private static string DIRECTORY_NAME = string.Empty;
        private static string FILE_NAME = string.Empty;
        private const string FORMATTER = "Type\t:\t{0}{1}Method\t:\t{2}{1}Details\t:\t{3}({8}){1}Date\t:\t{4}{1}User\t:\t{5} {1}{6}{7}{6}{1}";
        private const string FILLER = "===================================";
        private const string DOT = ".";

        #endregion

        #region Instance

        private static Logger instance;

        public static Logger Instance
        {
            get
            {
                // Checks if the instance is null and creates it for first time use
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }

        #endregion

        #region Constructor

        private Logger() { } 

        #endregion

        #region Logging methods

        /// <summary>
        /// Logs the information to the log file
        /// </summary>
        /// <param name="type">Log type</param>
        /// <param name="classMethodName">class and Method name</param>
        /// <param name="exceptionObject">exception to be logged</param>
        public void WriteToLog(string classMethodName, Object exceptionObject)
        {
            string logMessage = exceptionObject == null ? string.Empty : exceptionObject.ToString();
            string hostName = string.Empty;
            try
            {
                hostName = string.Concat(IPGlobalProperties.GetIPGlobalProperties().HostName, DOT, IPGlobalProperties.GetIPGlobalProperties().DomainName);
            }
            catch
            {
                //
            }
            try
            {
                DIRECTORY_NAME = BSET_TOOL_NAME + DIRECTORY_NAME_EXT;
                FILE_NAME = string.Format(CultureInfo.CurrentCulture, FILE_NAME_EXT, DateTime.Today.ToString(DATE_FORMAT)); 

                // Fetch the directory name
                string directoryName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + DIRECTORY_NAME+@"\"; 

                // Check for the existence of the directory
                if (!Directory.Exists(directoryName))
                {
                    // Create one as it's not exist
                    Directory.CreateDirectory(directoryName);
                }

                // Writing the log in the roaming folder
                File.AppendAllText(directoryName+FILE_NAME,
                    string.Format(CultureInfo.CurrentCulture, FORMATTER,
                    EXCEPTION,  //{0}
                    Environment.NewLine, //{1}
                    classMethodName, //{2}
                    logMessage, //{3}
                    DateTime.Now, //{4}
                    Environment.UserName, //{5}
                    FILLER, //{6}
                    TOOL_NAME, //{7}
                    hostName)); //{8} 
            }
            catch
            {   
                //
            }
        }

        #endregion
    }
}
