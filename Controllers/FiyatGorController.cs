using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using FirebirdSql.Data.FirebirdClient;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace TemaCrm.Controllers
{
    public class FiyatGorController : Controller
    {
        FbConnection con = new FbConnection(@"User ID=SYSDBA;Password=masterkey; Database=DESKTOP-EPQ611O/3080:D:\TANDOGAN\TANDOGAN.fdb");

        public class FiyatList
        {
            public int STOK_NO { get; set; }
            public string STOK_KODU { get; set; }
            public string STOK_ADI { get; set; }
            public string BIRIM { get; set; }
            public string BIRIMX { get; set; }
            public decimal LISTE_FIYATI { get; set; }
            public string BARKOD { get; set; }

        }

        public IActionResult Index()
        {
            List<FiyatList> veriler = new List<FiyatList>();

            var sql = @"SELECT  S.STOK_NO,STOK_KODU,STOK_ADI,A.BIRIM , A.BARKOD, B.BIRIMX,
                BIRIMX* LISTE_FIYATI / ((SELECT FIRST 1 BB.BIRIMX FROM STOKBIRI BB
                INNER JOIN FIYADETA FF ON FF.BIRIM = BB.BIRIM  AND FF.STOK_NO = A.STOK_NO   AND FF.FIYAT_NO = 11
                WHERE BB.STOK_NO = A.STOK_NO)) AS LISTE_FIYATI
                FROM STOKBARK A
                INNER JOIN FIYADETA F ON  A.STOK_NO = F.STOK_NO
                INNER JOIN STOKBIRI B ON A.BIRIM = B.BIRIM AND A.STOK_NO = B.STOK_NO
                INNER JOIN STOKKART S ON A.STOK_NO = S.STOK_NO
                INNER JOIN VERGORAN V on V.VERGI_ORAN_NO = S.KDV_ORAN_NO
                WHERE F.FIYAT_NO = 11   AND S.BLOKE = 'H' AND S.AKTIF = 'E' ";
            //
          
            con.Open();

            FbCommand co = new FbCommand(sql, con);
            FbDataReader re = co.ExecuteReader();
            while (re.Read())
            {
                veriler.Add(new FiyatList()
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

    }
}


