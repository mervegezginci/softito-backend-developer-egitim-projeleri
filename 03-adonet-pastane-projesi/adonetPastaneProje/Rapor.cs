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
    public partial class Rapor : Form
    {
        public Rapor()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softPastane;Integrated Security=true");


        private void müşterilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Musteriler frm = new Musteriler();
            frm.Show();
            this.Close();
        }

        private void siparişlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Siparisler frm = new Siparisler();
            frm.Show();
            this.Close();
        }

        private void ürünlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Urunler frm = new Urunler();
            frm.Show();
            this.Close();
        }

        private void raporlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rapor frm = new Rapor();
            frm.Show();
            this.Close();
        }

        public void RaporGetir(string sorgu)
        {
            conn.Open();
            SqlDataAdapter dr = new SqlDataAdapter(sorgu, conn);
            DataSet dt = new DataSet();
            dr.Fill(dt);
            dataGridView1.DataSource = dt.Tables[0];
            conn.Close();
        }

        // toplam urun sayısı
        private void button1_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT COUNT(*) AS [Toplam Ürün Sayısı] FROM Urun");
        }

        // toplam satış geliri
        private void button7_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT SUM(ToplamFiyat) AS [Toplam Ciro] FROM Siparis");
        }

        // son eklenen 5 müşteri
        private void button8_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT TOP 5 * FROM Musteriler ORDER BY musteriNo DESC");
        }

        // stoktaki toplam adet
        private void button9_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT SUM(Stok) AS [Toplam Stok] FROM Urun");
        }


        // pahalı ürünler
        private void button10_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT * FROM Urun WHERE Fiyat > 50");
        }

        //toplam sipariş sayısı
        private void button6_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT COUNT(*) AS [Toplam Sipariş Adedi] FROM Siparis");
        }

        // toplam musteri sayısı 
        private void button5_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT COUNT(*) AS [Toplam Müşteri] FROM Musteriler");
        }

        // en düsük fiyatlı ürün
        private void button4_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT MIN(Fiyat) AS [En Ucuz Ürün] FROM Urun");
        }

        //ortalama urun fiyatı
        private void button3_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT AVG(Fiyat) AS [Ortalama Fiyat] FROM Urun");
        }
        
        // en yüksek fiyatlı ürün
        private void button2_Click(object sender, EventArgs e)
        {
            RaporGetir("SELECT MAX(Fiyat) AS [En Pahalı Ürün] FROM Urun");
        }
    }
}
