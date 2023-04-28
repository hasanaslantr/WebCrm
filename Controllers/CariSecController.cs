using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using static TemaCrm.Controllers.FiyatGorController;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace TemaCrm.Controllers
{
    public class CariSecController : Controller
    {
        FbConnection con = new FbConnection(@"User ID=SYSDBA;Password=masterkey; Database=DESKTOP-EPQ611O/3080:D:\TANDOGAN\TANDOGAN.fdb");
        public class CariSecListe
        {
            public int CARI_NO { get; set; }
            public string CARI_KOD { get; set; }
            public string CARI_UNVANI { get; set; }

        }
        public string tmpISLEM_NOKTASI_NO, tmpISLEM_NOKTASI_ADI, tmpPERSONEL_NO, tmpPERSONEL_ADI,
          tmpKasaHesapNo, tmpKasaHesapNoAdi, tmpSTOK_YERI_NO, tmpSTOK_YERI_ADI,
          tmpSTOK_NO, tmpSTOK_ADI, tmpSTOK_TIP_NO, tmpSTOK_TIP_ADI, tmpBIRIMX,
          tmpBIRIM, tmpBANKA_HESAP_DNO, tmpALISSATIS_NO, tmpISLEM_ADI,
          tmpISLEM_KODU, tmpISLEM_YONU, tmpCARI_ISLEM_TURU, tmpXMODUL,
          tmpZORUNLU_XISLEM_KODLARI, tmpTARIH, tmpREFERANS_NO, tmpREFERANS_TARIHI, tmpGENEL_TOPLAM,
          tmpODEME_TURU, tmpKDV_ORANI_NO, tmpKDV_GIRIS_ORANI, tmpTAKIP_SEKLI, tmpALISSATIS_DETAY_NO,
          tmpTANIM_ISLEM_KODU, tmpTANIM_ISLEM_ADI, tmpTANIM_ISLEM_IADE, tmpVERGI_UYST_NO,
          tmpHATA_NO, tmpBranchCode, tmpFisNo, tmpALISSATIS_NO_INTERNAL, tmpUSER_ID,
          tmpGROUP_ID, tmpROGROUP_ID, tmpSECURITY, tmpOTOSTOK_ISLEM, KayitDefteri;
        public int tmpCARI_NO;
        decimal tmpBIRIM_FIYAT;
        private void SatisIslemKodlari()
        {
            try
            {
                tmpPERSONEL_NO = "5";
                tmpISLEM_NOKTASI_NO = "5";
                tmpSTOK_YERI_NO = "4";

                tmpTANIM_ISLEM_KODU = "ALIŞ";
                FbCommand ISLMKODL = new FbCommand(@"SELECT TANIM_ISLEM_KODU, TANIM_ISLEM_ADI, ISLEM_YONU, CARI_ISLEM_TURU, XMODUL, 
                            ZORUNLU_XISLEM_KODLARI,OTOSTOK_ISLEM FROM ISLMKODL  WHERE ISLEM_YONU = 1  AND TANIM_ISLEM_KODU ='" + tmpTANIM_ISLEM_KODU + "'", con);  //'" + tmpTANIM_ISLEM_KODU + "'"
                FbDataReader ISLMKODLOKU = ISLMKODL.ExecuteReader();
                while (ISLMKODLOKU.Read())
                {
                    tmpISLEM_KODU = ISLMKODLOKU[0].ToString();
                    tmpISLEM_ADI = ISLMKODLOKU[1].ToString();
                    tmpISLEM_YONU = ISLMKODLOKU[2].ToString();
                    tmpCARI_ISLEM_TURU = ISLMKODLOKU[3].ToString();
                    tmpXMODUL = ISLMKODLOKU[4].ToString();
                    tmpZORUNLU_XISLEM_KODLARI = ISLMKODLOKU[5].ToString();
                    tmpOTOSTOK_ISLEM = ISLMKODLOKU[6].ToString();
                }
                ISLMKODLOKU.Close();
            }
            catch (Exception)
            {

            }
        }
        public IActionResult Index()
        {
            List<CariSecListe> veriler = new List<CariSecListe>();

            var sql = @"SELECT FIRST 20 CARI_NO,CARI_KOD,CARI_UNVANI FROM CARIKART WHERE CARI_NO > 0";
            con.Open();
            FbCommand co = new FbCommand(sql, con);
            FbDataReader re = co.ExecuteReader();
            while (re.Read())
            {
                veriler.Add(new CariSecListe()
                {
                    CARI_NO = int.Parse(re["CARI_NO"].ToString()),
                    CARI_KOD = re["CARI_KOD"].ToString(),
                    CARI_UNVANI = re["CARI_UNVANI"].ToString() 
                });
            }
            re.Close();
            con.Close();
            return View(veriler);
        }
        [HttpGet]
        public IActionResult SutTopla(int CARI_NO,int ALISSATIS_NO)
        {
            ViewBag.HataMesage = "0"; 
            con.Open();
            CariBilgileriGetir(CARI_NO); 
            tmpALISSATIS_NO = ALISSATIS_NO.ToString(); 
            try
            {
                SatisIslemKodlari();
                if (tmpALISSATIS_NO=="" || tmpALISSATIS_NO==null || tmpALISSATIS_NO=="0")
                {
                    FbCommand komut = new FbCommand("SELECT GEN_ID(ALISSATIS_NO,1) FROM STOKKART WHERE STOK_NO = 0", con);
                    FbDataReader oku = komut.ExecuteReader();
                    while (oku.Read())
                    {
                        tmpALISSATIS_NO = oku[0].ToString();
                    }
                    oku.Close(); 
                } 
                FbCommand ALSAKONTOL = new FbCommand("SELECT COUNT(ALISSATIS_NO) FROM ALSAASIL WHERE ALISSATIS_NO = " + tmpALISSATIS_NO + "", con);
                FbDataReader ALSAKONTOLOKU = ALSAKONTOL.ExecuteReader();
                while (ALSAKONTOLOKU.Read())
                {
                    if (int.Parse(ALSAKONTOLOKU[0].ToString()) == 0)
                    {
                        FbCommand AlsaasilEkle = new FbCommand(@"INSERT INTO ALSAASIL(ALISSATIS_NO,OZEL_KOD,TARIH,VADE_TARIHI,KUR_TARIHI,ISLEM_ADI,
                            ISLEM_KODU,ISLEM_YONU,DOVIZ_BIRIMI,FATURA_DOVIZ_BIRIMI,DOVIZ_KURU,CARI_NO,BAKIYE_TIP_NO,STOK_NO,CARI_ISLEM_TURU,
                            XMODUL,ISLEM_NOKTASI_NO,PERSONEL_NO,STOK_YERI_NO,SATIR_KDV_MATRAH_YUVARLAMASI,KAPANDI,SECENEKLER)                  
                            VALUES(@ALISSATIS_NO,@OZEL_KOD,
                            @TARIH,@VADE_TARIHI,@KUR_TARIHI,@ISLEM_ADI,@ISLEM_KODU,@ISLEM_YONU,
                            @DOVIZ_BIRIMI,@FATURA_DOVIZ_BIRIMI,@DOVIZ_KURU,@CARI_NO,@BAKIYE_TIP_NO,@STOK_NO,@CARI_ISLEM_TURU,
                            @XMODUL,@ISLEM_NOKTASI_NO,@PERSONEL_NO,@STOK_YERI_NO,@SATIR_KDV_MATRAH_YUVARLAMASI,@KAPANDI,@SECENEKLER)", con);
                        AlsaasilEkle.Parameters.AddWithValue("@ALISSATIS_NO", tmpALISSATIS_NO);
                        AlsaasilEkle.Parameters.AddWithValue("@OZEL_KOD", "TemaMobilAktarım");
                        AlsaasilEkle.Parameters.AddWithValue("@TARIH", Convert.ToDateTime(DateTime.Now.ToString()));
                        AlsaasilEkle.Parameters.AddWithValue("@VADE_TARIHI", Convert.ToDateTime(DateTime.Now.ToString()));
                        AlsaasilEkle.Parameters.AddWithValue("@KUR_TARIHI", Convert.ToDateTime(DateTime.Now.ToString()));
                        AlsaasilEkle.Parameters.AddWithValue("@ISLEM_ADI", tmpISLEM_ADI);
                        AlsaasilEkle.Parameters.AddWithValue("@ISLEM_KODU", tmpISLEM_KODU);
                        AlsaasilEkle.Parameters.AddWithValue("@ISLEM_YONU", tmpISLEM_YONU);
                        AlsaasilEkle.Parameters.AddWithValue("@DOVIZ_BIRIMI", "TL");
                        AlsaasilEkle.Parameters.AddWithValue("@FATURA_DOVIZ_BIRIMI", "TL");
                        AlsaasilEkle.Parameters.AddWithValue("@DOVIZ_KURU", 1);
                        AlsaasilEkle.Parameters.AddWithValue("@CARI_NO", CARI_NO);
                        AlsaasilEkle.Parameters.AddWithValue("@BAKIYE_TIP_NO", 0);
                        AlsaasilEkle.Parameters.AddWithValue("@STOK_NO", 0);
                        AlsaasilEkle.Parameters.AddWithValue("@CARI_ISLEM_TURU", tmpCARI_ISLEM_TURU);
                        AlsaasilEkle.Parameters.AddWithValue("@XMODUL", tmpXMODUL);
                        AlsaasilEkle.Parameters.AddWithValue("@ISLEM_NOKTASI_NO", tmpISLEM_NOKTASI_NO);
                        AlsaasilEkle.Parameters.AddWithValue("@PERSONEL_NO", tmpPERSONEL_NO);
                        AlsaasilEkle.Parameters.AddWithValue("@STOK_YERI_NO", tmpSTOK_YERI_NO);
                        AlsaasilEkle.Parameters.AddWithValue("@SATIR_KDV_MATRAH_YUVARLAMASI", "H");
                        AlsaasilEkle.Parameters.AddWithValue("@KAPANDI", "E");
                        AlsaasilEkle.Parameters.AddWithValue("@SECENEKLER", "EHHHEHEEEHHHHHHHHHHHHHHH");
                        AlsaasilEkle.ExecuteNonQuery(); 
                        ViewBag.ALISSATIS_NO = tmpALISSATIS_NO;
                        ViewBag.CARI_NO = CARI_NO;
                    }
                }

                ViewBag.ALISSATIS_NO = tmpALISSATIS_NO;
                ViewBag.CARI_NO = CARI_NO;
            }
            catch (Exception ex)
            {
                CariBilgileriGetir(CARI_NO);
                ViewBag.HataMesage = ex.Message;
                //  return RedirectToAction("SutTopla", new { CARI_NO = CARI_NO, ALISSATIS_NO = ALISSATIS_NO });
                return View();
            }

            List<AlsaDeta> veriler = new List<AlsaDeta>();
            if (tmpALISSATIS_NO != "" || tmpALISSATIS_NO != null || tmpALISSATIS_NO != "0")
            {


                var sql = @"SELECT ALISSATIS_DETAY_NO,STOK_NO,STOK_ADI,BIRIM,UYGULAMA_FIYATI, MIKTAR , HAM_TUTAR FROM ALSADETA WHERE    ALISSATIS_NO = " + ALISSATIS_NO + "";
                FbCommand co = new FbCommand(sql, con);
                FbDataReader re = co.ExecuteReader();
                while (re.Read())
                {
                    veriler.Add(new AlsaDeta()
                    {

                        ALISSATIS_DETAY_NO = int.Parse(re["ALISSATIS_DETAY_NO"].ToString()),
                        STOK_NO = int.Parse(re["STOK_NO"].ToString()),
                        STOK_ADI = re["STOK_ADI"].ToString(),
                        BIRIM = re["BIRIM"].ToString(),
                        LISTE_FIYATI = decimal.Parse(re["UYGULAMA_FIYATI"].ToString()),
                        MIKTAR = re["MIKTAR"].ToString(),
                        GENEL_TOPLAM = decimal.Parse(re["HAM_TUTAR"].ToString()),

                    });
                }
                re.Close();
                return View(veriler);
                con.Close();
            }
            else
            {
                return View();
                con.Close();
            }
      

           
        }
        public class SutToplaAdd
        {
            public int secCARI_NO { get; set; }
            public string MIKTAR { get; set; }
            public string ACIKLAMA { get; set; }

        }
 

        private void CariBilgileriGetir(int id)
        {
            var sql = @"SELECT   CARI_NO,CARI_KOD,CARI_UNVANI FROM CARIKART WHERE CARI_NO = " + id + " ";
            FbCommand co = new FbCommand(sql, con);
            FbDataReader re = co.ExecuteReader();
            while (re.Read())
            {
                ViewBag.CARI_KOD = re["CARI_KOD"].ToString();
                ViewBag.CARI_ID = re["CARI_NO"].ToString();
                ViewBag.CARI_UNVANI = re["CARI_UNVANI"].ToString();
            }
            re.Close();
        } 
        public class UrunList
        {
            public int STOK_NO { get; set; }
            public string STOK_KODU { get; set; }
            public string STOK_ADI { get; set; }
            public string BIRIM { get; set; }
            public string BIRIMX { get; set; }
            public decimal LISTE_FIYATI { get; set; }
            public string BARKOD { get; set; }

        }


        [HttpGet]
        public IActionResult UrunAdd(int CARI_NO, int ALISSATIS_NO)
        {
            ViewBag.HataMesage = "0";
            try
            {
                List<UrunList> veriler = new List<UrunList>();

                var sql = @"SELECT FIRST 50 S.STOK_NO,STOK_KODU,STOK_ADI,A.BIRIM , A.BARKOD, B.BIRIMX,
                BIRIMX* LISTE_FIYATI / ((SELECT FIRST 1 BB.BIRIMX FROM STOKBIRI BB
                INNER JOIN FIYADETA FF ON FF.BIRIM = BB.BIRIM  AND FF.STOK_NO = A.STOK_NO   AND FF.FIYAT_NO = 11
                WHERE BB.STOK_NO = A.STOK_NO)) AS LISTE_FIYATI
                FROM STOKBARK A
                INNER JOIN FIYADETA F ON  A.STOK_NO = F.STOK_NO
                INNER JOIN STOKBIRI B ON A.BIRIM = B.BIRIM AND A.STOK_NO = B.STOK_NO
                INNER JOIN STOKKART S ON A.STOK_NO = S.STOK_NO
                INNER JOIN VERGORAN V on V.VERGI_ORAN_NO = S.KDV_ORAN_NO
                WHERE F.FIYAT_NO = 11   AND S.BLOKE = 'H' AND S.AKTIF = 'E' ";
                con.Open();
                ViewBag.ALISSATIS_NO = ALISSATIS_NO;
                ViewBag.CARI_NO = CARI_NO;
                FbCommand co = new FbCommand(sql, con);
                FbDataReader re = co.ExecuteReader();
                while (re.Read())
                {
                    veriler.Add(new UrunList()
                    {
                        STOK_NO = int.Parse(re["STOK_NO"].ToString()),
                        STOK_KODU = re["STOK_KODU"].ToString(),
                        STOK_ADI = re["STOK_ADI"].ToString(),
                        BIRIM = re["BIRIM"].ToString(),
                        BIRIMX = re["BIRIMX"].ToString(),
                        LISTE_FIYATI = decimal.Parse(re["LISTE_FIYATI"].ToString()),
                        BARKOD = re["BARKOD"].ToString(),

                    });
                }
                re.Close();
                con.Close();
                return View(veriler);
            }
            catch (Exception ex)
            {
                ViewBag.HataMesage = ex.Message;
                return View("UrunAdd", new { CARI_NO = CARI_NO, ALISSATIS_NO = ALISSATIS_NO });
            }
        }
        [HttpPost]
        public IActionResult UrunAdd(string MIKTAR, string STOK_NO, int ALISSATIS_NO, int CARI_NO)
        {
            try
            {
                ViewBag.HataMesage = "0";

                con.Open();
                SatisIslemKodlari();
                FbCommand StokBilgileri = new FbCommand(@"SELECT FIRST 1 S.STOK_NO,S.STOK_ADI,S.STOK_TIP_NO,S.TAKIP_SEKLI,V.GIRIS_ORANI,S.VERGI_UYST_NO,T.STOK_TIP_ADI,B.BIRIMX,
                           S.KDV_ORAN_NO,B.BIRIM, F.LISTE_FIYATI FROM STOKKART  S 
                           INNER JOIN VERGORAN V on  V.VERGI_ORAN_NO = S.KDV_ORAN_NO 
                           INNER JOIN STOKTIPI T ON  S.STOK_TIP_NO = T.STOK_TIP_NO
                           INNER JOIN FIYADETA F on F.STOK_NO = S.STOK_NO
                           INNER JOIN STOKBIRI B ON  S.STOK_NO = B.STOK_NO WHERE S.STOK_NO = " + STOK_NO + " ", con);
                FbDataReader StokBilgileriOku = StokBilgileri.ExecuteReader();
                while (StokBilgileriOku.Read())
                {
                    tmpSTOK_NO = StokBilgileriOku["STOK_NO"].ToString();
                    tmpSTOK_ADI = StokBilgileriOku["STOK_ADI"].ToString();
                    tmpSTOK_TIP_NO = StokBilgileriOku["STOK_TIP_NO"].ToString();
                    tmpSTOK_TIP_ADI = StokBilgileriOku["STOK_TIP_ADI"].ToString();
                    tmpKDV_ORANI_NO = StokBilgileriOku["KDV_ORAN_NO"].ToString();
                    tmpKDV_GIRIS_ORANI = StokBilgileriOku["GIRIS_ORANI"].ToString();
                    tmpVERGI_UYST_NO = StokBilgileriOku["VERGI_UYST_NO"].ToString();
                    tmpBIRIM = StokBilgileriOku["BIRIM"].ToString();
                    tmpBIRIMX = StokBilgileriOku["BIRIMX"].ToString();
                    tmpTAKIP_SEKLI = StokBilgileriOku["TAKIP_SEKLI"].ToString();
                    tmpBIRIM_FIYAT = decimal.Parse(StokBilgileriOku["LISTE_FIYATI"].ToString());
                }
                StokBilgileriOku.Close();
                FbCommand AlsaDetaEkle = new FbCommand(@"INSERT INTO ALSADETA(ALISSATIS_DETAY_NO,ALISSATIS_NO,ISLEM_KODU,STOK_NO,STOK_ADI,STOK_TIP_NO,STOK_TIP_ADI,
                                   DSTOK_NO,KDV_ORANI,VERGI_UYST_NO,MIKTAR,BIRIMX,TAKIP_SEKLI,DOVIZ_BIRIMI,DOVIZ_KURU,
                                   STOK_ISLEM_KODU,OTOSTOK_ISLEM,BIRIM,BIRIM_FIYAT,PERSONEL_NO,STOK_YERI_NO,ISLEM_YONU,OZEL_KOD)
                                   VALUES(@ALISSATIS_DETAY_NO,@ALISSATIS_NO,@ISLEM_KODU,@STOK_NO,@STOK_ADI,@STOK_TIP_NO,@STOK_TIP_ADI,
                                   @DSTOK_NO,@KDV_ORANI,@VERGI_UYST_NO,@MIKTAR,@BIRIMX,@TAKIP_SEKLI,@DOVIZ_BIRIMI,@DOVIZ_KURU,@STOK_ISLEM_KODU,
                                   @OTOSTOK_ISLEM,@BIRIM,@BIRIM_FIYAT,@PERSONEL_NO,@STOK_YERI_NO,@ISLEM_YONU,@OZEL_KOD)", con);
                AlsaDetaEkle.Parameters.AddWithValue("@ALISSATIS_DETAY_NO", null);
                AlsaDetaEkle.Parameters.AddWithValue("@ALISSATIS_NO", ALISSATIS_NO);
                AlsaDetaEkle.Parameters.AddWithValue("@ISLEM_KODU", tmpISLEM_KODU);
                AlsaDetaEkle.Parameters.AddWithValue("@STOK_NO", tmpSTOK_NO);
                AlsaDetaEkle.Parameters.AddWithValue("@STOK_ADI", tmpSTOK_ADI);
                AlsaDetaEkle.Parameters.AddWithValue("@STOK_TIP_NO", tmpSTOK_TIP_NO);
                AlsaDetaEkle.Parameters.AddWithValue("@STOK_TIP_ADI", tmpSTOK_TIP_ADI);
                AlsaDetaEkle.Parameters.AddWithValue("@DSTOK_NO", 0);
                AlsaDetaEkle.Parameters.AddWithValue("@KDV_ORANI", tmpKDV_GIRIS_ORANI);
                AlsaDetaEkle.Parameters.AddWithValue("@VERGI_UYST_NO", tmpVERGI_UYST_NO);
                AlsaDetaEkle.Parameters.AddWithValue("@MIKTAR", MIKTAR);
                AlsaDetaEkle.Parameters.AddWithValue("@BIRIM", tmpBIRIM);
                AlsaDetaEkle.Parameters.AddWithValue("@BIRIMX", tmpBIRIMX);
                AlsaDetaEkle.Parameters.AddWithValue("@TAKIP_SEKLI", tmpTAKIP_SEKLI);
                AlsaDetaEkle.Parameters.AddWithValue("@DOVIZ_BIRIMI", "TL");
                AlsaDetaEkle.Parameters.AddWithValue("@DOVIZ_KURU", 1);
                AlsaDetaEkle.Parameters.AddWithValue("@STOK_ISLEM_KODU", tmpZORUNLU_XISLEM_KODLARI);
                AlsaDetaEkle.Parameters.AddWithValue("@OTOSTOK_ISLEM", tmpOTOSTOK_ISLEM);
                AlsaDetaEkle.Parameters.AddWithValue("@BIRIM_FIYAT", tmpBIRIM_FIYAT);
                AlsaDetaEkle.Parameters.AddWithValue("@PERSONEL_NO", tmpPERSONEL_NO);
                AlsaDetaEkle.Parameters.AddWithValue("@STOK_YERI_NO", tmpSTOK_YERI_NO);
                AlsaDetaEkle.Parameters.AddWithValue("@ISLEM_YONU", tmpISLEM_YONU);
                AlsaDetaEkle.Parameters.AddWithValue("@OZEL_KOD", "TemaMobilAktarım");
                AlsaDetaEkle.ExecuteNonQuery();

                

                con.Close();
                return RedirectToAction("SutTopla",new { CARI_NO = CARI_NO,ALISSATIS_NO = ALISSATIS_NO});
            }
            catch (Exception ex)
            {
                ViewBag.HataMesage = ex.Message;
                return RedirectToAction("SutTopla", new { CARI_NO = CARI_NO, ALISSATIS_NO = ALISSATIS_NO });
            }
        } 
        public class AlsaDeta
        {
            public int ALISSATIS_DETAY_NO { get; set; }
            public int STOK_NO { get; set; } 
            public string STOK_ADI { get; set; }
            public string BIRIM { get; set; } 
            public decimal LISTE_FIYATI { get; set; }
            public string MIKTAR { get; set; }
            public decimal GENEL_TOPLAM { get; set; } 
        } 
    }
}



//string username = Request.Form["username"];