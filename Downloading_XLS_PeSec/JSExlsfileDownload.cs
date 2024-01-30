using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using AltoHttp;
using AltoHttp.BrowserIntegration.Chrome;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;

namespace Downloading_XLS_PeSec
{
    public partial class JSExlsfileDownload : Form
    {
        HttpDownloader httpDownloader;

        private Queue<string> _downloadUrls = new Queue<string>();

        string MyConString = "Data Source=DESKTOP-C5R82QL\\SQLEXPRESS;Initial Catalog=JSE_Test; Integrated Security=true;";

        private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
        private string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";

        OpenFileDialog openfiledialog1 = new OpenFileDialog();

        public JSExlsfileDownload()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //_downloadUrls.Enqueue("https://clientportal.jse.co.za/downloadable-files?RequestNode=/YieldX/Derivatives/Docs_DMTM/20240126_D_Daily_MTM_Report.xls");
            //downloadFile(_downloadUrls);
            downloadFile();
        }

        private void HttpDownloader_ProgressChanged(object sender, AltoHttp.ProgressChangedEventArgs e)
        {
            progressBar1.Value = (int)e.Progress;
            lblPercentage.Text = $"{e.Progress.ToString("0.00")} %";
            lblSpeed.Text = string.Format("{0} MB/s", (e.SpeedInBytes / 1024d / 1024d).ToString("0.00"));
            lblDownloaded.Text = string.Format("{0} MB/s", (httpDownloader.TotalBytesReceived / 1024d / 1024d).ToString("0.00"));
            lblstatus.Text = "Loading.................";

        }

        private void downloadFile()
        {
            // Starts the download
            lblstatus.Text = "Downloading...";
            btnGenerate.Enabled = false;
            progressBar1.Visible = true;
            lblDownloaded.Visible = true;

            StringDownload();

        }

        public void StringDownload()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            WebClient client = new WebClient();

            string _filePath = "";
            string _fileToDownload = "";


            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;

            int days = DateTime.DaysInMonth(2024, 01);
            int countdec = DateTime.DaysInMonth(2023, 12);


            for (int index = 0; index < days; index++)
            {
                _filePath = @"C:\Files\JSEDownloadedData_" + index + ".xls";

                DateTime dateTime;
                var year = 2024;
                DateTime.TryParse(string.Format("1/1/{0}", year), out dateTime);

                if (year == 2024) _fileToDownload = "https://clientportal.jse.co.za/downloadable-files?RequestNode=/YieldX/Derivatives/Docs_DMTM/2024010" + index + "_D_Daily_MTM_Report.xls";
                else if (year == 2023)
                {
                    int month = 0;
                    month++;
                    _fileToDownload = "https://clientportal.jse.co.za/downloadable-files?RequestNode=/YieldX/Derivatives/Docs_DMTM/2023" + month + "" + index + "_D_Daily_MTM_Report.xls";
                }

                if (_fileToDownload != null)
                {
                    client.DownloadFile(new Uri(_fileToDownload), @_filePath);
                }
            }

            lblstatus.Text = "Download Complete";
            // btnGenerate.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle error scenario
                throw e.Error;
            }
            if (e.Cancelled)
            {
                // handle cancelled scenario
            }

            StringDownload();
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            /*if (httpDownloader != null)
                httpDownloader.Pause();*/
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            /* if (httpDownloader != null)
                 httpDownloader.Resume(); */
        }

        private void button1_Click(object sender, EventArgs e)
        {

            openfiledialog1.ShowDialog();
            openfiledialog1.Filter = "allfiles|*.xls";
            lblDownloaded.Text = openfiledialog1.FileName;

            //  lblDownloaded
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable dt;
            // OpenFileDialog dlg = new OpenFileDialog();
            openfiledialog1.Filter = "Excel files | *.xls";

            //if (openfiledialog1.ShowDialog() == DialogResult.OK)
            //  {
            string filePath = openfiledialog1.FileName;
            string extension = Path.GetExtension(filePath);
            string conStr, sheetName;

            conStr = string.Empty;
            switch (extension)
            {

                case ".xls": //Excel 97-03
                    conStr = string.Format(Excel03ConString, filePath);
                    break;

                case ".xlsx": //Excel 07 to later
                    conStr = string.Format(Excel07ConString, filePath);
                    break;
            }

            //Read Data from the Sheet.
            using (OleDbConnection con = new OleDbConnection(conStr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter oda = new OleDbDataAdapter())
                    {
                        dt = new DataTable();
                        cmd.CommandText = "SELECT * From [Sheet1$]";
                        cmd.Connection = con;
                        con.Open();
                        oda.SelectCommand = cmd;
                        oda.Fill(dt);
                        con.Close();
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(MyConString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = "dbo.DailyMTM";

                    //[OPTIONAL]: Map the Excel columns with that of the database table

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlBulkCopy.ColumnMappings.Add(i, i);
                    }

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
            //}
        }
    }
}
