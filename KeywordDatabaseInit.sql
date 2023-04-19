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

INSERT INTO Location (LocationID, Name)
VALUES 
    (1, 'ill'),
    (2, 'kidsIll'),
    (3, 'meeting'),
    (4, 'home'),
    (5, 'office'),
    (6, 'off'),
    (7, 'remote'),
    (8, 'undecided');

INSERT INTO LocationKeywords (Keyword, Location)
VALUES
    ('seng', 1), ('dynen', 1), ('på langs', 1), ('vandret', 1), ('lægger mig', 1),
    ('syg', 1), ('ikke frisk', 1), ('ikke på toppen', 1), ('skidt', 1), ('helbred', 1),
    ('influenza', 1), ('feber', 1), ('forkølelse', 1), ('svimmel', 1), ('kvalme', 1),
    ('ondt i hovedet', 1), ('migræne', 1), ('toilet', 1),
    
    ('den lille', 2), ('de små', 2), ('familie', 2), ('børn', 2), ('barn', 2),
    ('pige', 2), ('dreng', 2), ('unger', 2), ('søn', 2), ('datter', 2), ('Felix', 2), ('Otto', 2),
    
    ('møde', 3),
    
    ('hjem', 4), ('ikke på kontoret', 4),
    
    ('kommer ind', 5), ('kommer jeg ind', 5), ('inde', 5), ('ind forbi', 5),
    ('retur', 5), ('er tilbage', 5),
    ('på kontoret', 5), ('på arbejdet', 5), ('kommer i firmaet', 5), ('konnes', 5),
    
    ('fri', 6), ('off', 6), ('går fra', 6), ('stopper', 6), ('holder', 6), ('lukker ned', 6), ('starter', 6),
    
    ('tager ud til', 7), ('tager ned til', 7), ('hos', 7);

GO;