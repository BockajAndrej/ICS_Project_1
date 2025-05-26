# ICS Project
## Team
- Andrej Bočkaj:

- Jakub Fiľo:

- Matyáš Bubrinka:
    Návrh figmy
    Vytvoření Repository, mapperů a UoW
    Tvorba a editace playlistu (MAUI)
- Tomáš Siakeľ:

- Vít Kološ:

## Popis aplikace
Databázová aplikace vytvořená v tomto projektu slouží k vytváření a správě playlistů a skladeb. Inspirací pro návrh i funkčnost byla streamovací platforma Spotify.

Grafické uživatelské rozhraní (GUI) obsahuje dvě hlavní obrazovky: **Playlists** a **Music Tracks**.  
Každá z těchto obrazovek je rozdělena na dvě podsekce:

- **Levá část** zobrazuje seznam playlistů.
- **Pravá část** zobrazuje detail vybrané položky:
  - Na obrazovce *Playlists* se zobrazují podrobnosti o vybraném playlistu.
  - Na obrazovce *Music Tracks* se zobrazuje list všech uložených skladeb.

Součástí rozhraní jsou také vyskakovací okna (pop-upy) pro vytváření nových playlistů a skladeb.

Hlavní databázové entity jsou: **Playlist**, **Artist**, **MusicTrack** a **Genre**.

- **Playlist** ukládá název, popis, počet skladeb, celkovou délku přehrávání a seznam skladeb, které obsahuje.  
- **Artist** ukládá své jméno a seznam skladeb, které vytvořil.  
- **MusicTrack** ukládá název, popis, délku, velikost souboru, URL a seznam playlistů, interpretů a žánrů, ke kterým náleží.
