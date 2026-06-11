INSERT INTO zanr (nazev) VALUES
    ('Rock'),
    ('Pop'),
    ('Classical'),
    ('Jazz'),
    ('Metal'),
    ('Hip-Hop'),
    ('Electronic'),
    ('Folk'),
    ('R&B'),
    ('Punk')
ON CONFLICT DO NOTHING;
