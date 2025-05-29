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
    public partial class BaoCao_CacMuc : Form
    {
        public BaoCao_CacMuc()
        {
            InitializeComponent();
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btbNhanVien_Click(object sender, EventArgs e)
        {
            frmModul4 bCaoNV = new frmModul4();
            bCaoNV.ShowDialog();
        }

        private void btnNhaCC_Click(object sender, EventArgs e)
        {
            frmModul3 bCaoNCC = new frmModul3();
            bCaoNCC.ShowDialog();
        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            module14 ploaiKH = new module14();
            ploaiKH.ShowDialog();
        }

        private void btnHangHoa_Click(object sender, EventArgs e)
        {
            module13 bCaoHang = new module13();
            bCaoHang.ShowDialog();
        }
    }
}
