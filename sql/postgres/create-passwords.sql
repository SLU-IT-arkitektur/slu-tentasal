-- PostgreSQL
-- starttime lagras i lokal svensk tid (Europe/Stockholm), utan tidszon.
CREATE TABLE passwords (
    campus    varchar(100) NOT NULL,
    sal       varchar(100) NULL,
    password  varchar(100) NOT NULL,
    starttime timestamp    NOT NULL
);

CREATE INDEX ix_passwords_campus_starttime ON passwords (campus, starttime);

-- Exempeldata
INSERT INTO passwords (campus, sal, password, starttime)
VALUES ('Ultuna', NULL, 'Tenta-Ultuna-42', (now() AT TIME ZONE 'Europe/Stockholm') + interval '10 minutes');
