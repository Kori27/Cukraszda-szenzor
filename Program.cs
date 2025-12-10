using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;

namespace CukraszdaConsoleApp
{
    class Program
    {
        //Korinna csinálta :) 
        // Lista az összes mérés tárolására
        static List<Measurement> _measurements = new List<Measurement>();

        static void Main(string[] args)
        {
            // Konzol színének beállítása rózsaszínre, UTF-8 kódolás a dekorációkhoz
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Fejléc kiírása a konzolra
            PrintHeader();

            // Véletlenszám-generátor létrehozása a szenzorok szimulációjához
            var random = new Random();

            // Szenzorhálózat létrehozása
            var network = new SensorNetwork(random);

            // Feliratkozás az eseményekre, amikor egy szenzor új mérési adatot küld
            foreach (var node in network.Nodes)
            {
                node.MeasurementGenerated += (s, e) =>
                {
                    // Az új mérés hozzáadása a mérési listához
                    _measurements.Add(e.Measurement);
                };
            }

            // Mérési ciklus
            // 5 mérési ciklus lefutása
            for (int i = 0; i < 5; i++)
            {
                // Divider kiírása a ciklus elején
                PrintDivider($"Mérési ciklus {i + 1}");

                // Mérés végrehajtása minden szenzoron
                network.DoMeasurementCycle();

                // Táblázatos kiírás az aktuális mérési ciklusról
                PrintMeasurementsTable(_measurements);

                // Kis késleltetés, hogy a konzolban látható legyen a mérés
                System.Threading.Thread.Sleep(400);
            }

            // LINQ lekérdezések 
            RunLinqQueries();

            // Adatmentés 
            // Mérési adatok mentése LiteDB adatbázisba
            DataManager.SaveToLiteDB(_measurements);

            // Mérési adatok exportálása JSON fájlba
            DataManager.ExportToJson(_measurements);

            // Végső divider a program végén
            PrintDivider("♥ Szimuláció vége ♥");

            // Felhasználó értesítése, hogy nyomjon egy gombot a kilépéshez
            Console.WriteLine("Nyomj egy gombot a kilépéshez...");
            Console.ReadKey();

            // Konzol szín visszaállítása alapértelmezett színre
            Console.ResetColor();
        }

        // LINQ lekérdezések
        static void RunLinqQueries()
        {
            // Divider kiírása a LINQ részhez
            PrintDivider("LINQ lekérdezések");

            // Átlag sütőhőmérsékletek számítása szenzoronként
            var avgTempBySensor = _measurements
                .Where(m => m.Type == MeasurementType.Temperature) // Csak hőmérséklet
                .GroupBy(m => m.SensorName) // Szenzor neve szerint csoportosítás
                .Select(g => new { Sensor = g.Key, Avg = g.Average(x => x.Value) }); // Átlag számítása

            Console.WriteLine("\nÁtlagos sütőhőmérsékletek:");
            foreach (var item in avgTempBySensor)
                Console.WriteLine($"* {item.Sensor,-18} : {item.Avg:F2} °C ♥");

            // Max viszkozitás meghatározása a cukormázhoz
            var maxViscosity = _measurements
                .Where(m => m.Type == MeasurementType.Viscosity)
                .Max(m => m.Value);

            Console.WriteLine($"\n* Max. cukormáz-viszkozitás: {maxViscosity:F2} ♥");

            // Riasztások keresése, ha a hőmérséklet > 230°C
            var alerts = _measurements
                .Where(m => m.Type == MeasurementType.Temperature && m.Value > 230)
                .ToList();

            // Riasztások kiírása
            if (alerts.Count > 0)
            {
                Console.WriteLine("\nRiasztások (T > 230 °C):");
                PrintMeasurementsTable(alerts);
            }
            else
            {
                Console.WriteLine("\nNincsenek riasztások. ♥");
            }
        }

        // Táblázatos kiírás
        private static void PrintMeasurementsTable(List<Measurement> measurements)
        {
            // Táblázat fejléc kiírása
            Console.WriteLine("♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥");
            Console.WriteLine("♥ ID | Sensor Name          | Type         | Value    | Time   ♥");
            Console.WriteLine("♥-----------------------------------------------------------♥");

            // Mérések soronkénti kiírása
            foreach (var m in measurements)
            {
                // Névcsonkolás, ha túl hosszú
                string name = m.SensorName.Length > 18 ? m.SensorName.Substring(0, 15) + "..." : m.SensorName;

                // Típus, érték és idő formázása
                string type = m.Type.ToString().PadRight(12);
                string value = m.Value.ToString("F2").PadLeft(7);
                string time = m.Timestamp.ToString("HH:mm:ss");

                // Táblázat sor kiírása
                Console.WriteLine($"♥ {m.SensorId,-2} | {name,-18} | {type} | {value} | {time} ♥");
            }

            // Táblázat lezárása
            Console.WriteLine("♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥\n");
        }

        // Fejléc kiírása 
        static void PrintHeader()
        {
            Console.WriteLine("*********************************");
            Console.WriteLine("*                               *");
            Console.WriteLine("*        ♥ Cukrászda ♥          *");
            Console.WriteLine("*   *     ~     *     ~     *    *");
            Console.WriteLine("*   ♥     *           *     ♥    *");
            Console.WriteLine("*                               *");
            Console.WriteLine("*********************************\n");
        }

        // Divider kiírása
        static void PrintDivider(string title)
        {
            Console.WriteLine("\n♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥");
            Console.WriteLine($"♥   {title}   ♥");
            Console.WriteLine("♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥~*~♥\n");
        }
    }
}
