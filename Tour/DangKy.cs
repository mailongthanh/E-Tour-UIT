﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tour
{
    public partial class DangKy : Form
    {
        CustomerDAL cusDAL = new CustomerDAL();
        ticketDAL tkDAL = new ticketDAL();
        ReservationDAL resDAL = new ReservationDAL();
        DataConnection dc = new DataConnection();

        String NameOfRouteType;
        System.Text.RegularExpressions.Regex rEMail = new System.Text.RegularExpressions.Regex(@"^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$");
        public DangKy()
        {
            InitializeComponent();
            tbCMND.ForeColor = Color.LightGray;
            tbCMND.Text = " Please Enter Identification Card Number";
        }

        private void DangKy_Load(object sender, EventArgs e)
        {
            LoadCombobox(cbDes);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime itime = DateTime.Now;
            lbTime.Text = itime.ToLongTimeString();

            DateTime idate = DateTime.Now;
            lbDate.Text = idate.ToLongDateString();
        }

        private void rdIn_CheckedChanged(object sender, EventArgs e)
        {
            if (rdDomestic.Checked == true)
            {
                tbCMND.Text = "Please Enter Identification Card Number";
                tbCMND.ForeColor = Color.LightGray;
                PnFore.Visible = false;
            }
            else
            {
                PnFore.Visible = true;
                tbCMND.Text = "Please Enter Passport Number";
                tbCMND.ForeColor = Color.LightGray;
            }
        }

        private void tbCMND_Enter(object sender, EventArgs e)
        {
            if (rdDomestic.Checked == true && tbCMND.Text == "Please Enter Identification Card Number")
            {
                tbCMND.Text = "";
                tbCMND.ForeColor = Color.Black;
            }
            else tbCMND.Text = ""; tbCMND.ForeColor = Color.Black;
        }

        private void tbCMND_Leave(object sender, EventArgs e)
        {
            if(rdDomestic.Checked == true && tbCMND.Text == "")
            {
                tbCMND.Text = "Please Enter Identification Card Number";
                tbCMND.ForeColor = Color.LightGray;
            }
            else if(RdForeign.Checked == true && tbCMND.Text=="")
            {
                tbCMND.Text = "Please Enter Passport Number";
                tbCMND.ForeColor = Color.LightGray;
            }
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            DialogResult msExit;

            msExit = MessageBox.Show("Confirm if you want to exit ?", "Travel Management System", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(msExit ==DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void reset()
        {
            tbName.Clear();
            tbSurname.Clear();
            tbEmail.Clear();
            tbTelephone.Clear();
            tbAddress.Clear();
            tbCMND.Clear();
            tbVehicle.Clear();
            tbAmount.Clear();
            tbDate.Clear();
            tbPrice.Clear();
            tbDiscount.Clear();
            tbTotal.Clear();
            cbDes.Text = "None";
            RdMale.Checked = true;
            rdDomestic.Checked = true;
            rtbreservation.Clear();
            rtbTicket.Clear();
        }
        private void btReset_Click(object sender, EventArgs e)
        {
            reset();
        }

        public bool CheckData()
        {
            DateTime date = GetTime(cbDes.Text);
            DateTime current = DateTime.Now;
            TimeSpan Interval = date.Subtract(current);

            if (NameOfRouteType == "National" && Interval.Days < 1)
            {
                MessageBox.Show("Ticket purchase deadline exceeded", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            else if(NameOfRouteType =="International" && Interval.Days < 7)
            {
                MessageBox.Show("Ticket purchase deadline exceeded", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if(RdForeign.Checked == true && PassEXP.Value.Date <= date.Date)
            {
                MessageBox.Show("Passport expire error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            else if(RdForeign.Checked == true && VisaEXP.Value.Date <= date.Date)
            {
                MessageBox.Show("Visa expire error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (String.IsNullOrEmpty(tbName.Text)|| String.IsNullOrEmpty(tbSurname.Text)|| String.IsNullOrEmpty(tbAddress.Text)||String.IsNullOrEmpty(tbCMND.Text))
            {
                MessageBox.Show("Please fill in all the information", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            string ID = "", MaVe = "", MaPhieu = "";
            tblTicket tk = new tblTicket();
            Customer cus = new Customer();
            Reservation res = new Reservation();
            String gender, typeCus;
            bool checkCus, checkRes;

            if (RdMale.Checked == true)
            {
                gender = "Male";
            }
            else gender = "Female";

            String Kind, Tourist;
            if (RdForeign.Checked == true)
            {
                Kind = "Passport";
                Tourist = "Foreign";
                typeCus = "CUS002";
            }
            else
            {
                Kind = "CMND";
                Tourist = "Domestic";
                typeCus = "CUS001";
            };

            if (CheckData())
            {
                cus.HoTen = tbSurname.Text + " " + tbName.Text;
                cus.DiaChi = tbAddress.Text;
                cus.SDT = tbTelephone.Text;
                cus.MaLoaiKhach = typeCus;
                cus.GioiTinh = gender;
                cus.CMND_Passport = tbCMND.Text;
                cus.HanPassport = PassEXP.Value;
                cus.HanVisa = VisaEXP.Value;

                if (RdForeign.Checked == true)
                {
                    checkCus = cusDAL.InsertForeign(cus, ref ID);
                }
                else checkCus = cusDAL.InsertDomestic(cus, ref ID);

                res.MaChuyen = cbDes.Text;
                checkRes = resDAL.Insert(res, ref MaPhieu);

                tk.MaDuKhach = Int32.Parse(ID);
                tk.MaPhieu = Int32.Parse(MaPhieu);
                tk.GiaVe = Int32.Parse(tbTotal.Text);


                if (checkCus && tkDAL.Insert(tk, ref MaVe) && checkRes)
                {
                    MessageBox.Show("Successfull add", "information", MessageBoxButtons.OK);
                }
                else MessageBox.Show("Fail add", "information", MessageBoxButtons.OK);

                Amount(cbDes.Text);

                rtbreservation.AppendText("\t       RESERVATION TICKET:\n\n"
                + "ID:\t\t\t\t" + MaPhieu
                + "\n----------------------------------------------------------------------------------------"
                + "\nTourID\t\t\t\t" + cbDes.Text
                + "\nName\t\t\t\t" + tbName.Text
                + "\nSurName\t\t\t\t" + tbSurname.Text
                + "\nGender\t\t\t\t" + gender
                + "\nPhoneNumber\t\t\t" + tbTelephone.Text
                + "\nTourist\t\t\t\t" + Tourist
                + "\n" + Kind + "\t\t\t\t" + tbCMND.Text
                + "\n----------------------------------------------------------------------------------------"
                + "\n" + lbDate.Text + "\t\t\t" + lbTime.Text
                + "\n----------------------------------------------------------------------------------------"
                + "\n\n\t       Thanks for Using \n\t       Travel Management System"
                    ) ;

                rtbTicket.AppendText("TICKET\n\n"
                    + "ID:\t\t\t\t" + MaVe
                    + "\n----------------------------------------------------------------------------------------"
                    + "\nReservation ID\t\t\t" + MaPhieu
                    + "\nName\t\t\t\t" + tbName.Text
                    + "\nSurName\t\t\t\t" + tbSurname.Text 
                    + "\nCost\t\t\t\t"+ tbTotal.Text + " VND"
                    + "\n----------------------------------------------------------------------------------------"
                    + "\n" + lbDate.Text + "\t\t\t" + lbTime.Text
                    + "\n----------------------------------------------------------------------------------------"
                    + "\n\n\t       Thanks for Using \n\t       Travel Management System"
                    );
                Clear();
            }
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            CSDLPhieuDatCho csdl = new CSDLPhieuDatCho();
            csdl.StartPosition = FormStartPosition.CenterParent;
            csdl.ShowDialog();
        }

        private void LoadCombobox(ComboBox cb)
        {
            SqlConnection con = dc.getConnect();
            con.Open();
            SqlCommand cmd = new SqlCommand("Select MaChuyen From ChuyenDuLich", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteNonQuery();
            con.Close();
            cb.DataSource = ds.Tables[0];
            cb.DisplayMember = "MaChuyen";
        }

        private void cbDes_SelectedValueChanged(object sender, EventArgs e)
        {
            string sql = "SELECT ChuyenDuLich.PhuongTien, ThoiGianKhoiHanh, TenLoaiTuyen, ThoiGianToChuc, GiaVe FROM ((ChuyenDuLich INNER JOIN Tuyen ON ChuyenDuLich.MaTuyen = Tuyen.MaTuyen) INNER JOIN LoaiTuyen ON Tuyen.MaLoaiTuyen = LoaiTuyen.MaLoaiTuyen)  WHERE MaChuyen = @MaChuyen";
            SqlConnection con = dc.getConnect();
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("@MaChuyen", cbDes.Text);
            SqlDataReader da = cmd.ExecuteReader();
            while (da.Read())
            {
                tbVehicle.Text = da.GetValue(0).ToString();
                tbDate.Text = da.GetValue(1).ToString();
                NameOfRouteType = da.GetValue(2).ToString();
                tbDuration.Text = da.GetValue(3).ToString();
                tbPrice.Text = da.GetValue(4).ToString();
            }
            Amount(cbDes.Text);     
        }

        private void backtotourbtn_Click(object sender, EventArgs e)
        {
            Tour t = new Tour();
            this.Hide();
            t.ShowDialog();
        }

        private void Amount(String MaChuyen)
        {
            int result = 0;
            result = GetMax(MaChuyen) - GetCount(MaChuyen);
            tbAmount.Text = result.ToString(); 
        }

        private int GetMax(string MaChuyen)
        {
            int max = 0;
            string sql = "SELECT SoLuongVeMax FROM ChuyenDuLich WHERE MaChuyen = @MaChuyen";
            SqlConnection con = dc.getConnect();
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("@MaChuyen", MaChuyen);
            SqlDataReader da = cmd.ExecuteReader();
            while (da.Read())
            {
                max = da.GetInt32(0);

            }
            return max;
        }
        private int GetCount(string MaChuyen)
        {
            int  count = 0;
            string sql = "SELECT COUNT(*) FROM PhieuDatCho WHERE MaChuyen = @MaChuyen";
            SqlConnection con = dc.getConnect();
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("@MaChuyen", MaChuyen);
            SqlDataReader dat = cmd.ExecuteReader();
            while (dat.Read())
            {
                count = dat.GetInt32(0);

            }
            return count;
        }
        void Clear()
        {
            tbName.Text = tbSurname.Text = tbAddress.Text = tbTelephone.Text = tbEmail.Text = tbCMND.Text = "";
        }
        private void tbTelephone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
               (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void tbEmail_Validating(object sender, CancelEventArgs e)
        {
            if (tbEmail.Text.Length > 0)
            {
                if (!rEMail.IsMatch(tbEmail.Text))
                {
                    MessageBox.Show("Invalidate Email", "Error");
                    tbEmail.SelectAll();
                    btCreate.Enabled = false;
                }
                else
                {
                    btCreate.Enabled = true;
                }
            }
        }

        private void tbPrice_TextChanged(object sender, EventArgs e)
        {
            DateTime current = DateTime.Now;
            DateTime date = GetTime(cbDes.Text);
            TimeSpan interval = date - current;
            if (interval.Days >= 3 && NameOfRouteType == "National")
            {
                tbDiscount.Text = ((Int32.Parse(tbPrice.Text) * 20) / 100).ToString();
            }
            else if (interval.Days >= 10 && NameOfRouteType == "International")
            {
                tbDiscount.Text = ((Int32.Parse(tbPrice.Text) * 20) / 100).ToString();
            }
            else tbDiscount.Text = "0";
            if(String.IsNullOrEmpty(tbPrice.Text))
            { 
                tbDiscount.Text = tbTotal.Text = " "; 
            }
            else
            {
                tbTotal.Text = (Int32.Parse(tbPrice.Text) - Int32.Parse(tbDiscount.Text)).ToString();
            }
        }

        public DateTime GetTime(string MaChuyen)
        {
            DateTime date = new DateTime();
            string sql = "SELECT ThoiGianKhoiHanh FROM ChuyenDuLich WHERE MaChuyen = @MaChuyen";
            SqlConnection con = dc.getConnect();
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("@MaChuyen", MaChuyen);
            SqlDataReader da = cmd.ExecuteReader();
            while (da.Read())
            {
                date = da.GetDateTime(0);
            }
            return date;
        }
        private void tbSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Space)
            {
                return;
            }
            e.Handled = true;
        }

        private void tbCMND_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void tbTelephone_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void btngotocsdl_Click(object sender, EventArgs e)
        {
            CSDLPhieuDatCho t = new CSDLPhieuDatCho();
            this.Hide();
            t.ShowDialog();
            this.Show();
        }

        private void tbName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tbSurname_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
