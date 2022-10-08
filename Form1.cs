using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace demo_yatirim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        internal decimal toplamBakiye = 0;

        #region VERİ İŞLEMLERİ
        public class Veri
        {
            public string rate;
            public string lastprice;
            public string lastpricestr;
            public string hacim;
            public string hacimstr;
            public string min;
            public string minstr;
            public string max;
            public string maxstr;
            public string time;
            public string text;
            public string code;
            public Veri(string rate, string lastprice, string lastpricestr, string hacim, string hacimstr, string min, string minstr, string max, string maxstr, string time, string text, string code)
            {
                this.rate = rate;
                this.lastprice = lastprice;
                this.lastpricestr = lastpricestr;
                this.hacim = hacim;
                this.hacimstr = hacimstr;
                this.min = min;
                this.minstr = minstr;
                this.max = max;
                this.maxstr = maxstr;
                this.time = time;
                this.text = text;
                this.code = code;
            }
        }
        private void verileriJsonYaz()
        {
            try
            {
                var webClient = new WebClient();
                webClient.Headers.Add("authorization", "apikey 0Co33gAiFkdisDJFQjnLUL:1mJpECHiaVykJHyLgPEMfQ");
                webClient.Headers.Add("content-type", "application/json");
                string myTable = webClient.DownloadString("https://api.collectapi.com/economy/hisseSenedi?code=ACSEL");
                myTable = myTable.Replace("{\"success\":true,\"result\":", "");
                myTable = myTable.Substring(0, myTable.Length - 1);
                var jsonTable = JsonConvert.DeserializeObject(myTable);
                if (File.Exists("veriler.json"))
                {
                    FileStream fs = new FileStream("veriler.json", FileMode.Truncate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(jsonTable);
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    File.Create("veriler.json").Close();
                    if (File.Exists("veriler.json"))
                    {
                        FileStream fs = new FileStream("veriler.json", FileMode.Truncate, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(jsonTable);
                        sw.Close();
                        fs.Close();
                    }
                    else
                    {
                        MessageBox.Show("Yenileme sırasında hata oluştu");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Üzgünüz yenileme başarısız oldu!\nTekrar deneyebilirsin :-|");
                jsonVerileriOku();
            }
        }
        private void jsonVerileriOku()
        {
            FileStream fs = new FileStream("veriler.json", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            try
            {
                string jsonData = "";

                while (!sr.EndOfStream)
                {
                    jsonData += sr.ReadLine();
                }

                var list = JsonConvert.DeserializeObject<List<Veri>>(jsonData);
                foreach (var hisse in list)
                {
                    ListViewItem lvi = new ListViewItem(hisse.code.Replace("https:", ""));
                    lvi.SubItems.Add(hisse.lastprice);
                    lvi.SubItems.Add(hisse.rate);
                    if (Convert.ToDouble(hisse.rate) > 0)
                    {
                        lvi.UseItemStyleForSubItems = false;
                        lvi.SubItems[2].BackColor = Color.FromArgb(127, 183, 126);
                    }
                    else if (Convert.ToDouble(hisse.rate) == 0)
                    {
                        lvi.UseItemStyleForSubItems = false;
                        lvi.SubItems[2].BackColor = Color.FromArgb(211, 206, 223);
                    }
                    else
                    {
                        lvi.UseItemStyleForSubItems = false;
                        lvi.SubItems[2].BackColor = Color.FromArgb(255, 139, 139);
                    }
                    fiyatList.Items.Add(lvi);
                }
            }
            catch
            {

            }
            finally
            {
                sr.Close();
                fs.Close();
            }
        }
        #endregion

        #region HİSSE VE PORTFÖY İŞLEMLERİ
        private Veri hisseBilgileri(string arananHisse)
        {
            FileStream fs = new FileStream("veriler.json", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string jsonData = "";

            while (!sr.EndOfStream)
            {
                jsonData += sr.ReadLine();
            }

            var list = JsonConvert.DeserializeObject<List<Veri>>(jsonData);
            foreach (var hisse in list)
            {
                if (arananHisse == hisse.code.Replace("https:", ""))
                {
                    return hisse;
                }
            }
            Veri veri = new Veri("", "", "", "", "", "", "", "", "", "", "", "");
            return veri;
            sr.Close();
            fs.Close();
        }
        private void hisseBilgileriGetir()
        {
            verileriJsonYaz();
            jsonVerileriOku();
            refreshState.Visible = false;
            refreshList.Enabled = true;
        }
        private void hisseAdlariGetir()
        {
            FileStream fs = new FileStream("veriler.json", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string jsonData = "";

            while (!sr.EndOfStream)
            {
                jsonData += sr.ReadLine();
            }

            var list = JsonConvert.DeserializeObject<List<Veri>>(jsonData);
            foreach (var hisse in list)
            {
                //hisse listesi burada istenilen yere eklenebilir
            }
            sr.Close();
            fs.Close();

        }
        private double fiyatFormatla(string fiyat)
        {
            string deger = fiyat;
            string result = "";
            bool ind = false;
            bool ind2 = false;
            for (int i = 0; i < deger.Length; i++)
            {
                if (deger[i] == '.')
                {
                    ind = true;
                }
                else if (deger[i] == ',')
                {
                    ind2 = true;
                }
            }
            if (ind2)
            {
                result = deger;
            }
            else if (ind)
            {
                deger = deger.Replace(".", ",");
                result = deger;
            }
            else
            {
                result = fiyat + ",01";
            }
            double res = Convert.ToDouble(result);
            return res;
        }
        private void hisseAlTxt(string hisseAd, decimal hisseMaliyet, decimal hisseAdet)
        {
            if ((Convert.ToDecimal(hisseAdet) * Convert.ToDecimal(hisseMaliyet)) < Convert.ToDecimal(toplamBakiye))
            {
                Veri veri = hisseBilgileri(hisseAd);

                FileStream fs = new FileStream("portfoy.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                var toplamTutar = Convert.ToDouble(hisseAdet) * fiyatFormatla(veri.lastprice) * (-1);

                sw.Write(veri.code.Replace("https:", "") + "|" + hisseAdet.ToString() + "|" + hisseMaliyet.ToString() + "|" + toplamTutar.ToString() + "|" + DateTime.Now.ToString("MM/dd/yyyy") + "_");

                sw.Close();
                fs.Close();
                bakiyeDuzenleTxt(Convert.ToDouble(toplamTutar));
                portfoyGuncelle();
                MessageBox.Show("Alım başarıyla gerçekleştirildi.");
            }
            else
            {
                MessageBox.Show("Yetersiz Bakiye!!!");
            }
        }
        private void hisseSatTxt(string hisseAd, decimal hisseAdet)
        {
            Veri veri = hisseBilgileri(hisseAd);
            FileStream fs = new FileStream("portfoy.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            var toplamTutar = Convert.ToDouble(hisseAdet) * fiyatFormatla(veri.lastprice);
            sw.Write(veri.code.Replace("https:", "") + "|" + (hisseAdet * (-1)).ToString() + "|" + veri.lastprice + "|" + toplamTutar.ToString() + "|" + DateTime.Now.ToString("MM/dd/yyyy") + "_");


            sw.Close();
            fs.Close();
            bakiyeDuzenleTxt(Convert.ToDouble(toplamTutar));
            portfoyGuncelle();
        }
        private void hisseTamamınıSat()
        {
            if (portfoydeVarmi(txt_secilenHisse.Text))
            {
                FileStream fs = new FileStream("satilanlar.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                var veri = hisseBilgileri(txt_secilenHisse.Text);
                sw.Write(veri.code.Replace("https:", "") + "|" + veri.lastprice + "|" + maliyetBul(txt_secilenHisse.Text) + "_");
                sw.Close();
                fs.Close();
                MessageBox.Show(txt_secilenHisse.Text + " satış emri başarıyla gerçekleştirildi");
                portfoyGuncelle();
                // Şuan bu sistem seçilen hisseyi tamamen satıyor. Ama biz hepsini değil belli bir adetini silmek istiyoruz.
                // bunun için yeniden hisse alır gibi yapacağız ama hisse adetini '-' şeklinde gireceğiz.
            }
            else
            {
                MessageBox.Show("Portföyünüzde bu hisse yok !!!");
            }
        }
        private bool SatilanlarHisseKontrol(string hisseAd)
        {
            var deger = false;
            FileStream fs = new FileStream("satilanlar.txt", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            var icerik = "";
            while (!sr.EndOfStream)
            {
                icerik += sr.ReadLine();
            }
            var veri = icerik.Split('_');
            for (int i = 0; i < veri.Length - 1; i++)
            {
                if (veri[i].Split('|')[0] == hisseAd)
                {
                    deger = true;
                }
            }
            sr.Close();
            fs.Close();
            return deger;
        }
        private string maliyetBul(string hisseAd)
        {
            var deger = "";
            foreach (ListViewItem item in portfoyList.Items)
            {
                if (item.SubItems[0].Text == hisseAd)
                {
                    deger = item.SubItems[2].Text;
                }
            }
            return deger;
        }
        private void portfoySifirAdetliSil()
        {
            foreach (ListViewItem item in portfoyList.Items)
            {
                if (item.SubItems[1].Text == "0")
                {
                    item.Remove();
                }
            }
        }
        internal void portfoyGuncelle()
        {
            filterClean();
            portfoyList.Items.Clear();
            FileStream fs = new FileStream("portfoy.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string veri = "";
            while (!sr.EndOfStream)
            {
                veri += sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            var veri1 = veri.Split('_');

            for (int i = 0; i < veri1.Length - 1; i++)
            {
                var veri2 = veri1[i].Split('|');
                bool dahaOnceVarmi = false;
                int indeks = -1;
                for (int j = 0; j < portfoyList.Items.Count; j++)
                {
                    if (veri2[0] == portfoyList.Items[j].SubItems[0].Text)
                    {
                        indeks = j;
                        dahaOnceVarmi = true;
                    }
                }
                if (dahaOnceVarmi)
                {
                    portfoyList.Items[indeks].SubItems[1].Text = (Convert.ToInt32(veri2[1]) + Convert.ToInt32(portfoyList.Items[indeks].SubItems[1].Text)).ToString();
                    portfoyList.Items[indeks].SubItems[2].Text = ((Convert.ToDouble(veri2[2]) + Convert.ToDouble(portfoyList.Items[indeks].SubItems[2].Text)) / 2).ToString();
                    portfoyList.Items[indeks].SubItems[3].Text = veri2[4];
                }
                else
                {
                    ListViewItem lvi = new ListViewItem(veri2[0]);
                    lvi.SubItems.Add(veri2[1]);
                    lvi.SubItems.Add(veri2[2]);
                    lvi.SubItems.Add(veri2[4]);
                    foreach (ListViewItem item in fiyatList.Items)
                    {
                        if (item.SubItems[0].Text == veri2[0])
                        {
                            lvi.SubItems.Add(item.SubItems[1].Text);
                        }
                    }
                    portfoyList.Items.Add(lvi);
                }
            }
            portfoySifirAdetliSil();
        }
        private bool portfoydeVarmi(string hisseAd)
        {
            bool deger = false;
            foreach (ListViewItem item in portfoyList.Items)
            {
                if (item.SubItems[0].Text == hisseAd)
                {
                    deger = true;
                }
            }
            return deger;
        }
        private int portfoyMiktariOgren(string hisseAd)
        {
            int deger = 0;
            foreach (ListViewItem item in portfoyList.Items)
            {
                if (item.SubItems[0].Text == hisseAd)
                {
                    deger = Convert.ToInt32(item.SubItems[1].Text);
                }
            }
            if (deger <= 0)
            {
                deger = 0;
            }
            return deger;
        }

        #endregion

        #region BAKİYE İŞLEMLERİ
        internal void bakiyeGuncelleTxt()
        {
            FileStream fs = new FileStream("bakiye.txt", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string icerik = "";
            while (!sr.EndOfStream)
            {
                icerik += sr.ReadLine();
            }
            sr.Close();
            //icerik = icerik.Substring(0, icerik.Length - 1);

            if (icerik == "")
            {
                toplamBakiye = 0;
            }
            else
            {
                toplamBakiye = Convert.ToDecimal(icerik);
            }
            realMoney.Text = String.Format("{0:C2}", toplamBakiye);
            settings ayarlar = new settings();
            if (ayarlar.bakiyeSifirlaKontrol)
            {
                realMoney.Text = String.Format("{0:C0}", 1000000);
            }
        }
        internal void bakiyeDuzenleTxt(double degisim)
        {
            FileStream fs = new FileStream("bakiye.txt", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string icerik = "";
            while (!sr.EndOfStream)
            {
                icerik += sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            FileStream fs1 = new FileStream("bakiye.txt", FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1);
            var deger = (Convert.ToDouble(icerik) + degisim).ToString();
            deger = fiyatFormatla(deger.ToString()).ToString();
            sw.Write(deger);
            sw.Close();
            fs1.Close();
            bakiyeGuncelleTxt();
        }
        private void sonucGuncelle()
        {
            var sonucFiyat = Convert.ToDecimal(txt_miktar.Value) * Convert.ToDecimal(alim_fiyat.Text.Replace(".", ","));
            sonucLabel.Text =
                txt_secilenHisse.Text
                + "\n" +
                txt_miktar.Value.ToString()
                + "\n" +
                sonucFiyat.ToString();
        }
        #endregion

        #region FORM KONTROLLERİ VE İŞLEMLERİ
        private void Form1_Load(object sender, EventArgs e)
        {
            bakiyeGuncelleTxt();
            jsonVerileriOku();
            portfoyGuncelle();
        }
        private async void refreshList_Click(object sender, EventArgs e)
        {
            refreshState.Visible = true;
            refreshList.Enabled = false;
            fiyatList.Items.Clear();
            hisseBilgileriGetir();
        }
        private void listFilter_TextChanged(object sender, EventArgs e)
        {
            if (txt_Search.Text != "")
            {
                for (int i = fiyatList.Items.Count - 1; i >= 0; i--)
                {
                    var item = fiyatList.Items[i];
                    if (item.Text.ToLower().Contains(txt_Search.Text.ToLower()))
                    {
                        item.BackColor = SystemColors.Highlight;
                        item.ForeColor = SystemColors.HighlightText;
                    }
                    else
                    {
                        fiyatList.Items.Remove(item);
                    }
                }
            }
            else if (txt_Search.Text == "")
            {
                cleanFilter_Click(sender, e);
            }
        }
        private void cleanFilter_Click(object sender, EventArgs e)
        {
            filterClean();
        }
        private void txt_miktar_ValueChanged(object sender, EventArgs e)
        {
            sonucGuncelle();
        }
        private void fiyatList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txt_secilenHisse.Text = fiyatList.SelectedItems[0].SubItems[0].Text;
                alim_fiyat.Text = fiyatList.SelectedItems[0].SubItems[1].Text;
                sonucGuncelle();
            }
            catch
            {
            }
        }
        private void btn_al_Click(object sender, EventArgs e)
        {
            string hisseAdi = txt_secilenHisse.Text;
            if (hisseAdi.Length <= 3)
            {
                MessageBox.Show("Hisse Adını kontrol edin!!!");
                return;
            }
            decimal hisseFiyat = Convert.ToDecimal(alim_fiyat.Text.Replace(".", ","));
            if (!(hisseFiyat > 0))
            {
                MessageBox.Show("Hisse Fiyatını kontrol edin!!!");
                return;
            }
            decimal hisseAdet = Convert.ToDecimal(txt_miktar.Value);
            if (!(hisseAdet > 0))
            {
                MessageBox.Show("Hisse Adetini kontrol edin!!!");
                return;
            }
            string sonucResult =
                hisseAdi
                + "\n" +
                "MALİYET : " + hisseFiyat.ToString()
                + "\n" +
                "A.ADET  : " + hisseAdet.ToString()
                + "\n \n" +
                "TOPLAM TUTAR : " + (hisseFiyat * hisseAdet).ToString()
                + "\n \n ALIM EMRİNİ ONAYLIYOR MUSUNUZ? \n \n " +
                "Not: Onaylandıktan sonra portföyü yenileyerek hisseyi görüntüleyebilirsiniz";

            DialogResult result = MessageBox.Show(
                text: sonucResult,
                caption: hisseAdi + " ALIM",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading);

            if (result == DialogResult.Yes)
            {
                hisseAlTxt(hisseAdi, hisseFiyat, hisseAdet);
            }
        }
        private void portfoyRefresh_Click(object sender, EventArgs e)
        {
            portfoyGuncelle();
        }
        private void portfoyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem item in fiyatList.Items)
                {
                    if (item.SubItems[0].Text == portfoyList.SelectedItems[0].SubItems[0].Text)
                    {
                        txt_secilenHisse.Text = item.SubItems[0].Text;
                        alim_fiyat.Text = item.SubItems[1].Text;
                    }
                }
                sonucGuncelle();
            }
            catch
            {
            }
        }
        private void btn_sat_Click(object sender, EventArgs e)
        {
            if (portfoydeVarmi(txt_secilenHisse.Text))
            {
                if (((int)txt_miktar.Value) <= portfoyMiktariOgren(txt_secilenHisse.Text))
                {
                    string sonucResult = txt_secilenHisse.Text + "\n" + "FİYAT : " + alim_fiyat.Text + "\n" + "A.ADET  : " + txt_secilenHisse.Text + "\n \n" + "\n \n SATIŞ EMRİNİ ONAYLIYOR MUSUNUZ? \n \n ";

                    DialogResult result = MessageBox.Show(text: sonucResult, caption: txt_secilenHisse.Text + " SATIM", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.RtlReading);

                    if (result == DialogResult.Yes)
                    {
                        hisseSatTxt(txt_secilenHisse.Text, Convert.ToDecimal(txt_miktar.Value));
                        MessageBox.Show("Satış işlemi başarıyla gerçekleştirildi.");
                    }
                }
                else
                {
                    MessageBox.Show("Satmak istediğiniz adet kadar portföyünüzde hisse bulunmuyor.\nLütfen miktarı elinizde bulunan miktara göre ayarlayın");
                }
            }
            else
            {
                MessageBox.Show("Portföyünüzde bu hisse yok!!!");
            }
        }
        private void btn_analiz_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://tr.tradingview.com/");
        }
        private void btn_ayarlar_Click(object sender, EventArgs e)
        {
            settings ayarlar = new settings();
            ayarlar.ShowDialog();
            bakiyeGuncelleTxt();
            portfoyGuncelle();
        }
        private void filterClean()
        {
            txt_Search.Clear();
            fiyatList.Items.Clear();
            jsonVerileriOku();
        }
        #endregion
    }
}
