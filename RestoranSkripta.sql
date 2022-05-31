CREATE TABLE jelo (
    id_jela      INTEGER NOT NULL  IDENTITY(1,1),
    naziv_jela   VARCHAR(50) NOT NULL,
    opis_jela    text
)
 
ALTER TABLE Jelo ADD constraint jelo_pk PRIMARY KEY CLUSTERED (id_jela)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )

CREATE TABLE meni (
    id_menija      INTEGER NOT NULL  IDENTITY(1,1),
    naziv_menija   VARCHAR(50) NOT NULL,
    aktivan        bit NOT NULL
)

ALTER TABLE Meni ADD constraint meni_pk PRIMARY KEY CLUSTERED (id_menija)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )

CREATE TABLE nameniju (
    id_menija   INTEGER NOT NULL,
    id_jela     INTEGER NOT NULL,
    cena             bigint NOT NULL
)

ALTER TABLE NaMeniju ADD constraint NaMeniju_pk PRIMARY KEY CLUSTERED (id_menija, id_jela)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )

CREATE TABLE sastojci (
    id_sastojka      INTEGER NOT NULL IDENTITY(1,1),
    naziv_sastojka   VARCHAR(50) NOT NULL,
    opis_sastojka    text
)

ALTER TABLE Sastojci ADD constraint sastojci_pk PRIMARY KEY CLUSTERED (id_sastojka)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )

CREATE TABLE sesastoji (
    id_sastojka   INTEGER NOT NULL,
    id_jela           INTEGER NOT NULL,
    kolicina               INTEGER NOT NULL
)

ALTER TABLE SeSastoji ADD constraint sesastoji_pk PRIMARY KEY CLUSTERED (id_sastojka, id_jela)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )

ALTER TABLE NaMeniju
    ADD CONSTRAINT nameniju_jelo_fk FOREIGN KEY ( id_jela )
        REFERENCES jelo ( id_jela )

ALTER TABLE NaMeniju
    ADD CONSTRAINT meni_fk FOREIGN KEY ( id_menija )
        REFERENCES meni ( id_menija )

ALTER TABLE SeSastoji
    ADD CONSTRAINT jelo_fk FOREIGN KEY ( id_jela )
        REFERENCES jelo ( id_jela )

ALTER TABLE SeSastoji
    ADD CONSTRAINT sastojci_fk FOREIGN KEY ( id_sastojka )
        REFERENCES sastojci ( id_sastojka )
