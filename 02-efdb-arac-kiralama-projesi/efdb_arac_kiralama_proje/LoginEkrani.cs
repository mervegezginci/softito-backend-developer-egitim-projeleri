using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace efdb_arac_kiralama_proje
{
    public partial class LoginEkrani : Form
    {
        public LoginEkrani()
        {
            InitializeComponent();
        }

        softAracKiralamaEntities1 conn = new softAracKiralamaEntities1();

        private bool GirisYap(string ad, string sifre)
        {
            var sorgu = from s in conn.Kullanicilars where s.KullaniciAdi == ad && s.Sifre == sifre select s;
            if (sorgu.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (GirisYap(textBox1.Text, textBox2.Text))
            {
                Menu menu = new Menu();
                menu.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("hatalı giriş, tekrar deneyiniz!");
                textBox1.Clear();
                textBox2.Clear();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioUyeDegilim.Checked)
            {
                groupBoxRegister.Visible = true;
            }
        }

        private void LoginEkrani_Load(object sender, EventArgs e)
        {
            groupBoxLogin.Visible = true;
            groupBoxRegister.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Kullanicilar k = new Kullanicilar();

            k.KullaniciAdi = textBox4.Text;
            k.Sifre = textBox3.Text;

            conn.Kullanicilars.Add(k);
            conn.SaveChanges();

            MessageBox.Show("Kayıt başarılı!");

            textBox4.Clear();
            textBox3.Clear();
        }
    }
}
