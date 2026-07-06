using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace efdb_arac_kiralama_proje
{
    public partial class KiralamaIslemleri : Form
    {
        public KiralamaIslemleri()
        {
            InitializeComponent();
        }

        softAracKiralamaEntities1 conn = new softAracKiralamaEntities1();

        decimal gunlukUcret = 0;

        // LİSTELE
        public void Listele()
        {
            dataGridView1.DataSource = conn.Kiralamalars
                .Select(x => new
                {
                    x.KiralamaNo,
                    Musteri = x.Musteriler.AdSoyad,
                    Arac = x.Araclar.Model,
                    x.AlisTarihi,
                    x.TeslimTarihi,
                    x.ToplamUcret
                })
                .ToList();
        }

        // TOPLAM HESAPLA
        public void Hesapla()
        {
            TimeSpan fark = dateTimePicker2.Value.Date - dateTimePicker1.Value.Date;

            int gun = fark.Days;

            if (gun <= 0)
                gun = 1;

            decimal toplam = gun * gunlukUcret;

            label6.Text = toplam.ToString("0.00") + " TL";

            label6.Visible = true;
        }

        // fiş göster
        public string FisGoster(Kiralamalar veri)
        {
            var arac = conn.Araclars.FirstOrDefault(x => x.AracNo == veri.AracNo);
            var musteri = conn.Musterilers.FirstOrDefault(x => x.MusteriNo == veri.MusteriNo);

            int gun = (veri.TeslimTarihi.Value.Date - veri.AlisTarihi.Value.Date).Days;
            if (gun <= 0) gun = 1;

            string fis =
                "=== ARAÇ KİRALAMA FİŞİ ===\n\n" +
                "Ad Soyad : " + musteri.AdSoyad + "\n" +
                "Araç     : " + arac.Markalar.MarkaAdi + "-" + arac.Model + "\n" +
                "Tarih    : " + veri.AlisTarihi?.ToString("dd.MM.yyyy") +
                            " - " + veri.TeslimTarihi?.ToString("dd.MM.yyyy") + "\n" +
                "Ücret    : " + veri.ToplamUcret?.ToString("0.00") + " TL\n" +
                "Gün      : " + gun + " Gün\n\n" +
                "==========================";

            return fis;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Menu ana = new Menu();
            ana.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Kiralamalar kiralama = new Kiralamalar();

            kiralama.MusteriNo = Convert.ToInt32(comboBox1.SelectedValue);
            kiralama.AracNo = Convert.ToInt32(comboBox2.SelectedValue);

            kiralama.AlisTarihi = dateTimePicker1.Value;
            kiralama.TeslimTarihi = dateTimePicker2.Value;

            kiralama.ToplamUcret = Convert.ToDecimal(label6.Text.Replace("TL", "").Trim());

            conn.Kiralamalars.Add(kiralama);
            conn.SaveChanges();

            Listele();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int kiralamaNo = Convert.ToInt32(comboBox1.Tag);

            var guncelle = conn.Kiralamalars
                .FirstOrDefault(x => x.KiralamaNo == kiralamaNo);

            if (guncelle != null)
            {
                guncelle.MusteriNo = Convert.ToInt32(comboBox1.SelectedValue);
                guncelle.AracNo = Convert.ToInt32(comboBox2.SelectedValue);

                guncelle.AlisTarihi = dateTimePicker1.Value;
                guncelle.TeslimTarihi = dateTimePicker2.Value;

                string temizFiyat = label6.Text.Split('-')[0].Replace("TL", "").Trim();

                guncelle.ToplamUcret = Convert.ToDecimal(temizFiyat);

                conn.SaveChanges();
                Listele();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int kiralamaNo = Convert.ToInt32(comboBox1.Tag);

            var sil = conn.Kiralamalars
                .FirstOrDefault(x => x.KiralamaNo == kiralamaNo);

            if (sil != null)
            {
                conn.Kiralamalars.Remove(sil);

                conn.SaveChanges();
                Listele();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = conn.Kiralamalars.Where(x => x.Musteriler.AdSoyad.Contains(textBox5.Text))
                .Select(x => new
                {
                    x.KiralamaNo,
                    Musteri = x.Musteriler.AdSoyad,
                    Arac = x.Araclar.Model,
                    x.AlisTarihi,
                    x.TeslimTarihi,
                    x.ToplamUcret
                })
                .ToList();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            int kiralamaNo = Convert.ToInt32(row.Cells["KiralamaNo"].Value);

            var veri = conn.Kiralamalars
                .FirstOrDefault(x => x.KiralamaNo == kiralamaNo);

            if (veri != null)
            {
                comboBox1.Tag = veri.KiralamaNo;

                comboBox1.SelectedValue = veri.MusteriNo;
                comboBox2.SelectedValue = veri.AracNo;

                dateTimePicker1.Value = Convert.ToDateTime(veri.AlisTarihi);
                dateTimePicker2.Value = Convert.ToDateTime(veri.TeslimTarihi);

                // 👇 BURASI DOĞRU YER
                richTextBox1.Visible = true;
                richTextBox1.Text = FisGoster(veri);

                TimeSpan gun = veri.TeslimTarihi.Value.Date - veri.AlisTarihi.Value.Date;

                int toplamGun = gun.Days;
                if (toplamGun <= 0) toplamGun = 1;

                decimal ucret = veri.ToplamUcret ?? 0;

                label6.Text = ucret.ToString("0.00") + " TL";
                label6.Visible = true;
                label11.Visible = true;
                panel1.Visible = true;
            }

            var arac = conn.Araclars.FirstOrDefault(x => x.AracNo == veri.AracNo);

            if (arac != null && !string.IsNullOrEmpty(arac.ResimYolu))
            {
                if (System.IO.File.Exists(arac.ResimYolu))
                {
                    pictureBox1.Image = Image.FromFile(arac.ResimYolu);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }

        }

        private void KiralamaIslemleri_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
            richTextBox1.Visible = false;
            label6.Visible = false;
            label11.Visible = false;

            // temiz başlangıç
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;

            pictureBox1.Image = null;

            richTextBox1.Clear();

            var musteriler = conn.Musterilers.ToList();
            musteriler.Insert(0, new Musteriler
            {
                MusteriNo = 0,
                AdSoyad = "Müşteri seçiniz"
            });

            comboBox1.DataSource = musteriler;
            comboBox1.DisplayMember = "AdSoyad";
            comboBox1.ValueMember = "MusteriNo";


            var araclar = conn.Araclars.ToList();
            araclar.Insert(0, new Araclar
            {
                AracNo = 0,
                Model = "Araç seçiniz"
            });

            comboBox2.DataSource = araclar;
            comboBox2.DisplayMember = "Model";
            comboBox2.ValueMember = "AracNo";

            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null)
                return;

            int aracNo;

            if (!int.TryParse(comboBox2.SelectedValue.ToString(), out aracNo))
                return;

            var arac = conn.Araclars
                .FirstOrDefault(x => x.AracNo == aracNo);

            if (arac != null)
            {
                gunlukUcret = Convert.ToDecimal(arac.GunlukUcret);

                Hesapla();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Hesapla();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            Hesapla();
        }

       
    }
}
