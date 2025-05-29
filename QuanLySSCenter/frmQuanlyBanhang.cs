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
    public partial class frmQuanlyBanhang : Form
    {
        string sCon = "Data Source=LAPTOP-0B1SRHV7;Initial Catalog = BTLNhom7; Integrated Security = True; TrustServerCertificate=True";

        public frmQuanlyBanhang()
        {
            InitializeComponent();
            txtSLBan.TextChanged += (sender, eventArgs) => TinhTien();
            txtDGBan.TextChanged += (sender, eventArgs) => TinhTien();
            txtChietKhau.TextChanged += (sender, eventArgs) => TinhTien();
            txtMaHH.TextChanged += (sender, eventArgs) => CapNhatDG_Ban();
        }
        private void frmQuanlyBanhang_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    string sQuery = @"
                SELECT HOADON_BAN.SOHDB, NGAYBAN, TONGTIEN, CHIETKHAU,
                        THANHTOAN, MANV, MAKH, 
                       MAHANG, SL_BAN, DONGIA_BAN, THANHTIEN

                FROM HOADON_BAN
                JOIN CHITIET_BAN ON HOADON_BAN.SOHDB = CHITIET_BAN.SOHDB";

                    SqlDataAdapter adapter = new SqlDataAdapter(sQuery, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataBanHang.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TinhTien()
        {
            if (decimal.TryParse(txtSLBan.Text, out decimal soLuong) &&
                decimal.TryParse(txtDGBan.Text, out decimal donGia))
            {
                decimal thanhTien = soLuong * donGia;
                txtThanhTien.Text = thanhTien.ToString("N0");

                // Tổng tiền = Thành tiền (tạm thời) nếu có thể nhập nhiều dòng thì cần tính lại toàn bộ
                txtTongTien.Text = thanhTien.ToString("N0");

                if (decimal.TryParse(txtChietKhau.Text, out decimal chietKhau))
                {
                    decimal thanhToan = thanhTien - chietKhau;
                    txtThanhToan.Text = thanhToan.ToString("N0");
                }
            }
            else
            {
                txtThanhTien.Text = "";
                txtTongTien.Text = "";
                txtThanhToan.Text = "";
            }
        }

        private void CapNhatDG_Ban()
        {
            string maHang = txtMaHH.Text.Trim();
            if (maHang.Length != 10) // Chỉ gọi nếu mã hàng dài đủ 5 ký tự
            {
                txtDGBan.Text = "";
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtMaHH.Text.Trim()))
            {
                using (SqlConnection con = new SqlConnection(sCon))
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT TOP 1 DONGIA_NHAP FROM HOADON_NHAP JOIN CHITIET_NHAP ON HOADON_NHAP.SOHDN = CHITIET_NHAP.SOHDN WHERE MAHANG = @MaHang ORDER BY NGAYNHAP DESC";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@MaHang", txtMaHH.Text.Trim());

                        object result = cmd.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal donGiaNhap))
                        {
                            decimal donGiaBan = donGiaNhap * 1.3m; // Tính đơn giá bán = đơn giá nhập * 1.3
                            txtDGBan.Text = donGiaBan.ToString("N0");
                        }
                        else
                        {
                            txtDGBan.Text = ""; // Nếu không tìm thấy đơn giá nhập, xóa giá trị
                            MessageBox.Show("Không tìm thấy đơn giá nhập cho mặt hàng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lấy đơn giá nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                txtDGBan.Text = ""; // Nếu mã hàng trống, xóa giá trị
            }

            // Gọi TinhTien() để cập nhật các giá trị khác
            TinhTien();
        }

        private void btnThemHDB_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    //Hoá đơn bán
                    string sSoHDB = txtSoHDB.Text.Trim();
                    string sNgayBan = dateNgayBan.Value.ToString("yyyy-MM-dd");
                    string sChietKhau = txtChietKhau.Text.Trim();
                    string sMaNV = txtMaNV.Text.Trim();
                    string sMaHKH = txtMaKH.Text.Trim();

                    CapNhatDG_Ban();

                    // Kiểm tra hóa đơn đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM HOADON_BAN WHERE SoHDB = @SoHDB";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@SoHDB", sSoHDB);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        string insertHoaDon = @"INSERT INTO HOADON_BAN (SoHDB, NgayBan, TongTien, ChietKhau, ThanhToan, MaNV, MaKH)

                        VALUES (@SoHDB, @NgayBan, @TongTien, @ChietKhau, @ThanhToan, @MaNV, @MaKH)";
                        SqlCommand cmd1 = new SqlCommand(insertHoaDon, con);
                        
                        cmd1.Parameters.AddWithValue("@SoHDB", sSoHDB);
                        cmd1.Parameters.AddWithValue("@NgayBan", sNgayBan);
                        TinhTien();
                        cmd1.Parameters.AddWithValue("@TongTien", txtTongTien.Text);
                        // Tính toán tổng tiền và thành tiền

                        // Tổng tiền
                        txtSLBan.TextChanged += (s, eventArgs) => TinhTien();
                        txtDGBan.TextChanged += (s, eventArgs) => TinhTien();

                        cmd1.Parameters.AddWithValue("@ChietKhau", sChietKhau);

                        decimal thanhToan = 0;
                        if (decimal.TryParse(txtThanhToan.Text, out decimal thanhToanValue))
                        {
                            thanhToan = thanhToanValue;
                        }
                        cmd1.Parameters.AddWithValue("@ThanhToan", thanhToan);
                        cmd1.Parameters.AddWithValue("@MaNV", sMaNV);
                        cmd1.Parameters.AddWithValue("@MaKH", sMaHKH);


                        cmd1.ExecuteNonQuery();
                    }

                    //Chi tiết bán
                    string sMaHang = txtMaHH.Text.Trim();
                    string sSLBan = txtSLBan.Text.Trim();
                    string sDonGia_Ban = txtDGBan.Text.Trim();
                                  
                    string insertChiTiet = @"INSERT INTO CHITIET_BAN
                    (SOHDB, MAHANG, SL_BAN, DONGIA_BAN, THANHTIEN)
                VALUES (@SoHDB, @MaHang, @SL_Ban, @DonGia_Ban, @ThanhTien)";

                    SqlCommand cmd2 = new SqlCommand(insertChiTiet, con);
                    cmd2.Parameters.AddWithValue("@SoHDB", txtSoHDB.Text);             
                    cmd2.Parameters.AddWithValue("@MaHang", sMaHang);
                    cmd2.Parameters.AddWithValue("@SL_Ban", sSLBan);
                    cmd2.Parameters.AddWithValue("@DonGia_Ban", sDonGia_Ban);
                    decimal thanhTien = 0;
                    if (decimal.TryParse(txtThanhTien.Text, out decimal thanhTienValue))
                    {
                        thanhTien = thanhTienValue;
                    }
                    cmd2.Parameters.AddWithValue("@ThanhTien", thanhTien);

                    if (string.IsNullOrWhiteSpace(txtSoHDB.Text) ||
                        string.IsNullOrWhiteSpace(txtMaNV.Text) ||
                        string.IsNullOrWhiteSpace(txtMaKH.Text) ||
                        string.IsNullOrWhiteSpace(txtMaHH.Text) ||
                        string.IsNullOrWhiteSpace(txtSLBan.Text) ||
                        string.IsNullOrWhiteSpace(txtDGBan.Text))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!decimal.TryParse(txtSLBan.Text, out decimal soLuong) ||
                        !decimal.TryParse(txtDGBan.Text, out decimal donGia) ||
                        !decimal.TryParse(txtChietKhau.Text, out decimal chietKhau))
                    {
                        MessageBox.Show("Số lượng, đơn giá hoặc chiết khấu không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    cmd2.ExecuteNonQuery();

                    MessageBox.Show("Thêm chi tiết hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close();
                    LoadData(); // Cập nhật lại datagridview
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim().ToUpper();
            string SoHDB = null, MaHang = null;

            // Cắt chuỗi theo dấu phẩy
            string[] parts = tuKhoa.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string raw in parts)
            {
                string part = raw.Trim();
                if (part.StartsWith("HDB"))
                    SoHDB = part;
                else if (part.StartsWith("HH"))
                    MaHang = part;
            }

            if (SoHDB == null && MaHang == null)
            {
                MessageBox.Show("Vui lòng nhập Số HDB hoặc Mã hàng (HH).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(sCon))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_TimKiem_HoaDonBan", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SoHDB", SoHDB ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaHang", MaHang ?? (object)DBNull.Value);

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        dataBanHang.DataSource = dt;

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("Không tìm thấy kết quả phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        DataRow row = dt.Rows[0];

                        // Thông tin hóa đơn
                        txtSoHDB.Text = row["SOHDB"].ToString();
                        dateNgayBan.Value = Convert.ToDateTime(row["NGAYBAN"]);
                        txtTongTien.Text = row["TONGTIEN"].ToString();
                        txtChietKhau.Text = row["CHIETKHAU"].ToString();
                        txtThanhToan.Text = row["THANHTOAN"].ToString();
                        // Thông tin nhân viên và khách hàng
                        txtMaNV.Text = row["MANV"].ToString();
                        txtMaKH.Text = row["MAKH"].ToString();

                        // Chi tiết

                        txtMaHH.Text = row["MAHANG"].ToString();
                        txtSLBan.Text = row["SL_BAN"].ToString();
                        txtDGBan.Text = row["DONGIA_BAN"].ToString();
                        txtThanhTien.Text = row["THANHTIEN"].ToString(); 
                        txtThanhTien.Text = row["THANHTIEN"].ToString();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear(); // Xóa nội dung tìm kiếm
            ClearInputFields(); // Xóa trắng các trường nhập liệu

            // Tải lại tất cả dữ liệu từ cơ sở dữ liệu (không lọc gì cả)
            LoadData();

            MessageBox.Show("Đã làm mới toàn bộ dữ liệu và các trường nhập!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void ClearInputFields()
        {
            // Kiểm tra control có null không trước khi thao tác
            if (txtSoHDB != null) txtSoHDB.Clear(); else Console.WriteLine("txtSoHDN is null");
            if (dateNgayBan != null) dateNgayBan.Value = DateTime.Now; else Console.WriteLine("dateNgayBan is null");
            if (txtTongTien != null) txtTongTien.Clear(); else Console.WriteLine("txtTongTien is null");
            if (txtChietKhau != null) txtChietKhau.Clear(); else Console.WriteLine("txtChietKhau is null");
            if (txtThanhToan != null) txtThanhToan.Clear(); else Console.WriteLine("txtThanhToan is null");
            if (txtMaNV != null) txtMaNV.Clear(); else Console.WriteLine("txtMaNV is null");
            if (txtMaKH != null) txtMaKH.Clear(); else Console.WriteLine("txtMaKH is null");

            if (txtMaHH != null) txtMaHH.Clear(); else Console.WriteLine("txtMaHH is null");
            if (txtSLBan != null) txtSLBan.Clear(); else Console.WriteLine("txtSLBan is null");
            if (txtDGBan != null) txtDGBan.Clear(); else Console.WriteLine("txtDGBan is null");
            if (txtThanhTien != null) txtThanhTien.Clear(); else Console.WriteLine("txtThanhTien is null");
            if (txtTimKiem != null) txtTimKiem.Clear(); else Console.WriteLine("txtTimKiem is null");

            if (txtSoHDB != null) txtSoHDB.Focus();

        }

        private void dataBanHang_Click(object sender, DataGridViewCellEventArgs e)
        {

            // === Thông tin hóa đơn nhập ===
            txtSoHDB.Text = dataBanHang.Rows[e.RowIndex].Cells["SOHDB"].Value.ToString();
            dateNgayBan.Value = Convert.ToDateTime(dataBanHang.Rows[e.RowIndex].Cells["NGAYBAN"].Value);
            txtTongTien.Text = dataBanHang.Rows[e.RowIndex].Cells["TONGTIEN"].Value.ToString();
            txtChietKhau.Text = dataBanHang.Rows[e.RowIndex].Cells["CHIETKHAU"].Value.ToString();
            txtThanhToan.Text = dataBanHang.Rows[e.RowIndex].Cells["THANHTOAN"].Value.ToString();
            txtMaNV.Text = dataBanHang.Rows[e.RowIndex].Cells["MANV"].Value.ToString();
            txtMaKH.Text = dataBanHang.Rows[e.RowIndex].Cells["MAKH"].Value.ToString();


            // === Thông tin hóa đơn nhập ===
            txtMaHH.Text = dataBanHang.Rows[e.RowIndex].Cells["MAHANG"].Value.ToString();
            txtSLBan.Text = dataBanHang.Rows[e.RowIndex].Cells["SL_BAN"].Value.ToString();
            txtDGBan.Text = dataBanHang.Rows[e.RowIndex].Cells["DONGIA_BAN"].Value.ToString();
            txtThanhTien.Text = dataBanHang.Rows[e.RowIndex].Cells["THANHTIEN"].Value.ToString();


        }

        private static object GetRowIndex(DataGridViewCellEventArgs e)
        {
            return e.RowIndex;
        }

        private void btnSuaHDB_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                if (txtSoHDB.Text == "")
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn sửa hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Chuyển đổi an toàn dữ liệu đầu vào
                        

                        decimal.TryParse(txtDGBan.Text, out decimal DonGiaBan);
                        decimal.TryParse(txtThanhTien.Text, out decimal thanhTien);
                        decimal.TryParse(txtTongTien.Text, out decimal tongTien);
                        decimal.TryParse(txtChietKhau.Text, out decimal chietKhau);
                        decimal.TryParse(txtThanhToan.Text, out decimal thanhToan);
                        int.TryParse(txtSLBan.Text, out int slBan);

                        con.Open();

                        // Cập nhật bảng HOADON_NHAP
                        string query1 = @"UPDATE HOADON_BAN SET 
                                        NgayBan = @NgayBan,
                                        TongTien = @TongTien,
                                        ChietKhau = @ChietKhau,
                                        ThanhToan = @ThanhToan,
                                        MaNV = @MaNV,
                                        MaKH = @MaKH
                                    WHERE SoHDB = @SoHDB";


                        SqlCommand cmd1 = new SqlCommand(query1, con);
                        cmd1.Parameters.AddWithValue("@SoHDB", txtSoHDB.Text);
                        cmd1.Parameters.AddWithValue("@NgayBan", dateNgayBan.Value);
                        cmd1.Parameters.AddWithValue("@TongTien", tongTien);
                        cmd1.Parameters.AddWithValue("@ChietKhau", chietKhau);
                        cmd1.Parameters.AddWithValue("@ThanhToan", thanhToan);
                        cmd1.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                        cmd1.Parameters.AddWithValue("@MaKH", txtMaKH.Text);

                        cmd1.ExecuteNonQuery();

                        // Cập nhật bảng CHITIET_NHAP
                        string query2 = @"UPDATE CHITIET_BAN SET 
                                        SL_BAN = @SL_Ban,
                                        DONGIA_BAN = @DonGia_Ban,
                                        THANHTIEN = @ThanhTien
                                    WHERE SoHDB = @SoHDB AND MaHang = @MaHang";

                        SqlCommand cmd2 = new SqlCommand(query2, con);
                        cmd2.Parameters.AddWithValue("@MaHang", txtMaHH.Text);
                        cmd2.Parameters.AddWithValue("@SoHDB", txtSoHDB.Text);
                        cmd2.Parameters.AddWithValue("@SL_Ban", slBan);
                        cmd2.Parameters.AddWithValue("@DonGia_Ban", DonGiaBan);
                        cmd2.Parameters.AddWithValue("@ThanhTien", thanhTien);

                        cmd2.ExecuteNonQuery();

                        MessageBox.Show("Sửa hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HienThiDSHoaDon(); // Cập nhật lại danh sách hiển thị
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        con.Close(); // sửa đúng tên kết nối
                    }
                }
            }
        }
        private void HienThiDSHoaDon()
        {
            // Tải lại dữ liệu từ cơ sở dữ liệu
            LoadData();
            ClearInputFields(); // Xóa trắng các trường nhập liệu
        }

        private void btnXoaHDB_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(sCon))
            {
                if (txtSoHDB.Text == "")
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa hóa đơn này không? Dữ liệu liên quan sẽ bị xóa!", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        con.Open();

                        // Xóa chi tiết hóa đơn
                        string query1 = "DELETE FROM CHITIET_BAN WHERE SoHDB = @SoHDB";
                        SqlCommand cmd1 = new SqlCommand(query1, con);
                        cmd1.Parameters.AddWithValue("@SoHDB", txtSoHDB.Text);
                        cmd1.ExecuteNonQuery();

                        // Xóa hóa đơn chính
                        string query2 = "DELETE FROM HOADON_BAN WHERE SoHDB = @SoHDB";
                        SqlCommand cmd2 = new SqlCommand(query2, con);
                        cmd2.Parameters.AddWithValue("@SoHDB", txtSoHDB.Text);
                        cmd2.ExecuteNonQuery();

                        MessageBox.Show("Xóa hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HienThiDSHoaDon();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }
    }
}
