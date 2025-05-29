using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TP_BVSK;

namespace QuanLySSCenter
{
    public partial class TrangChu : Form
    {
        public TrangChu()
        {
            InitializeComponent();
        }

        private void btnTongQuan_Click(object sender, EventArgs e)
        {

        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            BaoCao_CacMuc baoCao = new BaoCao_CacMuc();
            //baoCao.MdiParent = this;
            baoCao.Show();
        }

        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            frmQuanlyNhaphang nhapHang = new frmQuanlyNhaphang();
            nhapHang.Show();
        }

        private void btnBanHang_Click(object sender, EventArgs e)
        {
            frmQuanlyBanhang banHang = new frmQuanlyBanhang();
            banHang.Show();
        }

        private void btnCongNo_Click(object sender, EventArgs e)
        {
            CongNo congNo = new CongNo();
            congNo.Show();
        }

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            CongNo congNo = new CongNo();
            congNo.Show();
        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            KhachHang khach = new KhachHang();
            khach.Show();
        }

        private void btnNhaCungCap_Click(object sender, EventArgs e)
        {
            frmNhaCungCap ncc = new frmNhaCungCap();
            ncc.Show();
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            NhanVien nv = new NhanVien();
            nv.Show();
        }
    }
}
