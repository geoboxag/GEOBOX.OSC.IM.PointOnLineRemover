# GEOBOX Punkt Tool
Diverse Funktionen für das AutoCAD Map 3D um Punkte zu bearbeiten, verändern oder entfernen.

> HINWEIS: Das Tool und die Funktionen können aktuell nur mit dem Produkt GEOBOX GIS Amtliche Vermessung verwendet werden.

## Funktion "Punkte entfernen (mit Liste)"
Mit dieser Funktion können Koordinaten von einer Datei (JSON) eingelesen werden. Anschliessend können die Objektklassen ausgewählt werden, in deren die Punkte gesucht und entfernet werden möchten.

### Punkte entfernen
#### Punktobjektklassen
In den Punktobjektklassen werden die Punkte die mit der Koordinate übereinstimmt gelöscht.
> HINWEIS: Linien die an dieser Koordinate einen Startpunkt, Endpunkt oder Stützpunkt haben werden nicht geändert!

### Linienobjektklassen
Bei den Linien wird der Stützpunkt dieser Koordinate gesucht und aus der Linie entfernt.
> HINWEIS: Linien die an dieser Koordinate einen Start- oder Endpunkt habn werden nicht geändert.

### JSON Datei
Für die Angabe der Koordinaten ist eine Datei im JSON-Format notwendig.
Der Inhalt sieht wie folgt aus:
```json
[
    {
        "ID": "260101",
        "East": 2652300.094,
        "North": 1260057.89
    },
    {
        "ID": "260102",
        "East": 2660250,
        "North": 1252610.768
    },
    {
        "ID": "260103",
        "East": 2660250.232,
        "North": 1251438.785
    }
]
```

## Voraussetzungen und Installation
### Voraussetzung
- Autodesk AutoCAD Map 3D 2025
- GEOBOX GIS Amtliche Vermessung
- Microsoft .NET Framework 8

### Installation
- Die DLL ist im AutoCAD Map 3D BIN-Verzeichnis abzulegen.
- Falls die Datei aus dem Internet heruntergeladen wurde, kann eine Sicherheitssperre vom Windows das Ausführen und Verwenden verhindern. In diesem Fall ist in den Eigenschaften der DLL im Abschnitt Sicherheit die CheckBox "Zulassen" zu aktivieren.
