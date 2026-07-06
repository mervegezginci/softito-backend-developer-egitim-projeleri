using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace adonetPastaneProje
{
    public partial class Login_Register : Form
    {
        public Login_Register()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softPastane;Integrated Security=true");

        // giriş yap butonu
        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand komut = new SqlCommand("select * from Kullanici where KullaniciAdi=@p1 and Sifre=@p2", conn);

            komut.Parameters.AddWithValue("@p1", textBox1.Text);
            komut.Parameters.AddWithValue("@p2", textBox2.Text);

            SqlDataReader rd = komut.ExecuteReader();  // okuma işlemi

            if (rd.Read())
            {
                string kullaniciAdSoyad = rd["AdSoyad"].ToString();
                MessageBox.Show("Giriş başarılı! Hoş geldiniz, " + kullaniciAdSoyad);

                Urunler frm = new Urunler();
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre");
                textBox1.Clear();
                textBox2.Text = "";
            }

            conn.Close();
        }


        // üye değilim radiobutton
        private void radioUyeDegilim_CheckedChanged(object sender, EventArgs e)
        {
            if (radioUyeDegilim.Checked)
            {
                groupBoxRegister.Visible = true;
            }
        }

        // kayıt ol butonu
        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();

            SqlCommand komut = new SqlCommand("insert into Kullanici (AdSoyad,KullaniciAdi,Sifre,Telefon) values (@p1,@p2,@p3,@p4)", conn);

            komut.Parameters.AddWithValue("@p1", textBox5.Text);
            komut.Parameters.AddWithValue("@p2", textBox6.Text);
            komut.Parameters.AddWithValue("@p3", textBox7.Text);
            komut.Parameters.AddWithValue("@p4", textBox8.Text);

            komut.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Kaydınız başarıyla tamamlanmıştır! Şimdi giriş yapabilirsiniz.");

            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();


        }

        private void Login_Register_Load(object sender, EventArgs e)
        {
            groupBoxLogin.Visible = true;
            groupBoxRegister.Visible = false;
        }
    }
}
