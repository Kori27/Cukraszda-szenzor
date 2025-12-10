# Cukraszda-szenzor

A projekt célja egy C# konzolalkalmazás készítése, amely szimulálja egy cukrászda szenzorhálózatát. A hálózat autonóm szenzorokból áll, amelyek figyelik a sütők és gépek állapotát (pl. hőmérséklet, páratartalom, viszkozitás). A mért adatok események segítségével kerülnek a főprogramba és mentődnek LiteDB adatbázisba és JSON fájlba. A program konzolon átlátható, táblázatos, dekoratív formában jeleníti meg az adatokat.

A projekt során a következő NuGet csomagokat használtuk:

- LiteDB – beágyazott NoSQL adatbázis kezelésére, amely lehetővé teszi a mérési adatok tárolását egy .db fájlban.

- Newtonsoft.Json – JSON fájlba történő exportáláshoz és az adatok olvasható formátumban történő mentéséhez.

Ezek a csomagok biztosítják a program adatkezelési és exportálási funkcióit, anélkül, hogy külső adatbázis-szerverre lenne szükség.
