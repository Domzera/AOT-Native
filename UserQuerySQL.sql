-- Use esta query para criar a tabela no PostegreSQL
-- Table: public.users

-- DROP TABLE IF EXISTS public.users;

CREATE TABLE IF NOT EXISTS public.users
(
    "userId" integer NOT NULL DEFAULT nextval('users_id_seq'::regclass),
    "userFirstName" character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "userLastName" character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "userMail" character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "userPassword" character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "userPhonenumber" character varying(20) COLLATE pg_catalog."default",
    "userAtivo" integer DEFAULT 1,
    CONSTRAINT users_pkey PRIMARY KEY ("userId"),
    CONSTRAINT users_email_key UNIQUE ("userMail")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.users
    OWNER to postgres;