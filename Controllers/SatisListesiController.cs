using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using static TemaCrm.Controllers.FiyatGorController;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace TemaCrm.Controllers
{
    public class SatisListesiController : Controller
    {
        FbConnection con = new FbConnection(@"User ID=SYSDBA;Password=masterkey; Database=DESKTOP-EPQ611O/3080:D:\TANDOGAN\TANDOGAN.fdb");

        public class SatisAlsa 
            {
            public int ALISSATIS_NO { get; set; }
            public string CARI_NO { get; set; }
            public string CARI_UNVANI { get; set; }
            public string ACIKLAMA { get; set; }
            public string PERSONEL_NO { get; set; }
            public string PERSONEL_UNVANI { get; set; }
            public string TARIH { get; set; }  
            public decimal GENEL_TOPLAM { get; set; }  

        }
    public IActionResult Index()
    {
            List<SatisAlsa> veriler = new List<SatisAlsa>();


            string date2 = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy");
            string date1 = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy");

            var sql = @"SELECT A.ALISSATIS_NO, A.CARI_NO,C.CARI_UNVANI, A.ACIKLAMA, A.TARIH,  A.GENEL_TOPLAM,P.ADISOYADI ,A.PERSONEL_NO 
            FROM ALSAASIL A  
            JOIN PERSONEL_LIST P ON P.PERSONEL_NO = A.PERSONEL_NO+0
            JOIN CARIKART C ON C.CARI_NO = A.CARI_NO+0   WHERE A.TARIH BETWEEN '"+date1+ "' AND '"+ date2 + "' AND  A.ISLEM_KODU = 'ALIŞ' AND A.ISLEM_YONU = 1" +
            "ORDER BY A.TARIH DESC";
           
            con.Open();

            FbCommand co = new FbCommand(sql, con);
            FbDataReader re = co.ExecuteReader();
            while (re.Read())
            {
                veriler.Add(new SatisAlsa()
                {
                    ALISSATIS_NO = int.Parse(re["ALISSATIS_NO"].ToString()),
                    CARI_NO = re["CARI_NO"].ToString(),
                    CARI_UNVANI = re["CARI_UNVANI"].ToString(),
                    ACIKLAMA = re["ACIKLAMA"].ToString(),
                    TARIH = re["TARIH"].ToString(),
                    GENEL_TOPLAM = decimal.Parse(re["GENEL_TOPLAM"].ToString()),
                    PERSONEL_UNVANI = re["ADISOYADI"].ToString(),
                    PERSONEL_NO = re["PERSONEL_NO"].ToString(),

                });
            }
            re.Close();
            con.Close();
            return View(veriler); 
    }
}
}
