CREATE TABLE IF NOT EXISTS zanr (
    id SERIAL PRIMARY KEY,
    nazev VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS album (
    id SERIAL PRIMARY KEY,
    nazev VARCHAR(200) NOT NULL,
    interpret VARCHAR(200) NOT NULL,
    rok INTEGER NOT NULL CHECK (rok >= 1900 AND rok <= 2100),
    zanr_id INTEGER NOT NULL REFERENCES zanr(id),
    popis TEXT
);

CREATE TABLE IF NOT EXISTS skladba (
    id SERIAL PRIMARY KEY,
    album_id INTEGER NOT NULL REFERENCES album(id) ON DELETE CASCADE,
    nazev VARCHAR(200) NOT NULL,
    delka_sekund INTEGER NOT NULL CHECK (delka_sekund > 0),
    cislo_stopy INTEGER NOT NULL CHECK (cislo_stopy > 0)
);
