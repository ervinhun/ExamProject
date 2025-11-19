-- DROP & CREATE SCHEMA
DROP SCHEMA IF EXISTS gameapp CASCADE;
CREATE SCHEMA IF NOT EXISTS gameapp;

-- On the neon it might be gen_random_uuid() instead of uuid_generate_v4() --

-- ==============================
-- ROLES
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.roles
(
    id          UUID PRIMARY KEY   NOT NULL,
    name        VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    is_deleted  BOOLEAN DEFAULT FALSE
);

-- ==============================
-- USERS
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.users
(
    id                        UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    full_name                 TEXT                     NOT NULL,
    email                     TEXT UNIQUE              NOT NULL,
    password                  TEXT                     NOT NULL,
    phone_no                  VARCHAR(100),
    jwt_token                 VARCHAR(255),
    jwt_refresh_token         VARCHAR(255),
    active_status_expiry_date DATE,
    created_at                TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at                TIMESTAMP WITH TIME ZONE,
    is_deleted                BOOLEAN                           DEFAULT FALSE
);

-- ==============================
-- USER ROLE
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.user_role
(
    id         UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    user_id    UUID                     NOT NULL REFERENCES gameapp.users (id),
    role_id    UUID                     NOT NULL REFERENCES gameapp.roles (id),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_by UUID REFERENCES gameapp.users (id)
);

-- ==============================
-- WALLET
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.wallet
(
    id         UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    balance    DECIMAL(12, 2)           NOT NULL,
    user_id    UUID                     NOT NULL UNIQUE REFERENCES gameapp.users (id),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL,
    is_deleted BOOLEAN                           DEFAULT FALSE
);

-- ==============================
-- PLAYER
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.player
(
    id        UUID PRIMARY KEY NOT NULL DEFAULT uuid_generate_v4(),
    user_id   UUID             NOT NULL REFERENCES gameapp.users (id),
    wallet_id UUID             NOT NULL REFERENCES gameapp.wallet (id),
    is_active BOOLEAN          NOT NULL DEFAULT false
);

-- ==============================
-- ADMIN
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.admin
(
    id      UUID PRIMARY KEY NOT NULL DEFAULT uuid_generate_v4(),
    user_id UUID             NOT NULL REFERENCES gameapp.users (id)
);

-- ==============================
-- TRANSACTIONS
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.transactions
(
    id                 UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    player_id          UUID                     NOT NULL REFERENCES gameapp.player (id),
    transaction_number TEXT UNIQUE              NOT NULL,
    amount             DECIMAL(12, 2)           NOT NULL,
    status             VARCHAR(50)              NOT NULL, -- active/approved/rejected
    reviewed_by        UUID REFERENCES gameapp.admin (id),
    created_at         TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at         TIMESTAMP WITH TIME ZONE
);

-- ==============================
-- GAME
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.game
(
    id         UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    start_date DATE                     NOT NULL,
    end_date   DATE                     NOT NULL,
    is_closed  BOOLEAN                           DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    closed_at  TIMESTAMP WITH TIME ZONE
);

-- ==============================
-- WINNING NUMBERS
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.winning_numbers
(
    id         UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    game_id    UUID                     NOT NULL REFERENCES gameapp.game (id),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==============================
-- BOARD (template)
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.board
(
    id          UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    user_id     UUID                     NOT NULL REFERENCES gameapp.users (id),
    numbers     TEXT                     NOT NULL, -- "1,5,17,9,13"
    field_count INTEGER                  NOT NULL, -- 5–8
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL,
    is_deleted  BOOLEAN                           DEFAULT FALSE
);

-- ==============================
-- PLAYING BOARD (actual game tickets)
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.playing_board
(
    id                     UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    user_id                UUID                     NOT NULL REFERENCES gameapp.users (id),
    board_id               UUID                     NOT NULL REFERENCES gameapp.board (id),
    game_id                UUID                     NOT NULL REFERENCES gameapp.game (id),
    numbers                TEXT                     NOT NULL,
    field_count            INTEGER                  NOT NULL,
    price                  DECIMAL(12, 2)           NOT NULL,
    is_repeat              BOOLEAN                           DEFAULT FALSE,
    repeat_count_remaining INTEGER                           DEFAULT 0,
    is_winning_board       BOOLEAN                           DEFAULT FALSE,
    created_at             TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==============================
-- PLAYING NUMBERS
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.playing_numbers
(
    id               UUID PRIMARY KEY         NOT NULL DEFAULT uuid_generate_v4(),
    playing_board_id UUID                     NOT NULL REFERENCES gameapp.playing_board (id),
    created_at       TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==============================
-- USER HISTORY
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.user_history
(
    id          BIGSERIAL PRIMARY KEY,
    public_id   UUID                     NOT NULL DEFAULT uuid_generate_v4(),
    user_id     UUID                     NOT NULL REFERENCES gameapp.users (id),
    title       TEXT                     NOT NULL,
    description TEXT,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==============================
-- PLAYING HISTORY
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.playing_history
(
    id          BIGSERIAL PRIMARY KEY,
    public_id   UUID                     NOT NULL DEFAULT uuid_generate_v4(),
    user_id     UUID                     NOT NULL REFERENCES gameapp.users (id),
    ticket_id   UUID                     NOT NULL REFERENCES gameapp.playing_board (id),
    game_id     UUID                     NOT NULL REFERENCES gameapp.game (id),
    title       TEXT                     NOT NULL,
    description TEXT,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL
);

