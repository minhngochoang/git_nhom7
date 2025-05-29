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
using TP_BVSK;

namespace QuanLySSCenter
{
    public partial class NhanVien : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";

        public NhanVien()
        {
            InitializeComponent();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void NhanVien_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(sCon);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
            }

            //Lấy dữ liệu về
            string sQuery = "SELECT * FROM NhanVien";
            SqlDataAdapter adapter = new SqlDataAdapter(sQuery, con);

            DataSet ds = new DataSet();

            adapter.Fill(ds, "NhanVien");
            guna2DataGridView1.DataSource = ds.Tables["NhanVien"];

            LoadData();


            con.Close();
        }

        private void LoadData()
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    string sQuery = "SELECT * FROM NhanVien";
                    SqlDataAdapter adapter = new SqlDataAdapter(sQuery, con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "NhanVien");
                    guna2DataGridView1.DataSource = ds.Tables["NhanVien"];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text.Trim(); // <-- Lấy từ TextBox nhập mã NV

            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên cần tìm.");
                return;
            }

            using (SqlConnection con = new SqlConnection(sCon))
            {
                string query = "SELECT * FROM NhanVien WHERE MaNV = @MaNV";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@MaNV", maNV);

                DataTable dt = new DataTable();
                da.Fill(dt);
                guna2DataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhân viên có mã: " + maNV);
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ThemNhanVien formThem = new ThemNhanVien();

            // Gắn sự kiện FormThemNCC đóng thì reload lại dữ liệu
            formThem.FormClosed += (s, args) => LoadData();

            formThem.ShowDialog();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string MaNV = guna2DataGridView1.SelectedRows[0].Cells["MaNV"].Value.ToString();
                SuaNhanVien suaForm = new SuaNhanVien(MaNV);
                suaForm.ShowDialog();

                // Tải lại dữ liệu sau khi sửa
                LoadData();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                // Lấy MaNCC từ hàng được chọn trong dataGridView1
                string MaNV = guna2DataGridView1.SelectedRows[0].Cells["MaNV"].Value.ToString();

                // Xác nhận trước khi xóa
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                    return;

                using (SqlConnection con = new SqlConnection(sCon))
                {
                    try
                    {
                        con.Open();
                        string query = "DELETE FROM NhanVien WHERE MaNV = @MaNV";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@MaNV", MaNV);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData(); // Tải lại dữ liệu sau khi xóa
                        }
                        else
                        {
                            MessageBox.Show("Không xóa được!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhà cung cấp để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
