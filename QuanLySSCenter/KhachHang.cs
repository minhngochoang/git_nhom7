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

namespace QuanLySSCenter
{
    public partial class KhachHang : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";
        public KhachHang()
        {
            InitializeComponent();
        }

        private void KhachHang_Load(object sender, EventArgs e)
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
            string sQuery = "SELECT * FROM KHACHHANG";
            SqlDataAdapter adapter = new SqlDataAdapter(sQuery, con);

            DataSet ds = new DataSet();

            adapter.Fill(ds, "KHACHHANG");
            guna2DataGridView1.DataSource = ds.Tables["KHACHHANG"];

            con.Close();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                string query = "SELECT * FROM KhachHang WHERE MaKH = @MaKH";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                if (guna2DataGridView1.CurrentRow != null)
                {
                    string MaKH = guna2DataGridView1.CurrentRow.Cells["MaKH"].Value.ToString();
                    da.SelectCommand.Parameters.AddWithValue("@MaKH", MaKH);
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một nhà cung cấp.");
                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                guna2DataGridView1.DataSource = dt;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ThemKhachHang formThem = new ThemKhachHang();

            // Gắn sự kiện FormThemNCC đóng thì reload lại dữ liệu
            //formThem.FormClosed += (s, args) => LoadData();

            formThem.ShowDialog();
        }
    }
}
