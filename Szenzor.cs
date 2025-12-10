using System;

namespace CukraszdaConsoleApp
{
    //Hanna csinálta 
    // Mérés típusok
    // Enum, ami a szenzor által mérhető paramétereket tartalmazza
    public enum MeasurementType
    {
        Temperature,    // Hőmérséklet
        Humidity,       // Páratartalom
        Viscosity,      // Viszkozitás (cukormáz, krém)
        PowderDust      // Por (cukor, liszt)
    }

    // Mérési adat
    // Egy mérés adatait tartalmazó osztály
    public class Measurement
    {
        public int SensorId { get; set; }           // Szenzor azonosítója
        public string SensorName { get; set; } = ""; // Szenzor neve
        public MeasurementType Type { get; set; }   // Mérés típusa (hő, páratartalom, stb.)
        public double Value { get; set; }           // Mérési érték
        public DateTime Timestamp { get; set; }     // Mérési időpont
    }

    // Delegált a méréshez
    // Egyszerű delegált, ami visszaad egy double értéket
    public delegate double MeasureDelegate();

    // Esemény argumentum
    // Az eseménynek átadott objektum, ami tartalmazza a mérést
    public class MeasurementEventArgs : EventArgs
    {
        public Measurement Measurement { get; }

        public MeasurementEventArgs(Measurement measurement)
        {
            Measurement = measurement;
        }
    }

    // Szenzor csomópont
    public class SensorNode
    {
        public int Id { get; }           // Csomópont azonosító
        public string Name { get; }      // Csomópont neve

        // Delegáltak a különböző mérési típusokhoz
        public MeasureDelegate TemperatureMeasure { get; set; }
        public MeasureDelegate HumidityMeasure { get; set; }
        public MeasureDelegate ViscosityMeasure { get; set; }
        public MeasureDelegate PowderDustMeasure { get; set; }

        // Esemény, ami akkor hívódik, amikor új mérés készül
        public event EventHandler<MeasurementEventArgs> MeasurementGenerated;

        private readonly Random _random; // Véletlenszám generátor a szimulációhoz

        // Konstruktor: beállítja az ID-t, nevet, generátorokat
        public SensorNode(int id, string name, Random random)
        {
            Id = id;
            Name = name;
            _random = random;

            // Delegáltakhoz véletlenszerű értékeket adunk:
            // Hőmérséklet 160–240 °C
            TemperatureMeasure = () => 160 + _random.NextDouble() * 80;

            // Páratartalom 30–80%
            HumidityMeasure = () => 30 + _random.NextDouble() * 50;

            // Viszkozitás 0.5–3.0
            ViscosityMeasure = () => 0.5 + _random.NextDouble() * 2.5;

            // Por 0–100 egység
            PowderDustMeasure = () => _random.NextDouble() * 100;
        }

        // Mérési ciklus
        // Egy ciklus minden mérési típus lefut
        public void DoMeasurementCycle()
        {
            CreateMeasurement(MeasurementType.Temperature, TemperatureMeasure);
            CreateMeasurement(MeasurementType.Humidity, HumidityMeasure);
            CreateMeasurement(MeasurementType.Viscosity, ViscosityMeasure);
            CreateMeasurement(MeasurementType.PowderDust, PowderDustMeasure);
        }

        // Mérés létrehozása és esemény
        private void CreateMeasurement(MeasurementType type, MeasureDelegate generator)
        {
            // Új mérési objektum létrehozása
            var measurement = new Measurement
            {
                SensorId = Id,
                SensorName = Name,
                Type = type,
                Value = generator(),       // Érték generálása a delegáltból
                Timestamp = DateTime.Now   // Aktuális idő
            };

            // Esemény kiváltása, ha van feliratkozó
            MeasurementGenerated?.Invoke(this, new MeasurementEventArgs(measurement));
        }
    }

    // Szenzorhálózat
    public class SensorNetwork
    {
        // Lista az összes szenzor csomópontról
        public System.Collections.Generic.List<SensorNode> Nodes { get; } = new System.Collections.Generic.List<SensorNode>();

        // Konstruktor: hozzáadjuk a hálózathoz a szenzorokat
        public SensorNetwork(Random random)
        {
            Nodes.Add(new SensorNode(1, "Sütő 1", random));
            Nodes.Add(new SensorNode(2, "Sütő 2", random));
            Nodes.Add(new SensorNode(3, "Tésztakeverő állomás", random));
            Nodes.Add(new SensorNode(4, "Díszítő pult", random));
            Nodes.Add(new SensorNode(5, "Krémkeverő gép", random));
        }

        // A hálózat minden csomópontja végrehajtja a mérési ciklust
        public void DoMeasurementCycle()
        {
            foreach (var node in Nodes)
                node.DoMeasurementCycle();
        }
    }
}
