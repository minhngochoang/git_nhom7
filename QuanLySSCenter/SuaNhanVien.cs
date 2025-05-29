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
using System.Text.RegularExpressions;


namespace QuanLySSCenter
{
    public partial class SuaNhanVien : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";

        private string MaNV;

        public SuaNhanVien(string MaNV)
        {
            InitializeComponent();
            this.MaNV = MaNV;

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void SuaNhanVien_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM NhanVien WHERE MaNV = @MaNV";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaNV", MaNV);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtMaNV.Text = reader["MaNV"].ToString();
                        txtTenNV.Text = reader["TenNV"].ToString();
                        txtSDTNV.Text = reader["SDT"].ToString();

                        txtMaNV.Enabled = false; // Không cho sửa mã
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhân viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                    this.Close();
                }
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra TenNCC
            if (string.IsNullOrWhiteSpace(txtTenNV.Text))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNV.Focus();
                return false;
            }


            // Kiểm tra SDT (chỉ chứa chữ số, độ dài 10-11)
            if (!string.IsNullOrWhiteSpace(txtSDTNV.Text))
            {
                if (!Regex.IsMatch(txtSDTNV.Text, @"^\d{10,11}$"))
                {
                    MessageBox.Show("Số điện thoại phải chứa 10-11 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDTNV.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Số điện thoại không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTNV.Focus();
                return false;
            }
            return true;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ValidateInput())
                return;

            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    string query = @"UPDATE NhanVien
                                 SET TenNV = @TenNV, 
                                     SDT = @SDT 
                                 WHERE MaNV = @MaNV";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                    cmd.Parameters.AddWithValue("@TenNV", txtTenNV.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDTNV.Text);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không cập nhật được!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
