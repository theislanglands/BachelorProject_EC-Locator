/* Creating Database */

CREATE DATABASE Keywords;
GO;

USE Keywords;
GO;

/* Creating tables */

DROP TABLE IF EXISTS Location;
CREATE TABLE Location
(
    LocationID INT NOT NULL,
    Name VARCHAR(16),
    PRIMARY KEY (LocationID)
);

DROP TABLE IF EXISTS LocationKeywords;
CREATE TABLE LocationKeywords
(
    Keyword  VARCHAR(24) NOT NULL,
    Location INT NOT NULL,
    PRIMARY KEY (Keyword),
    FOREIGN KEY (Location) REFERENCES Location (LocationID)
);



DROP TABLE IF EXISTS TimeKeywords;
CREATE TABLE TimeKeywords
(
    Keyword VARCHAR(24) NOT NULL,
    Time TIME NOT NULL,
    PRIMARY KEY (Keyword)
);

DROP TABLE IF EXISTS MinuteIndicatorKeywords;
CREATE TABLE MinuteIndicatorKeywords
(
    Keyword VARCHAR(24) NOT NULL,
    Minutes SMALLINT NOT NULL,
    PRIMARY KEY (Keyword)
);

DROP TABLE IF EXISTS StartIndicatorKeywords;
CREATE TABLE StartIndicatorKeywords
(
    Keyword VARCHAR(24) NOT NULL,
    PRIMARY KEY (Keyword)
);

DROP TABLE IF EXISTS StopIndicatorKeywords;
CREATE TABLE StopIndicatorKeywords
(
    Keyword VARCHAR(24) NOT NULL,
    PRIMARY KEY (Keyword)
);

GO;

/* Populating tables */
INSERT INTO TimeKeywords (Keyword, Time)
VALUES
    ('formiddag', '09:00:00'),
    ('eftermiddag', '12:00:00'),
    ('frokost', '11:15:00'),
    ('middag', '12:00:00'),
    ('aften', '16:00:00');

INSERT INTO MinuteIndicatorKeywords (Keyword, Minutes)
VALUES
    ('kvart over', 15),
    ('kvart i', -15),
    ('halv', -30);

INSERT INTO StopIndicatorKeywords (Keyword)
VALUES
    ('stopper'),
    ('holder');

INSERT INTO StartIndicatorKeywords (Keyword)
VALUES
    ('starter');

-- Insert data into the Location table
INSERT INTO Location (LocationID, Name)
VALUES (1, 'Office'), (2, 'Home'), (3, 'Meeting'), (4, 'Remote'), (5, 'DayOff'), (6, 'Ill'), (7, 'KidsIll'), (8, 'Undecided');

-- Insert data into the LocationKeywords table
INSERT INTO LocationKeywords (Keyword, Location)
VALUES
    ('sengedag', 6), ('ikke frisk', 6), ('vandret', 6), ('ikke på toppen', 6), ('dynen', 6), ('syg', 6), ('influenza', 6), ('ligger syg', 6), ('lagt syg', 6), ('feber', 6), ('sygdom', 6), ('forkølelse', 6), ('svimmel', 6), ('kvalme', 6), ('ondt i hovedet', 6), ('på langs', 6), ('syge', 6), ('helbred', 6), ('feberbarn', 6), ('lægger mig', 6), ('skidt', 6), ('under dynen', 6), ('toilet', 6),
    ('møde', 3),
    ('hjem', 2), ('hjemme', 2), ('hjemmefra', 2), ('på hjemmefra', 2), ('tager den hjemmefra', 2), ('tager jeg den hjemmefra', 2), ('hjemmekontoret', 2), ('hjemmeskansen', 2), ('ikke på kontoret', 2), ('kommer på kontoret', 2),
    ('kommer ind', 1), ('retur', 1), ('er tilbage', 1), ('på kontoret', 1), ('inde', 1), ('på arbejdet', 1), ('ind forbi', 1), ('er inde', 1), ('er inde ved', 1), ('kommer jeg ind', 1), ('kommer i firmaet', 1), ('konnes', 1),
    ('holder fri', 5), ('fri', 5), ('holder fridag', 5), ('fridag', 5), ('holder weekend', 5), ('off', 5),
    ('tager ud til', 4), ('tager ned til', 4), ('er hos', 4), ('ved', 4);





