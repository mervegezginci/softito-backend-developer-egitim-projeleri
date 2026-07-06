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
    public partial class Rapor : Form
    {
        public Rapor()
        {
            InitializeComponent();
        }

        softAracKiralamaEntities1 conn = new softAracKiralamaEntities1();

        // 1. Son 30 Günde Yapılan Kiralamalar
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime sinirTarihi = DateTime.Now.AddDays(-30);
            dataGridView1.DataSource = conn.Kiralamalars
                .Where(x => x.AlisTarihi >= sinirTarihi)
                .Select(x => new { x.Musteriler.AdSoyad, x.Araclar.Model, x.AlisTarihi, x.ToplamUcret })
                .ToList();
        }

        // 2. Son 1 Haftadaki Kiralamalar
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime sinirTarihi = DateTime.Now.AddDays(-7);
            dataGridView1.DataSource = conn.Kiralamalars
                .Where(x => x.AlisTarihi >= sinirTarihi)
                .Select(x => new { x.Musteriler.AdSoyad, x.Araclar.Model, x.AlisTarihi })
                .ToList();
        }

        // 3. Markaya Göre Araç Sayıları
        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = conn.Araclars
                .GroupBy(x => x.Markalar.MarkaAdi)
                .Select(g => new { Marka = g.Key, Adet = g.Count() })
                .ToList();
        }

        // 4. En Yüksek Ücretli 5 Kiralama
        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = conn.Kiralamalars
                .OrderByDescending(x => x.ToplamUcret)
                .Take(5)
                .Select(x => new { x.Musteriler.AdSoyad, x.ToplamUcret })
                .ToList();
        }

        // 5. Toplam Kiralama Sayısı (Tablo Görünümü)
        private void button5_Click(object sender, EventArgs e)
        {
            int sayi = conn.Kiralamalars.Count();
            dataGridView1.DataSource = new List<object> { new { Bilgi = "Toplam Kiralama Sayısı", Deger = sayi } };
        }

        // 6. Tüm Zamanların Toplam Geliri (Tablo Görünümü)
        private void button6_Click(object sender, EventArgs e)
        {
            decimal toplam = conn.Kiralamalars.Sum(x => x.ToplamUcret) ?? 0;
            dataGridView1.DataSource = new List<object> { new { Bilgi = "Toplam Elde Edilen Gelir", Deger = toplam.ToString("N2") + " TL" } };
        }

        // 7. Araçların Ortalama Günlük Ücreti (Tablo Görünümü)
        private void button7_Click(object sender, EventArgs e)
        {
            double ort = (double)(conn.Araclars.Average(x => x.GunlukUcret) ?? 0);
            dataGridView1.DataSource = new List<object> { new { Bilgi = "Ortalama Günlük Ücret", Deger = ort.ToString("N2") + " TL" } };
        }

        // 8. Sistemdeki Toplam Araç Sayısı
        private void button8_Click(object sender, EventArgs e)
        {
            int toplamArac = conn.Araclars.Count();

            dataGridView1.DataSource = new List<object>
            {
                new { Bilgi = "Sistemdeki Toplam Araç Sayısı", Deger = toplamArac }
            };
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Menu ana = new Menu();
            ana.Show();
            this.Hide();
        }



    }
}
