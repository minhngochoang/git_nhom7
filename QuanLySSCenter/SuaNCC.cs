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
    public partial class SuaNCC : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";

        private string MaNCC;

        public SuaNCC(string MaNCC)
        {
            InitializeComponent();
            this.MaNCC = MaNCC;
        }

        public SuaNCC()
        {
        }

        private void SuaNCC_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM NhaCungCap WHERE MaNCC = @MaNCC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaNCC", MaNCC);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtMaNCC.Text = reader["MaNCC"].ToString();
                        txtTenNCC.Text = reader["TenNCC"].ToString();
                        txtMST.Text = reader["MST"].ToString();
                        txtSDTNCC.Text = reader["SDT"].ToString();
                        txtDiaChiNCC.Text = reader["DiaChi"].ToString();

                        txtMaNCC.Enabled = false; // Không cho sửa mã
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
                return false;
            }

            // Kiểm tra MST (chỉ chứa chữ số)
            if (!string.IsNullOrWhiteSpace(txtMST.Text) && !Regex.IsMatch(txtMST.Text, @"^\d+$"))
            {
                MessageBox.Show("Mã số thuế chỉ được chứa chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMST.Focus();
                return false;
            }

            // Kiểm tra SDT (chỉ chứa chữ số, độ dài 10-11)
            if (!string.IsNullOrWhiteSpace(txtSDTNCC.Text))
            {
                if (!Regex.IsMatch(txtSDTNCC.Text, @"^\d{10,11}$"))
                {
                    MessageBox.Show("Số điện thoại phải chứa 10-11 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDTNCC.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Số điện thoại không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTNCC.Focus();
                return false;
            }

            // Kiểm tra DiaChi
            if (string.IsNullOrWhiteSpace(txtDiaChiNCC.Text))
            {
                MessageBox.Show("Địa chỉ không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChiNCC.Focus();
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
                    string query = @"UPDATE NhaCungCap 
                                 SET TenNCC = @TenNCC, 
                                     MST = @MST, 
                                     SDT = @SDT, 
                                     DiaChi = @DiaChi 
                                 WHERE MaNCC = @MaNCC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaNCC", txtMaNCC.Text);
                    cmd.Parameters.AddWithValue("@TenNCC", txtTenNCC.Text);
                    cmd.Parameters.AddWithValue("@MST", txtMST.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDTNCC.Text);
                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChiNCC.Text);

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

        private void txtMaNCC_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
