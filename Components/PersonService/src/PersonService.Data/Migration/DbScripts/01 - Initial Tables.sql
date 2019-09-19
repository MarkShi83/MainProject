CREATE TABLE person (
  id                    bigint                      NOT NULL,
  created               TIMESTAMP WITHOUT TIME ZONE NOT NULL,
  updated               TIMESTAMP WITHOUT TIME ZONE         ,
  first                 VARCHAR(255)                        ,
  last                  VARCHAR(255)                        ,
  age                   int                                 ,
  gender                int                                 ,

  CONSTRAINT person_pkey PRIMARY KEY (id)
);