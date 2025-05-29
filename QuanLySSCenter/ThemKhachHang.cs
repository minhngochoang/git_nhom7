using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QuanLySSCenter
{
    public partial class ThemKhachHang : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";
        public ThemKhachHang()
        {
            InitializeComponent();
        }

        private void ThemKhachHang_Load(object sender, EventArgs e)
        {
            
        }
    }
}
