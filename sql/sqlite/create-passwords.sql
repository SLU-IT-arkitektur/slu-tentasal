-- SQLite (för lokal utveckling)
-- Skapas automatiskt av appen i Development (SqliteDevSeeder), skriptet finns som referens.
-- starttime lagras som TEXT i formatet 'yyyy-MM-dd HH:mm:ss' i lokal svensk tid.
CREATE TABLE IF NOT EXISTS Passwords (
    campus    TEXT NOT NULL,
    password  TEXT NOT NULL,
    starttime TEXT NOT NULL
);

INSERT INTO Passwords (campus, password, starttime)
VALUES ('Ultuna', 'Tenta-Ultuna-42', strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime', '+10 minutes'));
