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

namespace TP_BVSK
{
    public partial class frmThemNCCmoi : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";
        public frmThemNCCmoi()
        {
            InitializeComponent();
        }

        private void frmThemNCCmoi_Load(object sender, EventArgs e)
        {
            
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    string query = @"INSERT INTO NhaCungCap 
                            (MaNCC, TenNCC, MST, SDT, DiaChi) 
                            VALUES (@MaNCC, @TenNCC, @MST, @SDT, @DiaChi)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaNCC", txtMaNCC.Text);
                    cmd.Parameters.AddWithValue("@TenNCC", txtTenNCC.Text);
                    cmd.Parameters.AddWithValue("@MST", txtMST.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDTNCC.Text);
                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChiNCC.Text);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close(); // Đóng form sau khi thêm
                    }
                    else
                    {
                        MessageBox.Show("Thêm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
