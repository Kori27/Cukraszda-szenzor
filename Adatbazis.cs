using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using Newtonsoft.Json;

namespace CukraszdaConsoleApp
{
    //Réka csinálta 
    // Adatkezelő osztály
    // Statikus osztály, ami felelős az adatok mentéséért
    public static class DataManager
    {
        // LiteDB-be mentés
        // A mérési adatokat egy beágyazott NoSQL adatbázisba (LiteDB) menti
        public static void SaveToLiteDB(List<Measurement> measurements, string path = "cukraszda.db")
        {
            // Adatbázis megnyitása (vagy létrehozása, ha nem létezik)
            using (var db = new LiteDatabase(path))
            {
                // "Measurements" nevű kollekció lekérése
                var col = db.GetCollection<Measurement>("Measurements");

                // Minden mérés beszúrása a kollekcióba
                foreach (var m in measurements)
                    col.Insert(m);
            } // using lezárja az adatbázis kapcsolatot automatikusan
        }

        // JSON export
        // A mérési adatokat JSON fájlba menti
        public static void ExportToJson(List<Measurement> measurements, string path = "measurements.json")
        {
            // JSON fájl létrehozása, indentált formázással a könnyebb olvashatóság érdekében
            File.WriteAllText(path, JsonConvert.SerializeObject(measurements, Formatting.Indented));
        }
    }
}
