using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zipfile_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadImage();
        }

        private void LoadImage()
        {
            // กำหนดเส้นทางไปยังไฟล์รูปภาพ
            pictureBox1.Image = Properties.Resources.fatman; // เปลี่ยนเส้นทางให้ตรงกับไฟล์รูปภาพของคุณ

          
        }
        private void btnCreateZip_Click(object sender, EventArgs e)
        {
            // อ่านค่าจาก TextBox
            string sourceFolderPath = txtSourceFolderPath.Text;
            string zipFolderPath = txtZipFolderPath.Text;
            string startDateStr = txtStartDate.Text;
            string endDateStr = txtEndDate.Text;

            // แปลงวันที่จาก string เป็น DateTime
            DateTime startDate = DateTime.ParseExact(startDateStr, "yyyy-MM-dd", null);
            DateTime endDate = DateTime.ParseExact(endDateStr, "yyyy-MM-dd", null);

            // ค้นหาไฟล์ในโฟลเดอร์ที่มีวันที่แก้ไขอยู่ในช่วงวันที่ที่กำหนด
            var filesToZip = Directory.GetFiles(sourceFolderPath)
                                      .Where(file => File.GetLastWriteTime(file) >= startDate &&
                                                     File.GetLastWriteTime(file) <= endDate)
                                      .ToList();

            if (filesToZip.Count == 0)
            {
                UpdateConsole($"ไม่พบไฟล์ที่มีวันที่แก้ไขระหว่าง {startDateStr} และ {endDateStr}");
                return;
            }

            // จัดกลุ่มไฟล์ตามวันที่แก้ไข
            var groupedFiles = filesToZip.GroupBy(file => File.GetLastWriteTime(file).Date);

            foreach (var group in groupedFiles)
            {
                DateTime date = group.Key;
                string dateString = date.ToString("yyyy-MM-dd");
                string zipFileName = $"Files_{dateString}.zip";
                string zipPath = Path.Combine(zipFolderPath, zipFileName);

                using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (string file in group)
                    {
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                        UpdateConsole($"เพิ่มไฟล์: {file}");
                    }
                }

                UpdateConsole($"สร้าง zip เรียบร้อย: {zipPath}");
            }

            UpdateConsole("กระบวนการเสร็จสมบูรณ์");
        }

        private void UpdateConsole(string message)
        {
            txtConsole.AppendText(message + Environment.NewLine);
        }
    }
}
