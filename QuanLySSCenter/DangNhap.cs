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
    public partial class DangNhap : Form
    {
        string sCon = @"Data Source=LAPTOP-0B1SRHV7;Initial Catalog=BTLNhom7;Integrated Security=True;TrustServerCertificate=True";
        public DangNhap()
        {
            InitializeComponent();
            InitUI();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.ControlBox = true;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
        }

        private void InitUI()
        {
            this.Text = "SS Center Login";
            this.Size = new Size(900, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Left Panel
            Panel leftPanel = new Panel()
            {
                BackColor = Color.FromArgb(135, 206, 250), // Light blue
                Dock = DockStyle.Left,
                Width = this.Width / 2
            };
            this.Controls.Add(leftPanel);

            Label loginLabel = new Label()
            {
                Text = "Đăng Nhập",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(120, 50)
            };
            leftPanel.Controls.Add(loginLabel);

            // Username field
            PictureBox userIcon = new PictureBox()
            {
                Image = SystemIcons.Information.ToBitmap(),
                Location = new Point(50, 130),
                Size = new Size(32, 32),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            leftPanel.Controls.Add(userIcon);

            TextBox txtUsername = new TextBox()
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(90, 130),
                Width = 250,
                ForeColor = Color.Gray,
                Text = "Tên đăng nhập"
            };
            leftPanel.Controls.Add(txtUsername);
            txtUsername.GotFocus += (s, e) => 
            { 
                if (txtUsername.Text == "Tên đăng nhập") 
                { txtUsername.Text = ""; txtUsername.ForeColor = Color.Black; } 
            };


            txtUsername.LostFocus += (s, e) =>
            {
                    if (string.IsNullOrWhiteSpace(txtUsername.Text))
                    { txtUsername.Text = "Tên đăng nhập"; txtUsername.ForeColor = Color.Gray; }
            };

            // Password field
            PictureBox lockIcon = new PictureBox()
            {
                Image = SystemIcons.Shield.ToBitmap(),
                Location = new Point(50, 190),
                Size = new Size(32, 32),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            leftPanel.Controls.Add(lockIcon);

            TextBox txtPassword = new TextBox()
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(90, 190),
                Width = 250,
                ForeColor = Color.Gray,
                Text = "Mật khẩu"
            };
            leftPanel.Controls.Add(txtPassword);

            txtPassword.GotFocus += (s, e) =>
            {
                if (txtPassword.Text == "Mật khẩu")
                { txtPassword.Text = ""; txtPassword.ForeColor = Color.Black; txtPassword.UseSystemPasswordChar = true; }
            };

            txtPassword.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                { txtPassword.Text = "Mật khẩu"; txtPassword.ForeColor = Color.Gray; txtPassword.UseSystemPasswordChar = false; }
            };

            // Login button
            Button btnLogin = new Button()
            {
                Text = "ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(100, 149, 237), // Cornflower Blue
                ForeColor = Color.White,
                Location = new Point(120, 260),
                Width = 150,
                Height = 40
            };
            btnLogin.Click += (s, e) =>
            {
                MessageBox.Show($"Chào {txtUsername.Text}!");
            };
            leftPanel.Controls.Add(btnLogin);

            // Right Panel
            Panel rightPanel = new Panel()
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(rightPanel);

            Label centerLabel = new Label()
            {
                Text = "SS Center",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 149, 237),
                AutoSize = true,
                Location = new Point(80, 50)
            };
            rightPanel.Controls.Add(centerLabel);

            PictureBox medicalImage = new PictureBox()
            {
                ImageLocation = "https://i.imgur.com/UPMNPEw.png", // Hình tương tự hình bạn đưa, có thể thay
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(30, 120),
                Size = new Size(300, 300)
            };
            rightPanel.Controls.Add(medicalImage);
        }


        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string user = txtTenDangNhap.Text.Trim();
            string pass = txtMatKhau.Text.Trim();

            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(user) || user == "Tên đăng nhập" || string.IsNullOrEmpty(pass) || pass == "Mật khẩu")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(sCon))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM NGUOIDUNG WHERE Ten_DN = @user AND MK = @pass";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@pass", pass);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Chuyển qua form TrangChu
                            this.Hide();
                            TrangChu frmTrangChu = new TrangChu();
                            frmTrangChu.FormClosed += (s2, e2) => this.Close(); // Đóng login khi TrangChu đóng
                            frmTrangChu.Show();
                        }
                        else
                        {
                            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối đến CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void DangNhap_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
